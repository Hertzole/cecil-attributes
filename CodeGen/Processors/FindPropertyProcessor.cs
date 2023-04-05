using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEditor;

namespace Hertzole.CecilAttributes.CodeGen
{
	public class FindPropertyProcessor : BaseProcessor
	{
		private static readonly Type[] singleStringArray = { typeof(string) };

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
			List<MemberData> members = ListPool<MemberData>.Get();
			List<Instruction> firstInstructions = ListPool<Instruction>.Get();
			List<(Instruction, Instruction)> ifsLast = ListPool<(Instruction, Instruction)>.Get();

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
				ListPool<MemberData>.Release(members);
				ListPool<Instruction>.Release(firstInstructions);
				ListPool<(Instruction, Instruction)>.Release(ifsLast);
				return;
			}

			if (!Type.TryGetMethod("OnEnable", out MethodDefinition enableMethod))
			{
				enableMethod = new MethodDefinition("OnEnable", MethodAttributes.Private | MethodAttributes.HideBySig, Module.TypeSystem.Void);
				Type.Methods.Add(enableMethod);
				// Must add a return at the end or else the method will be invalid.
				ILProcessor il = enableMethod.Body.GetILProcessor();
				il.Emit(OpCodes.Ret);
			}

			MethodReference getSerializedObject = Module.ImportReference(Type.GetMethodInBaseType("get_serializedObject"));
			MethodReference getTarget = Module.ImportReference(Type.GetMethodInBaseType("get_target"));
			MethodReference findProperty = Module.ImportReference(typeof(SerializedObject).GetMethod("FindProperty", singleStringArray));
			MethodReference findPropertyRelative = Module.ImportReference(typeof(SerializedProperty).GetMethod("FindPropertyRelative", singleStringArray));
			MethodReference logError = MethodsCache.GetDebugLogError(false, enableMethod.DeclaringType.Module);

			using (MethodEntryScope il = new MethodEntryScope(enableMethod))
			{
				string[] singlePath = new string[1];

				for (int i = 0; i < members.Count; i++)
				{
					CustomAttribute attribute = members[i].GetAttribute<FindProperty>();
					string path = attribute.GetConstructorArgument(0, string.Empty);

					Instruction beforeGetProperty = Instruction.Create(OpCodes.Ldarg_0);
					string[] paths;
					if (string.IsNullOrWhiteSpace(path))
					{
						singlePath[0] = members[i].Name;
						paths = singlePath;
					}
					else
					{
						paths = path.Split('/');
					}

					for (int j = 0; j < paths.Length; j++)
					{
						firstInstructions.Add(Instruction.Create(OpCodes.Ldarg_0));
					}

					for (int j = 0; j < paths.Length; j++)
					{
						il.Emit(firstInstructions[j]);
						il.EmitCall(getSerializedObject);

						for (int k = 0; k <= j; k++)
						{
							il.Emit(OpCodes.Ldstr, paths[k]);
							Instruction last = Instruction.Create(OpCodes.Call, k == 0 ? findProperty : findPropertyRelative);
							il.Emit(last);

							if (k == j)
							{
								ifsLast.Add((last, j == paths.Length - 1 ? beforeGetProperty : firstInstructions[j + 1]));
							}
						}

						il.EmitString($"There's no serialized property called '{paths[j]}' on {{0}}");
						il.EmitLdarg();
						il.EmitCall(getTarget);
						il.EmitCall(MethodsCache.GetStringFormat(1, enableMethod.DeclaringType.Module));
						il.EmitCall(logError);
						il.EmitReturn();
					}

					il.Emit(beforeGetProperty);
					il.EmitLdarg();
					il.EmitCall(getSerializedObject);

					for (int j = 0; j < paths.Length; j++)
					{
						il.Emit(OpCodes.Ldstr, paths[j]);
						il.EmitCall(j == 0 ? findProperty : findPropertyRelative, true);
					}

					if (members[i].IsProperty)
					{
						il.EmitCall(members[i].property.SetMethod);
					}
					else
					{
						il.Emit(OpCodes.Stfld, members[i].field);
					}
				}
			}

			ListPool<MemberData>.Release(members);
			ListPool<Instruction>.Release(firstInstructions);

			ILProcessor bodyIl = enableMethod.BeginEdit();

			for (int i = 0; i < ifsLast.Count; i++)
			{
				bodyIl.InsertAfter(ifsLast[i].Item1, Instruction.Create(OpCodes.Brtrue, ifsLast[i].Item2));
			}

			ListPool<(Instruction, Instruction)>.Release(ifsLast);

			enableMethod.EndEdit();
		}
	}
}