using System;
using System.Reflection;
using Mono.Cecil;

namespace Hertzole.CecilAttributes.CodeGen
{
	public static partial class WeaverExtensions
	{
		public static TypeReference Void(this ModuleDefinition module)
		{
			return module.ImportReference(typeof(void));
		}

		public static TypeReference GetTypeReference<T>(this ModuleDefinition module)
		{
			return module.ImportReference(typeof(T));
		}

		public static TypeReference GetTypeReference(this ModuleDefinition module, Type type)
		{
			return module.ImportReference(type);
		}

		public static MethodReference GetMethod<T>(this ModuleDefinition module, string methodName)
		{
			return module.GetMethod(typeof(T), methodName);
		}

		public static MethodReference GetMethod(this ModuleDefinition module, Type type, string methodName)
		{
			MethodInfo method = type.GetMethod(methodName);
			if (method == null)
			{
				throw new ArgumentException($"There's no method called {methodName} in {type}.");
			}

			return module.ImportReference(method);
		}

		public static MethodReference GetMethod<T>(this ModuleDefinition module, string methodName, params Type[] parameters)
		{
			return module.GetMethod(typeof(T), methodName, parameters);
		}

		public static MethodReference GetMethod(this ModuleDefinition module, Type type, string methodName, params Type[] parameters)
		{
			MethodInfo method = type.GetMethod(methodName, parameters);
			if (method == null)
			{
				throw new ArgumentException($"There's no method called {methodName} in {type} with {parameters.Length} parameters.");
			}

			return module.ImportReference(method);
		}

		public static MethodReference GetGenericMethod<T>(this ModuleDefinition module, string methodName, params Type[] parameters)
		{
			return module.GetGenericMethod(typeof(T), methodName, parameters);
		}

		public static MethodReference GetGenericMethod(this ModuleDefinition module, Type type, string methodName, params Type[] parameters)
		{
			foreach (MethodInfo method in type.GetMethods())
			{
				ParameterInfo[] p = method.GetParameters();

				if (method.Name != methodName || p.Length != parameters.Length)
				{
					continue;
				}

				for (int i = 0; i < p.Length; i++)
				{
					if (p[i].ParameterType.ToString() == parameters[i].ToString())
					{
						return module.ImportReference(method);
					}
				}
			}

			throw new ArgumentException($"There's no method called {methodName} in {type} with {parameters.Length} parameters.");
		}
		
		public static MethodReference GetConstructor<T>(this ModuleDefinition module, params Type[] parameters)
		{
			return module.GetConstructor(typeof(T), parameters);
		}

		public static MethodReference GetConstructor(this ModuleDefinition module, Type type, params Type[] parameters)
		{
			var result = module.ImportReference(type.GetConstructor(parameters));

			if (result == null)
			{
				throw new ArgumentException($"There's no constructor with those parameters in type {type.FullName}");
			}
            
			return result;
		}
	}
}