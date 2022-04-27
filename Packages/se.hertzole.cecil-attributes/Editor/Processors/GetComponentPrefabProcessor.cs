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
			EditorApplication.delayCall += ProcessOnLoad;
		}

		private static void ProcessOnLoad()
		{
			if (!CecilAttributesSettings.Instance.RunPrefabProcessor)
			{
				return;
			}
			
			Process();
		}

		public static void Process()
		{
			if (!ProcessorHelpers.CanProcess())
			{
				return;
			}

			EditorUtility.DisplayProgressBar("Cecil Attributes", "Scanning Prefabs", 0);

			IEnumerable<FieldInfo> fields = ProcessorHelpers.GetFieldsWithAttribute<GetComponentAttribute>();

			string[] allPrefabs = AssetDatabase.FindAssets("t:Prefab", new[]
			{
				"Assets/"
			});

			List<Object> scannedObjects = new List<Object>();
			List<Object> objectList = new List<Object>();
			Dictionary<FieldInfo, Object[]> fieldToObjects = new Dictionary<FieldInfo, Object[]>();

			int totalProgress = GetTotalProgress(fields, allPrefabs, objectList, fieldToObjects);
			int progress = 0;

			bool isDirty = false;
			
			foreach (KeyValuePair<FieldInfo, Object[]> pair in fieldToObjects)
			{
				foreach (Object o in pair.Value)
				{
					EditorUtility.DisplayProgressBar("Cecil Attributes", $"Scanning prefab {o.name} for {pair.Key.Name}", (float) progress / totalProgress);

					if (scannedObjects.Contains(o))
					{
						continue;
					}

					if (o is Component)
					{
						bool isPrefabDirty = PatchPrefab(o);
						scannedObjects.Add(o);
						if (isPrefabDirty)
						{
							isDirty = true;
						}
					}

					progress++;
				}
			}

			if (isDirty)
			{
				AssetDatabase.Refresh();
			}

			EditorUtility.ClearProgressBar();
		}

		private static int GetTotalProgress(IEnumerable<FieldInfo> fields, string[] allPrefabs, List<Object> objectList, Dictionary<FieldInfo, Object[]> objects)
		{
			int progress = 0;
			foreach (FieldInfo field in fields)
			{
				if (field.DeclaringType == null)
				{
					continue;
				}

				Object[] targetObjects = FindAllObjectsOfType(field.DeclaringType.FullName, allPrefabs, objectList);
				objects.Add(field, targetObjects);
				progress += targetObjects.Length;
			}

			return progress;
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

		private static bool PatchPrefab(Object obj)
		{
			string assetPath = AssetDatabase.GetAssetPath(obj);
			if (string.IsNullOrEmpty(assetPath))
			{
				return false;
			}

			GameObject root = PrefabUtility.LoadPrefabContents(assetPath);

			if (root == null)
			{
				PrefabUtility.UnloadPrefabContents(root);
				return false;
			}

			IGetComponent[] getComps = root.GetComponentsInChildren<IGetComponent>();
			bool isDirty = false;
			for (int j = 0; j < getComps.Length; j++)
			{
				bool isComponentDirty = getComps[j].FetchComponents();
				if (isComponentDirty)
				{
					isDirty = true;
				}
			}

			if(isDirty)
			{
				PrefabUtility.SaveAsPrefabAsset(root, assetPath);
			}
			PrefabUtility.UnloadPrefabContents(root);

			return true;
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