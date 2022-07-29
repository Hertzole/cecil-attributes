using System;
using UnityEngine;

namespace Hertzole.CecilAttributes.Tests
{
	public enum TestEnum
	{
		Value1 = 0,
		Value3 = 1,
		Consistency = 2
	}
	
	public class TestClass2 : TestClass1
	{
		[ResetStatic]
		public static int newIntValue;
	}

	public class TestClass1
	{
		[ResetStatic]
		public static int intValue = 42;
	}
	
	public class ResetStaticTestClass
	{
		[ResetStatic]
		public static int defaultPrimitive;
		[ResetStatic]
		public static int valuePrimitive = 42;
		[ResetStatic]
		public static Vector3Int defaultValueType;
		[ResetStatic]
		public static Vector3Int valueValueType = new Vector3Int(42, 69, 420);
		[ResetStatic]
		public static string defaultString;
		[ResetStatic]
		public static string valueString = "Hello world!";
		[ResetStatic]
		public static TestEnum defaultEnum;
		[ResetStatic]
		public static TestEnum valueEnum = TestEnum.Consistency;
		[ResetStatic]
		public static int DefaultPrimitive { get; set; }
		[ResetStatic]
		public static int ValuePrimitive { get; set; } = 42;
		[ResetStatic]
		public static Vector3Int DefaultValueType { get; set; }
		[ResetStatic]
		public static Vector3Int ValueValueType { get; set; } = new Vector3Int(42, 69, 420);
		[ResetStatic]
		public static string DefaultString { get; set; }
		[ResetStatic]
		public static string ValueString { get; set; } = "Hello world!";
		[ResetStatic]
		public static TestEnum DefaultEnum { get; set; }
		[ResetStatic]
		public static TestEnum ValueEnum { get; set; } = TestEnum.Consistency;

		[ResetStatic]
		public static event Action OnActionEvent;
		[ResetStatic]
		public static event Action<bool> OnActionParameterEvent;

		public static void InvokeEvents()
		{
			OnActionEvent?.Invoke();
			OnActionParameterEvent?.Invoke(true);
		}
	}
}