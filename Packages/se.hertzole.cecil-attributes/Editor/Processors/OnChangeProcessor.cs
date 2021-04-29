using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using UnityEngine;

namespace Hertzole.CecilAttributes.Editor
{
    public class OnChangeProcessor : BaseProcessor
    {
        private List<MemberData> fields = new List<MemberData>();
        private Dictionary<MemberData, TypeReference> originalTypes = new Dictionary<MemberData, TypeReference>();

        private const string FIELD_PREFIX = "CecilOnChangedProperty_";

        public override string Name { get { return "OnChange"; } }

        public override bool NeedsMonoBehaviour { get { return false; } }
        public override bool AllowEditor { get { return true; } }

        public override bool IsValidClass(TypeDefinition type)
        {
            if (type.HasFields)
            {
                foreach (FieldDefinition field in type.Fields)
                {
                    if (field.TryGetAttribute<OnChangeAttribute>(out _))
                    {
                        return true;
                    }
                }
            }

            if (type.HasProperties)
            {
                foreach (PropertyDefinition property in type.Properties)
                {
                    if (property.TryGetAttribute<OnChangeAttribute>(out _))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        //TODO: Make work with multiple attributes.
        public override (bool success, bool dirty) ProcessClass(ModuleDefinition module, TypeDefinition type)
        {
            fields.Clear();
            originalTypes.Clear();

            if (type.HasFields)
            {
                for (int i = 0; i < type.Fields.Count; i++)
                {
                    if (type.Fields[i].HasAttribute<OnChangeAttribute>())
                    {
                        fields.Add(new MemberData(type.Fields[i]));
                    }
                }
            }

            if (type.HasProperties)
            {
                for (int i = 0; i < type.Properties.Count; i++)
                {
                    if (type.Properties[i].HasAttribute<OnChangeAttribute>())
                    {
                        fields.Add(new MemberData(type.Properties[i]));
                    }
                }
            }

            if (fields.Count == 0)
            {
                return (true, false);
            }

            for (int i = 0; i < fields.Count; i++)
            {
                if (!ProcessField(fields[i]))
                {
                    return (false, false);
                }
            }

            return (true, true);
        }

        private bool ProcessField(MemberData data)
        {
            string originalName = data.Name;
            TypeReference originalType = data.Type;

            originalTypes[data] = originalType;

            string hookName = data.GetAttribute<OnChangeAttribute>().GetField<string>("hook", null);
            bool equalsCheck = data.GetAttribute<OnChangeAttribute>().GetField("equalCheck", true);

            if (data.IsProperty)
            {

            }
            else
            {
                MethodDefinition get = GenerateGetter(data.field, originalName, originalType);
                MethodDefinition set = GenerateSetter(data.field, originalName, originalType, hookName, equalsCheck);

                if (set == null)
                {
                    return false;
                }

                data.field.DeclaringType.Methods.Add(get);
                data.field.DeclaringType.Methods.Add(set);

                PropertyDefinition property = new PropertyDefinition(FIELD_PREFIX + originalName, PropertyAttributes.None, originalType)
                {
                    GetMethod = get,
                    SetMethod = set
                };

                property.DeclaringType = data.field.DeclaringType;
                property.DeclaringType.Properties.Add(property);
            }

            return true;
        }

        private MethodDefinition GenerateGetter(FieldDefinition field, string name, TypeReference type)
        {
            MethodDefinition method = new MethodDefinition($"get_{FIELD_PREFIX}{name}", GetAccessorFromField(field) | MethodAttributes.HideBySig | MethodAttributes.SpecialName, type);
            ILProcessor il = method.Body.GetILProcessor();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Ret);

            method.SemanticsAttributes = MethodSemanticsAttributes.Getter;

            return method;
        }

        private MethodDefinition GenerateSetter(FieldDefinition field, string name, TypeReference type, string hookName, bool equalsCheck)
        {
            MethodDefinition method = new MethodDefinition($"set_{FIELD_PREFIX}{name}", GetAccessorFromField(field) | MethodAttributes.HideBySig | MethodAttributes.SpecialName, field.Module.ImportReference(typeof(void)));

            method.Parameters.Add(new ParameterDefinition(type));
            method.Body.Variables.Add(new VariableDefinition(type));

            MethodDefinition hookMethod = GetHookMethod(hookName, field.DeclaringType, type);
            if (hookMethod == null)
            {
                return null;
            }

            ILProcessor il = method.Body.GetILProcessor();

            Instruction ret = Instruction.Create(OpCodes.Ret);

            if (equalsCheck)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, field);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Beq_S, ret);
            }

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Stloc_0);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, field);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, hookMethod);

            il.Append(ret);

            method.SemanticsAttributes = MethodSemanticsAttributes.Setter;

            return method;
        }

        private MethodDefinition GetHookMethod(string hookName, TypeDefinition parent, TypeReference type)
        {
            if (string.IsNullOrWhiteSpace(hookName))
            {
                return null;
            }

            for (int i = 0; i < parent.Methods.Count; i++)
            {
                if (parent.Methods[i].Name == hookName)
                {
                    if (parent.Methods[i].Parameters.Count != 2)
                    {
                        //Debug.LogError($"Could not ")
                    }
                    else
                    {
                        //TODO: Make sure types match.
                        return parent.Methods[i];
                    }
                }
            }

            Debug.LogError("Couldn't find a method with the name " + hookName + " to hook into.");
            return null;
        }

        public MethodAttributes GetAccessorFromField(FieldDefinition field)
        {
            if (field.Attributes.HasFlag(FieldAttributes.Public))
            {
                return MethodAttributes.Public;
            }
            else if (field.Attributes.HasFlag(FieldAttributes.Private))
            {
                return MethodAttributes.Private;
            }
            else
            {
                return MethodAttributes.Private;
            }
        }
    }
}
