using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

namespace Hertzole.CecilAttributes.CodeGen
{
	public class LogCalledProcessor : BaseProcessor
	{
		private readonly CecilAttributesSettings settings;
		
		public override string Name { get { return "LogCalled"; } }

		public override bool NeedsMonoBehaviour { get { return false; } }
		public override bool AllowEditor { get { return true; } }
		public override bool IncludeInBuild { get { return settings.IncludeLogsInBuild; } }

		public LogCalledProcessor()
		{
			settings = CecilAttributesSettings.Instance;
		}
		
		public override bool IsValidType()
		{
			if (Type.HasMethods)
			{
				for (int i = 0; i < Type.Methods.Count; i++)
				{
					if (Type.Methods[i].HasAttribute<LogCalledAttribute>())
					{
						return true;
					}
				}
			}

			if (Type.HasProperties)
			{
				for (int i = 0; i < Type.Properties.Count; i++)
				{
					if (Type.Properties[i].HasAttribute<LogCalledAttribute>())
					{
						return true;
					}
				}
			}

			return false;
		}

		public override void ProcessType()
		{
			List<MethodDefinition> methods = new List<MethodDefinition>();
			List<PropertyDefinition> properties = new List<PropertyDefinition>();

			for (int i = 0; i < Type.Methods.Count; i++)
			{
				if (Type.Methods[i].HasAttribute<LogCalledAttribute>())
				{
					methods.Add(Type.Methods[i]);
				}
			}

			for (int i = 0; i < Type.Properties.Count; i++)
			{
				if (Type.Properties[i].HasAttribute<LogCalledAttribute>())
				{
					properties.Add(Type.Properties[i]);
				}
			}

			if (methods.Count > 0)
			{
				string defaultMethodFormat = settings.MethodLogFormat;
				string defaultParametersSeparator = settings.ParametersSeparator;

				for (int i = 0; i < methods.Count; i++)
				{
					ProcessMethod(Type, Module, methods[i], defaultMethodFormat, defaultParametersSeparator);
				}
			}

			if (properties.Count > 0)
			{
				string defaultGetFormat = settings.PropertyGetLogFormat;
				string defaultSetFormat = settings.PropertySetLogFormat;

				for (int i = 0; i < properties.Count; i++)
				{
					ProcessProperty(Type, Module, properties[i], defaultGetFormat, defaultSetFormat);
				}
			}
		}

		private static void ProcessMethod(TypeReference type, ModuleDefinition module, MethodDefinition method, string format, string parameterSeparator)
		{
			List<Instruction> instructions = new List<Instruction>();
			List<string> parameters = new List<string>();

			if (method.HasParameters)
			{
				int offset = 0;

				for (int i = 0; i < method.Parameters.Count; i++)
				{
					if (method.Parameters[i].IsOut)
					{
						parameters.Add($"out {method.Parameters[i].Name}");
						offset++;
					}
					else
					{
						parameters.Add($"{method.Parameters[i].Name}: {{{i - offset + type.GenericParameters.Count}}}");
					}
				}
			}

			string message = format.FormatMessageLogCalled(type, method, parameterSeparator, parameters, null, false);

			instructions.Add(Instruction.Create(OpCodes.Ldstr, message));

			// Valid parameters are parameters than can show their value.
			int validParameters = type.GenericParameters.Count;

			for (int i = 0; i < method.Parameters.Count; i++)
			{
				if (method.Parameters[i].IsOut)
				{
					continue;
				}

				validParameters++;
			}

			if (validParameters > 3)
			{
				instructions.Add(WeaverHelpers.GetIntInstruction(validParameters));
				instructions.Add(Instruction.Create(OpCodes.Newarr, module.GetTypeReference<object>()));
			}

			for (int i = 0; i < type.GenericParameters.Count; i++)
			{
				if (validParameters > 3)
				{
					instructions.Add(Instruction.Create(OpCodes.Dup));
					instructions.Add(WeaverHelpers.GetIntInstruction(i));
				}

				instructions.Add(Instruction.Create(OpCodes.Ldtoken, type.GenericParameters[i].GetElementType()));
				instructions.Add(Instruction.Create(OpCodes.Call, module.GetMethod<Type>("GetTypeFromHandle", typeof(RuntimeTypeHandle))));

				if (validParameters > 3)
				{
					instructions.Add(Instruction.Create(OpCodes.Stelem_Ref));
				}
			}

			if (method.HasParameters)
			{
				// Used to offset int values if there are parameters that don't need to show their value. 
				int offset = 0;

				for (int i = 0; i < method.Parameters.Count; i++)
				{
					if (method.Parameters[i].IsOut)
					{
						offset++;
						continue;
					}

					if (validParameters > 3)
					{
						// Too many parameters, need to create an array.
						instructions.Add(Instruction.Create(OpCodes.Dup));
						instructions.Add(WeaverHelpers.GetIntInstruction(type.GenericParameters.Count + i - offset));
					}

					instructions.Add(GetLoadParameter(i, method.Parameters[i], method.IsStatic));

					if (!method.Parameters[i].ParameterType.Is<string>() && (method.Parameters[i].ParameterType.IsValueType || method.Parameters[i].ParameterType.IsByReference || method.Parameters[i].ParameterType.IsGenericParameter))
					{
						if (method.Parameters[i].ParameterType.IsByReference)
						{
							instructions.Add(Instruction.Create(OpCodes.Box, module.ImportReference(method.Parameters[i].ParameterType.Resolve())));
						}
						else
						{
							instructions.Add(Instruction.Create(OpCodes.Box, method.Parameters[i].ParameterType));
						}
					}

					if (validParameters > 3)
					{
						instructions.Add(Instruction.Create(OpCodes.Stelem_Ref));
					}
				}
			}

			if (validParameters > 0)
			{
				instructions.Add(Instruction.Create(OpCodes.Call, GetStringFormatMethod(module, validParameters)));
			}

			instructions.Add(Instruction.Create(OpCodes.Call, module.GetMethod<Debug>("Log", typeof(object))));

			method.Body.GetILProcessor().InsertBefore(method.Body.Instructions[0], instructions);
		}

		private static void ProcessProperty(TypeDefinition type, ModuleDefinition module, PropertyDefinition property, string getFormat, string setFormat)
		{
			CustomAttribute attribute = property.GetAttribute<LogCalledAttribute>();
			bool logGet = attribute.GetField("logPropertyGet", true);
			bool logSet = attribute.GetField("logPropertySet", true);

			if (!logGet && !logSet)
			{
				return;
			}

			List<Instruction> instructions = new List<Instruction>();

			bool isStatic = property.IsStatic();

			FieldReference loadField = property.GetBackingField();
			if (logGet && property.GetMethod != null)
			{
				string message = getFormat.FormatMessageLogCalled(type, property.GetMethod, null, null, property, false);

				instructions.Add(Instruction.Create(OpCodes.Ldstr, message));
				if (!isStatic)
				{
					instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
				}

				instructions.Add(Instruction.Create(isStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, loadField));

				if (loadField.FieldType.IsValueType || loadField.FieldType.IsGenericParameter)
				{
					instructions.Add(Instruction.Create(OpCodes.Box, module.ImportReference(loadField.FieldType)));
				}

				instructions.Add(Instruction.Create(OpCodes.Call, GetStringFormatMethod(module, 1)));
				instructions.Add(Instruction.Create(OpCodes.Call, module.ImportReference(typeof(Debug).GetMethod("Log", new[] { typeof(object) }))));

				property.GetMethod.Body.GetILProcessor().InsertBefore(property.GetMethod.Body.Instructions[0], instructions);
				instructions.Clear();
			}

			if (logSet && property.SetMethod != null)
			{
				VariableDefinition localVar = property.SetMethod.AddLocalVariable(module, loadField.FieldType, out int varIndex);
				string message = setFormat.FormatMessageLogCalled(type, property.SetMethod, null, null, property, true);
				if (!isStatic)
				{
					instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
				}

				instructions.Add(Instruction.Create(isStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, loadField));
				instructions.Add(GetStloc(varIndex, localVar));
				instructions.Add(Instruction.Create(OpCodes.Ldstr, message));
				instructions.Add(GetLdloc(varIndex, localVar));
				if (loadField.FieldType.IsValueType || loadField.FieldType.IsGenericParameter)
				{
					instructions.Add(Instruction.Create(OpCodes.Box, module.ImportReference(loadField.FieldType)));
				}

				instructions.Add(Instruction.Create(isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1));

				if (loadField.FieldType.IsValueType || loadField.FieldType.IsGenericParameter)
				{
					instructions.Add(Instruction.Create(OpCodes.Box, module.ImportReference(loadField.FieldType)));
				}

				instructions.Add(Instruction.Create(OpCodes.Call, GetStringFormatMethod(module, 2)));
				instructions.Add(Instruction.Create(OpCodes.Call, module.ImportReference(typeof(Debug).GetMethod("Log", new[] { typeof(object) }))));

				property.SetMethod.Body.GetILProcessor().InsertBefore(property.SetMethod.Body.Instructions[0], instructions);
			}
		}

		public static MethodReference GetStringFormatMethod(ModuleDefinition module, int amount)
		{
			switch (amount)
			{
				case 1:
					return module.ImportReference(typeof(string).GetMethod("Format", new[] { typeof(string), typeof(object) }));
				case 2:
					return module.ImportReference(typeof(string).GetMethod("Format", new[] { typeof(string), typeof(object), typeof(object) }));
				case 3:
					return module.ImportReference(typeof(string).GetMethod("Format", new[] { typeof(string), typeof(object), typeof(object), typeof(object) }));
				default:
					return module.ImportReference(typeof(string).GetMethod("Format", new[] { typeof(string), typeof(object[]) }));
			}
		}

		public static Instruction GetStloc(int index, VariableDefinition variable)
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
					return Instruction.Create(OpCodes.Stloc_S, variable);
			}
		}

		public static Instruction GetLdloc(int index, VariableDefinition variable, bool ldloc_a = false)
		{
			if (ldloc_a)
			{
				return Instruction.Create(OpCodes.Ldloca_S, variable);
			}

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
					return Instruction.Create(OpCodes.Ldloc_S, variable);
			}
		}

		private static Instruction GetLoadParameter(int index, ParameterDefinition parameter, bool isStatic)
		{
			switch (index)
			{
				case 0:
					return isStatic ? Instruction.Create(OpCodes.Ldarg_0) : Instruction.Create(OpCodes.Ldarg_1);
				case 1:
					return isStatic ? Instruction.Create(OpCodes.Ldarg_1) : Instruction.Create(OpCodes.Ldarg_2);
				case 2:
					return isStatic ? Instruction.Create(OpCodes.Ldarg_2) : Instruction.Create(OpCodes.Ldarg_3);
				case 3:
					return isStatic ? Instruction.Create(OpCodes.Ldarg_3) : Instruction.Create(OpCodes.Ldarg_S, parameter);
				default:
					return Instruction.Create(OpCodes.Ldarg_S, parameter);
			}
		}

		private static Instruction GetLoadIn(TypeReference parameterType, ModuleDefinition module)
		{
			TypeDefinition type = parameterType.Resolve();

			if (!type.IsPrimitive && !type.IsValueType && type.IsClass && type.IsAutoLayout)
			{
				return Instruction.Create(OpCodes.Ldind_Ref);
			}

			if (!type.IsPrimitive && type.IsValueType && !type.IsEnum)
			{
				return Instruction.Create(OpCodes.Ldobj, module.ImportReference(parameterType.Resolve()));
			}

			return Instruction.Create(OpCodes.Ldind_I4);
		}
	}
}