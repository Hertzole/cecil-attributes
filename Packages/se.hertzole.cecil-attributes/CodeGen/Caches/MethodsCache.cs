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

		public MethodReference DebugLogErrorContext
		{
			get
			{
				if (debugLogErrorContext == null)
				{
					debugLogErrorContext = module.GetMethod(typeof(Debug), "LogError", debugLogContextParams);
				}

				return debugLogErrorContext;
			}
		}
		public MethodReference UnityObjectEqualityOperation
		{
			get
			{
				if (unityObjectEqualityOperation == null)
				{
					unityObjectEqualityOperation = module.GetMethod(typeof(Object), "op_Equality", unityObjectEqualityOperationParams);
				}

				return unityObjectEqualityOperation;
			}
		}

		public MethodsCache(ModuleDefinition module)
		{
			this.module = module;
		}
	}
}