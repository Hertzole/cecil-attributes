using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

namespace Hertzole.CecilAttributes.CodeGen
{
    public class ResetStaticProcessor : BaseProcessor
    {
        private List<MemberData> fields = new List<MemberData>();

        private readonly string methodName = "CecilAttributesGenerated_ResetStatics";

        public override string Name { get { return "ResetStatic"; } }

        public override bool NeedsMonoBehaviour { get { return false; } }
        public override bool AllowEditor { get { return true; } }
        public override bool IncludeInBuild { get { return Settings.includeResetStaticInBuild; } }

        public override bool IsValidType()
        {
            if (Type.HasAttribute<ResetStaticAttribute>())
            {
                return true;
            }

            if (Type.HasFields)
            {
                foreach (FieldDefinition field in Type.Fields)
                {
                    if (field.HasAttribute<ResetStaticAttribute>())
                    {
                        return true;
                    }
                }
            }

            if (Type.HasProperties)
            {
                foreach (PropertyDefinition property in Type.Properties)
                {
                    if (property.HasAttribute<ResetStaticAttribute>())
                    {
                        return true;
                    }
                }
            }

            if (Type.HasEvents)
            {
                foreach (EventDefinition e in Type.Events)
                {
                    if (e.HasAttribute<ResetStaticAttribute>())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override void ProcessType()
        {
            fields.Clear();

            bool hasAttribute = Type.HasAttribute<ResetStaticAttribute>();

            if (Type.HasFields)
            {
                for (int i = 0; i < Type.Fields.Count; i++)
                {
                    if (hasAttribute && !Type.Fields[i].IsStatic)
                    {
                        continue;
                    }

                    if (hasAttribute && Type.Fields[i].HasAttribute<CompilerGeneratedAttribute>())
                    {
                        continue;
                    }

                    if (!hasAttribute && !Type.Fields[i].HasAttribute<ResetStaticAttribute>())
                    {
                        continue;
                    }

                    // No need to check for attribute since we just did above.
                    if (!hasAttribute && !Type.Fields[i].IsStatic)
                    {
                        Error($"{Type.Fields[i].FullName} isn't static. ResetStatic can only be on static fields.");
                        return;
                    }

                    fields.Add(new MemberData(Type.Fields[i]));
                }
            }

            if (Type.HasProperties)
            {
                for (int i = 0; i < Type.Properties.Count; i++)
                {
                    if (hasAttribute && !Type.Properties[i].GetMethod.IsStatic)
                    {
                        continue;
                    }

                    if (hasAttribute && Type.Properties[i].HasAttribute<CompilerGeneratedAttribute>())
                    {
                        continue;
                    }

                    if (!hasAttribute && !Type.Properties[i].HasAttribute<ResetStaticAttribute>())
                    {
                        continue;
                    }

                    // No need to check for attribute since we just did above.
                    if (!hasAttribute && ((Type.Properties[i].GetMethod != null && !Type.Properties[i].GetMethod.IsStatic) || (Type.Properties[i].SetMethod != null && !Type.Properties[i].SetMethod.IsStatic)))
                    {
                        //throw new NotSupportedException(Type.Properties[i].FullName + " isn't static. ResetStatic can only be on static properties.");
                        Error($"{Type.Properties[i].FullName} isn't static. ResetStatic can only be on static properties.");
                        return;
                    }

                    if (Type.Properties[i].SetMethod == null)
                    {
                        Error($"{Type.Properties[i].FullName} doesn't have a set method. You need to be able to set a property in order to reset it.");
                        return;
                    }

                    fields.Add(new MemberData(Type.Properties[i]));
                }
            }

            if (Type.HasEvents)
            {
                for (int i = 0; i < Type.Events.Count; i++)
                {
                    if (hasAttribute && !Type.Events[i].AddMethod.IsStatic)
                    {
                        continue;
                    }

                    if (hasAttribute && Type.Events[i].HasAttribute<CompilerGeneratedAttribute>())
                    {
                        continue;
                    }

                    if (!hasAttribute && !Type.Events[i].HasAttribute<ResetStaticAttribute>())
                    {
                        continue;
                    }

                    // No need to check for attribute since we just did above.
                    if (!hasAttribute && !Type.Events[i].AddMethod.IsStatic)
                    {
                        Error($"{Type.Events[i].FullName} isn't static. ResetStatic can only be on static events.");
                        return;
                    }

                    fields.Add(new MemberData(Type.Events[i]));
                }
            }

            if (!Type.TryGetMethod(methodName, out MethodDefinition resetMethod))
            {
                resetMethod = new MethodDefinition(methodName,
                    MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Static,
                    Module.ImportReference(typeof(void)));

                Type.Methods.Add(resetMethod);
            }

            if (!resetMethod.HasAttribute<RuntimeInitializeOnLoadMethodAttribute>())
            {
                MethodReference initAttributeCtor = Module.ImportReference(typeof(RuntimeInitializeOnLoadMethodAttribute).GetConstructor(new Type[] { typeof(RuntimeInitializeLoadType) }));
                CustomAttribute attribute = new CustomAttribute(initAttributeCtor);
                attribute.ConstructorArguments.Add(new CustomAttributeArgument(Module.ImportReference(typeof(RuntimeInitializeLoadType)), Settings.resetStaticMode));
                resetMethod.CustomAttributes.Add(attribute);
            }

            ILProcessor il = resetMethod.Body.GetILProcessor();

            int propertyIndex = 0;

            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i].IsGenericParameter)
                {
                    Error(fields[i].Name + " is a generic. You can't reset a static generic.");
                    return;
                }

                if (!fields[i].IsEvent)
                {
                    (List<Instruction> instructions, bool valueType) = GetStaticSet(Type, fields[i].IsProperty ? fields[i].property.GetBackingField() : fields[i].field);

                    if (instructions != null)
                    {
                        for (int j = instructions.Count - 1; j >= 0; j--)
                        {
                            il.Append(instructions[j]);
                        }
                    }

                    if (!valueType)
                    {
                        if (!fields[i].IsProperty)
                        {
                            il.Emit(OpCodes.Stsfld, fields[i].field);
                        }
                        else
                        {
                            il.Emit(OpCodes.Call, fields[i].property.SetMethod);
                        }
                    }
                    else
                    {
                        if (!fields[i].IsProperty)
                        {
                            il.Emit(OpCodes.Ldsflda, fields[i].field);
                            il.Emit(OpCodes.Initobj, fields[i].ResolvedType);
                        }
                        else
                        {
                            VariableDefinition varDef = new VariableDefinition(fields[i].ResolvedType);
                            resetMethod.Body.Variables.Add(varDef);
                            il.Emit(OpCodes.Ldloca_S, varDef);
                            il.Emit(OpCodes.Initobj, fields[i].ResolvedType);
                            il.Append(GetLocation(propertyIndex, varDef));
                            il.Emit(OpCodes.Call, fields[i].property.SetMethod);
                            propertyIndex++;
                        }
                    }
                }
                else
                {
                    il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Stsfld, Type.GetField(fields[i].eventDef.Name));
                }
            }

            il.Emit(OpCodes.Ret);
        }

        private static Instruction GetLocation(int index, VariableDefinition varDef)
        {
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
                    return Instruction.Create(OpCodes.Ldloc_S, varDef);
            }
        }

        private static (List<Instruction>, bool) GetStaticSet(TypeDefinition classType, FieldReference field)
        {
            List<Instruction> instructions = new List<Instruction>();

            if (classType.TryGetMethod(".cctor", out MethodDefinition cctor))
            {
                Instruction root = null;

                foreach (Instruction i in cctor.Body.Instructions)
                {
                    if (i.OpCode == OpCodes.Stsfld && i.Operand == field)
                    {
                        root = i;
                        break;
                    }
                }

                if (root == null)
                {
                    TypeDefinition type = field.FieldType.Resolve();

                    if (type.Is<bool>() || type.Is<int>() || type.Is<uint>() || type.Is<short>() || type.Is<ushort>() || type.Is<byte>() || type.Is<sbyte>())
                    {
                        instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                    }
                    else if (type.Is<long>() || type.Is<ulong>())
                    {
                        instructions.Add(Instruction.Create(OpCodes.Conv_I8));
                        instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));
                    }
                    else if (type.Is<float>())
                    {
                        instructions.Add(Instruction.Create(OpCodes.Ldc_R4, 0.0f));
                    }
                    else if (type.Is<double>())
                    {
                        instructions.Add(Instruction.Create(OpCodes.Ldc_R8, 0.0d));
                    }
                    else if (type.IsValueType)
                    {
                        // Handle value types differently.
                        return (null, true);
                    }
                    else
                    {
                        instructions.Add(Instruction.Create(OpCodes.Ldnull));
                    }

                    if (instructions.Count > 0)
                    {
                        return (instructions, false);
                    }
                    else
                    {
                        throw new NullReferenceException($"There's nothing that sets field {field.Name}. Did you set a default value?");
                    }
                }

                while (root.Previous != null && root.Previous.OpCode != OpCodes.Stsfld)
                {
                    instructions.Add(root.Previous);
                    root = root.Previous;
                }

                return (instructions, false);
            }
            else if (field.FieldType.Resolve().IsClass)
            {
                instructions.AddRange(ILHelper.GetStandardValue(field.FieldType));
                return (instructions, false);
            }

            throw new NullReferenceException();
        }
    }
}
