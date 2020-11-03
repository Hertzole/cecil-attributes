using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hertzole.CecilAttributes.Editor
{
    public class OnChangeProcessor : BaseProcessor
    {
        private const string FIELD_PREFIX = "cecilOnChangeField_";

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

        public override bool NeedsMonoBehaviour { get { return false; } }

        //TODO: Make work with multiple attributes.
        public override (bool success, bool dirty) ProcessClass(ModuleDefinition module, TypeDefinition type, Type realType)
        {
            bool dirty = false;

            if (type.HasFields)
            {
                // Create a new collection because we're adding fields in the loop and we don't ned to account for those.
                Collection<FieldDefinition> fields = new Collection<FieldDefinition>(type.Fields);

                Debug.Log(fields.Count);

                List<int> fieldsToRemove = new List<int>();
                List<FieldDefinition> fieldsToAdd = new List<FieldDefinition>();
                List<PropertyDefinition> propertiesToAdd = new List<PropertyDefinition>();

                for (int i = 0; i < fields.Count; i++)
                {
                    if (fields[i].TryGetAttribute<OnChangeAttribute>(out CustomAttribute attribute))
                    {
                        string targetMethod = attribute.GetConstructorArgument(0, string.Empty);
                        if (string.IsNullOrEmpty(targetMethod))
                        {
                            Debug.LogError(fields[i].Name + " does not have a target method to hook into the change event.");
                            return (false, false);
                        }

                        if (type.TryGetMethod(targetMethod, out MethodDefinition method))
                        {

                        }
                        else
                        {
                            Debug.LogError("The method given to " + fields[i].Name + " does not exist in " + type.FullName + ".");
                            return (false, false);
                        }

                        string fieldName = FIELD_PREFIX + fields[i].Name;
                        fieldsToAdd.Add(new FieldDefinition(fieldName, FieldAttributes.Private, fields[i].FieldType));

                        PropertyDefinition fieldProperty = new PropertyDefinition(fields[i].Name, PropertyAttributes.None, fields[i].FieldType);

                        fieldsToRemove.Add(i);
                        propertiesToAdd.Add(fieldProperty);

                        dirty = true;
                    }
                }

                for (int i = 0; i < fieldsToRemove.Count; i++)
                {
                    type.Fields.RemoveAt(fieldsToRemove[i]);
                }

                for (int i = 0; i < fieldsToAdd.Count; i++)
                {
                    type.Fields.Add(fieldsToAdd[i]);
                }

                for (int i = 0; i < propertiesToAdd.Count; i++)
                {
                    type.Properties.Add(propertiesToAdd[i]);
                }
            }

            return (true, dirty);
        }
    }
}
