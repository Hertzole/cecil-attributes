// using Mono.Cecil;
// using Mono.Cecil.Cil;
// using System;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;
//
// namespace Hertzole.CecilAttributes.Editor
// {
//     public class FindPropertyProcessor : BaseProcessor
//     {
//         private List<MemberData> members = new List<MemberData>();
//         private List<Instruction> instructions = new List<Instruction>();
//         private List<Instruction> firstInstructions = new List<Instruction>();
//         private List<Tuple<Instruction, Instruction>> ifsLast = new List<Tuple<Instruction, Instruction>>();
//
//         public override string Name { get { return "FindProperty"; } }
//
//         public override bool NeedsMonoBehaviour { get { return false; } }
//         public override bool AllowEditor { get { return true; } }
//         public override bool EditorOnly { get { return true; } }
//
//         public override bool IsValidClass(TypeDefinition type)
//         {
//             if (type.HasFields)
//             {
//                 for (int i = 0; i < type.Fields.Count; i++)
//                 {
//                     if (type.Fields[i].HasAttribute<FindProperty>())
//                     {
//                         return true;
//                     }
//                 }
//             }
//
//             if (type.HasProperties)
//             {
//                 for (int i = 0; i < type.Properties.Count; i++)
//                 {
//                     if (type.Properties[i].HasAttribute<FindProperty>())
//                     {
//                         return true;
//                     }
//                 }
//             }
//
//             return false;
//         }
//
//         public override (bool success, bool dirty) ProcessClass(ModuleDefinition module, TypeDefinition type)
//         {
//             members.Clear();
//             instructions.Clear();
//             firstInstructions.Clear();
//             ifsLast.Clear();
//
//             if (type.HasFields)
//             {
//                 for (int i = 0; i < type.Fields.Count; i++)
//                 {
//                     if (type.Fields[i].HasAttribute<FindProperty>())
//                     {
//                         members.Add(new MemberData(type.Fields[i]));
//                     }
//                 }
//             }
//
//             if (type.HasProperties)
//             {
//                 for (int i = 0; i < type.Properties.Count; i++)
//                 {
//                     if (type.Properties[i].HasAttribute<FindProperty>())
//                     {
//                         members.Add(new MemberData(type.Properties[i]));
//                     }
//                 }
//             }
//
//             if (members.Count == 0)
//             {
//                 // There are no valid fields/properties. Stop here.
//                 return (true, false);
//             }
//
//             bool createdOnEnable = false;
//
//             if (!type.TryGetMethod("OnEnable", out MethodDefinition enableMethod))
//             {
//                 enableMethod = new MethodDefinition("OnEnable", MethodAttributes.Private | MethodAttributes.HideBySig, module.ImportReference(typeof(void)));
//                 type.Methods.Add(enableMethod);
//                 createdOnEnable = true;
//             }
//
//             MethodReference getSerializedObject = module.ImportReference(type.GetMethodInBaseType("get_serializedObject"));
//             MethodReference getTarget = module.ImportReference(type.GetMethodInBaseType("get_target"));
//             MethodReference findProperty = module.ImportReference(typeof(SerializedObject).GetMethod("FindProperty", new Type[] { typeof(string) }));
//             MethodReference findPropertyRelative = module.ImportReference(typeof(SerializedProperty).GetMethod("FindPropertyRelative", new Type[] { typeof(string) }));
//             MethodReference logError = module.ImportReference(typeof(Debug).GetMethod("LogError", new Type[] { typeof(object) }));
//
//             for (int i = 0; i < members.Count; i++)
//             {
//                 CustomAttribute attribute = members[i].GetAttribute<FindProperty>();
//                 string path = attribute.GetConstructorArgument(0, string.Empty);
//
//                 Instruction beforeGetProperty = Instruction.Create(OpCodes.Ldarg_0);
//                 string[] paths = string.IsNullOrWhiteSpace(path) ? (new string[1] { members[i].Name }) : path.Split('/');
//
//                 for (int j = 0; j < paths.Length; j++)
//                 {
//                     firstInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
//                 }
//
//                 for (int j = 0; j < paths.Length; j++)
//                 {
//                     instructions.Add(firstInstructions[j]);
//                     instructions.Add(Instruction.Create(OpCodes.Call, getSerializedObject));
//
//                     for (int k = 0; k <= j; k++)
//                     {
//                         instructions.Add(Instruction.Create(OpCodes.Ldstr, paths[k]));
//                         Instruction last = Instruction.Create(OpCodes.Call, k == 0 ? findProperty : findPropertyRelative);
//                         instructions.Add(last);
//
//                         if (k == j)
//                         {
//                             ifsLast.Add(new Tuple<Instruction, Instruction>(last, j == paths.Length - 1 ? beforeGetProperty : firstInstructions[j + 1]));
//                         }
//                     }
//
//                     instructions.Add(Instruction.Create(OpCodes.Ldstr, "There's no serialized property called '" + paths[j] + "' on {0}"));
//                     instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
//                     instructions.Add(Instruction.Create(OpCodes.Call, getTarget));
//                     instructions.Add(Instruction.Create(OpCodes.Call, LogCalledProcessor.GetStringFormatMethod(module, 1)));
//                     instructions.Add(Instruction.Create(OpCodes.Call, logError));
//                     instructions.Add(Instruction.Create(OpCodes.Ret));
//                 }
//
//                 instructions.Add(beforeGetProperty);
//                 instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
//                 instructions.Add(Instruction.Create(OpCodes.Call, getSerializedObject));
//
//                 for (int j = 0; j < paths.Length; j++)
//                 {
//                     instructions.Add(Instruction.Create(OpCodes.Ldstr, paths[j]));
//                     instructions.Add(Instruction.Create(OpCodes.Callvirt, j == 0 ? findProperty : findPropertyRelative));
//                 }
//
//                 if (members[i].IsProperty)
//                 {
//                     instructions.Add(Instruction.Create(OpCodes.Call, members[i].property.SetMethod));
//                 }
//                 else
//                 {
//                     instructions.Add(Instruction.Create(OpCodes.Stfld, members[i].field));
//                 }
//             }
//
//             ILProcessor il = enableMethod.Body.GetILProcessor();
//             for (int i = 0; i < instructions.Count; i++)
//             {
//                 if (createdOnEnable)
//                 {
//                     il.Append(instructions[i]);
//                 }
//                 else
//                 {
//                     il.InsertBefore(enableMethod.Body.Instructions[i], instructions[i]);
//                 }
//             }
//
//             if (createdOnEnable)
//             {
//                 il.Emit(OpCodes.Ret);
//             }
//
//             for (int i = 0; i < ifsLast.Count; i++)
//             {
//                 il.InsertAfter(ifsLast[i].Item1, Instruction.Create(OpCodes.Brtrue_S, ifsLast[i].Item2));
//             }
//
//             return (true, true);
//         }
//     }
// }
