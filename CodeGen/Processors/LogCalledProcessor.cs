using System.Collections.Generic;
using System.Globalization;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Hertzole.CecilAttributes.CodeGen
{
	public class LogCalledProcessor : BaseProcessor
	{
		public override string Name { get { return "LogCalled"; } }

		public override bool NeedsMonoBehaviour { get { return false; } }
		public override bool AllowEditor { get { return true; } }
		public override bool IncludeInBuild { get { return Settings.includeLogsInBuild; } }

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
			List<MethodDefinition> methods = ListPool<MethodDefinition>.Get();
			List<PropertyDefinition> properties = ListPool<PropertyDefinition>.Get();

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
				string defaultMethodFormat = Settings.methodLogFormat;
				string defaultParametersSeparator = Settings.parametersSeparator;

				for (int i = 0; i < methods.Count; i++)
				{
					ProcessMethod(Type, Module, methods[i], defaultMethodFormat, defaultParametersSeparator);
				}
			}

			if (properties.Count > 0)
			{
				string defaultGetFormat = Settings.propertyGetLogFormat;
				string defaultSetFormat = Settings.propertySetLogFormat;

				for (int i = 0; i < properties.Count; i++)
				{
					ProcessProperty(Type, Module, properties[i], defaultGetFormat, defaultSetFormat);
				}
			}

			ListPool<MethodDefinition>.Release(methods);
			ListPool<PropertyDefinition>.Release(properties);
		}

		private void ProcessMethod(TypeReference type, ModuleDefinition module, MethodDefinition method, string format, string parameterSeparator)
		{
			using (MethodEntryScope il = new MethodEntryScope(method))
			{
				List<string> parameters = ListPool<string>.Get();
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
							parameters.Add($"{method.Parameters[i].Name}: {{{(i - offset + type.GenericParameters.Count).ToString(CultureInfo.InvariantCulture)}}}");
						}
					}
				}

				string message = format.FormatMessageLogCalled(type, method, parameterSeparator, parameters, null, false);
				ListPool<string>.Release(parameters);

				il.EmitString(message);

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
					il.EmitInt(validParameters);
					il.Emit(OpCodes.Newarr, module.ImportReference(typeof(object)));
				}

				for (int i = 0; i < type.GenericParameters.Count; i++)
				{
					if (validParameters > 3)
					{
						il.Emit(OpCodes.Dup);
						il.EmitInt(i);
					}

					il.Emit(OpCodes.Ldtoken, type.GenericParameters[i]);
					il.Emit(OpCodes.Call, MethodsCache.GetTypeFromHandle);

					if (validParameters > 3)
					{
						il.Emit(OpCodes.Stelem_Ref);
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
							il.Emit(OpCodes.Dup);
							il.EmitInt(i - offset + type.GenericParameters.Count);
						}

						il.Emit(GetLoadParameter(i, method.Parameters[i], method.IsStatic));

						if (!method.Parameters[i].ParameterType.Is<string>() && (method.Parameters[i].ParameterType.IsValueType || method.Parameters[i].ParameterType.IsByReference || method.Parameters[i].ParameterType.IsGenericParameter))
						{
							il.Emit(OpCodes.Box, method.Parameters[i].ParameterType.IsByReference ? module.ImportReference(method.Parameters[i].ParameterType.Resolve()) : method.Parameters[i].ParameterType);
						}

						if (validParameters > 3)
						{
							il.Emit(OpCodes.Stelem_Ref);
						}
					}
				}

				if (validParameters > 0)
				{
					il.EmitCall(MethodsCache.GetStringFormat(validParameters, method.DeclaringType.Module));
				}

				il.EmitCall(MethodsCache.GetDebugLog(false, method.DeclaringType.Module));
			}
		}

		private void ProcessProperty(TypeDefinition type, ModuleDefinition module, PropertyDefinition property, string getFormat, string setFormat)
		{
			CustomAttribute attribute = property.GetAttribute<LogCalledAttribute>();
			bool logGet = attribute.GetField("logPropertyGet", true);
			bool logSet = attribute.GetField("logPropertySet", true);

			if (!logGet && !logSet)
			{
				return;
			}

			bool isStatic = property.IsStatic();

			FieldReference loadField = property.GetBackingField();
			if (logGet && property.GetMethod != null)
			{
				using (MethodEntryScope il = new MethodEntryScope(property.GetMethod))
				{
					string message = getFormat.FormatMessageLogCalled(type, property.GetMethod, null, null, property, false);

					il.EmitString(message);
					if (!isStatic)
					{
						il.EmitLdarg();
					}

					il.Emit(isStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, loadField);

					if (loadField.FieldType.IsValueType || loadField.FieldType.IsGenericParameter)
					{
						il.Emit(OpCodes.Box, module.ImportReference(loadField.FieldType));
					}

					il.EmitCall(MethodsCache.GetStringFormat(1, property.DeclaringType.Module));
					il.EmitCall(MethodsCache.GetDebugLog(false, property.GetMethod.DeclaringType.Module));
				}
			}

			if (logSet && property.SetMethod != null)
			{
				using (MethodEntryScope il = new MethodEntryScope(property.SetMethod))
				{
					VariableDefinition localVar = property.SetMethod.AddLocalVariable(module, loadField.FieldType, out int _);
					string message = setFormat.FormatMessageLogCalled(type, property.SetMethod, null, null, property, true);
					if (!isStatic)
					{
						il.EmitLdarg();
					}

					il.Emit(isStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, loadField);
					il.EmitStloc(localVar);
					il.EmitString(message);
					il.EmitLdloc(localVar);
					if (loadField.FieldType.IsValueType || loadField.FieldType.IsGenericParameter)
					{
						il.Emit(OpCodes.Box, module.ImportReference(loadField.FieldType));
					}

					il.Emit(isStatic ? OpCodes.Ldarg_0 : OpCodes.Ldarg_1);

					if (loadField.FieldType.IsValueType || loadField.FieldType.IsGenericParameter)
					{
						il.Emit(OpCodes.Box, module.ImportReference(loadField.FieldType));
					}

					il.EmitCall(MethodsCache.GetStringFormat(2, property.DeclaringType.Module));
					il.EmitCall(MethodsCache.GetDebugLog(false, property.SetMethod.DeclaringType.Module));
				}
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
	}
}