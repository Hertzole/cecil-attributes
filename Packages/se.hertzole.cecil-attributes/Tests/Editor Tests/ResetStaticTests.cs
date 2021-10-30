using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

namespace Hertzole.CecilAttributes.Tests
{
	public class ResetStaticTests
	{
		private enum TestEnum
		{
			Value1 = 0,
			Value3 = 1, 
			Consistency = 2
		}
		
		private static class TestClass
		{
			[ResetStatic]
			public static int defaultPrimitive;
			[ResetStatic]
			public static int valuePrimitive = 42;
			[ResetStatic]
			public static int DefaultPrimitive { get; set; }
			[ResetStatic]
			public static int ValuePrimitive { get; set; } = 42;
			[ResetStatic]
			public static Vector3Int defaultValueType;
			[ResetStatic]
			public static Vector3Int valueValueType = new Vector3Int(42, 69, 420);
			[ResetStatic]
			public static Vector3Int DefaultValueType { get; set; }
			[ResetStatic]
			public static Vector3Int ValueValueType { get; set; } = new Vector3Int(42, 69, 420);
			[ResetStatic]
			public static string defaultString;
			[ResetStatic]
			public static string valueString = "Hello world!";
			[ResetStatic]
			public static string DefaultString { get; set; }
			[ResetStatic]
			public static string ValueString { get; set; } = "Hello world!";
			[ResetStatic]
			public static TestEnum defaultEnum;
			[ResetStatic]
			public static TestEnum valueEnum = TestEnum.Consistency;
			[ResetStatic]
			public static TestEnum DefaultEnum { get; set; }
			[ResetStatic]
			public static TestEnum ValueEnum { get; set; } = TestEnum.Consistency;
		}

		// private static class GenericTestClass<T> where T: MonoBehaviour
		// {
		// 	// [ResetStatic]
		// 	public static T field;
		// 	// [ResetStatic]
		// 	public static T Property { get; set; }
		// 	public static T Property1 { get; set; }
		// 	public static T Property2 { get; set; }
		// 	public static T Property3 { get; set; }
		// 	public static T Property4 { get; set; }
		// 	public static T Property5 { get; set; }
		//
		// 	private static void ResetStatic()
		// 	{
		// 		field = default;
		// 		Property = default;
		// 		Property1 = default;
		// 		Property2 = default;
		// 		Property3 = default;
		// 		Property4 = default;
		// 		Property5 = default;
		// 	}
		// }

		[UnitySetUp]
		public IEnumerator SetUp()
		{
			yield return new EnterPlayMode(false);
		}

		[UnityTest]
		public IEnumerator PrimitiveDefaultField()
		{
			yield return new ResetStaticTest<int>(0, 100, x => TestClass.defaultPrimitive = x, () => TestClass.defaultPrimitive);
		}
		
		[UnityTest]
		public IEnumerator PrimitiveValueField()
		{
			yield return new ResetStaticTest<int>(42, 100, x => TestClass.valuePrimitive = x, () => TestClass.valuePrimitive);
		}
		
		[UnityTest]
		public IEnumerator PrimitiveDefaultProperty()
		{
			yield return new ResetStaticTest<int>(0, 100, x => TestClass.DefaultPrimitive = x, () => TestClass.DefaultPrimitive);
		}
		
		[UnityTest]
		public IEnumerator PrimitiveValueProperty()
		{
			yield return new ResetStaticTest<int>(42, 100, x => TestClass.ValuePrimitive = x, () => TestClass.ValuePrimitive);
		}
		
		[UnityTest]
		public IEnumerator ValueTypeDefaultField()
		{
			yield return new ResetStaticTest<Vector3Int>(Vector3Int.zero, new Vector3Int(1, 2, 3), x => TestClass.defaultValueType = x, () => TestClass.defaultValueType);
		}
		
		[UnityTest]
		public IEnumerator ValueTypeValueField()
		{
			yield return new ResetStaticTest<Vector3Int>(new Vector3Int(42, 69, 420), new Vector3Int(1, 2, 3), x => TestClass.valueValueType = x, () => TestClass.valueValueType);
		}
		
		[UnityTest]
		public IEnumerator ValueTypeDefaultProperty()
		{
			yield return new ResetStaticTest<Vector3Int>(Vector3Int.zero, new Vector3Int(1, 2, 3), x => TestClass.DefaultValueType = x, () => TestClass.DefaultValueType);
		}
		
		[UnityTest]
		public IEnumerator ValueTypeValueProperty()
		{
			yield return new ResetStaticTest<Vector3Int>(new Vector3Int(42, 69, 420), new Vector3Int(1, 2, 3), x => TestClass.ValueValueType = x, () => TestClass.ValueValueType);
		}

		[UnityTest]
		public IEnumerator StringDefaultField()
		{
			yield return new ResetStaticTest<string>(null, "Hi!", x => TestClass.defaultString = x, () => TestClass.defaultString);
		}
		
		[UnityTest]
		public IEnumerator StringValueField()
		{
			yield return new ResetStaticTest<string>("Hello world!", "Hi!", x => TestClass.valueString = x, () => TestClass.valueString);
		}
		
		[UnityTest]
		public IEnumerator StringDefaultProperty()
		{
			yield return new ResetStaticTest<string>(null, "Hi!", x => TestClass.DefaultString = x, () => TestClass.DefaultString);
		}
		
		[UnityTest]
		public IEnumerator StringValueProperty()
		{
			yield return new ResetStaticTest<string>("Hello world!", "Hi!", x => TestClass.ValueString = x, () => TestClass.ValueString);
		}

		[UnityTest]
		public IEnumerator EnumDefaultField()
		{
			yield return new ResetStaticTest<TestEnum>(TestEnum.Value1, TestEnum.Value3, x => TestClass.defaultEnum = x, () => TestClass.defaultEnum);
		}
		
		[UnityTest]
		public IEnumerator EnumValueField()
		{
			yield return new ResetStaticTest<TestEnum>(TestEnum.Consistency, TestEnum.Value3, x => TestClass.valueEnum = x, () => TestClass.valueEnum);
		}
		
		[UnityTest]
		public IEnumerator EnumDefaultProperty()
		{
			yield return new ResetStaticTest<TestEnum>(TestEnum.Value1, TestEnum.Value3, x => TestClass.DefaultEnum = x, () => TestClass.DefaultEnum);
		}
		
		[UnityTest]
		public IEnumerator EnumValueProperty()
		{
			yield return new ResetStaticTest<TestEnum>(TestEnum.Consistency, TestEnum.Value3, x => TestClass.ValueEnum = x, () => TestClass.ValueEnum);
		}
		
		// [UnityTest]
		// public IEnumerator PrimitiveGenericField()
		// {
		// 	yield return new ResetStaticTest<int>(0, 100, x => GenericTestClass<int>.field = x, () => GenericTestClass<int>.field);
		// }
		//
		// [UnityTest]
		// public IEnumerator PrimitiveGenericProperty()
		// {
		// 	yield return new ResetStaticTest<int>(0, 100, x => GenericTestClass<int>.Property = x, () => GenericTestClass<int>.Property);
		// }

		private class ResetStaticTest<T> : IEditModeTestYieldInstruction
		{
			private readonly T originalValue;
			private readonly T newValue;
			private readonly Action<T> setNewValue;
			private readonly Func<T> getCurrentValue;
			
			public bool ExpectDomainReload { get { return false; } }
			public bool ExpectedPlaymodeState { get; private set; }

			public ResetStaticTest(T originalValue, T newValue, Action<T> setNewValue, Func<T> getCurrentValue)
			{
				this.originalValue = originalValue;
				this.newValue = newValue;
				this.setNewValue = setNewValue;
				this.getCurrentValue = getCurrentValue;
			}

			public IEnumerator Perform()
			{
				// Enter play mode
				Assert.IsTrue(Application.isPlaying);

				try
				{
					Assert.AreEqual(originalValue, getCurrentValue.Invoke(), "Value was not the default value.");
					setNewValue.Invoke(newValue);
				}
				catch (AssertionException e)
				{
					Debug.LogAssertion(e);
					EditorApplication.isPlaying = false;
					yield break;
				}

				// Exit play mode
				ExpectedPlaymodeState = false;
				EditorApplication.isPlaying = false;
				while (EditorApplication.isPlaying)
				{
					yield return null;
				}

				Assert.IsFalse(Application.isPlaying);

				try
				{
					Assert.AreEqual(newValue, getCurrentValue.Invoke(), "Value should be left to what it was.");
				}
				catch (AssertionException e)
				{
					Debug.LogAssertion(e);
					EditorApplication.isPlaying = false;
					yield break;
				}

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
					Assert.AreEqual(originalValue, getCurrentValue.Invoke(), "Value was not reset.");
				}
				catch (AssertionException e)
				{
					Debug.LogAssertion(e);
					EditorApplication.isPlaying = false;
					yield break;
				}
			}
		}
	}
}