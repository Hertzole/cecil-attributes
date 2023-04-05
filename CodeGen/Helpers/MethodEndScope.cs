using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Hertzole.CecilAttributes.CodeGen
{
	public readonly struct MethodEndScope : IMethodScope<MethodEndScope>
	{
		private readonly MethodDefinition targetMethod;
		public ILProcessor IL { get; }
		public List<Instruction> Instructions { get; }

		public Instruction Last { get; }

		public MethodEndScope(MethodDefinition targetMethod)
		{
			this.targetMethod = targetMethod;
			IL = targetMethod.BeginEdit();
			Instructions = ListPool<Instruction>.Get();

			if (targetMethod.Body.Instructions.Count == 0)
			{
				throw new Exception($"Method {this.targetMethod.FullName} has no instructions in its body.");
			}

			Last = targetMethod.Body.Instructions[targetMethod.Body.Instructions.Count - 1];
		}

		public void Dispose()
		{
			IL.InsertBefore(Last, Instructions);
			targetMethod.EndEdit();
			ListPool<Instruction>.Release(Instructions);
		}
	}
}