﻿using Mono.Cecil;
using UnityEngine;

namespace Hertzole.CecilAttributes.Editor
{
    public static partial class Weaver
    {
        private static readonly BaseProcessor[] processors = new BaseProcessor[]
        {
            new LogCalledProcessor(),
            new ResetStaticProcessor()
        };

        public static (bool success, bool dirty) ProcessAssembly(AssemblyDefinition assembly, bool isEditor)
        {
            bool dirty = false;

            foreach (ModuleDefinition module in assembly.Modules)
            {
                foreach (TypeDefinition type in module.GetTypes())
                {
                    if (type.Name != "<Module>")
                    {
                        for (int i = 0; i < processors.Length; i++)
                        {
                            if (processors[i].NeedsMonoBehaviour && !type.IsSubclassOf<MonoBehaviour>())
                            {
                                Debug.LogWarning(processors[i].Name + " needs to be in a MonoBehaviour.");
                                continue;
                            }

                            if (!processors[i].AllowEditor && isEditor)
                            {
                                Debug.LogWarning(processors[i].Name + " can't be used in the editor.");
                                continue;
                            }

                            if (!processors[i].IsValidClass(type))
                            {
                                continue;
                            }

                            (bool success, bool dirtyClass) = processors[i].ProcessClass(module, type);
                            if (dirtyClass)
                            {
                                dirty = true;
                            }

                            if (!success)
                            {
                                return (false, false);
                            }
                        }
                    }
                }
            }

            return (true, dirty);
        }
    }
}
