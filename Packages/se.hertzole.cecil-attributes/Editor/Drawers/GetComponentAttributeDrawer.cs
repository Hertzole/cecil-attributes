using System;
using System.Collections.Generic;
using System.Reflection;
using Hertzole.CecilAttributes.Interfaces;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hertzole.CecilAttributes.Editor
{
	[CustomPropertyDrawer(typeof(GetComponentAttribute))]
	public class GetComponentAttributeDrawer : PropertyDrawer
	{
		[InitializeOnLoadMethod]
		private static void ScanGetComponent()
		{
#if CECIL_ATTRIBUTES_PARRAEL_SYNC
			if (ParrelSync.ClonesManager.IsClone())
			{
				return;
			}
#endif
			
#if UNITY_2020_1_OR_NEWER
			TypeCache.FieldInfoCollection fields = TypeCache.GetFieldsWithAttribute<GetComponentAttribute>();
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
						if (typeFields[k].GetCustomAttribute<GetComponentAttribute>() != null)
						{
							fields.Add(typeFields[k]);
						}
					}
				}
			}
#endif

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
						string assetPath = AssetDatabase.GetAssetPath(targetObjects[i]);
						GameObject root = PrefabUtility.LoadPrefabContents(assetPath);
						IGetComponent[] getComps = root.GetComponentsInChildren<IGetComponent>();
						for (int j = 0; j < getComps.Length; j++)
						{
							getComps[j].FetchComponents();
						}

						PrefabUtility.SaveAsPrefabAsset(root, assetPath);
						PrefabUtility.UnloadPrefabContents(root);
						
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

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			bool show = true;

			if (attribute is GetComponentAttribute att)
			{
				show = att.showInInspector;
			}

			if (show)
			{
				bool oEnabled = GUI.enabled;
				GUI.enabled = false;
				EditorGUI.PropertyField(position, property, label);
				GUI.enabled = oEnabled;
			}

			if (property.objectReferenceValue == null && property.serializedObject.targetObject is IGetComponent comp)
			{
				comp.FetchComponents();
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			bool show = true;

			if (attribute is GetComponentAttribute att)
			{
				show = att.showInInspector;
			}

			if (show)
			{
				return base.GetPropertyHeight(property, label);
			}

			return -EditorGUIUtility.standardVerticalSpacing;
		}
	}
}