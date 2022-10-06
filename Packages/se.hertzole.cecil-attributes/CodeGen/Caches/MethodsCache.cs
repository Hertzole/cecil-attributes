using System;
using Mono.Cecil;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hertzole.CecilAttributes.CodeGen.Caches
{
	public class MethodsCache
	{
		private readonly ModuleDefinition module;

		private MethodReference debugLog;
		private MethodReference debugLogErrorContext;
		private MethodReference unityObjectEqualityOperation;
		private MethodReference getTypeFromHandle;
		private MethodReference stringFormat1;
		private MethodReference stringFormat2;
		private MethodReference stringFormat3;
		private MethodReference stringFormatParams;

		private static readonly Type[] debugLogParams = { typeof(object) };
		private static readonly Type[] debugLogContextParams = { typeof(string), typeof(Object) };
		private static readonly Type[] unityObjectEqualityOperationParams = { typeof(Object), typeof(Object) };
		private static readonly Type[] getTypeFromHandleParams = { typeof(RuntimeTypeHandle) };
		private static readonly Type[] stringFormat1Params = { typeof(string), typeof(object) };
		private static readonly Type[] stringFormat2Params = { typeof(string), typeof(object), typeof(object) };
		private static readonly Type[] stringFormat3Params = { typeof(string), typeof(object), typeof(object), typeof(object) };
		private static readonly Type[] stringFormatParamsParams = { typeof(string), typeof(object[]) };

		public MethodReference DebugLog
		{
			get
			{
				if (debugLog == null)
				{
					debugLog = module.GetMethod(typeof(Debug), "Log", debugLogParams);
				}

				return debugLog;
			}
		}

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

		public MethodReference GetTypeFromHandle
		{
			get
			{
				if (getTypeFromHandle == null)
				{
					getTypeFromHandle = module.GetMethod(typeof(Type), "GetTypeFromHandle", getTypeFromHandleParams);
				}

				return getTypeFromHandle;
			}
		}

		public MethodReference StringFormat1
		{
			get
			{
				if (stringFormat1 == null)
				{
					stringFormat1 = module.GetMethod(typeof(string), "Format", stringFormat1Params);
				}

				return stringFormat1;
			}
		}

		public MethodReference StringFormat2
		{
			get
			{
				if (stringFormat2 == null)
				{
					stringFormat2 = module.GetMethod(typeof(string), "Format", stringFormat2Params);
				}

				return stringFormat2;
			}
		}

		public MethodReference StringFormat3
		{
			get
			{
				if (stringFormat3 == null)
				{
					stringFormat3 = module.GetMethod(typeof(string), "Format", stringFormat3Params);
				}

				return stringFormat3;
			}
		}

		public MethodReference StringFormatParams
		{
			get
			{
				if (stringFormatParams == null)
				{
					stringFormatParams = module.GetMethod(typeof(string), "Format", stringFormatParamsParams);
				}

				return stringFormatParams;
			}
		}

		public MethodsCache(ModuleDefinition module)
		{
			this.module = module;
		}

		public MethodReference GetStringFormat(int paramCount)
		{
			switch (paramCount)
			{
				case 1:
					return StringFormat1;
				case 2:
					return StringFormat2;
				case 3:
					return StringFormat3;
				default:
					return StringFormatParams;
			}
		}
	}
}