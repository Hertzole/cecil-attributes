using System;
using Hertzole.CecilAttributes.Editor;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine.Profiling;

namespace Hertzole.CecilAttributes.CodeGen
{
    public class MarkInProfilerProcessor : BaseProcessor
    {
        public override string Name { get { return "MarkInProfiler"; } }

        public override bool NeedsMonoBehaviour { get { return false; } }

        public override bool AllowEditor { get { return false; } }

        public override bool IsValidType()
        {
            if (Type.HasMethods)
            {
                for (int i = 0; i < Type.Methods.Count; i++)
                {
                    if (Type.Methods[i].HasAttribute<MarkInProfilerAttribute>())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override void ProcessType()
        {
            for (int i = 0; i < Type.Methods.Count; i++)
            {
                if (Type.Methods[i].HasAttribute<MarkInProfilerAttribute>())
                {
                    InjectIntoMethod(Type.Methods[i], Type);
                }
            }
        }

        private void InjectIntoMethod(MethodDefinition method, TypeDefinition type)
        {
            ILProcessor il = method.Body.GetILProcessor();

            string name = CecilAttributesSettings.Instance.MarkInProfilerFormat.FormatTypesBase(type, method, null);

            il.InsertBefore(il.Body.Instructions[0], Instruction.Create(OpCodes.Ldstr, name));
            il.InsertAfter(il.Body.Instructions[0], Instruction.Create(OpCodes.Call, method.Module.ImportReference(typeof(Profiler).GetMethod("BeginSample", new Type[] { typeof(string) }))));

            il.InsertBefore(il.Body.Instructions[il.Body.Instructions.Count - 1], Instruction.Create(OpCodes.Call, method.Module.ImportReference(typeof(Profiler).GetMethod("EndSample", System.Type.EmptyTypes))));
        }
    }
}
