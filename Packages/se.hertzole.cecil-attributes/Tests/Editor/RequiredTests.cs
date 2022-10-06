using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Hertzole.CecilAttributes.Tests
{
	public class RequiredTests
	{
		[UnityTest]
		public IEnumerator Base_WithAwake()
		{
			yield return new RequiredTest<RequiredBaseWithAwake>("box");
		}

		[UnityTest]
		public IEnumerator Base_WithVirtualAwake()
		{
			yield return new RequiredTest<RequiredBaseWithVirtualAwake>("box");
		}

		[UnityTest]
		public IEnumerator Base_WithoutAwake()
		{
			yield return new RequiredTest<RequiredBaseWithoutAwake>("box");
		}

		[UnityTest]
		public IEnumerator Child_WithAwake_ParentWithoutAwake()
		{
			yield return new RequiredTest<RequiredChild_WithAwake_ParentWithoutAwake>("box", "ren");
		}

		[UnityTest]
		public IEnumerator Child_WithAwake_ParentWithVirtualAwake()
		{
			yield return new RequiredTest<RequiredChild_WithAwake_ParentWithVirtualAwake>("box", "ren");
		}

		private class RequiredTest<T> : IEditModeTestYieldInstruction where T : RequiredBase
		{
			private readonly string[] variableNames;

			public bool ExpectDomainReload { get { return false; } }
			public bool ExpectedPlaymodeState { get; private set; }

			public RequiredTest(params string[] variableNames)
			{
				this.variableNames = variableNames;
			}

			public IEnumerator Perform()
			{
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

					for (int i = 0; i < variableNames.Length; i++)
					{
						LogAssert.Expect(LogType.Error, $"'{variableNames[i]}' is not assigned on {go.name}. It is required. Please assign it in the inspector.");
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
	}
}