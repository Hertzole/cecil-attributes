#if !CECIL_ATTRIBUTES_EXPERIMENTAL_GETCOMPONENT
using System.Collections.Generic;
using System.Reflection;
using Hertzole.CecilAttributes.Interfaces;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
#if CECIL_ATTRIBUTES_DEBUG
using System.Diagnostics;
#endif

namespace Hertzole.CecilAttributes.Editor
{
	public static class GetComponentSceneProcessor
	{
		[InitializeOnLoadMethod]
		private static void RegisterEvent()
		{
			EditorSceneManager.sceneSaving += OnSceneSaving;
			EditorApplication.delayCall += ProcessOnLoad;
		}

		private static void ProcessOnLoad()
		{
			if (!CecilAttributesSettings.Instance.RunSceneProcessorOnReload)
			{
				return;
			}
			
			Process();
		}

		private static void OnSceneSaving(Scene scene, string path)
		{
			if (!CecilAttributesSettings.Instance.RunSceneProcessorOnReload)
			{
				return;
			}
			
			Process();
		}

		[PostProcessScene]
#if CECIL_ATTRIBUTES_DEBUG
		[MenuItem("Tools/Cecil Attributes/Scan Scene")]
#endif
		private static void ScanGetComponent()
		{
			if (!CecilAttributesSettings.Instance.RunSceneProcessorOnReload)
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

#if CECIL_ATTRIBUTES_DEBUG
			Stopwatch sw = Stopwatch.StartNew();
#endif

			IList<FieldInfo> fields = ProcessorHelpers.GetFieldsWithAttribute<GetComponentAttribute>();
			if (fields.Count == 0)
			{
				return;
			}

			for (int i = 0; i < fields.Count; i++)
			{
				if (fields[i].DeclaringType == null)
				{
					continue;
				}

				Object[] allObjects = Resources.FindObjectsOfTypeAll(fields[i].DeclaringType);
				for (int j = 0; j < allObjects.Length; j++)
				{
					if (!(allObjects[j] is Component go))
					{
						continue;
					}

					if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave ||
					    go.gameObject.scene.name == "DontDestroyOnLoad" || PrefabUtility.IsPartOfPrefabAsset(go) ||
					    PrefabUtility.IsPartOfPrefabInstance(go))
					{
						continue;
					}

					PatchComponent(go);
				}
			}

#if CECIL_ATTRIBUTES_DEBUG
			sw.Stop();
			UnityEngine.Debug.Log($"Scene process took {sw.ElapsedMilliseconds}ms");
#endif
		}

		private static void PatchComponent(Component go)
		{
			IGetComponent[] getComponents = go.GetComponents<IGetComponent>();
			for (int i = 0; i < getComponents.Length; i++)
			{
				getComponents[i].FetchComponents();
			}
		}
	}
}
#endif