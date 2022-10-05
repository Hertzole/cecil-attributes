using System;
using Mono.Cecil;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hertzole.CecilAttributes.CodeGen.Caches
{
	public class MethodsCache
	{
		private readonly ModuleDefinition module;

		private MethodReference debugLogErrorContext;
		private MethodReference unityObjectEqualityOperation;

		private static readonly Type[] debugLogContextParams = { typeof(string), typeof(Object) };
		private static readonly Type[] unityObjectEqualityOperationParams = { typeof(Object), typeof(Object) };

		public MethodReference DebugLogErrorContext { get { return debugLogErrorContext ??= module.GetMethod(typeof(Debug), "LogError", debugLogContextParams); } }
		public MethodReference UnityObjectEqualityOperation { get { return unityObjectEqualityOperation ??= module.GetMethod(typeof(Object), "op_Equality", unityObjectEqualityOperationParams); } }

		public MethodsCache(ModuleDefinition module)
		{
			this.module = module;
		}
	}
}