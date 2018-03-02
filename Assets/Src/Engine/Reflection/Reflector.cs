﻿namespace SpaceGame.Reflection {
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;


public static class Reflector {

    private static List<MethodInfo> methodSet;
    private static readonly List<Assembly> filteredAssemblies;
    private static readonly Dictionary<string, Delegate> pointerLookup;

    //Should be able to look up a method by signature
    //Should only create 1 delegate per method pointer
    //Should be able to enumerate all methods with signature/attribute

    static Reflector() {
        pointerLookup = new Dictionary<string, Delegate>();
        methodSet = new List<MethodInfo>();
        filteredAssemblies = new List<Assembly>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblies.Length; i++) {
            Assembly assembly = assemblies[i];
            if (FilterAssembly(assembly)) {
                filteredAssemblies.Add(assembly);
            }
        }
        FindPublicStaticMethods();
    }

    //todo probably add an attribute requirement as well to narrow search space even more
    private static void FindPublicStaticMethods() {
        methodSet = new List<MethodInfo>();
        for (int i = 0; i < filteredAssemblies.Count; i++) {
            Type[] types = filteredAssemblies[i].GetTypes();
            for (int typeIndex = 0; typeIndex < types.Length; typeIndex++) {
                var methods = types[typeIndex].GetMethods(BindingFlags.Static | BindingFlags.Public);
                for (int methodIdx = 0; methodIdx < methods.Length; methodIdx++) {
                    MethodInfo info = methods[methodIdx];
                    if (info.GetCustomAttributes(true).Length > 0 && !info.IsDefined(typeof(ExtensionAttribute), true) && !info.IsGenericMethod && !info.IsGenericMethodDefinition) {
                        pointerLookup[MethodPointerUtils.CreateSignature(info)] = CreateDelegate(info);
                    }
                    methodSet.AddRange(methods);
                }
            }
        }
    }

    public static Delegate FindDelegateWithSignature(string signature) {
        Delegate fn;
        if (signature == null) return null;
        return pointerLookup.TryGetValue(signature, out fn) ? fn : null;
    }

    public static FieldInfo GetProperty(object obj, string property) {
        return obj.GetType().GetField(property);
    }

    public static List<MethodPointer> FindMethodPointersWithAttribute(Type attrType, Type retnType, params Type[] parameterTypes) {
        var list = new List<MethodPointer>();
        for (int i = 0; i < methodSet.Count; i++) {
            MethodInfo methodInfo = methodSet[i];
            if (HasAttribute(methodInfo, attrType) && MatchesSignature(methodInfo, retnType, parameterTypes)) {
                list.Add(new MethodPointer(methodInfo));
            }
        }
        return list;
    }

    public static List<MethodPointer> FindMethodPointersWithAttribute<T>(Type retnType, params Type[] parameterTypes) where T : Attribute {
        return FindMethodPointersWithAttribute(typeof(T), retnType, parameterTypes);
    }

    public static List<Type> FindSubClasses(Type type, bool includeInputType = false) {
        if (type.IsGenericTypeDefinition) {
            return FindGenericSubClasses(type);
        }
        var retn = new List<Type>();
        for (int i = 0; i < filteredAssemblies.Count; i++) {
            Assembly assembly = filteredAssemblies[i];
            Type[] types = assembly.GetTypes();
            for (int t = 0; t < types.Length; t++) {
                if (types[t].IsGenericTypeDefinition) {
                    continue;
                }
                if (types[t].IsSubclassOf(type) || includeInputType && types[t] == type) {
                    retn.Add(types[t]);
                }
            }
        }
        return retn;
    }

    public static List<Type> FindSubClasses<T>(bool includeInputType = false) {
        return FindSubClasses(typeof(T), includeInputType);
    }

    private static List<Type> FindGenericSubClasses(Type type, bool includeInputType = false) {
        var retn = new List<Type>();
        for (int i = 0; i < filteredAssemblies.Count; i++) {
            Assembly assembly = filteredAssemblies[i];
            Type[] types = assembly.GetTypes();
            for (int t = 0; t < types.Length; t++) {
                if (IsDerivedOfGenericType(types[t], type, includeInputType)) {
                    retn.Add(types[t]);
                }
            }
        }
        return retn;
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
        return FindSubClasses<T>(includeInputType).FindAll((type) => {
            return type.GetCustomAttributes(typeof(U), false).Length > 0;
        });
    }

    private static Delegate FindMethod(Type type, string methodName, Type retnType = null, params Type[] arguements) {
        if (retnType == null) retnType = typeof(void);
        //todo not sure if its faster to user cached method types..probably is
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
        for (int i = 0; i < methods.Length; i++) {
            MethodInfo method = methods[i];
            if (method.Name == methodName && MatchesSignature(method, retnType, arguements)) {
                return null;// CreateDelegate(method);
            }
        }
        return null;
    }

    private static bool MatchesSignature(MethodInfo methodInfo, Type retnType, Type[] parameters = null) {
        if (retnType == null) retnType = typeof(void);
        if (methodInfo.ReturnType != retnType) return false;
        if (parameters == null) return true;
        var methodParameters = methodInfo.GetParameters();
        if (methodParameters.Length != parameters.Length) return false;
        for (int i = 0; i < methodParameters.Length; i++) {
            if (methodParameters[i].ParameterType != parameters[i]) {
                return false;
            }
        }
        return true;
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

//#if UNITY_EDITOR
//    private static List<Meta> customPropertyDrawerTypes;
//    private static Dictionary<Type, UnityEditor.PropertyDrawer> drawerCache;
//
//    private struct Meta {
//        public Type attributeArgumentType;
//        public Type attributeDrawerType;
//
//        public Meta(Type attributeDrawerType, Type attributeArgumentType) {
//            this.attributeDrawerType = attributeDrawerType;
//            this.attributeArgumentType = attributeArgumentType;
//        }
//    }
//
//    //todo this isnt well cached
//    private static List<Meta> GetPropertyDrawerTypes(Assembly assembly) {
//        if (customPropertyDrawerTypes != null) return customPropertyDrawerTypes;
//
//        customPropertyDrawerTypes = new List<Meta>();
//        BindingFlags bindFlags = BindingFlags.NonPublic | BindingFlags.Instance;
//
//        Type[] assemblyTypes = assembly.GetTypes();
//
//        for (int i = 0; i < assemblyTypes.Length; i++) {
//            object[] attributes = assemblyTypes[i].GetCustomAttributes(typeof(UnityEditor.CustomPropertyDrawer), true);
//            if (attributes.Length > 0) {
//                FieldInfo m_TypeFieldInfo = attributes[0].GetType().GetField("m_Type", bindFlags);
//                Type m_Type = m_TypeFieldInfo.GetValue(attributes[0]) as Type;
//                customPropertyDrawerTypes.Add(new Meta(assemblyTypes[i], m_Type));
//            }
//        }
//        return customPropertyDrawerTypes;
//    }
//
//    private static Dictionary<Type, Type> extendedDrawerTypeMap;
//
//    private static void BuildExtendedDrawerTypeMap() {
//        if (extendedDrawerTypeMap != null) return;
//        extendedDrawerTypeMap = new Dictionary<Type, Type>();
//        List<Type> extendedDrawerSubclasses = FindSubClasses<PropertyDrawerX>();
//        for (int i = 0; i < extendedDrawerSubclasses.Count; i++) {
//            Type drawerType = extendedDrawerSubclasses[i];
//            object[] attributes = drawerType.GetCustomAttributes(typeof(PropertyDrawerFor), false);
//            for (int j = 0; j < attributes.Length; j++) {
//                PropertyDrawerFor attr = attributes[j] as PropertyDrawerFor;
//                extendedDrawerTypeMap[attr.type] = drawerType;
//            }
//        }
//    }
//
//    public static Type GetExtendedDrawerTypeFor(Type type) {
//        BuildExtendedDrawerTypeMap();
//        Type drawerType = extendedDrawerTypeMap.Get(type);
//        while (drawerType == null && type.BaseType != null && type.BaseType != typeof(object)) {
//            type = type.BaseType;
//            drawerType = extendedDrawerTypeMap.Get(type);
//        }
//        return drawerType;
//    }
//    
//    private static Dictionary<SerializedPropertyX, PropertyDrawerX> drawerInstanceCache = new Dictionary<SerializedPropertyX, PropertyDrawerX>();
//    public static PropertyDrawerX GetCustomPropertyDrawerFor(SerializedPropertyX property) {
//        if (drawerCache == null) {
//            drawerCache = new Dictionary<Type, UnityEditor.PropertyDrawer>();
//        }
//
//        PropertyDrawerX drawerX = drawerInstanceCache.Get(property);
//        if (drawerX == null) {
//            var drawerType = GetExtendedDrawerTypeFor(property.Type);
//            if (drawerType == null) return null;
//            drawerX = Activator.CreateInstance(drawerType) as PropertyDrawerX;
//            drawerInstanceCache[property] = drawerX;
//        }
//        return drawerX;
//    }
//    
//    public static UnityEditor.PropertyDrawer GetCustomPropertyDrawerFor(Type type, params Assembly[] assemblies) {
//        if (drawerCache == null) {
//            drawerCache = new Dictionary<Type, UnityEditor.PropertyDrawer>();
//        }
//        UnityEditor.PropertyDrawer drawer;
//        if (drawerCache.TryGetValue(type, out drawer)) {
//            return drawer;
//        }
//
//        if (type == typeof(UnityEngine.RangeAttribute)) {
//
//            drawer = Activator.CreateInstance(typeof(UnityEditor.EditorGUI).Assembly.GetType("UnityEditor.RangeDrawer")) as UnityEditor.PropertyDrawer;
//            drawerCache[type] = drawer;
//            return drawer;
//        }
//
//        for (int a = 0; a < assemblies.Length; a++) {
//            List<Meta> metaList = GetPropertyDrawerTypes(assemblies[a]);
//            for (int i = 0; i < metaList.Count; i++) {
//                Meta drawerMeta = metaList[i];
//                Type attrArgument = drawerMeta.attributeArgumentType;
//                if (type == attrArgument || type.IsSubclassOf(attrArgument)) {
//                    drawer = Activator.CreateInstance(drawerMeta.attributeDrawerType) as UnityEditor.PropertyDrawer;
//                    drawerCache[type] = drawer;
//                    return drawer;
//                }
//            }
//        }
//        return null;
//    }
//#endif
    private static bool FilterAssembly(Assembly assembly) {
        return assembly.FullName.StartsWith("Assembly-CSharp");
        return assembly.ManifestModule.Name != "<In Memory Module>"
        && !assembly.FullName.StartsWith("System")
        && !assembly.FullName.StartsWith("Mono")
        && !assembly.FullName.StartsWith("Syntax")
        && !assembly.FullName.StartsWith("I18N")
        && !assembly.FullName.StartsWith("Boo")
        && !assembly.FullName.StartsWith("mscorlib")
        && !assembly.FullName.StartsWith("nunit")
        && !assembly.FullName.StartsWith("ICSharp")
        && !assembly.FullName.StartsWith("Unity")
        && !assembly.FullName.StartsWith("Microsoft")
        && assembly.Location.IndexOf("App_Web") == -1
        && assembly.Location.IndexOf("App_global") == -1
        && assembly.FullName.IndexOf("CppCodeProvider") == -1
        && assembly.FullName.IndexOf("WebMatrix") == -1
        && assembly.FullName.IndexOf("SMDiagnostics") == -1
        && assembly.FullName.IndexOf("UnityScript") == -1
        && !string.IsNullOrEmpty(assembly.Location);
    }
}


}