#if !UNITY_2019_4_OR_NEWER
#error Cecil attributes requires Unity 2019.4 or later!
#endif

#if UNITY_2020_2_OR_NEWER
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Unity.CompilationPipeline.Common.Diagnostics;
using Unity.CompilationPipeline.Common.ILPostProcessing;

namespace Hertzole.CecilAttributes.CodeGen
{
	public sealed class AttributesILProcessor : ILPostProcessor
	{
		private readonly List<DiagnosticMessage> diagnostics = new List<DiagnosticMessage>();

		private Weaver weaver;

		public override ILPostProcessor GetInstance()
		{
			return this;
		}

		public override bool WillProcess(ICompiledAssembly compiledAssembly)
		{
			return compiledAssembly.References.Any(filePath => Path.GetFileNameWithoutExtension(filePath) == WeaverConstants.RUNTIME_ASSEMBLY);
		}

		public override ILPostProcessResult Process(ICompiledAssembly compiledAssembly)
		{
			if (!WillProcess(compiledAssembly))
			{
				return null;
			}

			return ForceProcess(compiledAssembly);
		}

		public ILPostProcessResult ForceProcess(ICompiledAssembly compiledAssembly)
		{
			diagnostics.Clear();

			if (weaver == null)
			{
				weaver = new Weaver(diagnostics);
			}
			
			AssemblyDefinition assemblyDefinition = WeaverHelpers.AssemblyDefinitionFor(compiledAssembly);
			if (assemblyDefinition == null)
			{
				diagnostics.AddError($"Cannot read assembly definition: {compiledAssembly.Name}");
				return null;
			}

			ModuleDefinition mainModule = assemblyDefinition.MainModule;
			if (mainModule != null)
			{
				return weaver.ProcessAssembly(compiledAssembly);
			}

			diagnostics.AddError($"Cannot get main module from assembly definition: {compiledAssembly.Name}");
			return null;
		}
	}
}
#endif