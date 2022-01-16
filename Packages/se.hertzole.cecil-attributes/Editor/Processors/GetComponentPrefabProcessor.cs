using System;
using System.Collections.Generic;
using System.Reflection;
using Hertzole.CecilAttributes.Interfaces;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hertzole.CecilAttributes.Editor
{
	public static class GetComponentPrefabProcessor
	{
		[InitializeOnLoadMethod]
		#if CECIL_ATTRIBUTES_DEBUG
		[MenuItem("Tools/Cecil Attributes/Scan Prefabs")]
#endif
		private static void ScanGetComponent()
		{
			Process();
		}
		
		public static void Process()
		{
			if (!ProcessorHelpers.CanProcess())
			{
				return;
			}

			IEnumerable<FieldInfo> fields = ProcessorHelpers.GetFieldsWithAttribute<GetComponentAttribute>();

			string[] allPrefabs = AssetDatabase.FindAssets("t:Prefab", new[]
			{
				"Assets/"
			});

			List<Object> scannedObjects = new List<Object>();
			List<Object> objectList = new List<Object>();

			foreach (FieldInfo field in fields)
			{
				if (field.DeclaringType == null)
				{
					continue;
				}

				Object[] targetObjects = FindAllObjectsOfType(field.DeclaringType.FullName, allPrefabs, objectList);
				for (int i = 0; i < targetObjects.Length; i++)
				{
					if (scannedObjects.Contains(targetObjects[i]))
					{
						continue;
					}

					if (targetObjects[i] is Component)
					{
						PatchPrefab(targetObjects[i]);

						scannedObjects.Add(targetObjects[i]);
					}
				}
			}

			AssetDatabase.Refresh();
		}

		private static Object[] FindAllObjectsOfType(string typeName, string[] guids, List<Object> objectsList)
		{
			objectsList.Clear();

			for (int i = 0; i < guids.Length; i++)
			{
				Object[] assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GUIDToAssetPath(guids[i]));
				for (int j = 0; j < assets.Length; j++)
				{
					if (assets[j] != null && IsSubclassOf(assets[j].GetType(), typeName))
					{
						objectsList.Add(assets[j]);
					}
				}
			}

			return objectsList.Count > 0 ? objectsList.ToArray() : Array.Empty<Object>();
		}

		private static void PatchPrefab(Object obj)
		{
			string assetPath = AssetDatabase.GetAssetPath(obj);
			GameObject root = PrefabUtility.LoadPrefabContents(assetPath);
			
			if (root == null || string.IsNullOrEmpty(assetPath))
			{
				return;
			}
			
			IGetComponent[] getComps = root.GetComponentsInChildren<IGetComponent>();
			for (int j = 0; j < getComps.Length; j++)
			{
				getComps[j].FetchComponents();
			}

			PrefabUtility.SaveAsPrefabAsset(root, assetPath);
			PrefabUtility.UnloadPrefabContents(root);
		}

		private static bool IsSubclassOf(Type type, string typeName)
		{
			if (type.FullName == typeName)
			{
				return true;
			}

			type = type.BaseType;

			while (type != null)
			{
				if (type.FullName == typeName)
				{
					return true;
				}

				type = type.BaseType;
			}

			return false;
		}
	}
}