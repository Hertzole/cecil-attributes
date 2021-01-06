using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Hertzole.CecilAttributes.Editor
{
    public class ResetStaticProcessor : BaseProcessor
    {
        private List<MemberData> fields = new List<MemberData>();

        private readonly string methodName = "CecilAttributesGenerated_ResetStatics";

        public override string Name { get { return "ResetStatic"; } }

        public override bool NeedsMonoBehaviour { get { return false; } }
        public override bool AllowEditor { get { return true; } }
        public override bool IncludeInBuild { get { return CecilAttributesSettings.Instance.IncludeResetStaticInBuild; } }

        public override bool IsValidClass(TypeDefinition type)
        {
            if (type.HasAttribute<ResetStaticAttribute>())
            {
                return true;
            }

            if (type.HasFields)
            {
                foreach (FieldDefinition field in type.Fields)
                {
                    if (field.HasAttribute<ResetStaticAttribute>())
                    {
                        return true;
                    }
                }
            }

            if (type.HasProperties)
            {
                foreach (PropertyDefinition property in type.Properties)
                {
                    if (property.HasAttribute<ResetStaticAttribute>())
                    {
                        return true;
                    }
                }
            }

            if (type.HasEvents)
            {
                foreach (EventDefinition e in type.Events)
                {
                    if (e.HasAttribute<ResetStaticAttribute>())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override (bool success, bool dirty) ProcessClass(ModuleDefinition module, TypeDefinition type)
        {
            fields.Clear();

            bool hasAttribute = type.HasAttribute<ResetStaticAttribute>();

            if (type.HasFields)
            {
                for (int i = 0; i < type.Fields.Count; i++)
                {
                    if (hasAttribute && !type.Fields[i].IsStatic)
                    {
                        continue;
                    }

                    if (hasAttribute && type.Fields[i].HasAttribute<CompilerGeneratedAttribute>())
                    {
                        continue;
                    }

                    if (!hasAttribute && !type.Fields[i].HasAttribute<ResetStaticAttribute>())
                    {
                        continue;
                    }

                    // No need to check for attribute since we just did above.
                    if (!hasAttribute && !type.Fields[i].IsStatic)
                    {
                        Debug.LogError($"{type.Fields[i].FullName} isn't static. ResetStatic can only be on static fields.");
                        return (false, false);
                    }

                    fields.Add(new MemberData(type.Fields[i]));
                }
            }

            if (type.HasProperties)
            {
                for (int i = 0; i < type.Properties.Count; i++)
                {
                    if (hasAttribute && !type.Properties[i].GetMethod.IsStatic)
                    {
                        continue;
                    }

                    if (hasAttribute && type.Properties[i].HasAttribute<CompilerGeneratedAttribute>())
                    {
                        continue;
                    }

                    if (!hasAttribute && !type.Properties[i].HasAttribute<ResetStaticAttribute>())
                    {
                        continue;
                    }

                    // No need to check for attribute since we just did above.
                    if (!hasAttribute && ((type.Properties[i].GetMethod != null && !type.Properties[i].GetMethod.IsStatic) || (type.Properties[i].SetMethod != null && !type.Properties[i].SetMethod.IsStatic)))
                    {
                        //throw new NotSupportedException(type.Properties[i].FullName + " isn't static. ResetStatic can only be on static properties.");
                        Debug.LogError($"{type.Properties[i].FullName} isn't static. ResetStatic can only be on static properties.");
                        return (false, false);
                    }

                    if (type.Properties[i].SetMethod == null)
                    {
                        Debug.LogError($"{type.Properties[i].FullName} doesn't have a set method. You need to be able to set a property in order to reset it.");
                        return (false, false);
                    }

                    fields.Add(new MemberData(type.Properties[i]));
                }
            }

            if (type.HasEvents)
            {
                for (int i = 0; i < type.Events.Count; i++)
                {
                    if (hasAttribute && !type.Events[i].AddMethod.IsStatic)
                    {
                        continue;
                    }

                    if (hasAttribute && type.Events[i].HasAttribute<CompilerGeneratedAttribute>())
                    {
                        continue;
                    }

                    if (!hasAttribute && !type.Events[i].HasAttribute<ResetStaticAttribute>())
                    {
                        continue;
                    }

                    // No need to check for attribute since we just did above.
                    if (!hasAttribute && !type.Events[i].AddMethod.IsStatic)
                    {
                        Debug.LogError($"{type.Events[i].FullName} isn't static. ResetStatic can only be on static events.");
                        return (false, false);
                    }

                    fields.Add(new MemberData(type.Events[i]));
                }
            }

            if (!type.TryGetMethod(methodName, out MethodDefinition resetMethod))
            {
                resetMethod = new MethodDefinition(methodName,
                    MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.Static,
                    module.ImportReference(typeof(void)));

                type.Methods.Add(resetMethod);
            }

            if (!resetMethod.HasAttribute<RuntimeInitializeOnLoadMethodAttribute>())
            {
                MethodReference initAttributeCtor = module.ImportReference(typeof(RuntimeInitializeOnLoadMethodAttribute).GetConstructor(new Type[] { typeof(RuntimeInitializeLoadType) }));
                CustomAttribute attribute = new CustomAttribute(initAttributeCtor);
                attribute.ConstructorArguments.Add(new CustomAttributeArgument(module.ImportReference(typeof(RuntimeInitializeLoadType)), CecilAttributesSettings.Instance.ResetStaticMode));
                resetMethod.CustomAttributes.Add(attribute);
            }

            ILProcessor il = resetMethod.Body.GetILProcessor();

            int propertyIndex = 0;

            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i].IsGenericParameter)
                {
                    Debug.LogError(fields[i].Name + " is a generic. You can't reset a static generic.");
                    return (false, false);
                }

                if (!fields[i].IsEvent)
                {
                    (List<Instruction> instructions, bool valueType) = GetStaticSet(type, fields[i].IsProperty ? fields[i].property.GetStaticBackingField() : fields[i].field);

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
                    il.Emit(OpCodes.Stsfld, type.GetField(fields[i].eventDef.Name));
                }
            }

            il.Emit(OpCodes.Ret);

            return (true, true);
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

        private static (List<Instruction>, bool) GetStaticSet(TypeDefinition classType, FieldDefinition field)
        {
            bool valueType = false;
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
                        instructions.Add(Instruction.Create(OpCodes.Ldc_R4, (float)0.0f));
                    }
                    else if (type.Is<double>())
                    {
                        instructions.Add(Instruction.Create(OpCodes.Ldc_R8, (double)0.0d));
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
                        return (instructions, valueType);
                    }
                    else
                    {
                        throw new NullReferenceException("There's nothing that sets field " + field.Name + ". Did you set a default value?");
                    }
                }

                while (root.Previous != null && root.Previous.OpCode != OpCodes.Stsfld)
                {
                    instructions.Add(root.Previous);
                    root = root.Previous;
                }

                return (instructions, valueType);
            }
            else if (field.FieldType.Resolve().IsClass)
            {
                instructions.Add(Instruction.Create(OpCodes.Ldnull));
                return (instructions, valueType);
            }

            throw new NullReferenceException();
        }
    }
}
