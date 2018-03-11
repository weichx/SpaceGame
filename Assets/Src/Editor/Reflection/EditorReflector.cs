using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using SpaceGame.Reflection;
using UnityEngine;

namespace SpaceGame.Editor.Reflection {

    public static class EditorReflector {

        public class PointableMethod : Attribute { }

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
            int count = 0;
            for (int i = 0; i < assemblies.Length; i++) {
                Assembly assembly = assemblies[i];
                if (FilterAssembly(assembly)) {
                    Type[] exportedTypes = assembly.GetExportedTypes();
                    if (assembly.FullName.IndexOf("Editor", StringComparison.Ordinal) != -1) {
                        GetPropertyDrawers(exportedTypes);
                    }
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
                    cache.AddItemToCache(TypeDrawerCache, drawer.type, type);
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

        public static Type GetPropertyDrawerForType(ReflectedProperty property) {
            return cache.GetItemFromCache<Type, Type>(TypeDrawerCache, property.Type);
        }

        
        public static ReflectedPropertyDrawer CreateReflectedPropertyDrawer(Type type) {
            Type drawerType = cache.GetItemFromCache<Type, Type>(TypeDrawerCache, type);
            if (drawerType != null) {
                return Activator.CreateInstance(drawerType) as ReflectedPropertyDrawer;
            }
            return new GenericPropertyDrawer();
        }
        
//        private static List<Meta> customPropertyDrawerTypes;
//        private static Dictionary<Type, UnityEditor.PropertyDrawer> drawerCache;
//
//        private struct Meta {
//
//            public Type attributeArgumentType;
//            public Type attributeDrawerType;
//
//            public Meta(Type attributeDrawerType, Type attributeArgumentType) {
//                this.attributeDrawerType = attributeDrawerType;
//                this.attributeArgumentType = attributeArgumentType;
//            }
//
//        }
//
//        //todo this isnt well cached
//        private static List<Meta> GetPropertyDrawerTypes(Assembly assembly) {
//            if (customPropertyDrawerTypes != null) return customPropertyDrawerTypes;
//
//            customPropertyDrawerTypes = new List<Meta>();
//            BindingFlags bindFlags = BindingFlags.NonPublic | BindingFlags.Instance;
//
//            Type[] assemblyTypes = assembly.GetTypes();
//
//            for (int i = 0; i < assemblyTypes.Length; i++) {
//                object[] attributes = assemblyTypes[i].GetCustomAttributes(typeof(UnityEditor.CustomPropertyDrawer), true);
//                if (attributes.Length > 0) {
//                    FieldInfo m_TypeFieldInfo = attributes[0].GetType().GetField("m_Type", bindFlags);
//                    Debug.Assert(m_TypeFieldInfo != null, nameof(m_TypeFieldInfo) + " != null");
//                    Type m_Type = m_TypeFieldInfo.GetValue(attributes[0]) as Type;
//                    customPropertyDrawerTypes.Add(new Meta(assemblyTypes[i], m_Type));
//                }
//            }
//            return customPropertyDrawerTypes;
//        }
//
//        private static void BuildExtendedDrawerTypeMap() {
//            if (extendedDrawerTypeMap != null) return;
//            extendedDrawerTypeMap = new Dictionary<Type, Type>();
//            List<Type> extendedDrawerSubclasses = FindSubClasses<PropertyDrawerX>();
//            for (int i = 0; i < extendedDrawerSubclasses.Count; i++) {
//                Type drawerType = extendedDrawerSubclasses[i];
//                object[] attributes = drawerType.GetCustomAttributes(typeof(PropertyDrawerFor), false);
//                for (int j = 0; j < attributes.Length; j++) {
//                    PropertyDrawerFor attr = attributes[j] as PropertyDrawerFor;
//                    if (attr != null) extendedDrawerTypeMap[attr.type] = drawerType;
//                }
//            }
//        }
//
//        public static Type GetExtendedDrawerTypeFor(Type type) {
//            BuildExtendedDrawerTypeMap();
//            Type drawerType = extendedDrawerTypeMap.Get(type);
//            while (drawerType == null && type.BaseType != null && type.BaseType != typeof(object)) {
//                type = type.BaseType;
//                drawerType = extendedDrawerTypeMap.Get(type);
//            }
//            return drawerType;
//        }
//
//        private static Dictionary<ReflectedProperty, PropertyDrawerX> drawerInstanceCache = new Dictionary<ReflectedProperty, PropertyDrawerX>();
//
//        public static PropertyDrawerX GetCustomPropertyDrawerFor(ReflectedProperty property) {
//            if (drawerCache == null) {
//                drawerCache = new Dictionary<Type, UnityEditor.PropertyDrawer>();
//            }
//
//            PropertyDrawerX drawerX = drawerInstanceCache.Get(property);
//
//            if (drawerX == null) {
//                var drawerType = GetExtendedDrawerTypeFor(property.Type);
//                if (drawerType == null) return null;
//                drawerX = Activator.CreateInstance(drawerType) as PropertyDrawerX;
//                drawerInstanceCache[property] = drawerX;
//            }
//            return drawerX;
//        }
//
//        public static UnityEditor.PropertyDrawer GetCustomPropertyDrawerFor(Type type, params Assembly[] assemblies) {
//            if (drawerCache == null) {
//                drawerCache = new Dictionary<Type, UnityEditor.PropertyDrawer>();
//            }
//            UnityEditor.PropertyDrawer drawer;
//            if (drawerCache.TryGetValue(type, out drawer)) {
//                return drawer;
//            }
//
//            if (type == typeof(UnityEngine.RangeAttribute)) {
//
//                drawer = Activator.CreateInstance(typeof(UnityEditor.EditorGUI).Assembly.GetType("UnityEditor.RangeDrawer")) as UnityEditor.PropertyDrawer;
//                drawerCache[type] = drawer;
//                return drawer;
//            }
//
//            for (int a = 0; a < assemblies.Length; a++) {
//                List<Meta> metaList = GetPropertyDrawerTypes(assemblies[a]);
//                for (int i = 0; i < metaList.Count; i++) {
//                    Meta drawerMeta = metaList[i];
//                    Type attrArgument = drawerMeta.attributeArgumentType;
//                    if (type == attrArgument || type.IsSubclassOf(attrArgument)) {
//                        drawer = Activator.CreateInstance(drawerMeta.attributeDrawerType) as UnityEditor.PropertyDrawer;
//                        drawerCache[type] = drawer;
//                        return drawer;
//                    }
//                }
//            }
//            return null;
//        }

        private static bool FilterAssembly(Assembly assembly) {
            string name = assembly.FullName;
            return name.StartsWith("Assembly-CSharp") && name.IndexOf("-firstpass", StringComparison.Ordinal) == -1;
        }


    }

}