using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable InconsistentNaming

namespace Hertzole.CecilAttributes.Tests
{
	public class FindPropertyTests
	{
		private readonly List<Object> objects = new List<Object>();

#pragma warning disable CS0414 // The field 'FindPropertyTests._test' is assigned but its value is never used
		private class TargetClass : MonoBehaviour
		{
			[SerializeField]
			private string stringValue = default;
			[SerializeField]
			private int intValue = default;
			[SerializeField]
			private NestedClass nested = default;

			private float floatValue = default;
		}

		[Serializable]
		private class NestedClass
		{
			[SerializeField]
			private Vector3 vector = default;
		}

		[CustomEditor(typeof(TargetClass))]
		private class TargetEditor : Editor
		{
			[FindProperty]
			public SerializedProperty stringValue;
			[FindProperty]
			public SerializedProperty intValue;
		}

		[CustomEditor(typeof(TargetClass))]
		private class TargetEditor_OnEnable : Editor
		{
			[FindProperty]
			public SerializedProperty stringValue;
			[FindProperty]
			public SerializedProperty intValue;

			private void OnEnable()
			{
				Debug.Log("OnEnable");
			}
		}

		[CustomEditor(typeof(TargetClass))]
		private class TargetEditor_CustomNames : Editor
		{
			[FindProperty("stringValue")]
			public SerializedProperty strVal;
			[FindProperty("intValue")]
			public SerializedProperty iValue;
		}

		[CustomEditor(typeof(TargetClass))]
		private class TargetEditor_OnEnable_CustomNames : Editor
		{
			[FindProperty("stringValue")]
			public SerializedProperty strVal;
			[FindProperty("intValue")]
			public SerializedProperty iValue;

			private void OnEnable()
			{
				Debug.Log("OnEnable");
			}
		}

		[CustomEditor(typeof(TargetClass))]
		private class TargetEditor_Nested : Editor
		{
			[FindProperty]
			public SerializedProperty stringValue;
			[FindProperty]
			public SerializedProperty intValue;
			[FindProperty("nested/vector")]
			public SerializedProperty vector;
		}

		[CustomEditor(typeof(TargetClass))]
		private class TargetEditor_OnEnable_Nested : Editor
		{
			[FindProperty]
			public SerializedProperty stringValue;
			[FindProperty]
			public SerializedProperty intValue;
			[FindProperty("nested/vector")]
			public SerializedProperty vector;

			private void OnEnable()
			{
				Debug.Log("OnEnable");
			}
		}

		[CustomEditor(typeof(TargetClass))]
		private class TargetEditor_CustomNames_Nested : Editor
		{
			[FindProperty("stringValue")]
			public SerializedProperty strValue;
			[FindProperty("intValue")]
			public SerializedProperty iValue;
			[FindProperty("nested/vector")]
			public SerializedProperty v;
		}

		[CustomEditor(typeof(TargetClass))]
		private class TargetEditor_OnEnable_CustomNames_Nested : Editor
		{
			[FindProperty("stringValue")]
			public SerializedProperty strValue;
			[FindProperty("intValue")]
			public SerializedProperty iValue;
			[FindProperty("nested/vector")]
			public SerializedProperty v;

			private void OnEnable()
			{
				Debug.Log("OnEnable");
			}
		}
#pragma warning restore CS0414 // The field 'FindPropertyTests._test' is assigned but its value is never used

		[SetUp]
		public void SetUp()
		{
			objects.Clear();
		}

		[TearDown]
		public void TearDown()
		{
			for (int i = 0; i < objects.Count; i++)
			{
				Object.DestroyImmediate(objects[i]);
			}

			objects.Clear();
		}

		[Test]
		public void FindProperties_WithoutOnEnable()
		{
			TestEditor<TargetEditor>(editor =>
			{
				Assert.IsNotNull(editor.stringValue);
				Assert.IsNotNull(editor.intValue);
			});
		}

		[Test]
		public void FindProperties_WithOnEnable()
		{
			TestEditor<TargetEditor_OnEnable>(editor =>
			{
				Assert.IsNotNull(editor.stringValue);
				Assert.IsNotNull(editor.intValue);
			});
		}

		[Test]
		public void FindProperties_WithoutOnEnable_CustomNames()
		{
			TestEditor<TargetEditor_CustomNames>(editor =>
			{
				Assert.IsNotNull(editor.strVal);
				Assert.IsNotNull(editor.iValue);
			});
		}

		[Test]
		public void FindProperties_WithOnEnable_CustomNames()
		{
			TestEditor<TargetEditor_OnEnable_CustomNames>(editor =>
			{
				Assert.IsNotNull(editor.strVal);
				Assert.IsNotNull(editor.iValue);
			});
		}

		[Test]
		public void FindProperties_WithoutOnEnable_Nested()
		{
			TestEditor<TargetEditor_Nested>(editor =>
			{
				Assert.IsNotNull(editor.stringValue);
				Assert.IsNotNull(editor.intValue);
				Assert.IsNotNull(editor.vector);
			});
		}

		[Test]
		public void FindProperties_WithOnEnable_Nested()
		{
			TestEditor<TargetEditor_OnEnable_Nested>(editor =>
			{
				Assert.IsNotNull(editor.stringValue);
				Assert.IsNotNull(editor.intValue);
				Assert.IsNotNull(editor.vector);
			});
		}

		[Test]
		public void FindProperties_WithoutOnEnable_CustomNames_Nested()
		{
			TestEditor<TargetEditor_CustomNames_Nested>(editor =>
			{
				Assert.IsNotNull(editor.strValue);
				Assert.IsNotNull(editor.iValue);
				Assert.IsNotNull(editor.v);
			});
		}

		[Test]
		public void FindProperties_WithOnEnable_CustomNames_Nested()
		{
			TestEditor<TargetEditor_OnEnable_CustomNames_Nested>(editor =>
			{
				Assert.IsNotNull(editor.strValue);
				Assert.IsNotNull(editor.iValue);
				Assert.IsNotNull(editor.v);
			});
		}

		private void TestEditor<T>(Action<T> assertEditor) where T : Editor
		{
			GameObject go = new GameObject();
			TargetClass component = go.AddComponent<TargetClass>();
			objects.Add(go);

			Editor editor = Editor.CreateEditor(component, typeof(T));

			Assert.IsTrue(editor is T);

			T targetEditor = editor as T;

			assertEditor.Invoke(targetEditor);
		}
	}
}