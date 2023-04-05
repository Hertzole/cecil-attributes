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
		private MethodReference debugLogContext;
		private MethodReference debugLogError;
		private MethodReference debugLogErrorContext;
		private MethodReference unityObjectEqualityOperation;
		private MethodReference getTypeFromHandle;
		private MethodReference stringFormat1;
		private MethodReference stringFormat2;
		private MethodReference stringFormat3;
		private MethodReference stringFormatParams;
		private MethodReference getComponent;

		private static readonly Type[] debugLogParams = { typeof(object) };
		private static readonly Type[] debugLogContextParams = { typeof(object), typeof(Object) };
		private static readonly Type[] debugLogErrorParams = { typeof(object) };
		private static readonly Type[] debugLogErrorContextParams = { typeof(object), typeof(Object) };
		private static readonly Type[] unityObjectEqualityOperationParams = { typeof(Object), typeof(Object) };
		private static readonly Type[] getTypeFromHandleParams = { typeof(RuntimeTypeHandle) };
		private static readonly Type[] stringFormat1Params = { typeof(string), typeof(object) };
		private static readonly Type[] stringFormat2Params = { typeof(string), typeof(object), typeof(object) };
		private static readonly Type[] stringFormat3Params = { typeof(string), typeof(object), typeof(object), typeof(object) };
		private static readonly Type[] stringFormatParamsParams = { typeof(string), typeof(object[]) };

		private MethodReference DebugLog
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
		
		private MethodReference DebugLogContext
		{
			get
			{
				if (debugLogContext == null)
				{
					debugLogContext = module.GetMethod(typeof(Debug), "Log", debugLogContextParams);
				}

				return debugLogContext;
			}
		}
		
		private MethodReference DebugLogError
		{
			get
			{
				if (debugLogError == null)
				{
					debugLogError = module.GetMethod(typeof(Debug), "LogError", debugLogErrorParams);
				}

				return debugLogError;
			}
		}

		private MethodReference DebugLogErrorContext
		{
			get
			{
				if (debugLogErrorContext == null)
				{
					debugLogErrorContext = module.GetMethod(typeof(Debug), "LogError", debugLogErrorContextParams);
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

		private MethodReference StringFormat1
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

		private MethodReference StringFormat2
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

		private MethodReference StringFormat3
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

		private MethodReference StringFormatParams
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
		
		public MethodReference GetComponent
		{
			get
			{
				if (getComponent == null)
				{
					getComponent = module.GetMethod(typeof(Component), "GetComponent");
				}

				return getComponent;
			}
		}

		public MethodsCache(ModuleDefinition module)
		{
			this.module = module;
		}

		public MethodReference GetDebugLog(bool context, ModuleDefinition targetModule = null)
		{
			return context ? (targetModule != null ? targetModule.ImportReference(DebugLogContext) : DebugLogContext) : (targetModule != null ? targetModule.ImportReference(DebugLog) : DebugLog);
		}
		
		public MethodReference GetDebugLogError(bool context, ModuleDefinition targetModule = null)
		{
			return context ? (targetModule != null ? targetModule.ImportReference(DebugLogErrorContext) : DebugLogErrorContext) : (targetModule != null ? targetModule.ImportReference(DebugLogError) : DebugLogError);
		}

		public MethodReference GetStringFormat(int paramCount, ModuleDefinition targetModule = null)
		{
			switch (paramCount)
			{
				case 1:
					return targetModule != null ? targetModule.ImportReference(StringFormat1) : StringFormat1;
				case 2:
					return targetModule != null ? targetModule.ImportReference(StringFormat2) : StringFormat2;
				case 3:
					return targetModule != null ? targetModule.ImportReference(StringFormat3) : StringFormat3;
				default:
					return targetModule != null ? targetModule.ImportReference(StringFormatParams) : StringFormatParams;
			}
		}
	}
}