using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace Hertzole.CecilAttributes.CodeGen
{
	public interface IMethodScope<T> : IDisposable where T : IMethodScope<T>
	{
		ILProcessor IL { get; }
		List<Instruction> Instructions { get; }
	}
}