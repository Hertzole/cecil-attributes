using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Hertzole.CecilAttributes.Editor
{
    // Idea based on https://github.com/ByronMayne/Weaver

    //TODO: Make work with properties.
    public class TimedProcessor : BaseProcessor
    {
        private List<Instruction> instructions = new List<Instruction>();

        public override string Name { get { return "Timed"; } }

        public override bool NeedsMonoBehaviour { get { return false; } }
        public override bool AllowEditor { get { return true; } }
        public override bool IncludeInBuild { get { return CecilAttributesSettings.Instance.IncludeTimedInBuild; } }

        public override bool IsValidClass(TypeDefinition type)
        {
            if (type.HasMethods)
            {
                for (int i = 0; i < type.Methods.Count; i++)
                {
                    if (type.Methods[i].HasAttribute<TimedAttribute>())
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
                if (type.Methods[i].HasAttribute<TimedAttribute>())
                {
                    instructions.Clear();
                    MethodDefinition method = type.Methods[i];

                    string message = CecilAttributesSettings.Instance.TimedMethodFormat.FormatMessageTimed(type, method);

                    method.Body.InitLocals = true;

                    int stopwatchIndex = method.Body.Variables.Count;

                    method.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(Stopwatch))));

                    ILProcessor il = method.Body.GetILProcessor();

                    instructions.Add(Instruction.Create(OpCodes.Newobj, module.ImportReference(typeof(Stopwatch).GetConstructor(Type.EmptyTypes))));
                    instructions.Add(GetStloc(stopwatchIndex));

                    instructions.Add(GetLdloc(stopwatchIndex));
                    instructions.Add(Instruction.Create(OpCodes.Callvirt, module.ImportReference(typeof(Stopwatch).GetMethod("Start", Type.EmptyTypes))));

                    for (int j = instructions.Count - 1; j >= 0; j--)
                    {
                        il.InsertBefore(il.Body.Instructions[0], instructions[j]);
                    }

                    instructions.Clear();

                    instructions.Add(GetLdloc(stopwatchIndex));
                    instructions.Add(Instruction.Create(OpCodes.Callvirt, module.ImportReference(typeof(Stopwatch).GetMethod("Stop", Type.EmptyTypes))));

                    instructions.Add(Instruction.Create(OpCodes.Ldstr, message));

                    instructions.Add(GetLdloc(stopwatchIndex));
                    instructions.Add(Instruction.Create(OpCodes.Callvirt, module.ImportReference(typeof(Stopwatch).GetMethod("get_ElapsedMilliseconds", Type.EmptyTypes))));
                    instructions.Add(Instruction.Create(OpCodes.Box, module.ImportReference(typeof(long))));

                    instructions.Add(GetLdloc(stopwatchIndex));
                    instructions.Add(Instruction.Create(OpCodes.Callvirt, module.ImportReference(typeof(Stopwatch).GetMethod("get_ElapsedTicks", Type.EmptyTypes))));
                    instructions.Add(Instruction.Create(OpCodes.Box, module.ImportReference(typeof(long))));

                    instructions.Add(Instruction.Create(OpCodes.Call, module.ImportReference(typeof(string).GetMethod("Format", new Type[] { typeof(string), typeof(object), typeof(object) }))));
                    instructions.Add(Instruction.Create(OpCodes.Call, module.ImportReference(typeof(Debug).GetMethod("Log", new Type[] { typeof(object) }))));

                    for (int j = 0; j < instructions.Count; j++)
                    {
                        il.InsertBefore(il.Body.Instructions[il.Body.Instructions.Count - 1], instructions[j]);
                    }

                    dirty = true;
                }
            }

            return (true, dirty);
        }

        private Instruction GetStloc(int index)
        {
            switch (index)
            {
                case 0:
                    return Instruction.Create(OpCodes.Stloc_0);
                case 1:
                    return Instruction.Create(OpCodes.Stloc_1);
                case 2:
                    return Instruction.Create(OpCodes.Stloc_2);
                case 3:
                    return Instruction.Create(OpCodes.Stloc_3);
                default:
                    return Instruction.Create(OpCodes.Stloc_S, index);
            }
        }

        private Instruction GetLdloc(int index)
        {
            switch (index)
            {
                case 0:
                    return Instruction.Create(OpCodes.Ldloc_0);
                case 1:
                    return Instruction.Create(OpCodes.Ldloc_1);
                case 2:
                    return Instruction.Create(OpCodes.Ldloc_2);
                case 3:
                    return Instruction.Create(OpCodes.Ldloc_3);
                default:
                    return Instruction.Create(OpCodes.Ldloc_S, index);
            }
        }
    }
}
