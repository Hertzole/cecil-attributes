using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Hertzole.CecilAttributes.Editor
{
	public static class ProcessorHelpers
	{
		private static readonly Dictionary<Type, IList<FieldInfo>> typeCache = new Dictionary<Type, IList<FieldInfo>>();

		public static bool CanProcess()
		{
#if CECIL_ATTRIBUTES_PARRAEL_SYNC
			if (ParrelSync.ClonesManager.IsClone())
			{
				return false;
			}
#endif
			
			return true;
		}
		
		public static IList<FieldInfo> GetFieldsWithAttribute<T>() where T : Attribute
		{
			if (typeCache.TryGetValue(typeof(T), out IList<FieldInfo> cachedFields))
			{
				return cachedFields;
			}
			
#if UNITY_2020_1_OR_NEWER
			TypeCache.FieldInfoCollection fields = TypeCache.GetFieldsWithAttribute<T>();
#else
			List<FieldInfo> fields = new List<FieldInfo>();
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Type[] types = assemblies[i].GetTypes();
				for (int j = 0; j < types.Length; j++)
				{
					FieldInfo[] typeFields = types[j].GetFields();
					for (int k = 0; k < typeFields.Length; k++)
					{
						if (typeFields[k].GetCustomAttribute<T>() != null)
						{
							fields.Add(typeFields[k]);
						}
					}
				}
			}
#endif

			typeCache.Add(typeof(T), fields);

			return fields;
		}
	}
}