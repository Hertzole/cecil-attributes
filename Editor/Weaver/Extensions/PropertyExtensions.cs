using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;

namespace Hertzole.CecilAttributes.Editor
{
    public static partial class WeaverExtensions
    {
        public static PropertyDefinition GetProperty(this TypeDefinition type, string name)
        {
            if (!type.HasProperties)
            {
                throw new NullReferenceException("There are no properties in type " + type.FullName + ".");
            }

            for (int i = 0; i < type.Properties.Count; i++)
            {
                if (type.Properties[i].Name == name)
                {
                    return type.Properties[i];
                }
            }

            throw new ArgumentException("There's no property in type " + type.FullName + " called " + name + ".");
        }

        public static PropertyDefinition GetProperty(this CustomAttribute attribute, string name)
        {
            return attribute.AttributeType.Resolve().GetProperty(name);
        }

        public static FieldDefinition GetStaticBackingField(this PropertyDefinition property)
        {
            return GetBackingField(property, OpCodes.Ldsfld);
        }

        public static FieldDefinition GetBackingField(this PropertyDefinition property)
        {
            return GetBackingField(property, OpCodes.Ldfld);
        }

        private static FieldDefinition GetBackingField(PropertyDefinition property, OpCode opCode)
        {
            Collection<Instruction> instructions = property.GetMethod.Body.Instructions;
            for (int i = instructions.Count - 1; i >= 0; i--)
            {
                if (instructions[i].OpCode == opCode)
                {
                    return (FieldDefinition)instructions[i].Operand;
                }
            }

            throw new NullReferenceException(property.FullName + " doesn't have a return field.");
        }
    }
}
