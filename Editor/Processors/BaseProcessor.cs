using Mono.Cecil;
using System;

namespace Hertzole.CecilAttributes.Editor
{
    public abstract class BaseProcessor
    {
        public abstract bool IsValidClass(TypeDefinition type);

        public abstract (bool success, bool dirty) ProcessClass(ModuleDefinition module, TypeDefinition type, Type realType);
    }
}
