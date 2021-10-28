using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

namespace Hertzole.CecilAttributes.CodeGen
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

        public static bool TryGetMethodInBaseType(this TypeDefinition type, string methodName, out MethodDefinition method, params TypeReference[] parameterTypes)
        {
            TypeDefinition typedef = type;
            method = null;
            while (typedef != null)
            {
                for (int i = 0; i < typedef.Methods.Count; i++)
                {
                    if (typedef.Methods[i].Name == methodName && typedef.Methods[i].Parameters.Count == parameterTypes.Length)
                    {
                        bool validParamaters = true;

                        for (int j = 0; j < parameterTypes.Length; j++)
                        {
                            if (typedef.Methods[i].Parameters[j].ParameterType != parameterTypes[j])
                            {
                                validParamaters = false;
                                break;
                            }
                        }

                        if (validParamaters)
                        {
                            method = typedef.Methods[i];
                            return true;
                        }
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

        public static GenericInstanceMethod MakeGenericMethod(this MethodReference self, params TypeReference[] genericTypes)
        {
            GenericInstanceMethod result = new GenericInstanceMethod(self);
            foreach (TypeReference argument in genericTypes)
            {
                result.GenericArguments.Add(argument);
            }

            return result;
        }
        
        public static VariableDefinition AddLocalVariable<T>(this MethodDefinition m, out int index)
        {
            if (m.Module == null)
            {
                Debug.LogError("This method has yet to be added to the assembly and doesn't have a module. Please provide a module.");
                index = 0;
                return null;
            }

            return m.AddLocalVariable(m.Module, m.Module.ImportReference(typeof(T)), out index);
        }

        public static VariableDefinition AddLocalVariable(this MethodDefinition m, TypeReference type, out int index)
        {
            if (m.Module == null)
            {
                Debug.LogError("This method has yet to be added to the assembly and doesn't have a module. Please provide a module.");
                index = 0;
                return null;
            }

            return m.AddLocalVariable(m.Module, type, out index);
        }

        public static VariableDefinition AddLocalVariable<T>(this MethodDefinition m, ModuleDefinition module, out int index)
        {
            return m.AddLocalVariable(module, module.ImportReference(typeof(T)), out index);
        }

        public static VariableDefinition AddLocalVariable(this MethodDefinition m, ModuleDefinition module, TypeReference type, out int index)
        {
            index = m.Body.Variables.Count;

            VariableDefinition variable = new VariableDefinition(module.ImportReference(type));
            m.Body.Variables.Add(variable);
            
            return variable;
        }
    }
}
