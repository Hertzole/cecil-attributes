using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using UnityEngine.Profiling;

namespace Hertzole.CecilAttributes.Editor
{
    public class MarkInProfilerProcessor : BaseProcessor
    {
        public override string Name { get { return "MarkInProfiler"; } }

        public override bool NeedsMonoBehaviour { get { return false; } }

        public override bool AllowEditor { get { return false; } }

        public override bool IsValidClass(TypeDefinition type)
        {
            if (type.HasMethods)
            {
                for (int i = 0; i < type.Methods.Count; i++)
                {
                    if (type.Methods[i].HasAttribute<MarkInProfilerAttribute>())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override (bool success, bool dirty) ProcessClass(ModuleDefinition module, TypeDefinition type)
        {
            bool dirty = false;

            for (int i = 0; i < type.Methods.Count; i++)
            {
                if (type.Methods[i].HasAttribute<MarkInProfilerAttribute>())
                {
                    InjectIntoMethod(type.Methods[i]);
                }
            }

            return (true, dirty);
        }

        private void InjectIntoMethod(MethodDefinition method)
        {
            ILProcessor il = method.Body.GetILProcessor();

            il.InsertBefore(il.Body.Instructions[0], Instruction.Create(OpCodes.Ldstr, method.Name));
            il.InsertAfter(il.Body.Instructions[0], Instruction.Create(OpCodes.Call, method.Module.ImportReference(typeof(Profiler).GetMethod("BeginSample", new Type[] { typeof(string) }))));

            il.InsertBefore(il.Body.Instructions[il.Body.Instructions.Count - 1], Instruction.Create(OpCodes.Call, method.Module.ImportReference(typeof(Profiler).GetMethod("EndSample", Type.EmptyTypes))));
        }
    }
}
