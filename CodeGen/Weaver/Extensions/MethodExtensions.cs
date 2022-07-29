using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using UnityEngine;

namespace Hertzole.CecilAttributes.CodeGen
{
    public static partial class WeaverExtensions
    {
        private static readonly Dictionary<MethodDefinition, List<VariableDefinitionInfo>> editingMethods = new Dictionary<MethodDefinition, List<VariableDefinitionInfo>>();
        
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
        
        public static VariableDefinition AddLocalVariable<T>(this MethodDefinition m, string name = "")
        {
            if (m.Module == null)
            {
                throw new NullReferenceException($"This method has yet to be added to the assembly and doesn't have a module. Please provide a module.");
            }

            return m.AddLocalVariable(m.Module, m.Module.ImportReference(typeof(T)), name);
        }

        public static VariableDefinition AddLocalVariable(this MethodDefinition m, TypeReference type, string name = "")
        {
            if (m.Module == null)
            {
                throw new NullReferenceException($"This method has yet to be added to the assembly and doesn't have a module. Please provide a module.");
            }

            return m.AddLocalVariable(m.Module, type, name);
        }

        public static VariableDefinition AddLocalVariable<T>(this MethodDefinition m, ModuleDefinition module, string name = "")
        {
            return m.AddLocalVariable(module, module.ImportReference(typeof(T)), name);
        }

        public static VariableDefinition AddLocalVariable(this MethodDefinition m, ModuleDefinition module, TypeReference type, string name = "")
        {
            VariableDefinition variable = new VariableDefinition(module.ImportReference(type));
            m.Body.Variables.Add(variable);

            if (!string.IsNullOrEmpty(name) && editingMethods.TryGetValue(m, out List<VariableDefinitionInfo> list))
            {
                list.Add(new VariableDefinitionInfo(variable, name));
            }

            return variable;
        }

        public static VariableDefinition AddLocalVariable(this MethodDefinition m, ModuleDefinition module, TypeReference type, out int index)
        {
            index = m.Body.Variables.Count;

            VariableDefinition variable = new VariableDefinition(module.ImportReference(type));
            m.Body.Variables.Add(variable);
            
            return variable;
        }
        
        public static ParameterDefinition AddParameter<T>(this MethodDefinition m, string name = null)
        {
            if (m.Module == null)
            {
                throw new NullReferenceException($"This method has yet to be added to the assembly and doesn't have a module. Please provide a module.");
            }

            return m.AddParameter<T>(m.Module, name);
        }

        public static ParameterDefinition AddParameter<T>(this MethodDefinition m, ModuleDefinition module, string name = null)
        {
            return m.AddParameter(module, module.ImportReference(typeof(T)), name);
        }

        public static ParameterDefinition AddParameter(this MethodDefinition m, Type type, string name = null)
        {
            return AddParameter(m, m.Module.ImportReference(type), name);
        }
        
        public static ParameterDefinition AddParameter(this MethodDefinition m, TypeReference type, string name = null)
        {
            if (m.Module == null)
            {
                throw new NullReferenceException($"This method has yet to be added to the assembly and doesn't have a module. Please provide a module.");
            }

            return m.AddParameter(m.Module, type, name);
        }

        public static ParameterDefinition AddParameter(this MethodDefinition m, ModuleDefinition module, TypeReference type, string name = null)
        {
            ParameterDefinition parameter = new ParameterDefinition(module.ImportReference(type));
            m.Parameters.Add(parameter);

            if (!string.IsNullOrEmpty(name))
            {
                parameter.Name = name;
            }

            return parameter;
        }
        
        public static ILProcessor BeginEdit(this MethodDefinition method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method), "Can't begin edit because the method is null.");
            }
            
            editingMethods.Add(method, new List<VariableDefinitionInfo>());
            
            return method.Body.GetILProcessor();
        }
        
        public static void EndEdit(this MethodDefinition method)
        {
            if (!editingMethods.TryGetValue(method, out List<VariableDefinitionInfo> varList))
            {
                method.Body.Optimize();
                return;
            }
            
            if (method.DebugInformation.Scope == null)
            {
                ScopeDebugInformation scope = new ScopeDebugInformation(method.Body.Instructions[0], method.Body.Instructions[method.Body.Instructions.Count - 1]);
                method.DebugInformation.Scope = scope;
            }

            for (int i = 0; i < varList.Count; i++)
            {
                method.DebugInformation.Scope.Variables.Add(new VariableDebugInformation(varList[i].variableDefinition, varList[i].name));
            }
            
            method.Body.Optimize();
            editingMethods.Remove(method);
        }
        
        // Shamefully stolen from https://github.com/MirrorNG/MirrorNG/blob/master/Assets/Mirror/Weaver/Extensions.cs#L159
        /// <summary>
        /// Given a method of a generic class such as ArraySegment`T.get_Count,
        /// and a generic instance such as ArraySegment`int
        /// Creates a reference to the specialized method  ArraySegment`int`.get_Count
        /// <para> Note that calling ArraySegment`T.get_Count directly gives an invalid IL error </para>
        /// </summary>
        /// <param name="self"></param>
        /// <param name="instanceType"></param>
        /// <returns></returns>
        public static MethodReference MakeHostInstanceGeneric(this MethodReference self, GenericInstanceType instanceType)
        {
            MethodReference reference = new MethodReference(self.Name, self.ReturnType, instanceType)
            {
                CallingConvention = self.CallingConvention,
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis
            };

            foreach (ParameterDefinition parameter in self.Parameters)
            {
                reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
            }

            foreach (GenericParameter p in self.GenericParameters)
            {
                reference.GenericParameters.Add(new GenericParameter(p.Name, reference));
            }

            return self.Module.ImportReference(reference);
        }
        
        private readonly struct VariableDefinitionInfo
        {
            public readonly VariableDefinition variableDefinition;
            public readonly string name;

            public VariableDefinitionInfo(VariableDefinition variableDefinition, string name)
            {
                this.variableDefinition = variableDefinition;
                this.name = name;
            }
        }
    }
}
