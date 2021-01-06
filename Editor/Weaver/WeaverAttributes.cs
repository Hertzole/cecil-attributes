using Mono.Cecil;
using System;
using UnityEditor;
using UnityEngine;

namespace Hertzole.CecilAttributes.Editor
{
    public static partial class Weaver
    {
        private static readonly BaseProcessor[] processors = new BaseProcessor[]
        {
            new LogCalledProcessor(),
            new ResetStaticProcessor(),
            new FindPropertyProcessor(),
            new TimedProcessor(),
            new MarkInProfilerProcessor()
        };

        public static (bool success, bool dirty) ProcessAssembly(AssemblyDefinition assembly, bool isEditor)
        {
            bool dirty = false;

            foreach (ModuleDefinition module in assembly.Modules)
            {
                foreach (TypeDefinition type in module.GetTypes())
                {
                    if (type.HasAttribute<CecilAttributesProcessedAttribute>())
                    {
                        continue;
                    }
                    bool typeModified = false;

                    if (type.Name != "<Module>")
                    {
                        for (int i = 0; i < processors.Length; i++)
                        {
                            if (BuildPipeline.isBuildingPlayer && !processors[i].IncludeInBuild)
                            {
                                continue;
                            }

                            if (!processors[i].IsValidClass(type))
                            {
                                continue;
                            }

                            if (processors[i].NeedsMonoBehaviour && !type.IsSubclassOf<MonoBehaviour>())
                            {
                                Debug.LogWarning(processors[i].Name + " needs to be in a MonoBehaviour. (" + type.FullName + ")");
                                continue;
                            }

                            if (!processors[i].AllowEditor && isEditor)
                            {
                                Debug.LogWarning(processors[i].Name + " can't be used in the editor. (" + type.FullName + ")");
                                continue;
                            }

                            if (processors[i].EditorOnly && !isEditor)
                            {
                                Debug.LogWarning(processors[i].Name + " can only be used in editor scripts. (" + type.FullName + ")");
                                continue;
                            }

                            (bool success, bool dirtyClass) = processors[i].ProcessClass(module, type);
                            if (dirtyClass)
                            {
                                dirty = true;
                                typeModified = true;
                            }

                            if (!success)
                            {
                                return (false, false);
                            }
                        }
                    }

                    if (typeModified)
                    {
                        type.CustomAttributes.Add(new CustomAttribute(type.Module.ImportReference(typeof(CecilAttributesProcessedAttribute).GetConstructor(Type.EmptyTypes))));
                    }
                }
            }

            return (true, dirty);
        }
    }
}
