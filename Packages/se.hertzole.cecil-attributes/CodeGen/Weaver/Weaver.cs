using System;
using System.Collections.Generic;
using System.IO;
using Hertzole.CecilAttributes.Editor;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Unity.CompilationPipeline.Common.Diagnostics;
using Unity.CompilationPipeline.Common.ILPostProcessing;

namespace Hertzole.CecilAttributes.CodeGen
{
	public sealed class Weaver
	{
		private readonly List<DiagnosticMessage> diagnostics;

		private static readonly BaseProcessor[] processors =
		{
			// new FindPropertyProcessor(),
			new LogCalledProcessor()
			// new MarkInProfilerProcessor(),
			// new ResetStaticProcessor(),
			// new TimedProcessor()
		};

		public Weaver(List<DiagnosticMessage> diagnostics)
		{
			this.diagnostics = diagnostics;
		}

		public void Error(string message)
		{
			diagnostics.AddError(message);
		}

		public void Error(MethodDefinition methodDefinition, string message)
		{
			diagnostics.AddError(methodDefinition, message);
		}

		public void Error(SequencePoint sequencePoint, string message)
		{
			diagnostics.AddError(sequencePoint, message);
		}

		public ILPostProcessResult ProcessAssembly(ICompiledAssembly assembly)
		{
			AssemblyDefinition assemblyDefinition = WeaverHelpers.AssemblyDefinitionFor(assembly);

			bool isBuildingPlayer = true;
			// if (defines != null && defines.Length > 0)
			// {
			// 	for (int i = 0; i < defines.Length; i++)
			// 	{
			// 		if (defines[i] == "UNITY_EDITOR")
			// 		{
			// 			isBuildingPlayer = false;
			// 			break;
			// 		}
			// 	}
			// }

			for (int i = 0; i < assemblyDefinition.Modules.Count; i++)
			{
				ModuleDefinition module = assemblyDefinition.Modules[i];

				for (int j = 0; j < processors.Length; j++)
				{
					processors[j].Weaver = this;
					processors[j].Module = module;
				}

				IEnumerable<TypeDefinition> types = module.GetTypes();
				foreach (TypeDefinition type in types)
				{
					if (type.HasAttribute<CecilAttributesProcessedAttribute>())
					{
						continue;
					}

					for (int j = 0; j < processors.Length; j++)
					{
						processors[j].Type = type;

						if (!processors[j].IsValidType())
						{
							continue;
						}

						processors[j].ProcessType();
					}

					type.CustomAttributes.Add(new CustomAttribute(module.ImportReference(typeof(CecilAttributesProcessedAttribute).GetConstructor(Type.EmptyTypes))));
				}
			}
			
			var pe = new MemoryStream();
			var pdb = new MemoryStream();

			var writerParameters = new WriterParameters
			{
				SymbolWriterProvider = new PortablePdbWriterProvider(),
				SymbolStream = pdb,
				WriteSymbols = true
			};

			assemblyDefinition.Write(pe, writerParameters);

			return new ILPostProcessResult(new InMemoryAssembly(pe.ToArray(), pdb.ToArray()), diagnostics);
		}
	}
}