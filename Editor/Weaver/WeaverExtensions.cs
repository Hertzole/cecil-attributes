using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using UnityEditor.Compilation;

namespace Hertzole.CecilAttributes.Editor
{
    public static class WeaverExtensions
    {
        // https://github.com/vis2k/Mirror/blob/master/Assets/Mirror/Editor/Weaver/Extensions.cs#L10
        public static bool Is(this TypeReference tr, Type t)
        {
            if (t.IsGenericType)
            {
                return tr.GetElementType().FullName == t.FullName;
            }
            else
            {
                return tr.FullName == t.FullName;
            }
        }

        public static bool Is<T>(this TypeReference tr)
        {
            return Is(tr, typeof(T));
        }

        // https://github.com/vis2k/Mirror/blob/master/Assets/Mirror/Editor/Weaver/Extensions.cs#L23
        public static bool IsSubclassOf(this TypeDefinition td, Type type)
        {
            if (!td.IsClass)
            {
                return false;
            }

            TypeReference parent = td.BaseType;

            if (parent == null)
            {
                return false;
            }

            if (parent.Is(type))
            {
                return true;
            }

            if (parent.CanBeResolved())
            {
                return IsSubclassOf(parent.Resolve(), type);
            }

            return false;
        }

        public static bool IsSubclassOf<T>(this TypeDefinition td)
        {
            return IsSubclassOf(td, typeof(T));
        }

        // https://github.com/vis2k/Mirror/blob/master/Assets/Mirror/Editor/Weaver/Extensions.cs#L87
        public static bool CanBeResolved(this TypeReference parent)
        {
            while (parent != null)
            {
                if (parent.Scope.Name == "Windows")
                {
                    return false;
                }

                if (parent.Scope.Name == "mscorlib")
                {
                    TypeDefinition resolved = parent.Resolve();
                    return resolved != null;
                }

                try
                {
                    parent = parent.Resolve().BaseType;
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

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

        public static bool HasAttribute<T>(Collection<CustomAttribute> attributes)
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

        public static CustomAttribute GetAttribute<T>(this PropertyDefinition prop) where T : Attribute
        {
            if (!prop.HasCustomAttributes)
            {
                throw new NullReferenceException(prop.FullName + " does not have any attributes.");
            }

            return GetAttribute<T>(prop.CustomAttributes);
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

        public static bool TryGetAttribute<T>(this TypeDefinition type, out CustomAttribute attribute) where T : Attribute
        {
            if (type.HasCustomAttributes)
            {
                attribute = null;
                return false;
            }

            return TryGetAttribute<T>(type.CustomAttributes, out attribute);
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

        public static FieldDefinition GetField(this TypeDefinition type, string name)
        {
            if (!type.HasFields)
            {
                throw new NullReferenceException("There are no fields in type " + type.FullName + ".");
            }

            for (int i = 0; i < type.Fields.Count; i++)
            {
                if (type.Fields[i].Name == name)
                {
                    return type.Fields[i];
                }
            }

            throw new ArgumentException("There's no field in type " + type.FullName + " called " + name + ".");
        }

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
            if (CompilationPipeline.codeOptimization == CodeOptimization.Release)
            {
                foreach (Instruction i in property.GetMethod.Body.Instructions)
                {
                    if (i.OpCode == opCode && i.Next != null && i.Next.OpCode == OpCodes.Ret)
                    {
                        return (FieldDefinition)i.Operand;
                    }
                }
            }
            else
            {
                Collection<Instruction> instructions = property.GetMethod.Body.Instructions;
                for (int i = instructions.Count - 1; i >= 0; i--)
                {
                    if (instructions[i].OpCode == opCode)
                    {
                        return (FieldDefinition)instructions[i].Operand;
                    }
                }
            }

            throw new NullReferenceException(property.FullName + " doesn't have a return field.");
        }

        //public static T GetField<T>(this CustomAttribute attribute, string field, T defaultValue)
        //{
        //    if (!attribute.HasFields)
        //    {
        //        return defaultValue;
        //    }

        //    for (int i = 0; i < attribute.Fields.Count; i++)
        //    {
        //        if (attribute.Fields[i].Name == field)
        //        {
        //            return (T)attribute.Fields[i].Argument.Value;
        //        }
        //    }

        //    return defaultValue;
        //}

        //public static T GetProperty<T>(this CustomAttribute attribute, string property, T defaultValue)
        //{
        //    for (int i = 0; i < attribute.Properties.Count; i++)
        //    {
        //        if (attribute.Properties[i].Name == property)
        //        {
        //            return (T)attribute.Properties[i].Argument.Value;
        //        }
        //    }

        //    return defaultValue;
        //}

        public static bool ImplementsInterface(this TypeDefinition type, InterfaceImplementation baseInterface)
        {
            if (!type.HasInterfaces)
            {
                return false;
            }

            for (int i = 0; i < type.Interfaces.Count; i++)
            {
                if (type.Interfaces[i].InterfaceType.FullName == baseInterface.InterfaceType.FullName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
