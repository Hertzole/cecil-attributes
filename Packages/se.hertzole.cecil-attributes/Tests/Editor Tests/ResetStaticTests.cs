using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Hertzole.CecilAttributes.Tests
{
	public class ResetStaticTests
	{
		// private static class GenericResetStaticTestClass<T> where T: MonoBehaviour
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
			yield return new ResetStaticTest<int>(0, 100, x => ResetStaticTestClass.defaultPrimitive = x, () => ResetStaticTestClass.defaultPrimitive);
		}

		[UnityTest]
		public IEnumerator PrimitiveValueField()
		{
			yield return new ResetStaticTest<int>(42, 100, x => ResetStaticTestClass.valuePrimitive = x, () => ResetStaticTestClass.valuePrimitive);
		}

		[UnityTest]
		public IEnumerator PrimitiveDefaultProperty()
		{
			yield return new ResetStaticTest<int>(0, 100, x => ResetStaticTestClass.DefaultPrimitive = x, () => ResetStaticTestClass.DefaultPrimitive);
		}

		[UnityTest]
		public IEnumerator PrimitiveValueProperty()
		{
			yield return new ResetStaticTest<int>(42, 100, x => ResetStaticTestClass.ValuePrimitive = x, () => ResetStaticTestClass.ValuePrimitive);
		}

		[UnityTest]
		public IEnumerator ValueTypeDefaultField()
		{
			yield return new ResetStaticTest<Vector3Int>(Vector3Int.zero, new Vector3Int(1, 2, 3), x => ResetStaticTestClass.defaultValueType = x, () => ResetStaticTestClass.defaultValueType);
		}

		[UnityTest]
		public IEnumerator ValueTypeValueField()
		{
			yield return new ResetStaticTest<Vector3Int>(new Vector3Int(42, 69, 420), new Vector3Int(1, 2, 3), x => ResetStaticTestClass.valueValueType = x, () => ResetStaticTestClass.valueValueType);
		}

		[UnityTest]
		public IEnumerator ValueTypeDefaultProperty()
		{
			yield return new ResetStaticTest<Vector3Int>(Vector3Int.zero, new Vector3Int(1, 2, 3), x => ResetStaticTestClass.DefaultValueType = x, () => ResetStaticTestClass.DefaultValueType);
		}

		[UnityTest]
		public IEnumerator ValueTypeValueProperty()
		{
			yield return new ResetStaticTest<Vector3Int>(new Vector3Int(42, 69, 420), new Vector3Int(1, 2, 3), x => ResetStaticTestClass.ValueValueType = x, () => ResetStaticTestClass.ValueValueType);
		}

		[UnityTest]
		public IEnumerator StringDefaultField()
		{
			yield return new ResetStaticTest<string>(null, "Hi!", x => ResetStaticTestClass.defaultString = x, () => ResetStaticTestClass.defaultString);
		}

		[UnityTest]
		public IEnumerator StringValueField()
		{
			yield return new ResetStaticTest<string>("Hello world!", "Hi!", x => ResetStaticTestClass.valueString = x, () => ResetStaticTestClass.valueString);
		}

		[UnityTest]
		public IEnumerator StringDefaultProperty()
		{
			yield return new ResetStaticTest<string>(null, "Hi!", x => ResetStaticTestClass.DefaultString = x, () => ResetStaticTestClass.DefaultString);
		}

		[UnityTest]
		public IEnumerator StringValueProperty()
		{
			yield return new ResetStaticTest<string>("Hello world!", "Hi!", x => ResetStaticTestClass.ValueString = x, () => ResetStaticTestClass.ValueString);
		}

		[UnityTest]
		public IEnumerator EnumDefaultField()
		{
			yield return new ResetStaticTest<TestEnum>(TestEnum.Value1, TestEnum.Value3, x => ResetStaticTestClass.defaultEnum = x, () => ResetStaticTestClass.defaultEnum);
		}

		[UnityTest]
		public IEnumerator EnumValueField()
		{
			yield return new ResetStaticTest<TestEnum>(TestEnum.Consistency, TestEnum.Value3, x => ResetStaticTestClass.valueEnum = x, () => ResetStaticTestClass.valueEnum);
		}

		[UnityTest]
		public IEnumerator EnumDefaultProperty()
		{
			yield return new ResetStaticTest<TestEnum>(TestEnum.Value1, TestEnum.Value3, x => ResetStaticTestClass.DefaultEnum = x, () => ResetStaticTestClass.DefaultEnum);
		}

		[UnityTest]
		public IEnumerator EnumValueProperty()
		{
			yield return new ResetStaticTest<TestEnum>(TestEnum.Consistency, TestEnum.Value3, x => ResetStaticTestClass.ValueEnum = x, () => ResetStaticTestClass.ValueEnum);
		}

		[UnityTest]
		public IEnumerator ActionEvent()
		{
			yield return new ResetStaticEventTest(x => ResetStaticTestClass.OnActionEvent += x);
		}

		[UnityTest]
		public IEnumerator ActionParameterEvent()
		{
			yield return new ResetStaticEventTest<bool>(x => ResetStaticTestClass.OnActionParameterEvent += x);
		}

		[UnityTest]
		public IEnumerator Inheritance()
		{
			yield return new ResetStaticTest<int>(0, 100, x => TestClass2.newIntValue = x, () => TestClass2.newIntValue);
			yield return new ResetStaticTest<int>(42, 69, x => TestClass1.intValue = x, () => TestClass1.intValue);
		}

		// [UnityTest]
		// public IEnumerator PrimitiveGenericField()
		// {
		// 	yield return new ResetStaticTest<int>(0, 100, x => GenericResetStaticTestClass<int>.field = x, () => GenericResetStaticTestClass<int>.field);
		// }
		//
		// [UnityTest]
		// public IEnumerator PrimitiveGenericProperty()
		// {
		// 	yield return new ResetStaticTest<int>(0, 100, x => GenericResetStaticTestClass<int>.Property = x, () => GenericResetStaticTestClass<int>.Property);
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
				}
			}
		}

		private class ResetStaticEventTest : IEditModeTestYieldInstruction
		{
			private readonly Action<Action> subscribeToMethod;

			protected bool eventCalled;
			
			public bool ExpectDomainReload { get { return false; } }
			public bool ExpectedPlaymodeState { get; private set; }

			public ResetStaticEventTest(Action<Action> subscribeToMethod)
			{
				this.subscribeToMethod = subscribeToMethod;
			}
			
			public IEnumerator Perform()
			{
				try
				{
					Assert.IsTrue(Application.isPlaying, "Not playing.");
				}
				catch (AssertionException e)
				{
					Debug.LogAssertion(e);
					EditorApplication.isPlaying = false;
					yield break;
				}

				eventCalled = false;

				SubscribeEvent();
				ResetStaticTestClass.InvokeEvents();

				try
				{
					Assert.IsTrue(eventCalled, "Event was not called.");
				}
				catch (AssertionException e)
				{
					Debug.LogAssertion(e);
					EditorApplication.isPlaying = false;
					yield break;
				}

				eventCalled = false;
				
				// Exit play mode
				ExpectedPlaymodeState = false;
				EditorApplication.isPlaying = false;
				while (EditorApplication.isPlaying)
				{
					yield return null;
				}

				Assert.IsFalse(Application.isPlaying);

				// Enter play mode
				ExpectedPlaymodeState = true;
				EditorApplication.UnlockReloadAssemblies();
				EditorApplication.isPlaying = true;
				while (!EditorApplication.isPlaying)
				{
					yield return null;
				}

				try
				{
					Assert.IsTrue(Application.isPlaying);
					Assert.IsFalse(eventCalled, "Event was not reset.");
				}
				catch (AssertionException e)
				{
					Debug.LogAssertion(e);
					EditorApplication.isPlaying = false;
					yield break;
				}

				ResetStaticTestClass.InvokeEvents();
				
				try
				{
					Assert.IsFalse(eventCalled, "Event was not reset after event.");
				}
				catch (AssertionException e)
				{
					Debug.LogAssertion(e);
					EditorApplication.isPlaying = false;
				}
			}

			protected virtual void SubscribeEvent()
			{
				subscribeToMethod.Invoke(() => eventCalled = true);
			}
		}

		private class ResetStaticEventTest<T> : ResetStaticEventTest
		{
			private readonly Action<Action<T>> subscribeToMethodGeneric;
			
			public ResetStaticEventTest(Action<Action<T>> subscribeToMethod) : base(null)
			{
				subscribeToMethodGeneric = subscribeToMethod;
			}

			protected override void SubscribeEvent()
			{
				subscribeToMethodGeneric.Invoke(x => eventCalled = true);
			}
		}
	}
}