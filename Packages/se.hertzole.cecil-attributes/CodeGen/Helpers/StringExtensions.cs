using System.Collections.Generic;
using System.Text;
using Mono.Cecil;

namespace Hertzole.CecilAttributes.CodeGen
{
    public static partial class Extensions
    {
        private static void ReplaceReplaceablesBase(StringBuilder sb, TypeReference type, MethodDefinition method, PropertyDefinition property)
        {
            if (type != null)
            {
                sb.Replace("%class%", type.GetFriendlyName());
                sb.Replace("%CLASS%", type.GetFriendlyName().ToUpperInvariant());
                sb.Replace("%class_full%", type.GetFriendlyFullName());
                sb.Replace("%CLASS_FULL%", type.GetFriendlyFullName().ToUpperInvariant());
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

        public static string FormatTypesBase(this string target, TypeReference type, MethodDefinition method, PropertyDefinition property)
        {
            StringBuilder sb = new StringBuilder(target);
            ReplaceReplaceablesBase(sb, type, method, property);
            return sb.ToString();
        }

        public static string FormatMessageLogCalled(this string target, TypeReference type, MethodDefinition method, string parametersSeparator, List<string> parameters, PropertyDefinition property, bool propertySet)
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

        public static string FormatMessageTimed(this string target, TypeReference type, MethodDefinition method, PropertyDefinition property = null)
        {
            StringBuilder sb = new StringBuilder(target);
            ReplaceReplaceablesBase(sb, type, method, property);

            sb.Replace("%milliseconds%", "{0}");
            sb.Replace("%ticks%", "{1}");

            return sb.ToString();
        }
    }
}
