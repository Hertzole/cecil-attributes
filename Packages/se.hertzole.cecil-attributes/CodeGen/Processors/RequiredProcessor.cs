using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Hertzole.CecilAttributes.CodeGen
{
	public sealed class RequiredProcessor : BaseProcessor
	{
		public override string Name { get { return nameof(RequiredProcessor); } }
		public override bool NeedsMonoBehaviour { get { return true; } }
		public override bool AllowEditor { get { return false; } }

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
			MethodDefinition awake = FindMethod(Type, "Awake");
			if (awake == null)
			{
				return;
			}

			//TODO: Get from pool.
			List<FieldDefinition> fields = new List<FieldDefinition>();
			List<Instruction> targetInstructions = new List<Instruction>();

			for (int i = 0; i < Type.Fields.Count; i++)
			{
				if (Type.Fields[i].HasAttribute<RequiredAttribute>())
				{
					if (fields.Count > 0)
					{
						targetInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
					}

					fields.Add(Type.Fields[i]);
				}
			}

			using (MethodEntryScope il = new MethodEntryScope(awake))
			{
				VariableDefinition error = awake.AddLocalVariable<bool>("error");
				Instruction end = ILHelper.Ldloc(error);

				// bool error = false
				il.EmitBool(false);
				il.EmitStloc(error);

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

					// Debug.LogError("Field is null", this)
					il.EmitString($"'{field.Name}' is not assigned. It is required. Please assign it in the inspector.");
					il.EmitLdarg();
					il.EmitCall(MethodsCache.DebugLogErrorContext);
					// error = true
					il.EmitBool(true);
					il.EmitStloc(error);
				}

				il.Emit(end);
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
	}
}