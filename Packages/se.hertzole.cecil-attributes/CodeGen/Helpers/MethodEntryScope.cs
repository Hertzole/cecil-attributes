using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Hertzole.CecilAttributes.CodeGen
{
	public readonly struct MethodEntryScope : IMethodScope<MethodEntryScope>
	{
		private readonly MethodDefinition targetMethod;
		public ILProcessor IL { get; }
		public List<Instruction> Instructions { get; }
		
		public Instruction First { get; }

		public MethodEntryScope(MethodDefinition targetMethod)
		{
			this.targetMethod = targetMethod;
			IL = targetMethod.BeginEdit();
			//TODO: Pool lists
			Instructions = new List<Instruction>();
			First = targetMethod.Body.Instructions[0];
		}

		public void Dispose()
		{
			IL.InsertAt(0, Instructions);
			targetMethod.EndEdit();
		}
	}
}