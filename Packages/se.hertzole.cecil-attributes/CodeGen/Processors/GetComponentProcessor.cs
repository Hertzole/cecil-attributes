// using Mono.Cecil;
// using Mono.Cecil.Cil;
// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace Hertzole.CecilAttributes.Editor
// {
//     public class GetComponentProcessor : BaseProcessor
//     {
//         public override string Name { get { return nameof(GetComponentProcessor); } }
//         public override bool NeedsMonoBehaviour { get { return true; } }
//         public override bool AllowEditor { get { return true; } }
//         public override bool IncludeInBuild { get { return false; } }
//
//         public override bool IsValidClass(TypeDefinition type)
//         {
//             if (type.HasFields)
//             {
//                 for (int i = 0; i < type.Fields.Count; i++)
//                 {
//                     if (type.Fields[i].HasAttribute<GetComponentAttribute>())
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
//             ILProcessor il = null;
//
//             if (type.TryGetMethodInBaseType("Reset", out MethodDefinition resetMethod, Array.Empty<TypeDefinition>()))
//             {
//                 //TODO: Check if base type
//             }
//             else
//             {
//                 resetMethod = new MethodDefinition("Reset", MethodAttributes.Private | MethodAttributes.HideBySig, module.ImportReference(typeof(void)));
//                 il = resetMethod.Body.GetILProcessor();
//                 il.Emit(OpCodes.Ret);
//                 type.Methods.Add(resetMethod);
//             }
//
//             if (type.TryGetMethodInBaseType("OnValidate", out MethodDefinition validateMethod, Array.Empty<TypeDefinition>()))
//             {
//                 //TODO: Check if base type
//
//             }
//             else
//             {
//                 validateMethod = new MethodDefinition("OnValidate", MethodAttributes.Private | MethodAttributes.HideBySig, module.ImportReference(typeof(void)));
//                 il = validateMethod.Body.GetILProcessor();
//                 il.Emit(OpCodes.Ret);
//                 type.Methods.Add(validateMethod);
//             }
//
//             MethodDefinition getComponentsMethod = CreateGetComponentMethod(module, type);
//             type.Methods.Add(getComponentsMethod);
//
//             InsertCallToMethod(resetMethod, getComponentsMethod);
//             InsertCallToMethod(validateMethod, getComponentsMethod);
//
//             return (true, true);
//         }
//
//         private MethodDefinition CreateGetComponentMethod(ModuleDefinition module, TypeDefinition type)
//         {
//             MethodDefinition m = new MethodDefinition($"CecilAttributes__Generated__Get__Standard__Components__{type.Name}",
//                 MethodAttributes.Private | MethodAttributes.HideBySig, module.ImportReference(typeof(void)));
//
//             ILProcessor il = m.Body.GetILProcessor();
//
//             Instruction ret = Instruction.Create(OpCodes.Ret);
//
//             List<Instruction> ifStarts = new List<Instruction>();
//             List<FieldDefinition> fields = new List<FieldDefinition>();
//
//             for (int i = 0; i < type.Fields.Count; i++)
//             {
//                 if (type.Fields[i].HasAttribute<GetComponentAttribute>())
//                 {
//                     ifStarts.Add(Instruction.Create(OpCodes.Ldarg_0));
//                     fields.Add(type.Fields[i]);
//                 }
//             }
//
//             for (int i = 0; i < fields.Count; i++)
//             {
//                 WriteField(fields[i], module, il, ifStarts[i], i == fields.Count - 1 ? ret : ifStarts[i + 1]);
//             }
//
//             il.Append(ret);
//
//             return m;
//         }
//
//         private void WriteField(FieldDefinition field, ModuleDefinition module, ILProcessor il, Instruction start, Instruction next)
//         {
//             GetComponentTarget target = field.GetAttribute<GetComponentAttribute>().GetField("target", GetComponentTarget.Self);
//
//             MethodReference getComponentCall = null;
//
//             switch (target)
//             {
//                 case GetComponentTarget.Self:
//                     getComponentCall = module.ImportReference(typeof(Component).GetMethod("GetComponent", Type.EmptyTypes)).MakeGenericMethod(field.FieldType);
//                     break;
//                 case GetComponentTarget.Parent:
//                     getComponentCall = module.ImportReference(typeof(Component).GetMethod("GetComponentInParent", Type.EmptyTypes)).MakeGenericMethod(field.FieldType);
//                     break;
//                 case GetComponentTarget.Children:
//                     getComponentCall = module.ImportReference(typeof(Component).GetMethod("GetComponentInChildren", Type.EmptyTypes)).MakeGenericMethod(field.FieldType);
//                     break;
//             }
//
//             il.Append(start);
//             il.Emit(OpCodes.Ldfld, field);
//             il.Emit(OpCodes.Brtrue_S, next);
//
//             il.Emit(OpCodes.Ldarg_0);
//             il.Emit(OpCodes.Ldarg_0);
//             il.Emit(OpCodes.Call, getComponentCall);
//             il.Emit(OpCodes.Stfld, field);
//         }
//
//         private void InsertCallToMethod(MethodDefinition targetMethod, MethodDefinition methodToInsert)
//         {
//             ILProcessor il = targetMethod.Body.GetILProcessor();
//             Instruction[] instructions = new Instruction[]
//             {
//                 Instruction.Create(OpCodes.Ldarg_0),
//                 Instruction.Create(OpCodes.Call, methodToInsert)
//             };
//
//             il.InsertBefore(il.Body.Instructions[0], instructions);
//         }
//     }
// }
