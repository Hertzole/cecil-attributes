using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace Hertzole.CecilAttributes.Editor
{
    public struct MemberData : IEquatable<MemberData>
    {
        public FieldDefinition field;
        public PropertyDefinition property;
        public EventDefinition eventDef;

        public bool IsProperty { get { return field == null && eventDef == null; } }
        public bool IsEvent { get { return eventDef != null; } }
        public string Name
        {
            get
            {
                if (field != null)
                {
                    return field.Name;
                }
                else if (property != null)
                {
                    return property.Name;
                }
                else
                {
                    return eventDef.Name;
                }
            }
        }

        public TypeReference Type
        {
            get
            {
                if (field != null)
                {
                    return field.Module.ImportReference(field.FieldType);
                }
                else if (property != null)
                {
                    return property.Module.ImportReference(property.PropertyType);
                }
                else
                {
                    return eventDef.Module.ImportReference(eventDef.EventType);
                }
            }
        }

        public TypeReference ResolvedType { get { return Type.Module.ImportReference(Type.Resolve()); } }

        public bool IsGenericParameter
        {
            get
            {
                if (field != null)
                {
                    return field.FieldType.IsGenericParameter;
                }
                else if (property != null)
                {
                    return property.PropertyType.IsGenericParameter;
                }
                else
                {
                    return eventDef.EventType.IsGenericParameter;
                }
            }
        }

        public MemberData(FieldDefinition field)
        {
            this.field = field;
            property = null;
            eventDef = null;
        }

        public MemberData(PropertyDefinition property)
        {
            this.property = property;
            field = null;
            eventDef = null;
        }

        public MemberData(EventDefinition e)
        {
            eventDef = e;
            field = null;
            property = null;
        }

        public bool TryGetAttribute<T>(out CustomAttribute attribute) where T : Attribute
        {
            if (field != null)
            {
                return field.TryGetAttribute<T>(out attribute);
            }
            else if (property != null)
            {
                return property.TryGetAttribute<T>(out attribute);
            }
            else
            {
                return eventDef.TryGetAttribute<T>(out attribute);
            }
        }

        public CustomAttribute GetAttribute<T>() where T : Attribute
        {
            if (field != null)
            {
                return field.GetAttribute<T>();
            }
            else if (property != null)
            {
                return property.GetAttribute<T>();
            }
            else
            {
                return eventDef.GetAttribute<T>();
            }
        }

        public override bool Equals(object obj)
        {
            return obj is MemberData data && Equals(data);
        }

        public bool Equals(MemberData other)
        {
            return EqualityComparer<FieldDefinition>.Default.Equals(field, other.field) &&
                   EqualityComparer<PropertyDefinition>.Default.Equals(property, other.property) &&
                   EqualityComparer<EventDefinition>.Default.Equals(eventDef, other.eventDef);
        }

        public override int GetHashCode()
        {
            int hashCode = 1130756317;
            hashCode = hashCode * -1521134295 + EqualityComparer<FieldDefinition>.Default.GetHashCode(field);
            hashCode = hashCode * -1521134295 + EqualityComparer<PropertyDefinition>.Default.GetHashCode(property);
            hashCode = hashCode * -1521134295 + EqualityComparer<EventDefinition>.Default.GetHashCode(eventDef);
            return hashCode;
        }

        public static bool operator ==(MemberData left, MemberData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MemberData left, MemberData right)
        {
            return !(left == right);
        }
    }
}
