using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UAssembly = UnityEditor.Compilation.Assembly;

namespace Hertzole.CecilAttributes.Editor
{
    public static class CompileHook
    {
        private const bool WEAVE_UNITY_ASSEMBLIES = false;

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
                    OnCompilationFinished(assembly.outputPath, new CompilerMessage[0]);
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
            //Debug.Log($"OnCompileFinished | Is building player: {BuildPipeline.isBuildingPlayer}, Is Debug build: {Debug.isDebugBuild} | {assemblyPath}");

            if (CompilerMessagesHasError(messages))
            {
                return;
            }

            if (!WEAVE_UNITY_ASSEMBLIES && (assemblyPath.Contains("Unity.") || assemblyPath.Contains("UnityEngine.") || assemblyPath.Contains("UnityEditor.")))
            {
                return;
            }

            // Allow editors for now until further testing.
            if (assemblyPath.Contains("-Editor") || assemblyPath.Contains(".Editor"))
            {
                return;
            }

            // Don't weave itself.
            string assemblyName = Path.GetFileNameWithoutExtension(assemblyPath);
            if (assemblyName == WeaverConstants.RUNTIME_ASSEMBLY || assemblyName == WeaverConstants.EDITOR_ASSEMBLY)
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

            HashSet<string> dependencies = GetDependencies(assemblyPath);

            if (!Weaver.Process(unityCoreModule, runtimeAssembly, null, new[] { assemblyPath }, dependencies.ToArray()))
            {
                SessionState.SetBool(WeaverConstants.WEAVE_SUCCESS, false);
                Debug.LogError("Weaving failed for " + assemblyPath);
            }
        }

        private static HashSet<string> GetDependencies(string assemblyPath)
        {
            HashSet<string> dependencies = new HashSet<string>
            {
                Path.GetDirectoryName(assemblyPath)
            };
            foreach (UAssembly assembly in CompilationPipeline.GetAssemblies())
            {
                if (assembly.outputPath != assemblyPath)
                {
                    continue;
                }

                foreach (string assemblyReference in assembly.compiledAssemblyReferences)
                {
                    dependencies.Add(Path.GetDirectoryName(assemblyReference));
                }
            }

            return dependencies;
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
