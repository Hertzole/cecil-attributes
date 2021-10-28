using System;
using System.Collections.Generic;
using System.IO;
using Hertzole.CecilAttributes.CodeGen;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Unity.CompilationPipeline.Common.Diagnostics;
using Unity.CompilationPipeline.Common.ILPostProcessing;
using UnityEditor;
using UnityEngine;

namespace Hertzole.CecilAttributes.CodeGen
{
	public sealed class Weaver
	{
		private readonly List<DiagnosticMessage> diagnostics;

		private static readonly BaseProcessor[] processors =
		{
			new FindPropertyProcessor(),
			new LogCalledProcessor(),
			new MarkInProfilerProcessor(),
			new ResetStaticProcessor(),
			new TimedProcessor()
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

			bool isBuildingPlayer = BuildPipeline.isBuildingPlayer;
			bool isEditor = assembly.Name.Contains("-Editor") || assembly.Name.Contains(".Editor");

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
						if (!processors[j].IncludeInBuild && isBuildingPlayer)
						{
							continue;
						}
						
						processors[j].Type = type;

						if (!processors[j].IsValidType())
						{
							continue;
						}

						if (!processors[j].AllowEditor && isEditor)
						{
							Debug.LogWarning($"{processors[i].Name} can't be used in the editor. ({type.FullName})");
							continue;
						}

						if (processors[j].EditorOnly && !isEditor)
						{
							Debug.LogWarning($"{processors[j].Name} can only be used in editor scripts. ({type.FullName})");
							continue;
						}

						if (processors[j].NeedsMonoBehaviour && !type.IsSubclassOf<MonoBehaviour>())
						{
							Debug.LogWarning($"{processors[j].Name} needs to be in a MonoBehaviour. ({type.FullName})");
							continue;
						}

						processors[j].ProcessType();
					}

					type.CustomAttributes.Add(new CustomAttribute(module.ImportReference(typeof(CecilAttributesProcessedAttribute).GetConstructor(Type.EmptyTypes))));
				}
			}
			
			MemoryStream pe = new MemoryStream();
			MemoryStream pdb = new MemoryStream();

			WriterParameters writerParameters = new WriterParameters
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