using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

        private const string SubclassCache = nameof(SubclassCache);
        private const string GenericSubclassCache = nameof(GenericSubclassCache);
        private const string NonGenericSubclassCache = nameof(NonGenericSubclassCache);
        private const string PointableMethodCache = nameof(PointableMethodCache);
        private const string TypeDrawerCache = nameof(TypeDrawerCache);
        private const string DelegateCache = nameof(DelegateCache);

        private static GenericCache cache;
        private static List<MethodInfo> methodSet;
        private static readonly List<Type> allTypes;
        private static readonly List<Type> genericTypes;
        private static readonly List<Type> nonGenericTypes;

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

        public static object GetDefaultForType(Type type) {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
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
            
        }
        
        public static IReadOnlyList<Type> FindConstructableSubClasses(Type type) {
            return cache.GetOrAddToCache("CreatableSubClasses", type, () => {
                List<Type> retVal = new List<Type>(4);
                for (int i = 0; i < allTypes.Count; i++) {
                    Type t = allTypes[i];
                    if (t.IsSubclassOf(type) && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null) {
                        retVal.Add(t);
                    }
                }
                return retVal;
            });
        }

        public static IReadOnlyList<Type> FindSubClasses(Type type) {
            return cache.GetOrAddToCache(SubclassCache, type, () => {
                List<Type> retVal = new List<Type>(4);
                for (int i = 0; i < allTypes.Count; i++) {
                    Type t = allTypes[i];
                    if (t.IsSubclassOf(type)) {
                        retVal.Add(t);
                    }
                }
                return retVal;
            });
        }

        public static IReadOnlyList<Type> FindSubClasses<T>() {
            return FindSubClasses(typeof(T));
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
                type = type.BaseType;
                drawerDesc = cache.GetItemFromCache<Type, PropertyDrawerDescription>(TypeDrawerCache, type);
                // if there is no option to include subclasses, keep looking
                if (drawerDesc != null && drawerDesc.option != PropertyDrawerForOption.IncludeSubclasses) {
                    drawerDesc = null;
                }
                else {
                    cache.AddItemToCache(TypeDrawerCache, originalType, drawerDesc);
                }
            }

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
            return Activator.CreateInstance(drawerType) as ReflectedPropertyDrawer;
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
            }//
//
            return name.IndexOf("-firstpass", StringComparison.Ordinal) == -1;
        }

    }

}