using System.Collections.Generic;
using Hertzole.CecilAttributes.CodeGen.Caches;
using Hertzole.CecilAttributes.Editor;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Hertzole.CecilAttributes.CodeGen
{
    public abstract class BaseProcessor
    {
        public Weaver Weaver { get; set; }
        public ModuleDefinition Module { get; set; }
        public TypeDefinition Type { get; set; }
        public MethodsCache MethodsCache { get; set; }
        public CecilAttributesSettings.SettingData Settings { get; set; }
        
        public abstract string Name { get; }

        public abstract bool NeedsMonoBehaviour { get; }

        public abstract bool AllowEditor { get; }

        public virtual bool EditorOnly { get { return false; } }

        public virtual bool IncludeInBuild { get { return true; } }

        private readonly HashSet<TypeDefinition> processedTypes = new HashSet<TypeDefinition>();

        public bool ShouldProcess(TypeDefinition type)
        {
            if (processedTypes.Count == 0)
            {
                return true;
            }

            return !processedTypes.Contains(type);
        }
        
        public abstract bool IsValidType();

        public abstract void ProcessType();
        
        protected void Error(string message)
        {
            Weaver.Error(message);
        }

        protected void Error(MethodDefinition methodDefinition, string message)
        {
            Weaver.Error(methodDefinition, message);
        }

        protected void Error(SequencePoint sequencePoint, string message)
        {
            Weaver.Error(sequencePoint, message);
        }

        protected void MarkAsProcessed(TypeDefinition typeDefinition)
        {
            processedTypes.Add(typeDefinition);
        }
    }
}
