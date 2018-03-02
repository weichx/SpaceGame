using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SpaceGame.Reflection {

    public class MethodPointerUtils {
        
        private static readonly StringBuilder builder = new StringBuilder();
        private static readonly List<string> arguementList = new List<string>();
        
        public static string CreateSignature(MethodInfo info) {
            if (info == null) throw new Exception("Method info is null");
            if (info.DeclaringType != null) {
                string type = info.DeclaringType.Name;
                string method = info.Name;
                string retnType = info.ReturnType.Name;
                ParameterInfo[] parameters = info.GetParameters();
                arguementList.Clear();
                for (int i = 0; i < parameters.Length; i++) {
                    arguementList[i] = parameters[i].ParameterType.Name;
                }
                return CreateSignature(type, method, retnType, arguementList);
            }

            return null;
        }

        public static string CreateSignature(string type, string method, string retnType, List<string> parameters) {
            builder.Clear();
            builder.Append(FilterFloatTypeName(retnType));
            builder.Append(" ");
            builder.Append(type);
            builder.Append(".");
            builder.Append(method);
            builder.Append("(");
            if (parameters != null) {
                for (int i = 0; i < parameters.Count - 1; i++) {
                    builder.Append(FilterFloatTypeName(parameters[i]));
                    builder.Append(", ");
                 
                }

                builder.Append(FilterFloatTypeName(parameters[parameters.Count - 1]));
                builder.Append(")");
            }
            return builder.ToString();
        }

        public static string FilterFloatTypeName(string parameterType) {
            return parameterType == "Single" ? "float" : parameterType;
        }

    }

}