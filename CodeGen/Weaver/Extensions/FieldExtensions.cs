using System;
using Mono.Cecil;

namespace Hertzole.CecilAttributes.CodeGen
{
    public static partial class WeaverExtensions
    {
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
    }
}
