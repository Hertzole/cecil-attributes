﻿using System;
using System.Collections.Generic;
using Hertzole.CecilAttributes.Interfaces;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hertzole.CecilAttributes.CodeGen
{
	public class GetComponentProcessor : BaseProcessor
	{
		private const string FETCH_COMPONENTS = "__CECIL__ATTRIBUTES__GENERATED__FetchComponents";
		public override string Name { get { return nameof(GetComponentProcessor); } }
		public override bool NeedsMonoBehaviour { get { return true; } }
		public override bool AllowEditor { get { return false; } }
		public override bool IncludeInBuild { get { return false; } }

		private readonly List<(FieldDefinition field, GetComponentTarget target, bool includeInactive)> targetFields = new List<(FieldDefinition, GetComponentTarget, bool)>();

		public override bool IsValidType()
		{
			if (Type.HasFields)
			{
				foreach (FieldDefinition field in Type.Fields)
				{
					if (field.HasAttribute<GetComponentAttribute>())
					{
						return true;
					}
				}
			}

			return false;
		}

		public override void ProcessType()
		{
			if (!SetInterface(Type, out MethodDefinition fetchComponentsMethod, out MethodDefinition parentFetchComponents, out bool isChild))
			{
				return;
			}
			
			targetFields.Clear();
			
			foreach (FieldDefinition field in Type.Fields)
			{
				if (field.TryGetAttribute<GetComponentAttribute>(out CustomAttribute attribute))
				{
					targetFields.Add((
						field, 
						attribute.GetField(nameof(GetComponentAttribute.target), GetComponentTarget.Self),
						attribute.GetField(nameof(GetComponentAttribute.includeInactive), false)));
				}
			}

			ProcessFields(fetchComponentsMethod, parentFetchComponents, isChild);
		}

		private static bool SetInterface(TypeDefinition type, out MethodDefinition fetchComponentsMethod, out MethodDefinition parentFetchComponents, out bool isChild)
		{
			isChild = type.TryGetMethodInBaseType(FETCH_COMPONENTS, out parentFetchComponents);

			if(!isChild && type.ImplementsInterface<IGetComponent>())
			{
				fetchComponentsMethod = null;
				return false;
			}
			
			fetchComponentsMethod = type.AddMethod(
				"__CECIL__ATTRIBUTES__GENERATED__FetchComponents",
				MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual);
			
			if (!isChild)
			{
				fetchComponentsMethod.Attributes |= MethodAttributes.NewSlot;
				
				type.Interfaces.Add(new InterfaceImplementation(type.Module.GetTypeReference<IGetComponent>()));
				MethodDefinition interfaceMethod = type.AddMethodOverride(
					"Hertzole.CecilAttributes.Interfaces.IGetComponent.FetchComponents", 
					MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
					type.Module.GetMethod<IGetComponent>(nameof(IGetComponent.FetchComponents)));
				
				ILProcessor il = interfaceMethod.BeginEdit();
				
				// __CECIL__ATTRIBUTES__GENERATED__FetchComponents()
				il.EmitLdarg();
				il.Emit(OpCodes.Callvirt, fetchComponentsMethod);
				il.Emit(OpCodes.Ret);
				
				interfaceMethod.EndEdit();
			}

			return true;
		}

		private void ProcessFields(MethodDefinition method, MethodReference parentMethod, bool isChild)
		{
			ILProcessor il = method.BeginEdit();

			// Create the method in reverse.

			Instruction previous = Instruction.Create(OpCodes.Ret);
			il.Append(previous);

			for (int i = targetFields.Count - 1; i >= 0; i--)
			{
				// field = GetComponent<Type>()
				if (!targetFields[i].field.FieldType.IsList())
				{
					il.InsertAt(0, Instruction.Create(OpCodes.Stfld, targetFields[i].field));
				}

				if (targetFields[i].field.FieldType.IsArray())
				{
					il.InsertAt(0, Instruction.Create(OpCodes.Call, GetComponentMethod(targetFields[i].field.FieldType.GetCollectionType(), targetFields[i].target, true, false)));
				}
				else if (targetFields[i].field.FieldType.IsList())
				{
					il.InsertAt(0, Instruction.Create(OpCodes.Call, GetComponentMethod(targetFields[i].field.FieldType.GetCollectionType(), targetFields[i].target, true, true)));
					il.InsertAt(0, Instruction.Create(OpCodes.Ldfld, targetFields[i].field));
				}
				else
				{
					il.InsertAt(0, Instruction.Create(OpCodes.Call, GetComponentMethod(targetFields[i].field.FieldType, targetFields[i].target, false, false)));
				}
				
				// This is dumb. Why can't the bool be on the same place on both lists and arrays??
				if ((targetFields[i].field.FieldType.IsArray() && targetFields[i].target != GetComponentTarget.Self) || (!targetFields[i].field.FieldType.IsCollection() && targetFields[i].target == GetComponentTarget.Children))
				{
					il.InsertAt(0, ILHelper.Bool(targetFields[i].includeInactive));
				}
				
				il.InsertAt(0, ILHelper.Ldarg(il));

				if (targetFields[i].field.FieldType.IsList() && targetFields[i].target != GetComponentTarget.Self)
				{
					il.InsertAt(0, ILHelper.Bool(targetFields[i].includeInactive));
				}
				
				Instruction insideIfCheck = il.InsertAt(0, ILHelper.Ldarg(il));

				if (targetFields[i].field.FieldType.IsList())
				{
					// list = new List<Type>()
					il.InsertAt(0, Instruction.Create(OpCodes.Stfld, targetFields[i].field));
					il.InsertAt(0, Instruction.Create(OpCodes.Newobj, 
						Module.GetConstructor(typeof(List<>), System.Type.EmptyTypes)
						      .MakeHostInstanceGeneric(Module.GetTypeReference(typeof(List<>)).MakeGenericInstanceType(targetFields[i].field.FieldType.GetCollectionType()))));
					il.InsertAt(0, ILHelper.Ldarg(il));

					// if (list == null)
					il.InsertAt(0, Instruction.Create(OpCodes.Brtrue, insideIfCheck));
					il.InsertAt(0, Instruction.Create(OpCodes.Ldfld, targetFields[i].field));
					insideIfCheck = il.InsertAt(0, ILHelper.Ldarg(il));
				}

				if (targetFields[i].field.FieldType.IsArray() || targetFields[i].field.FieldType.IsList())
				{
					// if (field == null || field.Length/Count == 0)
					il.InsertAt(0, Instruction.Create(OpCodes.Brtrue, previous));
					// If it's an array, load the length.
					if (targetFields[i].field.FieldType.IsArray())
					{
						il.InsertAt(0, Instruction.Create(OpCodes.Ldlen));
					}
					else if (targetFields[i].field.FieldType.IsList()) // Else load the count
					{
						il.InsertAt(0, Instruction.Create(OpCodes.Callvirt, 
							Module.GetMethod(typeof(List<>), "get_Count")
							      .MakeHostInstanceGeneric(Module.GetTypeReference(typeof(List<>)).MakeGenericInstanceType(targetFields[i].field.FieldType.GetCollectionType()))));
					}

					il.InsertAt(0, Instruction.Create(OpCodes.Ldfld, targetFields[i].field));
					il.InsertAt(0, ILHelper.Ldarg(il));

					il.InsertAt(0, Instruction.Create(OpCodes.Brfalse, insideIfCheck));
					il.InsertAt(0, Instruction.Create(OpCodes.Ldfld, targetFields[i].field));
					il.InsertAt(0, ILHelper.Ldarg(il));
				}
				else
				{
					// if (field == null)
					il.InsertAt(0, Instruction.Create(OpCodes.Brfalse, previous));
					il.InsertAt(0, Instruction.Create(OpCodes.Call, Module.GetMethod<Object>("op_Equality", typeof(Object), typeof(Object))));
					il.InsertAt(0, Instruction.Create(OpCodes.Ldnull));
					il.InsertAt(0, Instruction.Create(OpCodes.Ldfld, targetFields[i].field));
					previous = il.InsertAt(0, ILHelper.Ldarg(il));
				}
			}
			
			if (isChild)
			{
				// base.FetchComponents()
				il.InsertAt(0, Instruction.Create(OpCodes.Call, parentMethod));
				il.InsertAt(0, ILHelper.Ldarg(il));
			}
			
			method.EndEdit();
		}

		private static readonly Type[] onlyBools = new Type[1]
		{
			typeof(bool)
		};

		private static readonly Type[] onlyList = new Type[1]
		{
			typeof(List<>)
		};

		private static readonly Type[] boolsAndList = new Type[2]
		{
			typeof(bool),
			typeof(List<>)
		};
		
		private MethodReference GetComponentMethod(TypeReference fieldType, GetComponentTarget target, bool collection, bool list)
		{
			if (collection)
			{
				if (!list)
				{
					switch (target)
					{
						case GetComponentTarget.Self:
							return Module.GetMethod<Component>(nameof(Component.GetComponents), System.Type.EmptyTypes).MakeGenericMethod(fieldType);
						case GetComponentTarget.Parent:
							return Module.GetMethod<Component>(nameof(Component.GetComponentsInParent), onlyBools).MakeGenericMethod(fieldType);
						case GetComponentTarget.Children:
							return Module.GetMethod<Component>(nameof(Component.GetComponentsInChildren), onlyBools).MakeGenericMethod(fieldType);
						default:
							throw new ArgumentOutOfRangeException(nameof(target), target, null);
					}
				}
				else
				{
					switch (target)
					{
						case GetComponentTarget.Self:
							return Module.GetGenericMethod<Component>(nameof(Component.GetComponents), onlyList).MakeGenericMethod(fieldType);
						case GetComponentTarget.Parent:
							return Module.GetGenericMethod<Component>(nameof(Component.GetComponentsInParent), boolsAndList).MakeGenericMethod(fieldType);
						case GetComponentTarget.Children:
							return Module.GetGenericMethod<Component>(nameof(Component.GetComponentsInChildren), boolsAndList).MakeGenericMethod(fieldType);
						default:
							throw new ArgumentOutOfRangeException(nameof(target), target, null);
					}
				}
			}

			switch (target)
			{
				case GetComponentTarget.Self:
					return Module.GetMethod<Component>(nameof(Component.GetComponent), System.Type.EmptyTypes).MakeGenericMethod(fieldType);
				case GetComponentTarget.Parent:
					return Module.GetMethod<Component>(nameof(Component.GetComponentInParent), System.Type.EmptyTypes).MakeGenericMethod(fieldType);
				case GetComponentTarget.Children:
					return Module.GetMethod<Component>(nameof(Component.GetComponentInChildren), onlyBools).MakeGenericMethod(fieldType);
				default:
					throw new ArgumentOutOfRangeException(nameof(target), target, null);
			}
		}
	}
}