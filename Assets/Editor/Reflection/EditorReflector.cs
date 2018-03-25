using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using Weichx.ReflectionAttributes;

namespace Weichx.EditorReflection {

    public static class EditorReflector {

        public class PointableMethod : Attribute { }

        private class PropertyDrawerDescription {

            public readonly Type type;
            public readonly PropertyDrawerForOption option;

            public PropertyDrawerDescription(Type type, PropertyDrawerForOption option = PropertyDrawerForOption.None) {
                this.type = type;
                this.option = option;
            }

        }

        private const BindingFlags BindFlags = BindingFlags.Public |
                                               BindingFlags.NonPublic |
                                               BindingFlags.Instance;

        private const string SubclassCache = nameof(SubclassCache);
        private const string PointableMethodCache = nameof(PointableMethodCache);
        private const string TypeDrawerCache = nameof(TypeDrawerCache);
        private const string FieldInfoCache = nameof(FieldInfoCache);

        private static GenericCache cache;
        private static List<MethodInfo> methodSet;
        private static readonly List<Type> allTypes;
        private static readonly List<Type> genericTypes;
        private static readonly List<Type> nonGenericTypes;
        private static readonly Type[] typeArgs = new Type[1];

        static EditorReflector() {
            allTypes = new List<Type>(128);
            cache = new GenericCache();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++) {
                Assembly assembly = assemblies[i];
                if (FilterAssembly(assembly)) {
                    Type[] exportedTypes = assembly.GetExportedTypes();
                    GetPropertyDrawers(exportedTypes);
                    allTypes.AddRange(exportedTypes);
                }
            }
            FindPointableMethods();
            genericTypes = allTypes.FindAll((type) => type.IsGenericType);
            nonGenericTypes = allTypes.FindAll((type) => !type.IsGenericType);
        }

        private static void GetPropertyDrawers(Type[] types) {
            for (int i = 0; i < types.Length; i++) {
                Type type = types[i];
                IEnumerable<PropertyDrawerFor> itr = type.GetCustomAttributes<PropertyDrawerFor>(true);
                foreach (PropertyDrawerFor drawer in itr) {
                    cache.AddItemToCache(TypeDrawerCache, drawer.type, new PropertyDrawerDescription(type, drawer.options));
                }
            }
        }

        public static object MakeInstance(Type type, Type[] signature, object[] parameters) {
            return type.GetConstructor(signature)?.Invoke(parameters);

        }

        public static object MakeInstance(Type type) {
            return Activator.CreateInstance(type, true);
        }

        public static T MakeInstance<T>(Type type) {
            return (T) Activator.CreateInstance(type, true);
        }

        public static T MakeInstance<T>() {
            return (T) Activator.CreateInstance(typeof(T), true);
        }

        public static object GetDefaultForType(Type type) {
            return type.IsValueType ? MakeInstance(type) : null;
        }

        private static void FindPointableMethods() {
            methodSet = new List<MethodInfo>();
            Dictionary<string, Delegate> pointerCache = cache.GetCache<string, Delegate>(PointableMethodCache);
            for (int i = 0; i < allTypes.Count; i++) {
                MethodInfo[] methods = allTypes[i].GetMethods(BindingFlags.Static | BindingFlags.Public);
                for (int methodIdx = 0; methodIdx < methods.Length; methodIdx++) {
                    MethodInfo methodInfo = methods[methodIdx];
                    if (IsPointableMethod(methodInfo)) {
                        pointerCache[MethodPointerUtils.CreateSignature(methodInfo)] = CreateDelegate(methodInfo);
                    }
                    methodSet.AddRange(methods);
                }
            }
        }

        private static bool IsPointableMethod(MethodInfo methodInfo) {
            return (methodInfo.GetCustomAttributes(typeof(PointableMethod), true).Length > 0 &&
                    !methodInfo.IsDefined(typeof(ExtensionAttribute), true) &&
                    !methodInfo.IsGenericMethod && !methodInfo.IsGenericMethodDefinition);
        }

        public static Delegate FindDelegateWithSignature(string signature) {
            return cache.GetItemFromCache<string, Delegate>(PointableMethodCache, signature);
        }

        public static FieldInfo[] GetFields(Type type) {
            return cache.GetOrAddToCache(FieldInfoCache, type, () => {
                return GetFieldsIncludingBaseClasses(type);
            });
        }

        /// <summary>
        ///   Returns all the fields of a type, working around the fact that reflection
        ///   does not return private fields in any other part of the hierarchy than
        ///   the exact class GetFields() is called on.
        /// </summary>
        /// <param name="type">Type whose fields will be returned</param>
        /// <param name="bindingFlags">Binding flags to use when querying the fields</param>
        /// <returns>All of the type's fields, including its base types</returns>
        public static FieldInfo[] GetFieldsIncludingBaseClasses(Type type, BindingFlags bindingFlags = BindFlags) {
            FieldInfo[] fieldInfos = type.GetFields(bindingFlags);

            // If this class doesn't have a base, don't waste any time
            if (type.BaseType == typeof(object)) {
                return fieldInfos;
            }
            else { // Otherwise, collect all types up to the furthest base class
                var fieldInfoList = new List<FieldInfo>(fieldInfos);
                while (type.BaseType != null && type.BaseType != typeof(object)) {
                    type = type.BaseType;
                    fieldInfos = type.GetFields(bindingFlags);

                    // Look for fields we do not have listed yet and merge them into the main list
                    for (int index = 0; index < fieldInfos.Length; ++index) {
                        bool found = false;

                        for (int searchIndex = 0; searchIndex < fieldInfoList.Count; ++searchIndex) {
                            bool match =
                                (fieldInfoList[searchIndex].DeclaringType == fieldInfos[index].DeclaringType) &&
                                (fieldInfoList[searchIndex].Name == fieldInfos[index].Name);

                            if (match) {
                                found = true;
                                break;
                            }
                        }

                        if (!found) {
                            fieldInfoList.Add(fieldInfos[index]);
                        }
                    }
                }

                return fieldInfoList.ToArray();
            }
        }

        public static Type[] FindSubClassesWithNull(Type type) {
            return cache.GetOrAddToCache(nameof(FindSubClassesWithNull), type, (t0) => {
                List<Type> retVal = new List<Type>(4);
                retVal.Add(null);
                for (int i = 0; i < allTypes.Count; i++) {
                    Type t = allTypes[i];
                    if (t.IsSubclassOf(t0)) {
                        retVal.Add(t);
                    }
                }
                return retVal.ToArray();
            });
        }

        public static string[] FindSubClassNamesWithNull(Type type) {
            return cache.GetOrAddToCache(nameof(FindSubClassNamesWithNull), type, (t) => {
                Type[] types = FindSubClassesWithNull(t);
                string[] names = new string[types.Length];
                names[0] = "-- Null --";
                for (int i = 1; i < names.Length; i++) {
                    names[i] = types[i].Name;
                }
                return names;
            });
        }

        public static Type[] FindConstructableSubClasses(Type type) {
            return cache.GetOrAddToCache(nameof(FindConstructableSubClasses), type, (t0) => {
                List<Type> retVal = new List<Type>(4);
                for (int i = 0; i < allTypes.Count; i++) {
                    Type t = allTypes[i];
                    if (t.IsSubclassOf(t0) && !t.IsGenericTypeDefinition && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null) {
                        retVal.Add(t);
                    }
                }
                return retVal.ToArray();
            });
        }

        public static Type[] FindConstructableSubClassesWithNull(Type type) {
            return cache.GetOrAddToCache(nameof(FindConstructableSubClassesWithNull), type, (t0) => {
                List<Type> retVal = new List<Type>(4);
                retVal.Add(null);
                for (int i = 0; i < allTypes.Count; i++) {
                    Type t = allTypes[i];
                    if (t.IsSubclassOf(t0) && !t.IsGenericTypeDefinition && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null) {
                        retVal.Add(t);
                    }
                }
                return retVal.ToArray();
            });
        }

        public static string[] FindConstructableSubClassNamesWithNull(Type type) {
            return cache.GetOrAddToCache(nameof(FindConstructableSubClassNamesWithNull), type, (t) => {
                Type[] types = FindConstructableSubClasses(t);
                string[] names = new string[types.Length + 1];
                names[0] = "-- Null --";
                for (int i = 1; i < names.Length; i++) {
                    names[i] = types[i - 1].Name;
                }
                return names;
            });
        }

        public static string[] FindConstructableSubClassNames(Type type) {
            return cache.GetOrAddToCache(nameof(FindConstructableSubClassNames), type, (t) => {
                Type[] types = FindConstructableSubClasses(t);
                string[] names = new string[types.Length];
                for (int i = 0; i < types.Length; i++) {
                    names[i] = types[i].Name;
                }
                return names;
            });
        }

        public static Type[] FindSubClasses(Type type) {
            return cache.GetOrAddToCache(SubclassCache, type, (t0) => {
                List<Type> retVal = new List<Type>(4);
                for (int i = 0; i < allTypes.Count; i++) {
                    Type t = allTypes[i];
                    if (t.IsSubclassOf(t0)) {
                        retVal.Add(t);
                    }
                }
                return retVal.ToArray();
            });
        }

        public static Type[] FindSubClasses<T>() {
            return FindSubClasses(typeof(T));
        }

        public static bool HasConstructor(Type target, Type[] signature) {
            return target.GetConstructor(signature) != null;
        }

        public static bool IsDefaultConstructable(Type type) {
            return HasDefaultConstructor(type) && !type.IsAbstract && !type.IsArray;
        }
        
        public static bool HasDefaultConstructor(Type target) {
            return target.GetConstructor(Type.EmptyTypes) != null;
        }

        public static bool IsDerivedOfTypeWithGenericArgument(Type inputType, Type targetGeneric) {
            while (inputType != null && inputType != typeof(object)) {
                if (inputType.IsGenericType) {
                    if (inputType.GetGenericArguments()[0] == targetGeneric) {
                        return true;
                    }
                }
                inputType = inputType.BaseType;
            }
            return false;
        }

        private static bool IsDerivedOfGenericType(Type type, Type genericType, bool includeInputType) {
            while (true) {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType) {
                    return type != genericType || includeInputType;
                }
                else if (type.BaseType != null) {
                    type = type.BaseType;
                }
                else {
                    return false;
                }
            }
        }

        public static List<Type> FindSubClassesWithAttribute<T, U>(bool includeInputType = false) where T : class where U : Attribute {
            List<Type> subclasses = FindSubClasses(typeof(T)).ToList();
            return subclasses.FindAll((type) => type.GetCustomAttributes(typeof(U), true).Length > 0);
        }

        private static Delegate FindMethod(Type type, string methodName, Type retnType = null, params Type[] arguements) {
            if (retnType == null) retnType = typeof(void);
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            for (int i = 0; i < methods.Length; i++) {
                MethodInfo method = methods[i];
                if (method.Name == methodName && MatchesSignature(method, retnType, arguements)) {
                    return CreateDelegate(method);
                }
            }
            return null;
        }

        private static bool MatchesSignature(MethodInfo methodInfo, Type retnType, Type[] parameters = null) {
            if (retnType == null) retnType = typeof(void);
            if (methodInfo.ReturnType != retnType) return false;
            if (parameters == null) return true;
            ParameterInfo[] methodParameters = methodInfo.GetParameters();
            if (methodParameters.Length != parameters.Length) return false;
            for (int i = 0; i < methodParameters.Length; i++) {
                if (methodParameters[i].ParameterType != parameters[i]) {
                    return false;
                }
            }
            return true;
        }

        public static object GetAttribute(FieldInfo fieldInfo, Type attrType) {
            object[] attrs = fieldInfo.GetCustomAttributes(attrType, false);
            return (attrs.Length > 0) ? attrs[0] : null;
        }

        public static bool HasAttribute(FieldInfo fieldInfo, Type attrType) {
            return fieldInfo.GetCustomAttributes(attrType, false).Length > 0;
        }

        private static bool HasAttribute(MethodInfo methodInfo, Type attrType) {
            return methodInfo.GetCustomAttributes(attrType, false).Length > 0;
        }

        public static Delegate CreateDelegate(MethodInfo method) {
            List<Type> args = new List<Type>(method.GetParameters().Select(p => p.ParameterType));
            Type delegateType;
            if (method.ReturnType == typeof(void)) {
                delegateType = Expression.GetActionType(args.ToArray());
            }
            else {
                args.Add(method.ReturnType);
                delegateType = Expression.GetFuncType(args.ToArray());
            }
            return Delegate.CreateDelegate(delegateType, null, method);
        }

//
        public static Type GetPropertyDrawerForType(Type type) {
            Type originalType = type;
            PropertyDrawerDescription drawerDesc = cache.GetItemFromCache<Type, PropertyDrawerDescription>(TypeDrawerCache, type);

            while (drawerDesc == null && type.BaseType != null) {

                if (type.IsGenericType) {
                    Type genTypeDef = type.GetGenericTypeDefinition();
                    drawerDesc = cache.GetItemFromCache<Type, PropertyDrawerDescription>(TypeDrawerCache, genTypeDef);
                    if (drawerDesc != null) {
                        cache.AddItemToCache(TypeDrawerCache, originalType, drawerDesc);
                        return drawerDesc.type;
                    }
                }

                type = type.BaseType;
                drawerDesc = cache.GetItemFromCache<Type, PropertyDrawerDescription>(TypeDrawerCache, type);

                // if there is no option to include subclasses, keep looking
                if (drawerDesc != null && drawerDesc.option != PropertyDrawerForOption.IncludeSubclasses) {
                    drawerDesc = null;
                }

            }

            // if we dont find anything, create a generic one and cache it
            if (drawerDesc == null) {
                drawerDesc = new PropertyDrawerDescription(typeof(GenericPropertyDrawer));
                cache.AddItemToCache(TypeDrawerCache, originalType, drawerDesc);
            }

            return drawerDesc.type;
        }

        public static Type GetPropertyDrawerForReflectedProperty(ReflectedProperty property) {

            UsePropertyDrawer useDrawer = property.GetAttribute<UsePropertyDrawer>();

            if (useDrawer != null) {
                return GetPropertyDrawerForType(useDrawer.type);
            }

            return GetPropertyDrawerForType(property.Type);

        }

        public static ReflectedPropertyDrawer CreateReflectedPropertyDrawer(ReflectedProperty property) {
            Type drawerType = GetPropertyDrawerForReflectedProperty(property);
            Debug.Assert(drawerType != null, "drawerType != null");
            ReflectedPropertyDrawer drawer = MakeInstance(drawerType) as ReflectedPropertyDrawer;
            Debug.Assert(drawer != null, "drawer != null");
            drawer.SetProperty(property);
            return drawer;
        }

        private static bool FilterAssembly(Assembly assembly) {
            string name = assembly.FullName;

            if (!name.StartsWith("_") && !name.StartsWith("Assembly-CSharp")) {
                return false;
            }

            if (name.StartsWith("_StateChart")
                || name.StartsWith("_Persistence")
                || name.StartsWith("_Freespace")
                || name.StartsWith("_Attribute")
                || name.StartsWith("_GUI")
                || name.StartsWith("_Util")
                || name.StartsWith("_Plugin")) {
                return false;
            }
            return name.IndexOf("-firstpass", StringComparison.Ordinal) == -1;
        }

        public static object CreateGenericInstance(Type type, Type genericArgument) {
            Debug.Assert(type != null && genericArgument != null && type.IsGenericTypeDefinition);
            typeArgs[0] = genericArgument;
            Type makeme = type.MakeGenericType(typeArgs);
            Debug.Assert(makeme.GetConstructor(Type.EmptyTypes) != null, "makeme.GetConstructor(Type.EmptyTypes) != null");
            return MakeInstance(makeme);
        }

        public static object InvokeMethod(object target, string data) {
            MethodInfo methodInfo = target.GetType().GetMethod(data, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (methodInfo != null) {
                return methodInfo.Invoke(target, new object[0]);
            }
            return null;
        }

    }

}