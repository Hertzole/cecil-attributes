using System;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace Hertzole.CecilAttributes.CodeGen
{
    public static partial class WeaverExtensions
    {
        public static bool HasAttribute<T>(this TypeDefinition type) where T : Attribute
        {
            if (!type.HasCustomAttributes)
            {
                return false;
            }

            return HasAttribute<T>(type.CustomAttributes);
        }

        public static bool HasAttribute<T>(this PropertyDefinition prop) where T : Attribute
        {
            if (!prop.HasCustomAttributes)
            {
                return false;
            }

            return HasAttribute<T>(prop.CustomAttributes);
        }

        public static bool HasAttribute<T>(this FieldDefinition field) where T : Attribute
        {
            if (!field.HasCustomAttributes)
            {
                return false;
            }

            return HasAttribute<T>(field.CustomAttributes);
        }

        public static bool HasAttribute<T>(this EventDefinition evt) where T : Attribute
        {
            if (!evt.HasCustomAttributes)
            {
                return false;
            }

            return HasAttribute<T>(evt.CustomAttributes);
        }

        public static bool HasAttribute<T>(this MethodDefinition method) where T : Attribute
        {
            if (!method.HasCustomAttributes)
            {
                return false;
            }

            return HasAttribute<T>(method.CustomAttributes);
        }

        private static bool HasAttribute<T>(Collection<CustomAttribute> attributes)
        {
            string myName = typeof(T).FullName;

            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].Constructor.DeclaringType.ToString() == myName)
                {
                    return true;
                }
            }

            return false;
        }

        public static CustomAttribute GetAttribute<T>(this TypeDefinition type) where T : Attribute
        {
            if (!type.HasCustomAttributes)
            {
                throw new NullReferenceException(type.FullName + " does not have any attributes.");
            }

            return GetAttribute<T>(type.CustomAttributes);
        }

        public static CustomAttribute GetAttribute<T>(this PropertyDefinition prop) where T : Attribute
        {
            if (!prop.HasCustomAttributes)
            {
                throw new NullReferenceException(prop.FullName + " does not have any attributes.");
            }

            return GetAttribute<T>(prop.CustomAttributes);
        }

        public static CustomAttribute GetAttribute<T>(this FieldDefinition field) where T : Attribute
        {
            if (!field.HasCustomAttributes)
            {
                throw new NullReferenceException(field.FullName + " does not have any attributes.");
            }

            return GetAttribute<T>(field.CustomAttributes);
        }

        public static CustomAttribute GetAttribute<T>(this EventDefinition evt) where T : Attribute
        {
            if (!evt.HasCustomAttributes)
            {
                throw new NullReferenceException(evt.FullName + " does not have any attributes.");
            }

            return GetAttribute<T>(evt.CustomAttributes);
        }

        public static CustomAttribute GetAttribute<T>(this MethodDefinition method) where T : Attribute
        {
            if (!method.HasCustomAttributes)
            {
                throw new NullReferenceException(method.FullName + " does not have any attributes.");
            }

            return GetAttribute<T>(method.CustomAttributes);
        }

        private static CustomAttribute GetAttribute<T>(Collection<CustomAttribute> attributes) where T : Attribute
        {
            string myName = typeof(T).FullName;

            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].Constructor.DeclaringType.ToString() == myName)
                {
                    return attributes[i];
                }
            }

            throw new ArgumentException("There's no " + myName + " argument on this type.");
        }

        public static bool TryGetAttribute<T>(this TypeDefinition type, out CustomAttribute attribute) where T : Attribute
        {
            if (!type.HasCustomAttributes)
            {
                attribute = null;
                return false;
            }

            return TryGetAttribute<T>(type.CustomAttributes, out attribute);
        }

        public static bool TryGetAttribute<T>(this PropertyDefinition prop, out CustomAttribute attribute) where T : Attribute
        {
            if (!prop.HasCustomAttributes)
            {
                attribute = null;
                return false;
            }

            return TryGetAttribute<T>(prop.CustomAttributes, out attribute);
        }

        public static bool TryGetAttribute<T>(this FieldDefinition field, out CustomAttribute attribute) where T : Attribute
        {
            if (!field.HasCustomAttributes)
            {
                attribute = null;
                return false;
            }

            return TryGetAttribute<T>(field.CustomAttributes, out attribute);
        }

        public static bool TryGetAttribute<T>(this EventDefinition evt, out CustomAttribute attribute) where T : Attribute
        {
            if (evt.HasCustomAttributes)
            {
                attribute = null;
                return false;
            }

            return TryGetAttribute<T>(evt.CustomAttributes, out attribute);
        }

        public static bool TryGetAttribute<T>(this MethodDefinition method, out CustomAttribute attribute) where T : Attribute
        {
            if (!method.HasCustomAttributes)
            {
                attribute = null;
                return false;
            }

            return TryGetAttribute<T>(method.CustomAttributes, out attribute);
        }

        private static bool TryGetAttribute<T>(Collection<CustomAttribute> attributes, out CustomAttribute attribute) where T : Attribute
        {
            string myName = typeof(T).FullName;

            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i].Constructor.DeclaringType.ToString() == myName)
                {
                    attribute = attributes[i];
                    return true;
                }
            }

            attribute = null;
            return false;
        }

        // https://github.com/vis2k/Mirror/blob/master/Assets/Mirror/Editor/Weaver/Extensions.cs#L198
        public static T GetField<T>(this CustomAttribute ca, string field, T defaultValue)
        {
            foreach (CustomAttributeNamedArgument customField in ca.Fields)
            {
                if (customField.Name == field)
                {
                    return (T)customField.Argument.Value;
                }
            }

            return defaultValue;
        }
    }
}
