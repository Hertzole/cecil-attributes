using System.Diagnostics;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Hertzole.CecilAttributes.CodeGen
{
	// Idea based on https://github.com/ByronMayne/Weaver

	public class TimedProcessor : BaseProcessor
	{
		public override string Name { get { return "Timed"; } }

		public override bool NeedsMonoBehaviour { get { return false; } }
		public override bool AllowEditor { get { return true; } }
		public override bool IncludeInBuild { get { return Settings.includeTimedInBuild; } }

		public override bool IsValidType()
		{
			if (Type.HasMethods)
			{
				for (int i = 0; i < Type.Methods.Count; i++)
				{
					if (Type.Methods[i].HasAttribute<TimedAttribute>())
					{
						return true;
					}
				}
			}

			if (Type.HasProperties)
			{
				for (int i = 0; i < Type.Properties.Count; i++)
				{
					if (Type.Properties[i].HasAttribute<TimedAttribute>())
					{
						return true;
					}
				}
			}

			return false;
		}

		public override void ProcessType()
		{
			if (Type.HasMethods)
			{
				ProcessMethods(Module, Type);
			}

			if (Type.HasProperties)
			{
				ProcessProperties(Module, Type);
			}
		}

		private void ProcessMethods(ModuleDefinition module, TypeDefinition type)
		{
			for (int i = 0; i < type.Methods.Count; i++)
			{
				if (type.Methods[i].HasAttribute<TimedAttribute>())
				{
					MethodDefinition method = type.Methods[i];

					string message = Settings.timedMethodFormat.FormatMessageTimed(type, method);

					InjectIntoMethod(module, method, message);
				}
			}
		}

		private void ProcessProperties(ModuleDefinition module, TypeDefinition type)
		{
			for (int i = 0; i < type.Properties.Count; i++)
			{
				if (type.Properties[i].HasAttribute<TimedAttribute>())
				{
					PropertyDefinition property = type.Properties[i];

					if (property.GetMethod != null)
					{
						string message = Settings.timedPropertyGetFormat.FormatMessageTimed(type, property.GetMethod, property);

						InjectIntoMethod(module, property.GetMethod, message);
					}

					if (property.SetMethod != null)
					{
						string message = Settings.timedPropertySetFormat.FormatMessageTimed(type, property.SetMethod, property);

						InjectIntoMethod(module, property.SetMethod, message);
					}
				}
			}
		}

		private void InjectIntoMethod(ModuleDefinition module, MethodDefinition method, string message)
		{
			method.Body.InitLocals = true;
			VariableDefinition stopwatch = method.AddLocalVariable<Stopwatch>("stopwatch");

			using (MethodEntryScope il = new MethodEntryScope(method))
			{
				il.Emit(OpCodes.Newobj, module.GetConstructor<Stopwatch>(System.Type.EmptyTypes));
				il.EmitStloc(stopwatch);

				il.EmitLdloc(stopwatch);
				il.EmitCall(module.GetMethod<Stopwatch>(nameof(Stopwatch.Start), System.Type.EmptyTypes), true);
			}

			using (MethodEndScope il = new MethodEndScope(method))
			{
				il.EmitLdloc(stopwatch);
				il.EmitCall(module.GetMethod<Stopwatch>(nameof(Stopwatch.Stop), System.Type.EmptyTypes), true);

				il.EmitString(message);

				il.EmitLdloc(stopwatch);
				il.EmitCall(module.GetMethod<Stopwatch>("get_ElapsedMilliseconds", System.Type.EmptyTypes), true);
				il.Emit(OpCodes.Box, module.GetTypeReference<long>());

				il.EmitLdloc(stopwatch);
				il.EmitCall(module.GetMethod<Stopwatch>("get_ElapsedTicks", System.Type.EmptyTypes), true);
				il.Emit(OpCodes.Box, module.GetTypeReference<long>());

				il.EmitCall(MethodsCache.GetStringFormat(2));
				il.EmitCall(MethodsCache.DebugLog);
			}
		}
	}
}