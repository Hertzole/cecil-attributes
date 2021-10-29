using System;
using System.Collections.Generic;
using System.IO;
using Hertzole.CecilAttributes.Editor;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Unity.CompilationPipeline.Common.Diagnostics;
using Unity.CompilationPipeline.Common.ILPostProcessing;
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
			new TimedProcessor(),
			new GetComponentProcessor()
		};

		public Weaver(List<DiagnosticMessage> diagnostics)
		{
			this.diagnostics = diagnostics;
			CecilAttributesSettings.SettingData settings = CecilAttributesSettings.LoadSettingData();

			for (int i = 0; i < processors.Length; i++)
			{
				processors[i].Settings = settings;
				processors[i].Weaver = this;
			}
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

		public void Warn(string message)
		{
			diagnostics.AddWarning(message);
		}

		public void Warn(MethodDefinition methodDefinition, string message)
		{
			diagnostics.AddWarning(methodDefinition, message);
		}

		public void Warn(SequencePoint sequencePoint, string message)
		{
			diagnostics.AddWarning(sequencePoint, message);
		}

		public ILPostProcessResult ProcessAssembly(ICompiledAssembly assembly)
		{
			AssemblyDefinition assemblyDefinition = WeaverHelpers.AssemblyDefinitionFor(assembly);

#if UNITY_2020_2_OR_NEWER
			bool isBuildingPlayer = true;
			if (assembly.Defines != null && assembly.Defines.Length > 0)
			{
				for (int i = 0; i < assembly.Defines.Length; i++)
				{
					if (assembly.Defines[i] == "UNITY_EDITOR")
					{
						isBuildingPlayer = false;
						break;
					}
				}
			}
#else
			bool isBuildingPlayer = UnityEditor.BuildPipeline.isBuildingPlayer;
			UnityEngine.Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA BUILDING PLAYER??? " + isBuildingPlayer);
#endif
			bool isEditor = assembly.Name.Contains("-Editor") || assembly.Name.Contains(".Editor");

			for (int i = 0; i < assemblyDefinition.Modules.Count; i++)
			{
				ModuleDefinition module = assemblyDefinition.Modules[i];

				for (int j = 0; j < processors.Length; j++)
				{
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
							Logger.Log($"SKIP :: Skipping {processors[j]} because they are not allowed in a built player.");
							continue;
						}
						
						processors[j].Type = type;

						if (!processors[j].IsValidType())
						{
							Logger.Log($"SKIP :: Skipping {processors[j]} on type {type} because it was not a valid type.");
							continue;
						}

						if (!processors[j].AllowEditor && isEditor)
						{
							Logger.Log($"ERROR :: {processors[j]} on type {type} is not allowed because it's not allowed in the editor.");
							Error($"{processors[i].Name} can't be used in the editor. ({type.FullName})");
							break;
						}

						if (processors[j].EditorOnly && !isEditor)
						{
							Logger.Log($"ERROR :: {processors[j]} on type {type} is not allowed because it must be on an editor.");
							Error($"{processors[j].Name} can only be used in editor scripts. ({type.FullName})");
							break;
						}

						if (processors[j].NeedsMonoBehaviour && !type.IsSubclassOf<MonoBehaviour>())
						{
							Logger.Log($"ERROR :: {processors[j]} on type {type} is not allowed because it must be on a MonoBehaviour.");
							Error($"{processors[j].Name} needs to be in a MonoBehaviour. ({type.FullName})");
							break;
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