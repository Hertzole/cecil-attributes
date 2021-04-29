using Mono.Cecil;
using System;

namespace Hertzole.CecilAttributes.Editor
{
    public static partial class WeaverExtensions
    {
        public static MethodDefinition GetMethodInBaseType(this TypeDefinition type, string method)
        {
            TypeDefinition typedef = type;
            while (typedef != null)
            {
                for (int i = 0; i < typedef.Methods.Count; i++)
                {
                    if (typedef.Methods[i].Name == method)
                    {
                        return typedef.Methods[i];
                    }
                }

                try
                {
                    TypeReference parent = typedef.BaseType;
                    typedef = parent?.Resolve();
                }
                catch (AssemblyResolutionException)
                {
                    break;
                }
            }

            throw new ArgumentException($"There's no method called {method} in {type.FullName} or its base types.");
        }

        public static bool TryGetMethodInBaseType(this TypeDefinition type, string methodName, out MethodDefinition method)
        {
            TypeDefinition typedef = type;
            method = null;
            while (typedef != null)
            {
                for (int i = 0; i < typedef.Methods.Count; i++)
                {
                    if (typedef.Methods[i].Name == methodName)
                    {
                        method = typedef.Methods[i];
                        return true;
                    }
                }

                try
                {
                    TypeReference parent = typedef.BaseType;
                    typedef = parent?.Resolve();
                }
                catch (AssemblyResolutionException)
                {
                    break;
                }
            }

            return false;
        }

        public static MethodDefinition GetMethod(this TypeDefinition type, string methodName)
        {
            if (!type.HasMethods)
            {
                throw new NullReferenceException("There are no methods in type " + type.FullName + ".");
            }

            for (int i = 0; i < type.Methods.Count; i++)
            {
                if (type.Methods[i].Name == methodName)
                {
                    return type.Methods[i];
                }
            }

            throw new ArgumentException("There's no method with the name " + methodName + " in " + type.FullName + ".", nameof(methodName));
        }

        public static bool TryGetMethod(this TypeDefinition type, string methodName, out MethodDefinition method)
        {
            method = null;
            if (!type.HasMethods)
            {
                return false;
            }

            for (int i = 0; i < type.Methods.Count; i++)
            {
                if (type.Methods[i].Name == methodName)
                {
                    method = type.Methods[i];
                    return true;
                }
            }

            return false;
        }

        public static T GetConstructorArgument<T>(this CustomAttribute attribute, int index, T defaultValue)
        {
            if (!attribute.HasConstructorArguments)
            {
                return defaultValue;
            }

            return (T)attribute.ConstructorArguments[index].Value;
        }
    }
}
