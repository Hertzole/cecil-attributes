using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Hertzole.CecilAttributes.Tests
{
	public class RequiredTests
	{
		[UnityTest]
		public IEnumerator Base_WithAwake_LogsError()
		{
			yield return new RequiredTestLogsError<RequiredBaseWithAwake>();
		}

		[UnityTest]
		public IEnumerator Base_WithVirtualAwake_LogsError()
		{
			yield return new RequiredTestLogsError<RequiredBaseWithVirtualAwake>();
		}

		[UnityTest]
		public IEnumerator Base_WithoutAwake_LogsError()
		{
			yield return new RequiredTestLogsError<RequiredBaseWithoutAwake>();
		}

		[UnityTest]
		public IEnumerator Child_WithAwake_ParentWithoutAwake_LogsError()
		{
			yield return new RequiredTestLogsError<RequiredChild_WithAwake_ParentWithoutAwake>();
		}

		[UnityTest]
		public IEnumerator Child_WithAwake_ParentWithVirtualAwake_LogsError()
		{
			yield return new RequiredTestLogsError<RequiredChild_WithAwake_ParentWithVirtualAwake>();
		}

		[UnityTest]
		public IEnumerator Child_WithoutAwake_ParentWithAwake_LogsError()
		{
			yield return new RequiredTestLogsError<RequiredChild_WithoutAwake_ParentWithAwake>();
		}

		[UnityTest]
		public IEnumerator Child_WithoutAwake_ParentWithAwakeWithoutField_LogsError()
		{
			yield return new RequiredTestLogsError<RequiredChild_WithoutAwake_ParentWithAwakeWithoutField>();
		}

		[UnityTest]
		public IEnumerator Base_WithAwake_CallsAwake()
		{
			yield return new RequiredTestCallsAwake<RequiredBaseWithAwake>(go =>
			{
				BoxCollider box = go.gameObject.AddComponent<BoxCollider>();
				go.Box = box;
			}, "Base Awake");
		}

		[UnityTest]
		public IEnumerator Base_WithVirtualAwake_CallsAwake()
		{
			yield return new RequiredTestCallsAwake<RequiredBaseWithVirtualAwake>(go =>
			{
				BoxCollider box = go.gameObject.AddComponent<BoxCollider>();
				go.Box = box;
			}, "Base Awake");
		}

		[UnityTest]
		public IEnumerator Base_WithoutAwake_CallsAwake()
		{
			yield return new RequiredTestCallsAwake<RequiredBaseWithoutAwake>(go =>
			{
				BoxCollider box = go.gameObject.AddComponent<BoxCollider>();
				go.Box = box;
			});
		}

		[UnityTest]
		public IEnumerator Child_WithAwake_ParentWithoutAwake_CallsAwake()
		{
			yield return new RequiredTestCallsAwake<RequiredChild_WithAwake_ParentWithoutAwake>(go =>
			{
				BoxCollider box = go.gameObject.AddComponent<BoxCollider>();
				go.Box = box;
				Renderer ren = go.gameObject.AddComponent<MeshRenderer>();
				go.Ren = ren;
			}, "Child Awake");
		}

		[UnityTest]
		public IEnumerator Child_WithAwake_ParentWithVirtualAwake_CallsAwake()
		{
			yield return new RequiredTestCallsAwake<RequiredChild_WithAwake_ParentWithVirtualAwake>(go =>
			{
				BoxCollider box = go.gameObject.AddComponent<BoxCollider>();
				go.Box = box;
				Renderer ren = go.gameObject.AddComponent<MeshRenderer>();
				go.Ren = ren;
			}, "Base Awake", "Child Awake");
		}

		[UnityTest]
		public IEnumerator Child_WithoutAwake_ParentWithAwake_CallsAwake()
		{
			yield return new RequiredTestCallsAwake<RequiredChild_WithoutAwake_ParentWithAwake>(go =>
			{
				BoxCollider box = go.gameObject.AddComponent<BoxCollider>();
				go.Box = box;
				Renderer ren = go.gameObject.AddComponent<MeshRenderer>();
				go.Ren = ren;
			}, "Base Awake");
		}

		[UnityTest]
		public IEnumerator Child_WithoutAwake_ParentWithAwakeWithoutField_CallsAwake()
		{
			yield return new RequiredTestCallsAwake<RequiredChild_WithoutAwake_ParentWithAwakeWithoutField>(go =>
			{
				Renderer ren = go.gameObject.AddComponent<MeshRenderer>();
				go.Ren = ren;
			}, "Base Awake");
		}

		private class RequiredTestLogsError<T> : IEditModeTestYieldInstruction where T : RequiredBase
		{
			public bool ExpectDomainReload { get { return false; } }
			public bool ExpectedPlaymodeState { get; private set; }

			public IEnumerator Perform()
			{
				List<string> fields = new List<string>();

				Type parent = typeof(T);
				while (parent != null)
				{
					foreach (FieldInfo field in parent.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
					{
						if (field.GetCustomAttributes(typeof(RequiredAttribute), true).Length > 0)
						{
							fields.Add(field.Name);
						}
					}

					parent = parent.BaseType;

					if (parent == typeof(MonoBehaviour))
					{
						break;
					}
				}

				// The list must be reversed to be in the correct order.
				// It adds children first but when fields are checked, it will check the parents first.
				fields.Reverse();

				ExpectedPlaymodeState = false;
				EditorApplication.UnlockReloadAssemblies();
				while (EditorApplication.isPlaying)
				{
					yield return null;
				}

				Assert.IsFalse(Application.isPlaying);

				GameObject go = new GameObject("Test object");

				// Enter play mode
				ExpectedPlaymodeState = true;
				EditorApplication.UnlockReloadAssemblies();
				EditorApplication.isPlaying = true;
				while (!EditorApplication.isPlaying)
				{
					yield return null;
				}

				Assert.IsTrue(Application.isPlaying);

				try
				{
					go.AddComponent<T>();

					for (int i = 0; i < fields.Count; i++)
					{
						LogAssert.Expect(LogType.Error, $"'{fields[i]}' is not assigned on {go.name}. It is required. Please assign it in the inspector.");
					}
				}
				catch (AssertionException e)
				{
					Debug.LogAssertion(e);
					EditorApplication.isPlaying = false;
					Object.DestroyImmediate(go);
					yield break;
				}

				ExpectedPlaymodeState = false;
				EditorApplication.isPlaying = false;
				while (EditorApplication.isPlaying)
				{
					yield return null;
				}

				Object.DestroyImmediate(go);
			}
		}

		private class RequiredTestCallsAwake<T> : IEditModeTestYieldInstruction where T : RequiredBase
		{
			private readonly Action<T> setProperties;
			private readonly string[] logs;

			public bool ExpectDomainReload { get { return false; } }
			public bool ExpectedPlaymodeState { get; private set; }

			public RequiredTestCallsAwake(Action<T> setProperties, params string[] logs)
			{
				this.setProperties = setProperties;
				this.logs = logs;
			}

			public IEnumerator Perform()
			{
				List<string> fields = new List<string>();

				Type parent = typeof(T);
				while (parent != null)
				{
					foreach (FieldInfo field in parent.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
					{
						if (field.GetCustomAttributes(typeof(RequiredAttribute), true).Length > 0)
						{
							fields.Add(field.Name);
						}
					}

					parent = parent.BaseType;

					if (parent == typeof(MonoBehaviour))
					{
						break;
					}
				}

				// The list must be reversed to be in the correct order.
				// It adds children first but when fields are checked, it will check the parents first.
				fields.Reverse();

				ExpectedPlaymodeState = false;
				EditorApplication.UnlockReloadAssemblies();
				while (EditorApplication.isPlaying)
				{
					yield return null;
				}

				Assert.IsFalse(Application.isPlaying);

				GameObject go = new GameObject("Test object");

				// Enter play mode
				ExpectedPlaymodeState = true;
				EditorApplication.UnlockReloadAssemblies();
				EditorApplication.isPlaying = true;
				while (!EditorApplication.isPlaying)
				{
					yield return null;
				}

				Assert.IsTrue(Application.isPlaying);

				GameObject clone = null;

				try
				{
					T comp = go.AddComponent<T>();
					setProperties.Invoke(comp);

					// Expect all errors first.
					for (int i = 0; i < fields.Count; i++)
					{
						LogAssert.Expect(LogType.Error, $"'{fields[i]}' is not assigned on {go.name}. It is required. Please assign it in the inspector.");
					}

					clone = Object.Instantiate(go);

					// Then expect all logs.
					for (int i = 0; i < logs.Length; i++)
					{
						LogAssert.Expect(LogType.Log, logs[i]);
					}
				}
				catch (AssertionException e)
				{
					Debug.LogAssertion(e);
					EditorApplication.isPlaying = false;
					Object.DestroyImmediate(go);
					if (clone != null)
					{
						Object.DestroyImmediate(clone);
					}

					yield break;
				}

				EditorApplication.UnlockReloadAssemblies();
				ExpectedPlaymodeState = false;
				EditorApplication.isPlaying = false;
				while (EditorApplication.isPlaying)
				{
					yield return null;
				}

				Object.DestroyImmediate(go);

				if (clone != null)
				{
					Object.DestroyImmediate(clone);
				}
			}
		}
	}
}