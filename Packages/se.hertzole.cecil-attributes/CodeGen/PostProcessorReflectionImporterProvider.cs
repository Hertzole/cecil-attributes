using Mono.Cecil;

namespace Hertzole.CecilAttributes.CodeGen
{
    public sealed class PostProcessorReflectionImporterProvider : IReflectionImporterProvider
    {
        public IReflectionImporter GetReflectionImporter(ModuleDefinition moduleDefinition)
        {
            return new PostProcessorReflectionImporter(moduleDefinition);
        }
    }
}
