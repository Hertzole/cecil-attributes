using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Hertzole.CecilAttributes.Editor
{
    public class ResetStaticProcessor : BaseProcessor
    {
        private List<MemberData> fields = new List<MemberData>();

        public override string Name { get { return "ResetStatic"; } }

        public override bool NeedsMonoBehaviour { get { return false; } }
        public override bool AllowEditor { get { return true; } }

        public override bool IsValidClass(TypeDefinition type)
        {
            if (BuildPipeline.isBuildingPlayer && !CecilAttributesSettings.Instance.IncludeResetStaticInBuild)
            {
                return false;
            }

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
                        throw new NotSupportedException(type.Fields[i].FullName + " isn't static. ResetStatic can only be on static fields.");
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
                    if (!hasAttribute && !type.Properties[i].GetMethod.IsStatic)
                    {
                        throw new NotSupportedException(type.Properties[i].FullName + " isn't static. ResetStatic can only be on static properties.");
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
                        throw new NotSupportedException(type.Events[i].FullName + " isn't static. ResetStatic can only be on static events.");
                    }

                    fields.Add(new MemberData(type.Events[i]));
                }
            }

            if (!type.TryGetMethod("CecilAttributesGenerated_ResetStatics", out MethodDefinition resetMethod))
            {
                resetMethod = new MethodDefinition("CecilAttributesGenerated_ResetStatics",
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

            for (int i = 0; i < fields.Count; i++)
            {
                if (!fields[i].IsEvent)
                {
                    List<Instruction> loadValues = GetStaticSet(type, fields[i].IsProperty ? fields[i].property.GetStaticBackingField() : fields[i].field);

                    for (int j = loadValues.Count - 1; j >= 0; j--)
                    {
                        il.Append(loadValues[j]);
                    }

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
                    il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Stsfld, type.GetField(fields[i].eventDef.Name));
                }
            }

            il.Emit(OpCodes.Ret);

            return (true, true);
        }

        private static List<Instruction> GetStaticSet(TypeDefinition type, FieldDefinition field)
        {
            List<Instruction> instructions = new List<Instruction>();

            if (type.TryGetMethod(".cctor", out MethodDefinition cctor))
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
                    if (field.FieldType.Resolve().IsClass)
                    {
                        instructions.Add(Instruction.Create(OpCodes.Ldnull));
                        return instructions;
                    }

                    throw new NullReferenceException("There's nothing that sets field " + field.Name + ". Did you set a default value?");
                }

                while (root.Previous != null && root.Previous.OpCode != OpCodes.Stsfld)
                {
                    instructions.Add(root.Previous);
                    root = root.Previous;
                }

                return instructions;
            }
            else if (field.FieldType.Resolve().IsClass)
            {
                instructions.Add(Instruction.Create(OpCodes.Ldnull));
                return instructions;
            }

            throw new NullReferenceException();
        }
    }
}
