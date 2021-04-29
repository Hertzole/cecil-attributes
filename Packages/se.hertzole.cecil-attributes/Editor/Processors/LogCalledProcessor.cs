using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using UnityEngine;

namespace Hertzole.CecilAttributes.Editor
{
    public class LogCalledProcessor : BaseProcessor
    {
        public override string Name { get { return "LogCalled"; } }

        public override bool NeedsMonoBehaviour { get { return false; } }
        public override bool AllowEditor { get { return true; } }
        public override bool IncludeInBuild { get { return CecilAttributesSettings.Instance.IncludeLogsInBuild; } }

        public override bool IsValidClass(TypeDefinition type)
        {
            if (type.HasMethods)
            {
                for (int i = 0; i < type.Methods.Count; i++)
                {
                    if (type.Methods[i].HasAttribute<LogCalledAttribute>())
                    {
                        return true;
                    }
                }
            }

            if (type.HasProperties)
            {
                for (int i = 0; i < type.Properties.Count; i++)
                {
                    if (type.Properties[i].HasAttribute<LogCalledAttribute>())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override (bool success, bool dirty) ProcessClass(ModuleDefinition module, TypeDefinition type)
        {
            List<MethodDefinition> methods = new List<MethodDefinition>();
            List<PropertyDefinition> properties = new List<PropertyDefinition>();

            for (int i = 0; i < type.Methods.Count; i++)
            {
                if (type.Methods[i].HasAttribute<LogCalledAttribute>())
                {
                    methods.Add(type.Methods[i]);
                }
            }

            for (int i = 0; i < type.Properties.Count; i++)
            {
                if (type.Properties[i].HasAttribute<LogCalledAttribute>())
                {
                    properties.Add(type.Properties[i]);
                }
            }

            if (methods.Count > 0)
            {
                string defaultMethodFormat = CecilAttributesSettings.Instance.MethodLogFormat;
                string defaultParametersSeparator = CecilAttributesSettings.Instance.ParametersSeparator;
            
                for (int i = 0; i < methods.Count; i++)
                {
                    ProcessMethod(type, module, methods[i], defaultMethodFormat, defaultParametersSeparator);
                }
            }

            if (properties.Count > 0)
            {
                string defaultGetFormat = CecilAttributesSettings.Instance.PropertyGetLogFormat;
                string defaultSetFormat = CecilAttributesSettings.Instance.PropertySetLogFormat;

                for (int i = 0; i < properties.Count; i++)
                {
                    ProcessProperty(type, module, properties[i], defaultGetFormat, defaultSetFormat);
                }
            }

            return (true, true);
        }

        private static void ProcessMethod(TypeReference type, ModuleDefinition module, MethodDefinition method, string format, string parameterSeparator)
        {
            List<Instruction> instructions = new List<Instruction>();
            List<string> parameters = new List<string>();

            if (method.HasParameters)
            {
                int offset = 0;

                for (int i = 0; i < method.Parameters.Count; i++)
                {
                    if (method.Parameters[i].IsOut)
                    {
                        parameters.Add($"out {method.Parameters[i].Name}");
                        offset++;
                    }
                    else
                    {
                        parameters.Add($"{method.Parameters[i].Name}: {{{i - offset + type.GenericParameters.Count}}}");
                    }
                }
            }
            
            string message = format.FormatMessageLogCalled(type, method, parameterSeparator, parameters, null, false);

            instructions.Add(Instruction.Create(OpCodes.Ldstr, message));

            // Valid parameters are parameters than can show their value.
            int validParameters = type.GenericParameters.Count;

            for (int i = 0; i < method.Parameters.Count; i++)
            {
                if (method.Parameters[i].IsOut)
                {
                    continue;
                }

                validParameters++;
            }
            
            if (validParameters > 3)
            {
                instructions.Add(WeaverHelpers.GetIntInstruction(method.Parameters.Count));
                instructions.Add(Instruction.Create(OpCodes.Newarr, module.GetTypeReference<object>()));
            }

            for (int i = 0; i < type.GenericParameters.Count; i++)
            {
                instructions.Add(Instruction.Create(OpCodes.Ldtoken, type.GenericParameters[i].GetElementType()));
                instructions.Add(Instruction.Create(OpCodes.Call, module.GetMethod<Type>("GetTypeFromHandle", new Type[] { typeof(RuntimeTypeHandle) })));
            }
            
            if (method.HasParameters)
            {
                for (int i = 0; i < method.Parameters.Count; i++)
                {
                    if (method.Parameters[i].IsOut)
                    {
                        continue;
                    }

                    if (parameters.Count > 3)
                    {
                        // Too many parameters, need to create an array.
                        instructions.Add(Instruction.Create(OpCodes.Dup));
                        instructions.Add(WeaverHelpers.GetIntInstruction(i));
                    }
                    
                    instructions.Add(GetLoadParameter(i, method.Parameters[i], method.IsStatic));

                    if (!method.Parameters[i].ParameterType.Is<string>() && (method.Parameters[i].ParameterType.IsValueType || method.Parameters[i].ParameterType.IsByReference || method.Parameters[i].ParameterType.IsGenericParameter))
                    {
                        if (method.Parameters[i].ParameterType.IsByReference)
                        {
                            instructions.Add(Instruction.Create(OpCodes.Box, module.ImportReference(method.Parameters[i].ParameterType.Resolve())));
                        }
                        else
                        {
                            instructions.Add(Instruction.Create(OpCodes.Box, method.Parameters[i].ParameterType));
                        }
                    }

                    if (parameters.Count > 3)
                    {
                        instructions.Add(Instruction.Create(OpCodes.Stelem_Ref));
                    }
                }
            }
            
            if (validParameters > 0)
            {
                instructions.Add(Instruction.Create(OpCodes.Call, GetStringFormatMethod(module, validParameters)));
            }

            instructions.Add(Instruction.Create(OpCodes.Call, module.GetMethod<Debug>("Log", new Type[] { typeof(object) })));
            
            method.Body.GetILProcessor().InsertBefore(method.Body.Instructions[0], instructions);
        }

        private static void ProcessProperty(TypeDefinition type, ModuleDefinition module, PropertyDefinition property, string getFormat, string setFormat)
        {
            CustomAttribute attribute = property.GetAttribute<LogCalledAttribute>();
            bool logGet = attribute.GetField("logPropertyGet", true);
            bool logSet = attribute.GetField("logPropertySet", true);

            if (!logGet && !logSet)
            {
                return;
            }
            
            List<Instruction> instructions = new List<Instruction>();

            bool isStatic = property.IsStatic();
            
            FieldReference loadField = property.GetBackingField();
            if (logGet && property.GetMethod != null)
            {
                string message = getFormat.FormatMessageLogCalled(type, property.GetMethod, null, null, property, false);
                
                instructions.Add(Instruction.Create(OpCodes.Ldstr, message));
                if (!isStatic)
                {
                    instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                }
                
                instructions.Add(Instruction.Create(isStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, loadField));

                if (loadField.FieldType.IsValueType || loadField.FieldType.IsGenericParameter)
                {
                    instructions.Add(Instruction.Create(OpCodes.Box, module.ImportReference(loadField.FieldType)));
                }

                instructions.Add(Instruction.Create(OpCodes.Call, GetStringFormatMethod(module, 1)));
                instructions.Add(Instruction.Create(OpCodes.Call, module.ImportReference(typeof(Debug).GetMethod("Log", new Type[] { typeof(object) }))));
                
                property.GetMethod.Body.GetILProcessor().InsertBefore(property.GetMethod.Body.Instructions[0], instructions);
                instructions.Clear();
            }

            if (logSet && property.SetMethod != null)
            {
                VariableDefinition localVar = property.SetMethod.AddLocalVariable(module, loadField.FieldType, out int varIndex);
                string message = setFormat.FormatMessageLogCalled(type, property.SetMethod, null, null, property, true);
                if (!isStatic)
                {
                    instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                }

                instructions.Add(Instruction.Create(isStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, loadField));
                instructions.Add(GetStloc(varIndex, localVar));
                instructions.Add(Instruction.Create(OpCodes.Ldstr, message));
                instructions.Add(GetLdloc(varIndex, localVar));
                if (loadField.FieldType.IsValueType || loadField.FieldType.IsGenericParameter)
                {
                    instructions.Add(Instruction.Create(OpCodes.Box, module.ImportReference(loadField.FieldType)));
                }

                instructions.Add(Instruction.Create(isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1));

                if (loadField.FieldType.IsValueType || loadField.FieldType.IsGenericParameter)
                {
                    instructions.Add(Instruction.Create(OpCodes.Box, module.ImportReference(loadField.FieldType)));
                }

                instructions.Add(Instruction.Create(OpCodes.Call, GetStringFormatMethod(module, 2)));
                instructions.Add(Instruction.Create(OpCodes.Call, module.ImportReference(typeof(Debug).GetMethod("Log", new Type[] { typeof(object) }))));

                property.SetMethod.Body.GetILProcessor().InsertBefore(property.SetMethod.Body.Instructions[0], instructions);
            }
        }

        private static bool ProcessMethods(TypeDefinition type, ModuleDefinition module)
        {
            List<string> parameters = new List<string>();
            List<string> fancyParameters = new List<string>();
            List<Instruction> instructions = new List<Instruction>();

            bool dirty = false;

            if (type.HasMethods)
            {
                string methodFormat = CecilAttributesSettings.Instance.MethodLogFormat;
                string parametersSeparator = CecilAttributesSettings.Instance.ParametersSeparator;

                foreach (MethodDefinition method in type.Methods)
                {
                    if (!method.HasAttribute<LogCalledAttribute>())
                    {
                        continue;
                    }

                    instructions.Clear();

                    parameters.Clear();
                    fancyParameters.Clear();

                    if (method.HasParameters && method.Parameters.Count > 0)
                    {
                        int offset = 0;

                        for (int j = 0; j < method.Parameters.Count; j++)
                        {
                            if (method.Parameters[j].IsOut)
                            {
                                fancyParameters.Add("out " + method.Parameters[j].Name);
                                offset++;
                            }
                            else
                            {
                                string p = method.Parameters[j].Name + ": " + "{" + (j - offset) + "}";
                                parameters.Add(p);
                                fancyParameters.Add(p);
                            }
                        }
                    }

                    string message = methodFormat.FormatMessageLogCalled(type, method, parametersSeparator, fancyParameters, null, false);

                    ILProcessor il = method.Body.GetILProcessor();

                    instructions.Add(Instruction.Create(OpCodes.Ldstr, message));

                    if (parameters.Count > 0)
                    {
                        if (parameters.Count > 3)
                        {
                            instructions.Add(WeaverHelpers.GetIntInstruction(parameters.Count));
                            instructions.Add(Instruction.Create(OpCodes.Newarr, module.ImportReference(typeof(object))));
                        }

                        for (int i = 0; i < fancyParameters.Count; i++)
                        {
                            if (method.Parameters[i].IsOut)
                            {
                                continue;
                            }

                            if (parameters.Count > 3)
                            {
                                instructions.Add(Instruction.Create(OpCodes.Dup));
                                instructions.Add(WeaverHelpers.GetIntInstruction(i));
                            }

                            instructions.Add(GetLoadParameter(i, method.Parameters[i], method.IsStatic));

                            if (method.Parameters[i].ParameterType.IsByReference)
                            {
                                instructions.Add(GetLoadIn(method.Parameters[i].ParameterType, module));
                            }

                            if (method.Parameters[i].ParameterType.Resolve().FullName != "System.String" && (method.Parameters[i].ParameterType.IsValueType || method.Parameters[i].ParameterType.IsByReference))
                            {
                                if (method.Parameters[i].ParameterType.IsByReference)
                                {
                                    instructions.Add(Instruction.Create(OpCodes.Box, module.ImportReference(method.Parameters[i].ParameterType.Resolve())));
                                }
                                else
                                {
                                    instructions.Add(Instruction.Create(OpCodes.Box, method.Parameters[i].ParameterType));
                                }
                            }

                            if (parameters.Count > 3)
                            {
                                instructions.Add(Instruction.Create(OpCodes.Stelem_Ref));
                            }
                        }

                        instructions.Add(Instruction.Create(OpCodes.Call, GetStringFormatMethod(module, parameters.Count)));
                    }

                    instructions.Add(Instruction.Create(OpCodes.Call, module.ImportReference(typeof(Debug).GetMethod("Log", new Type[] { typeof(object) }))));

                    for (int j = 0; j < instructions.Count; j++)
                    {
                        il.InsertBefore(il.Body.Instructions[j], instructions[j]);
                    }

                    il.Body.Optimize();
                    dirty = true;
                }
            }

            return dirty;
        }

        private static bool ProcessProperties(TypeDefinition type, ModuleDefinition module)
        {
            bool dirty = false;

            List<Instruction> instructions = new List<Instruction>();

            if (type.HasProperties)
            {
                string propertyGetFormat = CecilAttributesSettings.Instance.PropertyGetLogFormat;
                string propertySetFormat = CecilAttributesSettings.Instance.PropertySetLogFormat;

                foreach (PropertyDefinition property in type.Properties)
                {
                    if (!property.HasAttribute<LogCalledAttribute>())
                    {
                        continue;
                    }

                    CustomAttribute attribute = property.GetAttribute<LogCalledAttribute>();
                    bool logGet = attribute.GetConstructorArgument(0, true);
                    bool logSet = attribute.GetConstructorArgument(1, true);

                    if (!logGet && !logSet)
                    {
                        continue;
                    }

                    bool isStatic = property.IsStatic();
                    if (logGet && property.GetMethod != null)
                    {
                        instructions.Clear();

                        // FieldDefinition loadField = property.GetBackingField();

                        string message = propertyGetFormat.FormatMessageLogCalled(type, property.GetMethod, null, null, property, false);


                        instructions.Add(Instruction.Create(OpCodes.Ldstr, message));
                        if (!isStatic)
                        {
                            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                        }

                        // instructions.Add(Instruction.Create(isStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, loadField));
                        //
                        // if (loadField.FieldType.IsValueType)
                        // {
                        //     instructions.Add(Instruction.Create(OpCodes.Box, module.ImportReference(loadField.FieldType)));
                        // }

                        instructions.Add(Instruction.Create(OpCodes.Call, GetStringFormatMethod(module, 1)));
                        instructions.Add(Instruction.Create(OpCodes.Call, module.ImportReference(typeof(Debug).GetMethod("Log", new Type[] { typeof(object) }))));

                        ILProcessor il = property.GetMethod.Body.GetILProcessor();

                        for (int i = 0; i < instructions.Count; i++)
                        {
                            il.InsertBefore(il.Body.Instructions[i], instructions[i]);
                        }

                        il.Body.Optimize();
                        dirty = true;
                    }

                    if (logSet && property.SetMethod != null)
                    {
                        instructions.Clear();

                        var loadField = property.GetBackingField();

                        property.SetMethod.Body.Variables.Add(new VariableDefinition(module.ImportReference(loadField.FieldType)));

                        string message = propertySetFormat.FormatMessageLogCalled(type, property.SetMethod, null, null, property, true);

                        if (!isStatic)
                        {
                            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                        }

                        instructions.Add(Instruction.Create(isStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, loadField));
                        instructions.Add(Instruction.Create(OpCodes.Stloc_0));
                        instructions.Add(Instruction.Create(OpCodes.Ldstr, message));
                        instructions.Add(Instruction.Create(OpCodes.Ldloc_0));
                        if (loadField.FieldType.IsValueType)
                        {
                            instructions.Add(Instruction.Create(OpCodes.Box, module.ImportReference(loadField.FieldType)));
                        }
                        if (isStatic)
                        {
                            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                        }
                        else
                        {
                            instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
                        }

                        if (loadField.FieldType.IsValueType)
                        {
                            instructions.Add(Instruction.Create(OpCodes.Box, module.ImportReference(loadField.FieldType)));
                        }
                        instructions.Add(Instruction.Create(OpCodes.Call, GetStringFormatMethod(module, 2)));
                        instructions.Add(Instruction.Create(OpCodes.Call, module.ImportReference(typeof(Debug).GetMethod("Log", new Type[] { typeof(object) }))));

                        ILProcessor il = property.SetMethod.Body.GetILProcessor();

                        for (int i = 0; i < instructions.Count; i++)
                        {
                            il.InsertBefore(il.Body.Instructions[i], instructions[i]);
                        }

                        il.Body.Optimize();
                        dirty = true;
                    }
                }
            }

            return dirty;
        }

        public static MethodReference GetStringFormatMethod(ModuleDefinition module, int amount)
        {
            switch (amount)
            {
                case 1:
                    return module.ImportReference(typeof(string).GetMethod("Format", new Type[] { typeof(string), typeof(object) }));
                case 2:
                    return module.ImportReference(typeof(string).GetMethod("Format", new Type[] { typeof(string), typeof(object), typeof(object) }));
                case 3:
                    return module.ImportReference(typeof(string).GetMethod("Format", new Type[] { typeof(string), typeof(object), typeof(object), typeof(object) }));
                default:
                    return module.ImportReference(typeof(string).GetMethod("Format", new Type[] { typeof(string), typeof(object[]) }));

            }
        }
        
        public static Instruction GetStloc(int index, VariableDefinition variable)
        {
            switch (index)
            {
                case 0:
                    return Instruction.Create(OpCodes.Stloc_0);
                case 1:
                    return Instruction.Create(OpCodes.Stloc_1);
                case 2:
                    return Instruction.Create(OpCodes.Stloc_2);
                case 3:
                    return Instruction.Create(OpCodes.Stloc_3);
                default:
                    return Instruction.Create(OpCodes.Stloc_S, variable);
            }
        }

        public static Instruction GetLdloc(int index, VariableDefinition variable, bool ldloc_a = false)
        {
            if (ldloc_a)
            {
                return Instruction.Create(OpCodes.Ldloca_S, variable);
            }

            switch (index)
            {
                case 0:
                    return Instruction.Create(OpCodes.Ldloc_0);
                case 1:
                    return Instruction.Create(OpCodes.Ldloc_1);
                case 2:
                    return Instruction.Create(OpCodes.Ldloc_2);
                case 3:
                    return Instruction.Create(OpCodes.Ldloc_3);
                default:
                    return Instruction.Create(OpCodes.Ldloc_S, variable);
            }
        }

        private static Instruction GetLoadParameter(int index, ParameterDefinition parameter, bool isStatic)
        {
            switch (index)
            {
                case 0:
                    return isStatic ? Instruction.Create(OpCodes.Ldarg_0) : Instruction.Create(OpCodes.Ldarg_1);
                case 1:
                    return isStatic ? Instruction.Create(OpCodes.Ldarg_1) : Instruction.Create(OpCodes.Ldarg_2);
                case 2:
                    return isStatic ? Instruction.Create(OpCodes.Ldarg_2) : Instruction.Create(OpCodes.Ldarg_3);
                case 3:
                    return isStatic ? Instruction.Create(OpCodes.Ldarg_3) : Instruction.Create(OpCodes.Ldarg_S, parameter);
                default:
                    return Instruction.Create(OpCodes.Ldarg_S, parameter);
            }
        }

        private static Instruction GetLoadIn(TypeReference parameterType, ModuleDefinition module)
        {
            TypeDefinition type = parameterType.Resolve();

            if (!type.IsPrimitive && !type.IsValueType && type.IsClass && type.IsAutoLayout)
            {
                return Instruction.Create(OpCodes.Ldind_Ref);
            }
            else if (!type.IsPrimitive && type.IsValueType && !type.IsEnum)
            {
                return Instruction.Create(OpCodes.Ldobj, module.ImportReference(parameterType.Resolve()));
            }
            else
            {
                return Instruction.Create(OpCodes.Ldind_I4);
            }
        }
    }
}
