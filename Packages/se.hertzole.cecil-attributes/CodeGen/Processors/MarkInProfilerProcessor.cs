using System;
using Mono.Cecil;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace Hertzole.CecilAttributes.CodeGen
{
	public class MarkInProfilerProcessor : BaseProcessor
	{
		private static readonly Type[] beginSampleTypes = { typeof(string) };
		private static readonly Type[] beginSampleTypesWithContext = { typeof(string), typeof(Object) };
		public override string Name { get { return "MarkInProfiler"; } }

		public override bool NeedsMonoBehaviour { get { return false; } }

		public override bool AllowEditor { get { return false; } }

		public override bool IsValidType()
		{
			if (Type.HasMethods)
			{
				for (int i = 0; i < Type.Methods.Count; i++)
				{
					if (Type.Methods[i].HasAttribute<MarkInProfilerAttribute>())
					{
						return true;
					}
				}
			}

			return false;
		}

		public override void ProcessType()
		{
			for (int i = 0; i < Type.Methods.Count; i++)
			{
				if (Type.Methods[i].HasAttribute<MarkInProfilerAttribute>())
				{
					InjectIntoMethod(Type.Methods[i], Type);
				}
			}
		}

		private void InjectIntoMethod(MethodDefinition method, TypeDefinition type)
		{
			string name = Settings.markInProfilerFormat.FormatTypesBase(type, method, null);

			using (MethodEntryScope il = new MethodEntryScope(method))
			{
				il.EmitString(name);

				if (type.IsSubclassOf<Object>())
				{
					il.EmitLdarg();
					il.EmitCall(Module.GetMethod<Profiler>("BeginSample", beginSampleTypesWithContext));
				}
				else
				{
					il.EmitCall(Module.GetMethod<Profiler>("BeginSample", beginSampleTypes));
				}
			}

			using (MethodEndScope il = new MethodEndScope(method))
			{
				il.EmitCall(Module.GetMethod<Profiler>("EndSample", System.Type.EmptyTypes));
			}
		}
	}
}