using Mono.Cecil;
using System.Collections.Generic;
using System.Text;

namespace Hertzole.CecilAttributes.Editor
{
    public static partial class Extensions
    {
        private static void ReplaceReplaceablesBase(StringBuilder sb, TypeDefinition type, MethodDefinition method, PropertyDefinition property)
        {
            if (type != null)
            {
                sb.Replace("%class%", type.Name);
                sb.Replace("%CLASS%", type.Name.ToUpperInvariant());
                sb.Replace("%class_full%", type.FullName);
                sb.Replace("%CLASS_FULL%", type.FullName.ToUpperInvariant());
            }
            if (method != null)
            {
                sb.Replace("%method%", method.Name);
                sb.Replace("%METHOD%", method.Name.ToUpperInvariant());
                sb.Replace("%method_full%", method.FullName);
                sb.Replace("%METHOD_FULL%", method.FullName.ToUpperInvariant());
            }
            if (property != null)
            {
                sb.Replace("%property%", property.Name);
                sb.Replace("%PROPERTY%", property.Name.ToUpperInvariant());
                sb.Replace("%property_full%", property.FullName);
                sb.Replace("%PROPERTY_FULL%", property.FullName.ToUpperInvariant());
            }
        }

        public static string FormatTypesBase(this string target, TypeDefinition type, MethodDefinition method, PropertyDefinition property)
        {
            StringBuilder sb = new StringBuilder(target);
            ReplaceReplaceablesBase(sb, type, method, property);
            return sb.ToString();
        }

        public static string FormatMessageLogCalled(this string target, TypeDefinition type, MethodDefinition method, string parametersSeparator, List<string> parameters, PropertyDefinition property, bool propertySet)
        {
            StringBuilder sb = new StringBuilder(target);
            ReplaceReplaceablesBase(sb, type, method, property);

            if (parameters != null && parameters.Count > 0)
            {
                sb.Replace("%parameters%", string.Join(parametersSeparator, parameters));
                sb.Replace("%PARAMETERS%", string.Join(parametersSeparator, parameters).ToUpperInvariant());
            }
            else
            {
                sb.Replace("%parameters%", string.Empty);
                sb.Replace("%PARAMETERS%", string.Empty);
            }

            if (property != null)
            {
                if (propertySet)
                {
                    sb.Replace("%old_value%", "{0}");
                    sb.Replace("%new_value%", "{1}");
                }
                else
                {
                    sb.Replace("%value%", "{0}");
                }
            }

            return sb.ToString();
        }

        public static string FormatMessageTimed(this string target, TypeDefinition type, MethodDefinition method, PropertyDefinition property = null)
        {
            StringBuilder sb = new StringBuilder(target);
            ReplaceReplaceablesBase(sb, type, method, property);

            sb.Replace("%milliseconds%", "{0}");
            sb.Replace("%ticks%", "{1}");

            return sb.ToString();
        }
    }
}
