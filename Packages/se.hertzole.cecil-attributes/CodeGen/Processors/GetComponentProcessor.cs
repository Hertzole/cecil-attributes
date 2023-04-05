#if CECIL_ATTRIBUTES_EXPERIMENTAL_GETCOMPONENT
#define EXPERIMENTAL
#endif

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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
		private readonly struct FieldInfo
		{
			public readonly FieldDefinition field;
			public readonly GetComponentTarget target;
			public readonly bool includeInactive;

			public FieldInfo(FieldDefinition field, GetComponentTarget target, bool includeInactive)
			{
				this.field = field;
				this.target = target;
				this.includeInactive = includeInactive;
			}
		}
		
		private const string FETCH_COMPONENTS = "__CECIL__ATTRIBUTES__GENERATED__FetchComponents";
		public override string Name { get { return nameof(GetComponentProcessor); } }
		public override bool NeedsMonoBehaviour { get { return true; } }
		public override bool AllowEditor { get { return false; } }
		public override bool IncludeInBuild { get { return false; } }

#if !EXPERIMENTAL
		private readonly List<(FieldDefinition field, GetComponentTarget target, bool includeInactive)> targetFields = new List<(FieldDefinition, GetComponentTarget, bool)>();
		private readonly List<TypeDefinition> parents = new List<TypeDefinition>();
#endif

		public override bool IsValidType()
		{
			return IsValidType(Type);
		}

		private static bool IsValidType(TypeDefinition type)
		{
			if (type.HasFields)
			{
				foreach (FieldDefinition field in type.Fields)
				{
					if (field.HasAttribute<GetComponentAttribute>())
					{
						return true;
					}
				}
			}

			return false;
		}

#if EXPERIMENTAL
		public override void ProcessType()
		{
			if (!Type.TryFindRoot(FindRootWithAttributePredicate, out TypeDefinition attributeRoot, out int attributeDepth))
			{
				attributeRoot = Type;
			}

			if (attributeRoot == null)
			{
				throw new NullReferenceException("No attribute root");
			}

			if (ShouldProcess(attributeRoot))
			{
				ProcessType(attributeRoot, attributeDepth);
				MarkAsProcessed(attributeRoot);
			}

			if (ShouldProcess(Type))
			{
				ProcessType(Type, 0);
				MarkAsProcessed(Type);
			}
		}

		private void ProcessType(TypeDefinition attributeRoot, int attributeDepth)
		{
			if (!Type.TryFindRoot(FindRootWithSerializationCallbackPredicate, out TypeDefinition serializationRoot, out int serializationDepth))
			{
				serializationRoot = Type;
			}
			
			if (serializationRoot == null)
			{
				throw new NullReferenceException("No serialization root");
			}

			using (ListPool<FieldInfo>.Get(out List<FieldInfo> fieldsList))
			{
				// Find all fields with the attribute.
				foreach (FieldDefinition field in attributeRoot.Fields)
				{
					if (field.TryGetAttribute<GetComponentAttribute>(out CustomAttribute attribute))
					{
						GetComponentTarget target = attribute.GetField(nameof(GetComponentAttribute.target), GetComponentTarget.Self);
						bool includeInactive = attribute.GetField(nameof(GetComponentAttribute.includeInactive), true);
						fieldsList.Add(new FieldInfo(field, target, includeInactive));
					}
				}

				GetOrAddSerializationInterface(serializationRoot, out MethodDefinition beforeSerialize);

				if (beforeSerialize == null)
				{
					throw new NullReferenceException("No before serialize method");
				}
			
				// Add the fetch method.
				MethodDefinition fetchComponents = GetOrAddFetchComponentsMethod(attributeRoot, serializationRoot, beforeSerialize);

				ProcessFields(fetchComponents, fieldsList);
			}
		}
		
		private static void GetOrAddSerializationInterface(TypeDefinition type, out MethodDefinition beforeSerialize)
		{
			beforeSerialize = null;
			
			if (!type.ImplementsInterface<ISerializationCallbackReceiver>())
			{
				type.Interfaces.Add(new InterfaceImplementation(type.Module.ImportReference(typeof(ISerializationCallbackReceiver))));
				
				beforeSerialize = type.AddMethodOverride("ISerializationCallbackReceiver.OnBeforeSerialize", 
					MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
					type.Module.ImportReference(typeof(ISerializationCallbackReceiver).GetMethod(nameof(ISerializationCallbackReceiver.OnBeforeSerialize))));
				
				beforeSerialize.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
				
				MethodDefinition afterDeserialize = type.AddMethodOverride("ISerializationCallbackReceiver.OnAfterDeserialize", 
					MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
					type.Module.ImportReference(typeof(ISerializationCallbackReceiver).GetMethod(nameof(ISerializationCallbackReceiver.OnAfterDeserialize))));
				
				afterDeserialize.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
			}
			else
			{
				if(!type.TryGetMethod("OnBeforeSerialize", out beforeSerialize))
				{
					beforeSerialize = type.GetMethod("ISerializationCallbackReceiver.OnBeforeSerialize");
				}
			}
		}

		private MethodDefinition GetOrAddFetchComponentsMethod(TypeDefinition type, TypeDefinition serializationRoot, MethodDefinition beforeSerialize)
		{
			MethodDefinition fetchComponents;
			
			if (type.TryGetMethodInBaseType(FETCH_COMPONENTS, out MethodDefinition parentFetchComponents))
			{
				fetchComponents = new MethodDefinition(FETCH_COMPONENTS, MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual, Module.TypeSystem.Void);

				fetchComponents.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));

				using (MethodEntryScope il = new MethodEntryScope(fetchComponents))
				{
					il.EmitLdarg();
					il.EmitCall(parentFetchComponents);
				}

				type.Methods.Add(fetchComponents);
				
				return fetchComponents;
			}
			
			fetchComponents = new MethodDefinition(FETCH_COMPONENTS, MethodAttributes.Family | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.HideBySig, Module.TypeSystem.Void);

			serializationRoot.Methods.Add(fetchComponents);

			fetchComponents.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
			
			using (MethodEntryScope il = new MethodEntryScope(beforeSerialize))
			{
				il.EmitLdarg();
				il.EmitCall(fetchComponents, true);
			}

			return fetchComponents;
		}

		private void ProcessFields(MethodDefinition method, List<FieldInfo> fields)
		{
			using (MethodEntryScope il = new MethodEntryScope(method))
			{
				using (ListPool<Instruction>.Get(out List<Instruction> jumpToInstructions))
				{
					for (int i = 0; i < fields.Count; i++)
					{
						jumpToInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
					}

					for (int i = 0; i < fields.Count; i++)
					{
						FieldInfo field = fields[i];

						if (!field.field.FieldType.IsArray() && !field.field.FieldType.IsList())
						{
							// if (field == null)
							il.Emit(jumpToInstructions[i]);
							il.EmitLoadField(field.field);
							il.EmitNull();
							il.EmitCall(method.DeclaringType.Module.ImportReference(MethodsCache.UnityObjectEqualityOperation));
							il.Emit(OpCodes.Brfalse, i == fields.Count - 1 ? il.First : jumpToInstructions[i + 1]);

							// field = GetComponent<fieldType>();
							il.EmitLdarg();
							il.EmitLdarg();

#if UNITY_2021_2_OR_NEWER // Include inactive does not exist in 2021.1 or older.
							if (field.target != GetComponentTarget.Self)
							{
								il.EmitBool(field.includeInactive);
							}
#endif

							il.EmitCall(GetComponentMethod(field.field.FieldType, field.target, false, false, field.field.DeclaringType.Module));
							il.Emit(OpCodes.Stfld, field.field);
						}
						else if (!field.field.FieldType.IsList() && field.field.FieldType.IsArray())
						{
							// field = GetComponents<fieldType>();
							il.Emit(jumpToInstructions[i]);
							il.EmitLdarg();

							if (field.target != GetComponentTarget.Self)
							{
								il.EmitBool(field.includeInactive);
							}

							il.EmitCall(GetComponentMethod(field.field.FieldType.GetCollectionType(), field.target, true, false, field.field.DeclaringType.Module));
							il.Emit(OpCodes.Stfld, field.field);
						}
						else if (field.field.FieldType.IsList())
						{
							Instruction jumpTo = ILHelper.Ldarg(null);

							// if (list == null)
							il.Emit(jumpToInstructions[i]);
							il.EmitLoadField(field.field);
							il.Emit(OpCodes.Brtrue, jumpTo);

							// list = new List<fieldType>();
							il.EmitLdarg();
							il.Emit(OpCodes.Newobj,
								Module.GetConstructor(typeof(List<>), System.Type.EmptyTypes)
								      .MakeHostInstanceGeneric(Module.GetTypeReference(typeof(List<>)).MakeGenericInstanceType(field.field.FieldType.GetCollectionType())));

							il.Emit(OpCodes.Stfld, field.field);

							// list.Clear();
							il.Emit(jumpTo);
							il.EmitLoadField(field.field);
							il.EmitCall(Module.GetMethod(typeof(List<>), "Clear").MakeHostInstanceGeneric((GenericInstanceType) field.field.FieldType), true);

							// GetComponent<fieldType>(list);
							il.EmitLdarg();

							if (field.target != GetComponentTarget.Self)
							{
								il.EmitBool(field.includeInactive);
							}

							il.EmitLdarg();
							il.EmitLoadField(field.field);

							il.EmitCall(GetComponentMethod(field.field.FieldType.GetCollectionType(), field.target, true, true, field.field.DeclaringType.Module));
						}
					}
				}
			}
		}

		private static bool FindRootWithAttributePredicate(TypeDefinition type)
		{
			if (!type.HasFields)
			{
				return false;
			}

			foreach (FieldDefinition field in type.Fields)
			{
				if (field.HasAttribute<GetComponentAttribute>())
				{
					return true;
				}
			}

			return false;
		}
		
		private static bool FindRootWithSerializationCallbackPredicate(TypeDefinition type)
		{
			if (!type.HasInterfaces)
			{
				return false;
			}
			
			foreach (InterfaceImplementation interfaceType in type.Interfaces)
			{
				if (interfaceType.InterfaceType.FullName == typeof(ISerializationCallbackReceiver).FullName)
				{
					return true;
				}
			}

			return false;
		}
#else
		public override void ProcessType()
		{
			ProcessType(Type, true);
		}

		private void ProcessType(TypeDefinition type, bool checkParents)
		{
			if (!SetInterface(checkParents, type, parents, out MethodDefinition fetchComponentsMethod, out MethodDefinition parentFetchComponents, out bool isChild))
			{
				return;
			}
			
			targetFields.Clear();
			
			foreach (FieldDefinition field in type.Fields)
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

		private bool SetInterface(bool checkParents, TypeDefinition type, IList<TypeDefinition> parentList, out MethodDefinition fetchComponentsMethod, out MethodDefinition parentFetchComponents, out bool isChild)
		{
			if (checkParents)
			{
				CheckParents(type, parentList);
			}

			// If it already implements IGetComponent or has the fetch components method, stop here.
			if (type.ImplementsInterface<IGetComponent>() || type.TryGetMethod(FETCH_COMPONENTS, out _))
			{
				fetchComponentsMethod = null;
				parentFetchComponents = null;
				isChild = false;
				return false;
			}

			// Try to fetch the parent fetch components method.
			// If it has it, we're a child of a class.
			isChild = type.TryGetMethodInBaseType(FETCH_COMPONENTS, out parentFetchComponents);

			fetchComponentsMethod = type.AddMethod(
				"__CECIL__ATTRIBUTES__GENERATED__FetchComponents",
				MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual,
				Module.GetTypeReference<bool>());
			
			// If we're not a child, implement the IGetComponent interface.
			if (!isChild)
			{
				fetchComponentsMethod.Attributes |= MethodAttributes.NewSlot;
				
				type.Interfaces.Add(new InterfaceImplementation(type.Module.GetTypeReference<IGetComponent>()));
				MethodDefinition interfaceMethod = type.AddMethodOverride(
					"Hertzole.CecilAttributes.Interfaces.IGetComponent.FetchComponents", 
					MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
					type.Module.GetTypeReference<bool>(),
					type.Module.GetMethod<IGetComponent>(nameof(IGetComponent.FetchComponents)));
				
				ILProcessor il = interfaceMethod.BeginEdit();
				
				// __CECIL__ATTRIBUTES__GENERATED__FetchComponents()
				il.EmitLdarg();
				il.EmitBool(false);
				il.Emit(OpCodes.Callvirt, fetchComponentsMethod);
				il.Emit(OpCodes.Ret);
				
				interfaceMethod.EndEdit();
			}

			return true;
		}

		private void CheckParents(TypeDefinition type, IList<TypeDefinition> parentList)
		{
			// Clear the list to make sure there are no left overs.
			parentList.Clear();
			
			// Go until we hit a null base type.
			TypeReference baseType = type.BaseType;
			while (baseType != null)
			{
				// Stop if it basically doesn't exist.
				if (!baseType.CanBeResolved())
				{
					break;
				}

				// Check if it's a valid type and doesn't implement IGetComponent.
				TypeDefinition resolved = baseType.Resolve();
				if (IsValidType(resolved) && !resolved.ImplementsInterface<IGetComponent>())
				{
					parentList.Add(resolved);
				}

				// Go to the next base type.
				baseType = resolved.BaseType;
			}
				
			// Go through in reverse because we want the top most class.
			for (int i = parentList.Count - 1; i >= 0; i--)
			{
				ProcessType(parentList[i], false);
			}
		}

		private void ProcessFields(MethodDefinition method, MethodReference parentMethod, bool isChild)
		{
			var dirtyParam = method.AddParameter<bool>();
			
			ILProcessor il = method.BeginEdit();

			// Create the method in reverse.

			Instruction previous = isChild ? Instruction.Create(OpCodes.Ret) : ILHelper.Ldarg(il, dirtyParam);
			il.Append(previous);
			if (!isChild)
			{
				il.Emit(OpCodes.Ret);
			}

			for (int i = targetFields.Count - 1; i >= 0; i--)
			{
				il.InsertAt(0, Instruction.Create(OpCodes.Starg_S, dirtyParam));
				il.InsertAt(0, ILHelper.Bool(true));
				
				// field = GetComponent<Type>()
				if (!targetFields[i].field.FieldType.IsList())
				{
					il.InsertAt(0, Instruction.Create(OpCodes.Stfld, targetFields[i].field));
				}

				if (targetFields[i].field.FieldType.IsArray())
				{
					il.InsertAt(0, Instruction.Create(OpCodes.Call, GetComponentMethod(targetFields[i].field.FieldType.GetCollectionType(), targetFields[i].target, true, false, targetFields[i].field.Module)));
				}
				else if (targetFields[i].field.FieldType.IsList())
				{
					il.InsertAt(0, Instruction.Create(OpCodes.Call, GetComponentMethod(targetFields[i].field.FieldType.GetCollectionType(), targetFields[i].target, true, true, targetFields[i].field.Module)));
					il.InsertAt(0, Instruction.Create(OpCodes.Ldfld, targetFields[i].field));
				}
				else
				{
					il.InsertAt(0, Instruction.Create(OpCodes.Call, GetComponentMethod(targetFields[i].field.FieldType, targetFields[i].target, false, false, targetFields[i].field.Module)));
				}

#if UNITY_2021_2_OR_NEWER
				if (targetFields[i].target != GetComponentTarget.Self && !targetFields[i].field.FieldType.IsCollection())
				{
					il.InsertAt(0, ILHelper.Bool(targetFields[i].includeInactive));
				}
#endif
				
				if(targetFields[i].target != GetComponentTarget.Self && targetFields[i].field.FieldType.IsArray())
				{
					il.InsertAt(0, ILHelper.Bool(targetFields[i].includeInactive));
				}

				il.InsertAt(0, ILHelper.Ldarg(il));

				if (targetFields[i].target != GetComponentTarget.Self && targetFields[i].field.FieldType.IsList())
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
					il.InsertAt(0, ILHelper.Ldarg(il));
				}
				
				if (!targetFields[i].field.FieldType.IsArray() && !targetFields[i].field.FieldType.IsList())
				{
					// if (field == null)
					il.InsertAt(0, Instruction.Create(OpCodes.Brfalse, previous));
					il.InsertAt(0, Instruction.Create(OpCodes.Call, MethodsCache.UnityObjectEqualityOperation));
					il.InsertAt(0, Instruction.Create(OpCodes.Ldnull));
					il.InsertAt(0, Instruction.Create(OpCodes.Ldfld, targetFields[i].field));
					previous = il.InsertAt(0, ILHelper.Ldarg(il));
				}
			}
			
			if (isChild)
			{
				// base.FetchComponents(dirty)
				il.InsertBefore(previous, ILHelper.Ldarg(il));
				il.InsertBefore(previous, ILHelper.Ldarg(il, dirtyParam));
				il.InsertBefore(previous, Instruction.Create(OpCodes.Call, parentMethod));
			}

			method.EndEdit();
		}
#endif

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
		
		private MethodReference GetComponentMethod(TypeReference fieldType, GetComponentTarget target, bool collection, bool list, ModuleDefinition module)
		{
			if (collection)
			{
				if (!list)
				{
					switch (target)
					{
						case GetComponentTarget.Self:
							return module.GetMethod<Component>(nameof(Component.GetComponents), System.Type.EmptyTypes).MakeGenericMethod(fieldType);
						case GetComponentTarget.Parent:
							return module.GetMethod<Component>(nameof(Component.GetComponentsInParent), onlyBools).MakeGenericMethod(fieldType);
						case GetComponentTarget.Children:
							return module.GetMethod<Component>(nameof(Component.GetComponentsInChildren), onlyBools).MakeGenericMethod(fieldType);
						default:
							throw new ArgumentOutOfRangeException(nameof(target), target, null);
					}
				}

				switch (target)
				{
					case GetComponentTarget.Self:
						return module.GetGenericMethod<Component>(nameof(Component.GetComponents), onlyList).MakeGenericMethod(fieldType);
					case GetComponentTarget.Parent:
						return module.GetGenericMethod<Component>(nameof(Component.GetComponentsInParent), boolsAndList).MakeGenericMethod(fieldType);
					case GetComponentTarget.Children:
						return module.GetGenericMethod<Component>(nameof(Component.GetComponentsInChildren), boolsAndList).MakeGenericMethod(fieldType);
					default:
						throw new ArgumentOutOfRangeException(nameof(target), target, null);
				}
			}

			switch (target)
			{
				case GetComponentTarget.Self:
					return module.GetMethod<Component>(nameof(Component.GetComponent), System.Type.EmptyTypes).MakeGenericMethod(fieldType);
				case GetComponentTarget.Parent:
#if UNITY_2021_2_OR_NEWER // Include inactive is not supported in previous versions.
					return module.GetMethod<Component>(nameof(Component.GetComponentInParent), onlyBools).MakeGenericMethod(fieldType);
#else
					return module.GetMethod<Component>(nameof(Component.GetComponentInParent), System.Type.EmptyTypes).MakeGenericMethod(fieldType);
#endif
				case GetComponentTarget.Children:
#if UNITY_2021_2_OR_NEWER // Include inactive is not supported in previous versions.
					return module.GetMethod<Component>(nameof(Component.GetComponentInChildren), onlyBools).MakeGenericMethod(fieldType);
#else
					return module.GetMethod<Component>(nameof(Component.GetComponentInChildren), System.Type.EmptyTypes).MakeGenericMethod(fieldType);
#endif
				default:
					throw new ArgumentOutOfRangeException(nameof(target), target, null);
			}
		}
	}
}
