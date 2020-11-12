using Mono.Cecil;

namespace Hertzole.CecilAttributes.Editor
{
    public abstract class BaseProcessor
    {
        public abstract string Name { get; }

        public abstract bool NeedsMonoBehaviour { get; }

        public abstract bool AllowEditor { get; }

        public virtual bool EditorOnly { get { return false; } }

        public abstract bool IsValidClass(TypeDefinition type);

        public abstract (bool success, bool dirty) ProcessClass(ModuleDefinition module, TypeDefinition type);
    }
}
