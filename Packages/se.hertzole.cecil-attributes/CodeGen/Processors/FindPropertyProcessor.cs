using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEditor;
using UnityEngine;

namespace Hertzole.CecilAttributes.CodeGen
{
    public class FindPropertyProcessor : BaseProcessor
    {
        private List<MemberData> members = new List<MemberData>();
        private List<Instruction> instructions = new List<Instruction>();
        private List<Instruction> firstInstructions = new List<Instruction>();
        private List<Tuple<Instruction, Instruction>> ifsLast = new List<Tuple<Instruction, Instruction>>();

        public override string Name { get { return "FindProperty"; } }

        public override bool NeedsMonoBehaviour { get { return false; } }
        public override bool AllowEditor { get { return true; } }
        public override bool EditorOnly { get { return true; } }

        public override bool IsValidType()
        {
            if (Type.HasFields)
            {
                for (int i = 0; i < Type.Fields.Count; i++)
                {
                    if (Type.Fields[i].HasAttribute<FindProperty>())
                    {
                        return true;
                    }
                }
            }

            if (Type.HasProperties)
            {
                for (int i = 0; i < Type.Properties.Count; i++)
                {
                    if (Type.Properties[i].HasAttribute<FindProperty>())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override void ProcessType()
        {
            members.Clear();
            instructions.Clear();
            firstInstructions.Clear();
            ifsLast.Clear();

            if (Type.HasFields)
            {
                for (int i = 0; i < Type.Fields.Count; i++)
                {
                    if (Type.Fields[i].HasAttribute<FindProperty>())
                    {
                        members.Add(new MemberData(Type.Fields[i]));
                    }
                }
            }

            if (Type.HasProperties)
            {
                for (int i = 0; i < Type.Properties.Count; i++)
                {
                    if (Type.Properties[i].HasAttribute<FindProperty>())
                    {
                        members.Add(new MemberData(Type.Properties[i]));
                    }
                }
            }

            if (members.Count == 0)
            {
                // There are no valid fields/properties. Stop here.
                return;
            }

            bool createdOnEnable = false;

            if (!Type.TryGetMethod("OnEnable", out MethodDefinition enableMethod))
            {
                enableMethod = new MethodDefinition("OnEnable", MethodAttributes.Private | MethodAttributes.HideBySig, Module.ImportReference(typeof(void)));
                Type.Methods.Add(enableMethod);
                createdOnEnable = true;
            }

            MethodReference getSerializedObject = Module.ImportReference(Type.GetMethodInBaseType("get_serializedObject"));
            MethodReference getTarget = Module.ImportReference(Type.GetMethodInBaseType("get_target"));
            MethodReference findProperty = Module.ImportReference(typeof(SerializedObject).GetMethod("FindProperty", new Type[] { typeof(string) }));
            MethodReference findPropertyRelative = Module.ImportReference(typeof(SerializedProperty).GetMethod("FindPropertyRelative", new Type[] { typeof(string) }));
            MethodReference logError = Module.ImportReference(typeof(Debug).GetMethod("LogError", new Type[] { typeof(object) }));

            for (int i = 0; i < members.Count; i++)
            {
                CustomAttribute attribute = members[i].GetAttribute<FindProperty>();
                string path = attribute.GetConstructorArgument(0, string.Empty);

                Instruction beforeGetProperty = Instruction.Create(OpCodes.Ldarg_0);
                string[] paths = string.IsNullOrWhiteSpace(path) ? (new string[1] { members[i].Name }) : path.Split('/');

                for (int j = 0; j < paths.Length; j++)
                {
                    firstInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                }

                for (int j = 0; j < paths.Length; j++)
                {
                    instructions.Add(firstInstructions[j]);
                    instructions.Add(Instruction.Create(OpCodes.Call, getSerializedObject));

                    for (int k = 0; k <= j; k++)
                    {
                        instructions.Add(Instruction.Create(OpCodes.Ldstr, paths[k]));
                        Instruction last = Instruction.Create(OpCodes.Call, k == 0 ? findProperty : findPropertyRelative);
                        instructions.Add(last);

                        if (k == j)
                        {
                            ifsLast.Add(new Tuple<Instruction, Instruction>(last, j == paths.Length - 1 ? beforeGetProperty : firstInstructions[j + 1]));
                        }
                    }

                    instructions.Add(Instruction.Create(OpCodes.Ldstr, $"There's no serialized property called '{paths[j]}' on {{0}}"));
                    instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                    instructions.Add(Instruction.Create(OpCodes.Call, getTarget));
                    instructions.Add(Instruction.Create(OpCodes.Call, MethodsCache.GetStringFormat(1)));
                    instructions.Add(Instruction.Create(OpCodes.Call, logError));
                    instructions.Add(Instruction.Create(OpCodes.Ret));
                }

                instructions.Add(beforeGetProperty);
                instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                instructions.Add(Instruction.Create(OpCodes.Call, getSerializedObject));

                for (int j = 0; j < paths.Length; j++)
                {
                    instructions.Add(Instruction.Create(OpCodes.Ldstr, paths[j]));
                    instructions.Add(Instruction.Create(OpCodes.Callvirt, j == 0 ? findProperty : findPropertyRelative));
                }

                if (members[i].IsProperty)
                {
                    instructions.Add(Instruction.Create(OpCodes.Call, members[i].property.SetMethod));
                }
                else
                {
                    instructions.Add(Instruction.Create(OpCodes.Stfld, members[i].field));
                }
            }

            ILProcessor il = enableMethod.Body.GetILProcessor();
            for (int i = 0; i < instructions.Count; i++)
            {
                if (createdOnEnable)
                {
                    il.Append(instructions[i]);
                }
                else
                {
                    il.InsertBefore(enableMethod.Body.Instructions[i], instructions[i]);
                }
            }

            if (createdOnEnable)
            {
                il.Emit(OpCodes.Ret);
            }

            for (int i = 0; i < ifsLast.Count; i++)
            {
                il.InsertAfter(ifsLast[i].Item1, Instruction.Create(OpCodes.Brtrue_S, ifsLast[i].Item2));
            }
        }
    }
}
