using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Hertzole.CecilAttributes.CodeGen
{
	public static class MethodScopeExtensions
	{
		public static void Emit<T>(this T scope, Instruction instruction) where T : IMethodScope<T>
		{
			scope.Instructions.Add(instruction);
		}

		public static Instruction Emit<T>(this T scope, OpCode opCode) where T : IMethodScope<T>
		{
			Instruction instruction = Instruction.Create(opCode);
			scope.Instructions.Add(instruction);
			return instruction;
		}

		public static Instruction Emit<T>(this T scope, OpCode opCode, CallSite site) where T : IMethodScope<T>
		{
			Instruction instruction = Instruction.Create(opCode, site);
			scope.Instructions.Add(instruction);
			return instruction;
		}

		public static Instruction Emit<T>(this T scope, OpCode opCode, FieldReference field) where T : IMethodScope<T>
		{
			Instruction instruction = Instruction.Create(opCode, field);
			scope.Instructions.Add(instruction);
			return instruction;
		}

		public static Instruction Emit<T>(this T scope, OpCode opCode, MethodReference method) where T : IMethodScope<T>
		{
			Instruction instruction = Instruction.Create(opCode, method);
			scope.Instructions.Add(instruction);
			return instruction;
		}

		public static Instruction Emit<T>(this T scope, OpCode opCode, ParameterDefinition parameter) where T : IMethodScope<T>
		{
			Instruction instruction = Instruction.Create(opCode, parameter);
			scope.Instructions.Add(instruction);
			return instruction;
		}

		public static Instruction Emit<T>(this T scope, OpCode opCode, TypeReference type) where T : IMethodScope<T>
		{
			Instruction instruction = Instruction.Create(opCode, type);
			scope.Instructions.Add(instruction);
			return instruction;
		}

		public static Instruction Emit<T>(this T scope, OpCode opCode, VariableDefinition variable) where T : IMethodScope<T>
		{
			Instruction instruction = Instruction.Create(opCode, variable);
			scope.Instructions.Add(instruction);
			return instruction;
		}
		
		public static Instruction Emit<T>(this T scope, OpCode opCode, Instruction variable) where T : IMethodScope<T>
		{
			Instruction instruction = Instruction.Create(opCode, variable);
			scope.Instructions.Add(instruction);
			return instruction;
		}

		public static Instruction Emit<T>(this T scope, OpCode opCode, sbyte value) where T : IMethodScope<T>
		{
			Instruction instruction = Instruction.Create(opCode, value);
			scope.Instructions.Add(instruction);
			return instruction;
		}

		public static Instruction Emit<T>(this T scope, OpCode opCode, byte value) where T : IMethodScope<T>
		{
			Instruction instruction = Instruction.Create(opCode, value);
			scope.Instructions.Add(instruction);
			return instruction;
		}

		public static Instruction Emit<T>(this T scope, OpCode opCode, int value) where T : IMethodScope<T>
		{
			Instruction instruction = Instruction.Create(opCode, value);
			scope.Instructions.Add(instruction);
			return instruction;
		}

		public static Instruction Emit<T>(this T scope, OpCode opCode, long value) where T : IMethodScope<T>
		{
			Instruction instruction = Instruction.Create(opCode, value);
			scope.Instructions.Add(instruction);
			return instruction;
		}

		public static Instruction Emit<T>(this T scope, OpCode opCode, float value) where T : IMethodScope<T>
		{
			Instruction instruction = Instruction.Create(opCode, value);
			scope.Instructions.Add(instruction);
			return instruction;
		}

		public static Instruction Emit<T>(this T scope, OpCode opCode, double value) where T : IMethodScope<T>
		{
			Instruction instruction = Instruction.Create(opCode, value);
			scope.Instructions.Add(instruction);
			return instruction;
		}

		public static Instruction Emit<T>(this T scope, OpCode opCode, string value) where T : IMethodScope<T>
		{
			Instruction instruction = Instruction.Create(opCode, value);
			scope.Instructions.Add(instruction);
			return instruction;
		}

		public static Instruction EmitBool<T>(this T scope, bool value) where T : IMethodScope<T>
		{
			Instruction instruction = ILHelper.Bool(value);
			scope.Instructions.Add(instruction);
			return instruction;
		}
		
		public static Instruction EmitInt<T>(this T scope, int value) where T : IMethodScope<T>
		{
			Instruction instruction = ILHelper.Int(value);
			scope.Instructions.Add(instruction);
			return instruction;
		}
		
		public static Instruction EmitString<T>(this T scope, string value) where T : IMethodScope<T>
		{
			Instruction instruction = Instruction.Create(OpCodes.Ldstr, value);
			scope.Instructions.Add(instruction);
			return instruction;
		}

		public static Instruction EmitStloc<T>(this T scope, VariableDefinition variableDefinition) where T : IMethodScope<T>
		{
			Instruction instruction = ILHelper.Stloc(variableDefinition);
			scope.Instructions.Add(instruction);
			return instruction;
		}
		
		public static Instruction EmitLdloc<T>(this T scope, VariableDefinition variableDefinition) where T : IMethodScope<T>
		{
			Instruction instruction = ILHelper.Ldloc(variableDefinition);
			scope.Instructions.Add(instruction);
			return instruction;
		}
		
		public static Instruction EmitLdarg<T>(this T scope, ParameterDefinition parameterDefinition = null, bool loadAddress = false) where T : IMethodScope<T>
		{
			Instruction instruction = ILHelper.Ldarg(scope.IL, parameterDefinition, loadAddress);
			scope.Instructions.Add(instruction);
			return instruction;
		}
		
		public static Instruction EmitLoadField<T>(this T scope, FieldReference fieldReference, bool loadAddress = false) where T : IMethodScope<T>
		{
			Instruction instruction = Instruction.Create(loadAddress ? OpCodes.Ldflda : OpCodes.Ldfld, fieldReference);
			scope.Instructions.Add(instruction);
			return instruction;
		}
		
		public static Instruction EmitNull<T>(this T scope) where T : IMethodScope<T>
		{
			Instruction instruction = Instruction.Create(OpCodes.Ldnull);
			scope.Instructions.Add(instruction);
			return instruction;
		}
		
		public static Instruction EmitCall<T>(this T scope, MethodReference methodReference, bool callVirtual = false) where T : IMethodScope<T>
		{
			Instruction instruction = Instruction.Create(callVirtual ? OpCodes.Callvirt : OpCodes.Call, methodReference);
			scope.Instructions.Add(instruction);
			return instruction;
		}
		
		public static Instruction EmitReturn<T>(this T scope) where T : IMethodScope<T>
		{
			Instruction instruction = Instruction.Create(OpCodes.Ret);
			scope.Instructions.Add(instruction);
			return instruction;
		}
	}
}