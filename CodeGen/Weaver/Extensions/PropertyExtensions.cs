﻿using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace Hertzole.CecilAttributes.CodeGen
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

        public static FieldReference GetBackingField(this PropertyDefinition property)
        {
            bool isStatic = property.IsStatic();

            return GetBackingField(property, isStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, isStatic ? OpCodes.Stsfld : OpCodes.Stfld);
        }

        private static FieldReference GetBackingField(PropertyDefinition property, OpCode ldfld, OpCode stfld)
        {
            MethodDefinition m = property.GetMethod ?? property.SetMethod;
            bool isSet = property.GetMethod == null;

            Collection<Instruction> instructions = m.Body.Instructions;
            for (int i = instructions.Count - 1; i >= 0; i--)
            {
                if ((isSet && instructions[i].OpCode == stfld) || (!isSet && instructions[i].OpCode == ldfld))
                {
                    return (FieldReference)instructions[i].Operand;
                }
            }

            throw new NullReferenceException(property.FullName + " doesn't have a return field.");
        }

        public static bool IsStatic(this PropertyDefinition property)
        {
            if ((property.GetMethod != null && property.GetMethod.IsStatic) || (property.SetMethod != null && property.SetMethod.IsStatic))
            {
                return true;
            }

            return false;
        }
    }
}
