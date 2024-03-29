﻿#if !UNITY_2020_2_OR_NEWER
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Unity.CompilationPipeline.Common.Diagnostics;
using Unity.CompilationPipeline.Common.ILPostProcessing;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Assembly = System.Reflection.Assembly;
using UAssembly = UnityEditor.Compilation.Assembly;

namespace Hertzole.CecilAttributes.CodeGen
{
	internal sealed class PostProcessor2019 : ILPostProcessor
	{
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
			return null;
		}
	}

	public static class CompileHook
	{
		private static Weaver weaver;
		private static readonly List<DiagnosticMessage> diagnostic = new List<DiagnosticMessage>();

		[InitializeOnLoadMethod]
		public static void OnInitializeOnLoad()
		{
			CompilationPipeline.assemblyCompilationFinished += OnCompilationFinished;

			if (!SessionState.GetBool(WeaverConstants.WEAVED, false))
			{
				SessionState.SetBool(WeaverConstants.WEAVED, true);
				SessionState.SetBool(WeaverConstants.WEAVE_SUCCESS, true);

				WeaveAllAssemblies();
			}
		}

		public static void WeaveAllAssemblies()
		{
			foreach (UAssembly assembly in CompilationPipeline.GetAssemblies())
			{
				if (File.Exists(assembly.outputPath))
				{
					OnCompilationFinished(assembly.outputPath, Array.Empty<CompilerMessage>());
				}
			}

#if UNITY_2019_3_OR_NEWER
			EditorUtility.RequestScriptReload();
#else
            UnityEditorInternal.InternalEditorUtility.RequestScriptReload();
#endif
		}

		private static bool CompilerMessagesHasError(CompilerMessage[] messages)
		{
			for (int i = 0; i < messages.Length; i++)
			{
				if (messages[i].type == CompilerMessageType.Error)
				{
					return true;
				}
			}

			return false;
		}

		private static void OnCompilationFinished(string assemblyPath, CompilerMessage[] messages)
		{
			if (CompilerMessagesHasError(messages))
			{
				return;
			}

			if (assemblyPath.Contains("Unity.") || assemblyPath.Contains("UnityEngine.") || assemblyPath.Contains("UnityEditor."))
			{
				return;
			}

			// Don't weave itself.
			string assemblyName = Path.GetFileNameWithoutExtension(assemblyPath);
			if (assemblyName == WeaverConstants.RUNTIME_ASSEMBLY || assemblyName == WeaverConstants.EDITOR_ASSEMBLY || assemblyName == WeaverConstants.CODEGEN_ASSEMBLY)
			{
				return;
			}

			string runtimeAssembly = FindRuntimeAssembly();
			if (string.IsNullOrEmpty(runtimeAssembly))
			{
				Debug.LogError("Failed to find runtime assembly.");
				return;
			}

			// This is normal.
			if (!File.Exists(runtimeAssembly))
			{
				return;
			}

			string unityCoreModule = UnityEditorInternal.InternalEditorUtility.GetEngineCoreModuleAssemblyPath();
			if (string.IsNullOrEmpty(unityCoreModule))
			{
				Debug.LogError("Failed to find UnityEngine assembly.");
				return;
			}

			List<string> dependencyPaths = new List<string>()
			{
				Path.GetDirectoryName(assemblyPath),
				runtimeAssembly,
				unityCoreModule
			};

			foreach (UAssembly assembly in CompilationPipeline.GetAssemblies())
			{
				if (assembly.outputPath != assemblyPath)
				{
					continue;
				}

				foreach (string assemblyReference in assembly.compiledAssemblyReferences)
				{
					dependencyPaths.Add(assemblyReference);
				}
			}

			string assemblyPathName = Path.GetFileName(assemblyPath);
			string outputDirectory = $"{Application.dataPath}/../{Path.GetDirectoryName(assemblyPath)}";
			ICompiledAssembly compiledAssembly = new ILPostProcessCompiledAssembly(assemblyPathName, dependencyPaths.ToArray(), null, outputDirectory);

			if (weaver == null)
			{
				weaver = new Weaver(diagnostic);
			}

			diagnostic.Clear();

			ILPostProcessResult result = weaver.ProcessAssembly(compiledAssembly);
			if (result.Diagnostics.Count > 0)
			{
				bool failed = false;	
			
				for (int i = 0; i < result.Diagnostics.Count; i++)
				{
					switch (result.Diagnostics[i].DiagnosticType)
					{
						case DiagnosticType.Error:
							Debug.LogError($"{nameof(ILPostProcessor)} Error - {result.Diagnostics[i].MessageData} {result.Diagnostics[i].File}:{result.Diagnostics[i].Line}");
							failed = true;
							break;
						case DiagnosticType.Warning:
							Debug.LogWarning($"{nameof(ILPostProcessor)} Warning - {result.Diagnostics[i].MessageData} {result.Diagnostics[i].File}:{result.Diagnostics[i].Line}");
							break;
					}
				}

				if (failed)
				{
					SessionState.SetBool(WeaverConstants.WEAVE_SUCCESS, false);
				}

				return;
			}

			WriteAssembly(result.InMemoryAssembly, outputDirectory, assemblyPathName);
		}

		private static void WriteAssembly(InMemoryAssembly inMemoryAssembly, string outputPath, string assemblyName)
		{
			if (inMemoryAssembly == null)
			{
				throw new ArgumentException("InMemoryAssembly has never been accessed or modified");
			}

			string asmPath = Path.Combine(outputPath, assemblyName);
			string pdbFileName = $"{Path.GetFileNameWithoutExtension(assemblyName)}.pdb";
			string pdbPath = Path.Combine(outputPath, pdbFileName);

			File.WriteAllBytes(asmPath, inMemoryAssembly.PeData);
			File.WriteAllBytes(pdbPath, inMemoryAssembly.PdbData);
		}

		private static string FindRuntimeAssembly()
		{
			foreach (UAssembly assembly in CompilationPipeline.GetAssemblies())
			{
				if (assembly.name == WeaverConstants.RUNTIME_ASSEMBLY)
				{
					return assembly.outputPath;
				}
			}

			return string.Empty;
		}
	}
}
#endif