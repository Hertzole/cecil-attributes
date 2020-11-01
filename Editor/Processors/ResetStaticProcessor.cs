using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hertzole.CecilAttributes.Editor
{
    public class ResetStaticProcessor : BaseProcessor
    {
        private struct FieldOrProperty
        {
            public FieldDefinition field;
            public PropertyDefinition property;
            public EventDefinition eventDef;

            public bool IsProperty { get { return field == null && eventDef == null; } }
            public bool IsEvent { get { return eventDef != null; } }

            public FieldOrProperty(FieldDefinition field)
            {
                this.field = field;
                property = null;
                eventDef = null;
            }

            public FieldOrProperty(PropertyDefinition property)
            {
                this.property = property;
                field = null;
                eventDef = null;
            }

            public FieldOrProperty(EventDefinition e)
            {
                eventDef = e;
                field = null;
                property = null;
            }
        }

        private List<FieldOrProperty> fields = new List<FieldOrProperty>();

        public override bool IsValidClass(TypeDefinition type)
        {
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

        public override (bool success, bool dirty) ProcessClass(ModuleDefinition module, TypeDefinition type, Type realType)
        {
            fields.Clear();

            if (type.HasFields)
            {
                for (int i = 0; i < type.Fields.Count; i++)
                {
                    if (type.Fields[i].HasAttribute<ResetStaticAttribute>())
                    {
                        if (!type.Fields[i].IsStatic)
                        {
                            throw new NotSupportedException(type.Fields[i].FullName + " isn't static. ResetStatic can only be on static fields.");
                        }

                        fields.Add(new FieldOrProperty(type.Fields[i]));
                    }
                }
            }

            if (type.HasProperties)
            {
                for (int i = 0; i < type.Properties.Count; i++)
                {
                    if (type.Properties[i].HasAttribute<ResetStaticAttribute>())
                    {
                        if (!type.Properties[i].GetMethod.IsStatic)
                        {
                            throw new NotSupportedException(type.Properties[i].FullName + " isn't static. ResetStatic can only be on static properties.");
                        }

                        fields.Add(new FieldOrProperty(type.Properties[i]));
                    }
                }
            }

            if (type.HasEvents)
            {
                for (int i = 0; i < type.Events.Count; i++)
                {
                    if (type.Events[i].HasAttribute<ResetStaticAttribute>())
                    {
                        if (!type.Events[i].AddMethod.IsStatic)
                        {
                            throw new NotSupportedException(type.Events[i].FullName + " isn't static. ResetStatic can only be on static events.");
                        }

                        fields.Add(new FieldOrProperty(type.Events[i]));
                    }
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
                attribute.ConstructorArguments.Add(new CustomAttributeArgument(module.ImportReference(typeof(RuntimeInitializeLoadType)), RuntimeInitializeLoadType.SubsystemRegistration));
                resetMethod.CustomAttributes.Add(attribute);
            }

            ILProcessor il = resetMethod.Body.GetILProcessor();

            for (int i = 0; i < fields.Count; i++)
            {
                if (!fields[i].IsEvent)
                {
                    Instruction loadValue = GetStaticSet(type, fields[i].IsProperty ? fields[i].property.GetBackingField() : fields[i].field);
                    il.Append(loadValue);
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

        private static Instruction GetStaticSet(TypeDefinition type, FieldDefinition field)
        {
            MethodDefinition cctor = type.GetMethod(".cctor");

            foreach (Instruction i in cctor.Body.Instructions)
            {
                if (i.OpCode == OpCodes.Stsfld && i.Operand == field)
                {
                    return i.Previous;
                }
            }

            throw new NullReferenceException("There's nothing that sets field " + field.FullName);
        }
    }
}
