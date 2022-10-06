using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

namespace Hertzole.CecilAttributes.CodeGen
{
	public sealed class RequiredProcessor : BaseProcessor
	{
		private readonly HashSet<TypeDefinition> processedTypes = new HashSet<TypeDefinition>();
		public override string Name { get { return nameof(RequiredProcessor); } }
		public override bool NeedsMonoBehaviour { get { return true; } }
		public override bool AllowEditor { get { return false; } }
		public override bool IncludeInBuild { get { return Settings.includeRequiredInBuild; } }

		private const MethodAttributes CHECK_METHOD_PRIVATE_ATTRIBUTES = MethodAttributes.Private | MethodAttributes.HideBySig;
		private const MethodAttributes CHECK_METHOD_OVERRIDE_ATTRIBUTES = MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual;

		public override bool IsValidType()
		{
			if (!Type.HasFields)
			{
				return false;
			}

			for (int i = 0; i < Type.Fields.Count; i++)
			{
				if (Type.Fields[i].HasAttribute<RequiredAttribute>())
				{
					return true;
				}
			}

			return false;
		}

		public override void ProcessType()
		{
			ProcessType(Type);
		}

		private void ProcessType(TypeDefinition type)
		{
			if (type.Fields.Count == 0)
			{
				return;
			}

			if (processedTypes.Contains(type))
			{
				return;
			}

			processedTypes.Add(type);

			const string generated_method_name = "CECILATTRIBUTES__GENERATED__CheckRequired";

			bool hasParent = type.TryGetMethodInBaseType(generated_method_name, out MethodDefinition parentMethod);

			if (hasParent)
			{
				parentMethod.MakeOverridable();

				ProcessType(parentMethod.DeclaringType);
			}

			MethodDefinition checkRequiredMethod = GetOrAddMethod(type, generated_method_name, hasParent ? CHECK_METHOD_OVERRIDE_ATTRIBUTES : CHECK_METHOD_PRIVATE_ATTRIBUTES, Module.TypeSystem.Boolean);
			MethodDefinition awake = GetOrAddMethod(type, "Awake", MethodAttributes.Private | MethodAttributes.HideBySig, Module.TypeSystem.Void);

			List<FieldDefinition> fields = ListPool<FieldDefinition>.Get();
			List<Instruction> targetInstructions = ListPool<Instruction>.Get();

			bool hasError = false;
			
			for (int i = 0; i < type.Fields.Count; i++)
			{
				if (type.Fields[i].HasAttribute<RequiredAttribute>())
				{
					if (type.Fields[i].FieldType.CanBeResolved() && !type.Fields[i].FieldType.Resolve().IsSubclassOf<UnityEngine.Object>())
					{
						Weaver.Error($"Field '{type.Fields[i].Name}' in '{type.FullName}' is marked with [Required] but is not a UnityEngine.Object.");
						hasError = true;
						continue;
					}
					
					if (fields.Count > 0)
					{
						targetInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
					}

					fields.Add(type.Fields[i]);
				}
			}

			if (hasError)
			{
				ListPool<FieldDefinition>.Release(fields);
				ListPool<Instruction>.Release(targetInstructions);
				return;
			}

			using (MethodEntryScope il = new MethodEntryScope(checkRequiredMethod))
			{
				checkRequiredMethod.Body.InitLocals = true;
				VariableDefinition error = checkRequiredMethod.AddLocalVariable<bool>("error");
				Instruction end = ILHelper.Ldloc(error);

				if (!hasParent)
				{
					// bool error = false
					il.EmitBool(false);
					il.EmitStloc(error);
				}
				else
				{
					// bool error = base.CheckRequired()
					il.EmitLdarg();
					il.EmitCall(Module.ImportReference(parentMethod));
					il.EmitStloc(error);
				}

				for (int i = 0; i < fields.Count; i++)
				{
					FieldDefinition field = fields[i];
					Instruction jumpTarget = i < fields.Count - 1 ? targetInstructions[i] : end;

					// if (field == null)
					if (i > 0)
					{
						il.Emit(targetInstructions[i - 1]);
					}
					else
					{
						il.EmitLdarg();
					}

					il.EmitLoadField(field);
					il.EmitNull();
					il.EmitCall(MethodsCache.UnityObjectEqualityOperation);
					il.Emit(OpCodes.Brfalse, jumpTarget);

					// Debug.LogError(string.Format("'field' is not assigned on {0}. It is required. Please assign it in the inspector.", gameObject.name), this)
					il.EmitString($"'{field.Name}' is not assigned on {{0}}. It is required. Please assign it in the inspector.");
					il.EmitLdarg();
					il.EmitCall(Module.GetMethod<Component>("get_gameObject"));
					il.EmitCall(Module.GetMethod<Object>("get_name"), true);
					il.EmitCall(MethodsCache.GetStringFormat(1));
					il.EmitLdarg();
					il.EmitCall(MethodsCache.DebugLogErrorContext);
					// error = true
					il.EmitBool(true);
					il.EmitStloc(error);
				}

				il.Emit(end);
			}

			ListPool<FieldDefinition>.Release(fields);
			ListPool<Instruction>.Release(targetInstructions);

			if (hasParent && type.TryGetMethodInBaseType("Awake", out MethodDefinition parentAwake))
			{
				parentAwake.MakeOverridable();

				awake.Attributes |= MethodAttributes.Virtual;

				if (awake.IsPrivate)
				{
					awake.Attributes &= ~MethodAttributes.Private;
					awake.Attributes |= MethodAttributes.Family;
				}
			}

			using (MethodEntryScope il = new MethodEntryScope(awake))
			{
				il.EmitLdarg();
				il.EmitCall(checkRequiredMethod);
				il.Emit(OpCodes.Brfalse, il.First);
				il.EmitReturn();
			}
		}

		private static MethodDefinition FindMethod(TypeDefinition type, string name)
		{
			for (int i = 0; i < type.Methods.Count; i++)
			{
				if (type.Methods[i].Name == name)
				{
					return type.Methods[i];
				}
			}

			return null;
		}

		private static MethodDefinition GetOrAddMethod(TypeDefinition type, string name, MethodAttributes attributes, TypeReference returnType, params TypeReference[] parameters)
		{
			MethodDefinition method = FindMethod(type, name);
			if (method != null)
			{
				return method;
			}

			method = new MethodDefinition(name, attributes, returnType);
			for (int i = 0; i < parameters.Length; i++)
			{
				method.Parameters.Add(new ParameterDefinition(parameters[i]));
			}

			ILProcessor il = method.Body.GetILProcessor();

			il.Emit(OpCodes.Ret);

			type.Methods.Add(method);
			return method;
		}
	}
}