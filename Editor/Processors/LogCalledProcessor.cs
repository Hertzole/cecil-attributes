using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
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
            bool dirty = false;

            if (ProcessMethods(type, module))
            {
                dirty = true;
            }

            if (ProcessProperties(type, module))
            {
                dirty = true;
            }

            return (true, dirty);
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

                    if (logGet && property.GetMethod != null)
                    {
                        instructions.Clear();

                        FieldDefinition loadField = property.GetBackingField();

                        string message = propertyGetFormat.FormatMessageLogCalled(type, property.GetMethod, null, null, property, false);

                        instructions.Add(Instruction.Create(OpCodes.Ldstr, message));
                        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                        instructions.Add(Instruction.Create(OpCodes.Ldfld, loadField));

                        if (loadField.FieldType.IsValueType)
                        {
                            instructions.Add(Instruction.Create(OpCodes.Box, module.ImportReference(loadField.FieldType)));
                        }

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

                        FieldDefinition loadField = property.GetBackingField();

                        property.SetMethod.Body.Variables.Add(new VariableDefinition(module.ImportReference(loadField.FieldType)));

                        string message = propertySetFormat.FormatMessageLogCalled(type, property.SetMethod, null, null, property, true);

                        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                        instructions.Add(Instruction.Create(OpCodes.Ldfld, loadField));
                        instructions.Add(Instruction.Create(OpCodes.Stloc_0));
                        instructions.Add(Instruction.Create(OpCodes.Ldstr, message));
                        instructions.Add(Instruction.Create(OpCodes.Ldloc_0));
                        if (loadField.FieldType.IsValueType)
                        {
                            instructions.Add(Instruction.Create(OpCodes.Box, module.ImportReference(loadField.FieldType)));
                        }
                        instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
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
