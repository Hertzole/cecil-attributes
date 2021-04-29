using Mono.Cecil;
using System;
using Mono.Collections.Generic;

namespace Hertzole.CecilAttributes.Editor
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
            else
            {
                return tr.FullName == ttype.FullName;
            }
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

        private static string MakeFriendlyName(TypeReference type, string name)
        {
            string friendlyName = name;
            if (type.HasGenericParameters)
            {
                int iBacktick = friendlyName.IndexOf('`');
                if (iBacktick > 0)
                {
                    friendlyName = friendlyName.Remove(iBacktick);
                }
                friendlyName += "<";
                Collection<GenericParameter> typeParameters = type.GenericParameters;
                for (int i = 0; i < typeParameters.Count; ++i)
                {
                    string typeParamName = $"{{{i}}}";
                    friendlyName += (i == 0 ? typeParamName : ", " + typeParamName);
                }
                friendlyName += ">";
            }

            return friendlyName;
        }

        public static bool ImplementsInterface(this TypeDefinition type, InterfaceImplementation baseInterface)
        {
            if (!type.HasInterfaces)
            {
                return false;
            }

            for (int i = 0; i < type.Interfaces.Count; i++)
            {
                if (type.Interfaces[i].InterfaceType.FullName == baseInterface.InterfaceType.FullName)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ImplementsInterface<T>(this TypeDefinition type)
        {
            return type.ImplementsInterface(new InterfaceImplementation(type.Module.ImportReference(typeof(T))));
        }
    }
}
