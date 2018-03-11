using SpaceGame.Util;

namespace SpaceGame.Reflection {
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
        var retn = new List<Type>(4);
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
        var retn = new List<Type>(4);
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
                return CreateDelegate(method);
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

    private static bool FilterAssembly(Assembly assembly) {
        return assembly.FullName.StartsWith("Assembly-CSharp");
    }
}


}