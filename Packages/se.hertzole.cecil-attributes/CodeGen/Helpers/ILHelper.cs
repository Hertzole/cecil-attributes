using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Hertzole.CecilAttributes.CodeGen
{
	public class ILHelper
	{
		public static Instruction Stloc(VariableDefinition variable)
		{
			Instruction result;
			
			switch (variable.Index)
			{
				case 0:
					result = Instruction.Create(OpCodes.Stloc_0);
					break;
				case 1:
					result = Instruction.Create(OpCodes.Stloc_1);
					break;
				case 2:
					result = Instruction.Create(OpCodes.Stloc_2);
					break;
				case 3:
					result = Instruction.Create(OpCodes.Stloc_3);
					break;
				default:
					result = Instruction.Create(OpCodes.Stloc_S, variable);
					break;
			}

			return result;
		}
		
		public static Instruction Ldloc(VariableDefinition variable, bool ldloc_a = false)
		{
			Instruction result;
			
			if (ldloc_a)
			{
				result = Instruction.Create(variable.Index < 256 ?  OpCodes.Ldloca_S : OpCodes.Ldloca, variable);
				return result;
			}

			switch (variable.Index)
			{
				case 0:
					result = Instruction.Create(OpCodes.Ldloc_0);
					break;
				case 1:
					result = Instruction.Create(OpCodes.Ldloc_1);
					break;
				case 2:
					result = Instruction.Create(OpCodes.Ldloc_2);
					break;
				case 3:
					result = Instruction.Create(OpCodes.Ldloc_3);
					break;
				default:
					result = Instruction.Create(OpCodes.Ldloc_S, variable);
					break;
			}

			return result;
		}

		public static Instruction Ldarg(ILProcessor il, ParameterDefinition parameter = null, bool ldarg_a = false)
		{
			Instruction result;
			int index = 0;
			if (parameter != null)
			{
				index = parameter.Index;

				if (ldarg_a)
				{
					result = Instruction.Create(index < 256 ? OpCodes.Ldarga_S : OpCodes.Ldarga, parameter);
					return result;
				}

				if (il != null)
				{
					if (index == -1 && parameter == il.Body.ThisParameter)
					{
						index = 0;
					}
					else if (il.Body.Method.HasThis)
					{
						index++;
					}
				}
			}

			switch (index)
			{
				case 0:
					result = Instruction.Create(OpCodes.Ldarg_0);
					return result;
				case 1:
					result = Instruction.Create(OpCodes.Ldarg_1);
					return result;
				case 2:
					result = Instruction.Create(OpCodes.Ldarg_2);
					return result;
				case 3:
					result = Instruction.Create(OpCodes.Ldarg_3);
					return result;
				default:
					result = Instruction.Create(index < 256 ? OpCodes.Ldarg_S : OpCodes.Ldarg, parameter);
					return result;
			}
		}

		public static Instruction Int(int value)
		{
			Instruction result;
			
			switch (value)
			{
				case -1:
					result = Instruction.Create(OpCodes.Ldc_I4_M1);
					break;
				case 0:
					result = Instruction.Create(OpCodes.Ldc_I4_0);
					break;
				case 1:
					result = Instruction.Create(OpCodes.Ldc_I4_1);
					break;
				case 2:
					result = Instruction.Create(OpCodes.Ldc_I4_2);
					break;
				case 3:
					result = Instruction.Create(OpCodes.Ldc_I4_3);
					break;
				case 4:
					result = Instruction.Create(OpCodes.Ldc_I4_4);
					break;
				case 5:
					result = Instruction.Create(OpCodes.Ldc_I4_5);
					break;
				case 6:
					result = Instruction.Create(OpCodes.Ldc_I4_6);
					break;
				case 7:
					result = Instruction.Create(OpCodes.Ldc_I4_7);
					break;
				case 8:
					result = Instruction.Create(OpCodes.Ldc_I4_8);
					break;
				default:
					if (value >= sbyte.MinValue && value < 128)
					{
						result = Instruction.Create(OpCodes.Ldc_I4_S, (sbyte) value);
					}
					else
					{
						result = Instruction.Create(OpCodes.Ldc_I4, value);
					}

					break;
			}

			return result;
		}

		public static Instruction ULong(ulong value)
		{
			if (value > int.MaxValue)
			{
				return Instruction.Create(OpCodes.Ldc_I8, (long)value);
			}
			else
			{
				return Int((int) value);
			}
		}
		
		public static Instruction Long(long value)
		{
			if (value > int.MaxValue)
			{
				return Instruction.Create(OpCodes.Ldc_I8, value);
			}
			else
			{
				return Int((int) value);
			}
		}

		public static Instruction Bool(bool value)
		{
			return Instruction.Create(value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
		}

		public static IEnumerable<Instruction> IfElse<T>(IReadOnlyList<T> items, Action<T, int, Instruction, List<Instruction>> ifCheck, Action<T, int, Instruction, List<Instruction>> body, Action<List<Instruction>> endElse)
		{
			// How it works:
			// 1. Create the end.
			// 2. Create all the bodies.
			// 3. Go backwards through all the if checks and give them the next target.
			//	  Since it starts at the end, it targets that first and then creates them
			//    in the reverse order, supplying new targets as they are created.
			// 4. Appends all the body instructions.
			// 5. Appends the end instructions.
			// 6. Appends all the if check instructions before each body block.

			Instruction[] targets =
#if UNITY_2021_3_OR_NEWER
				// Use a new array pool to avoid allocations.
				System.Buffers.ArrayPool<Instruction>.Shared.Rent(items.Count);
#else
				new Instruction[items.Count];
#endif

			Dictionary<int, List<Instruction>> bodyFills = DictionaryPool<int, List<Instruction>>.Get();

			List<Instruction> endFill = ListPool<Instruction>.Get();

			endElse.Invoke(endFill);
			targets[0] = endFill[0];
			
			for (int i = 0; i < items.Count; i++)
			{
				bodyFills.Add(i, ListPool<Instruction>.Get());
				body.Invoke(items[i], i, targets[0], bodyFills[i]);
			}

			Dictionary<int, List<Instruction>> checkFills = DictionaryPool<int, List<Instruction>>.Get();

			int index = 0;
			// Create all the if checks in reverse order.
			for (int i = items.Count - 1; i >= 0; i--)
			{
				index = i;
				checkFills.Add(i, ListPool<Instruction>.Get());
				ifCheck.Invoke(items[index], index, targets[i == items.Count - 1 ? 0 : i + 1], checkFills[index]);
				if (i > 0)
				{
					targets[i] = checkFills[i][0];
				}
			}

			List<Instruction> total = ListPool<Instruction>.Get();

			// Append the body instructions.
			foreach (List<Instruction> fill in bodyFills.Values)
			{
				total.AddRange(fill);
			}

			// Append the end instructions.
			total.AddRange(endFill);

			// Append all if checks before each body block.
			index = 0;
			foreach (List<Instruction> fill in bodyFills.Values)
			{
				total.InsertRange(total.IndexOf(fill[0]), checkFills[index]);
				
				index++;
			}

			foreach (List<Instruction> list in bodyFills.Values)
			{
				ListPool<Instruction>.Release(list);
			}

			foreach (List<Instruction> list in checkFills.Values)
			{
				ListPool<Instruction>.Release(list);
			}
			
			ListPool<Instruction>.Release(endFill);
			ListPool<Instruction>.Release(total);
			DictionaryPool<int, List<Instruction>>.Release(bodyFills);
			DictionaryPool<int, List<Instruction>>.Release(checkFills);
			
#if UNITY_2021_3_OR_NEWER
			System.Buffers.ArrayPool<Instruction>.Shared.Return(targets, true);
#endif

			return total.ToArray();
		}

		public static IEnumerable<Instruction> IfElse(Action<Instruction, List<Instruction>> ifCheck, Action<Instruction, List<Instruction>> body, Action<List<Instruction>> endElse)
		{
			List<Instruction> endFill = ListPool<Instruction>.Get();

			endElse.Invoke(endFill);
			Instruction target = endFill[0];
			
			List<Instruction> bodyFill = ListPool<Instruction>.Get();
			body.Invoke(target, bodyFill);

			List<Instruction> checkFill = ListPool<Instruction>.Get();

			ifCheck.Invoke(target, checkFill);

			List<Instruction> total = ListPool<Instruction>.Get();
			
			// Append the body instructions.
			total.AddRange(bodyFill);

			// Append the end instructions.
			total.AddRange(endFill);

			// Insert the if check before the body.
			total.InsertRange(total.IndexOf(bodyFill[0]), checkFill);

			Instruction[] result = total.ToArray();
			
			ListPool<Instruction>.Release(endFill);
			ListPool<Instruction>.Release(bodyFill);
			ListPool<Instruction>.Release(checkFill);
			ListPool<Instruction>.Release(total);

			return result;
		}

		public static IEnumerable<Instruction> GetStandardValue(TypeReference type)
		{
			List<Instruction> instructions = ListPool<Instruction>.Get();

			if (type.Is<bool>() || type.Is<int>() || type.Is<uint>() || type.Is<short>() || type.Is<ushort>() || type.Is<byte>() || type.Is<sbyte>())
			{
				instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));
			}
			else if (type.Is<long>() || type.Is<ulong>())
			{
				instructions.Add(Instruction.Create(OpCodes.Conv_I8));
				instructions.Add(Instruction.Create(OpCodes.Ldc_I4_0));
			}
			else if (type.Is<float>())
			{
				instructions.Add(Instruction.Create(OpCodes.Ldc_R4, 0.0f));
			}
			else if (type.Is<double>())
			{
				instructions.Add(Instruction.Create(OpCodes.Ldc_R8, 0.0d));
			}
			else if (type.IsValueType)
			{
				// Handle value types differently.
				Instruction[] result = instructions.ToArray();

				ListPool<Instruction>.Release(instructions);

				return result;
			}
			else
			{
				instructions.Add(Instruction.Create(OpCodes.Ldnull));
			}

			if (instructions.Count > 0)
			{
				Instruction[] result = instructions.ToArray();

				ListPool<Instruction>.Release(instructions);
				
				return result;
			}

			throw new NullReferenceException($"Can't find a default value for type {type}.");
		}
	}
}