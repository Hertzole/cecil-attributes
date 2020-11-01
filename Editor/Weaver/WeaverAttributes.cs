using Mono.Cecil;
using System;
using UnityEngine;

namespace Hertzole.CecilAttributes.Editor
{
    public static partial class Weaver
    {
        private static readonly BaseProcessor[] processors = new BaseProcessor[]
        {
            new ResetStaticProcessor()
        };

        public static (bool success, bool dirty) ProcessAssembly(AssemblyDefinition assembly)
        {
            bool dirty = false;

            foreach (ModuleDefinition module in assembly.Modules)
            {
                foreach (TypeDefinition type in module.GetTypes())
                {
                    if (type.Name != "<Module>")
                    {
                        Type realType = Type.GetType(type.FullName + ", " + type.Module.Assembly.FullName);
                        if (realType != null)
                        {
                            if (realType.IsSubclassOf(typeof(MonoBehaviour)))
                            {
                                for (int i = 0; i < processors.Length; i++)
                                {
                                    if (!processors[i].IsValidClass(type))
                                    {
                                        continue;
                                    }

                                    (bool success, bool dirtyClass) = processors[i].ProcessClass(module, type, realType);
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
                }
            }

            return (true, dirty);
        }
    }
}
