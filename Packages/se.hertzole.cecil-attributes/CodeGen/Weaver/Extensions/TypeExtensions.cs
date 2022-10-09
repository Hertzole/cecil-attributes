using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;
using Mono.Collections.Generic;
using UnityEngine;

namespace Hertzole.CecilAttributes.CodeGen
{
	public static partial class WeaverExtensions
	{
		// https://github.com/vis2k/Mirror/blob/master/Assets/Mirror/Editor/Weaver/Extensions.cs#L10
		public static bool Is(this TypeReference tr, Type ttype)
		{
			if (ttype.IsGenericType)
			{
				return tr.GetElementType().FullName == ttype.FullName;
			}

			return tr.FullName == ttype.FullName;
		}

		public static bool Is<T>(this TypeReference tr)
		{
			return Is(tr, typeof(T));
		}

		// https://github.com/vis2k/Mirror/blob/master/Assets/Mirror/Editor/Weaver/Extensions.cs#L23
		public static bool IsSubclassOf(this TypeDefinition td, Type type)
		{
			if (!td.IsClass)
			{
				return false;
			}

			TypeReference parent = td.BaseType;

			if (parent == null)
			{
				return false;
			}

			if (parent.Is(type))
			{
				return true;
			}

			if (parent.CanBeResolved())
			{
				return IsSubclassOf(parent.Resolve(), type);
			}

			return false;
		}

		public static bool IsSubclassOf<T>(this TypeDefinition td)
		{
			return IsSubclassOf(td, typeof(T));
		}

		// https://github.com/vis2k/Mirror/blob/master/Assets/Mirror/Editor/Weaver/Extensions.cs#L87
		public static bool CanBeResolved(this TypeReference parent)
		{
			while (parent != null)
			{
				if (parent.Scope.Name == "Windows")
				{
					return false;
				}

				if (parent.Scope.Name == "mscorlib")
				{
					TypeDefinition resolved = parent.Resolve();
					return resolved != null;
				}

				try
				{
					parent = parent.Resolve().BaseType;
				}
				catch
				{
					return false;
				}
			}

			return true;
		}

		// https://stackoverflow.com/a/26429045/6257193
		public static string GetFriendlyName(this TypeReference type)
		{
			return MakeFriendlyName(type, type.Name);
		}

		// https://stackoverflow.com/a/26429045/6257193
		public static string GetFriendlyFullName(this TypeReference type)
		{
			return MakeFriendlyName(type, type.FullName);
		}

		private static string MakeFriendlyName(IGenericParameterProvider type, string name)
		{
			if (!type.HasGenericParameters)
			{
				return name;
			}
			
			StringBuilder sb = StaticPool<StringBuilder>.Get();
			sb.Clear();
			sb.Append(name);

			if (type.HasGenericParameters)
			{
				int iBacktick = name.IndexOf('`');
				if (iBacktick > 0)
				{
					sb.Remove(iBacktick, sb.Length - iBacktick);
				}

				sb.Append('<');

				Collection<GenericParameter> typeParameters = type.GenericParameters;
				for (int i = 0; i < typeParameters.Count; ++i)
				{
					string typeParamName = $"{{{i}}}";
					sb.Append(i == 0 ? typeParamName : $", {typeParamName}");
				}

				sb.Append('>');
			}

			string result = sb.ToString();

			sb.Clear();
			StaticPool<StringBuilder>.Release(sb);

			return result;
		}

		public static bool IsCollection(this TypeReference type)
		{
			return type.IsArray() || type.IsList() || type.IsDictionary();
		}

		public static bool IsArray(this TypeReference type)
		{
			return type.IsArray;
		}

		public static bool IsList(this TypeReference type)
		{
			return type.Is(typeof(List<>));
		}

		public static bool IsDictionary(this TypeReference type)
		{
			return type.Is(typeof(Dictionary<,>));
		}

		public static TypeReference GetCollectionType(this TypeReference type)
		{
			if (!type.IsCollection())
			{
				return type;
			}

			if (type.IsArray())
			{
				return type.Module.ImportReference(type.Resolve());
			}

			if (type.IsList() && type is GenericInstanceType generic && generic.GenericArguments.Count == 1)
			{
				TypeDefinition resolved = generic.GenericArguments[0].GetElementType().Resolve();
				if (resolved.Is<GameObject>() || resolved.IsSubclassOf<Component>())
				{
					return type.Module.ImportReference(resolved);
				}
			}

			return type;
		}

		public static bool ImplementsInterface<T>(this TypeDefinition type)
		{
			if (!type.HasInterfaces)
			{
				return false;
			}

			Type targetType = typeof(T);

			for (int i = 0; i < type.Interfaces.Count; i++)
			{
				if (type.Interfaces[i].InterfaceType.FullName == targetType.FullName)
				{
					return true;
				}
			}

			return false;
		}

		public static MethodDefinition AddMethod<T>(this TypeDefinition type, string name, MethodAttributes attributes)
		{
			return type.AddMethod(name, attributes, type.Module.ImportReference(typeof(T)));
		}

		public static MethodDefinition AddMethod(this TypeDefinition type, string name, MethodAttributes attributes)
		{
			return type.AddMethod(name, attributes, type.Module.TypeSystem.Void);
		}

		public static MethodDefinition AddMethod(this TypeDefinition type, string name, MethodAttributes attributes, TypeReference returnType)
		{
			MethodDefinition m = new MethodDefinition(name, attributes, returnType);
			type.Methods.Add(m);

			return m;
		}

		public static MethodDefinition AddMethodOverride<T>(this TypeDefinition type, string name, MethodAttributes attributes, params MethodReference[] overrides)
		{
			return type.AddMethodOverride(name, attributes, type.Module.ImportReference(typeof(T)), overrides);
		}

		public static MethodDefinition AddMethodOverride(this TypeDefinition type, string name, MethodAttributes attributes, params MethodReference[] overrides)
		{
			return type.AddMethodOverride(name, attributes, type.Module.Void(), overrides);
		}

		public static MethodDefinition AddMethodOverride(this TypeDefinition type, string name, MethodAttributes attributes, TypeReference returnType, MethodReference override1)
		{
			MethodDefinition m = new MethodDefinition(name, attributes, returnType);
			m.Overrides.Add(override1);

			type.Methods.Add(m);

			return m;
		}

		public static MethodDefinition AddMethodOverride(this TypeDefinition type, string name, MethodAttributes attributes, TypeReference returnType, MethodReference override1, MethodReference override2)
		{
			MethodDefinition m = new MethodDefinition(name, attributes, returnType);
			m.Overrides.Add(override1);
			m.Overrides.Add(override2);

			type.Methods.Add(m);

			return m;
		}

		public static MethodDefinition AddMethodOverride(this TypeDefinition type, string name, MethodAttributes attributes, TypeReference returnType, MethodReference override1, MethodReference override2, MethodReference override3)
		{
			MethodDefinition m = new MethodDefinition(name, attributes, returnType);
			m.Overrides.Add(override1);
			m.Overrides.Add(override2);
			m.Overrides.Add(override3);

			type.Methods.Add(m);

			return m;
		}

		public static MethodDefinition AddMethodOverride(this TypeDefinition type, string name, MethodAttributes attributes, TypeReference returnType, params MethodReference[] overrides)
		{
			MethodDefinition m = new MethodDefinition(name, attributes, returnType);
			for (int i = 0; i < overrides.Length; i++)
			{
				m.Overrides.Add(overrides[i]);
			}

			type.Methods.Add(m);

			return m;
		}
	}
}