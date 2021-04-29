using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Hertzole.CecilAttributes.Tests
{
    public class LogCalledTests
    {
        private class TestClass
        {
            [LogCalled] public int TestProperty { get; set; }
            [LogCalled(logPropertyGet = true, logPropertySet = false)] public int TestPropertyGet { get; set; }
            [LogCalled(logPropertyGet = false, logPropertySet = true)] public int TestPropertySet { get; set; }
            public int TestPropertyGetDirect { [LogCalled] get; set; }
            public int TestPropertySetDirect { get; [LogCalled] set; }
            [LogCalled] public int TestPropertyGetOnly { get; }

            private int testField;

            [LogCalled] public int TestPropertyField { get { return testField; } set { testField = value; } }
            [LogCalled(logPropertyGet = true, logPropertySet = false)] public int TestPropertyFieldGet { get { return testField; } set { testField = value; } }
            [LogCalled(logPropertyGet = false, logPropertySet = true)] public int TestPropertyFieldSet { get { return testField; } set { testField = value; } }
            public int TestPropertyFieldGetDirect { [LogCalled] get { return testField; } set { testField = value; } }
            public int TestPropertyFieldSetDirect { get { return testField; } [LogCalled] set { testField = value; } }
            [LogCalled] public int TestPropertyFieldGetOnly { get { return testField; } }
            [LogCalled] public int TestPropertyFieldSetOnly { set { testField = value; } }

            [LogCalled] public static int StaticTestProperty { get; set; }
            [LogCalled(logPropertyGet = true, logPropertySet = false)] public static int StaticTestPropertyGet { get; set; }
            [LogCalled(logPropertyGet = false, logPropertySet = true)] public static int StaticTestPropertySet { get; set; }
            public static int StaticTestPropertyGetDirect { [LogCalled] get; set; }
            public static int StaticTestPropertySetDirect { get; [LogCalled] set; }
            [LogCalled] public static int StaticTestPropertyGetOnly { get; }

            private static int staticField;

            [LogCalled] public static int StaticTestPropertyField { get { return staticField; } set { staticField = value; } }
            [LogCalled(logPropertyGet = true, logPropertySet = false)] public static int StaticTestPropertyFieldGet { get { return staticField; } set { staticField = value; } }
            [LogCalled(logPropertyGet = false, logPropertySet = true)] public static int StaticTestPropertyFieldSet { get { return staticField; } set { staticField = value; } }
            public static int StaticTestPropertyFieldGetDirect { [LogCalled] get { return staticField; } set { staticField = value; } }
            public static int StaticTestPropertyFieldSetDirect { get { return staticField; } [LogCalled] set { staticField = value; } }
            [LogCalled] public static int StaticTestPropertyFieldGetOnly { get { return staticField; } }
            [LogCalled] public static int StaticTestPropertyFieldSetOnly { set { staticField = value; } }

            [LogCalled] public Vector3 TestValueTypeProperty { get; set; }
            [LogCalled(logPropertyGet = true, logPropertySet = false)] public Vector3 TestValueTypePropertyGet { get; set; }
            [LogCalled(logPropertyGet = false, logPropertySet = true)] public Vector3 TestValueTypePropertySet { get; set; }
            public Vector3 TestValueTypePropertyGetDirect { [LogCalled] get; set; }
            public Vector3 TestValueTypePropertySetDirect { get; [LogCalled] set; }
            [LogCalled] public Vector3 TestValueTypePropertyGetOnly { get; }

            private Vector3 valueTypeField;

            [LogCalled] public Vector3 TestValueTypePropertyField { get { return valueTypeField; } set { valueTypeField = value; } }
            [LogCalled(logPropertyGet = true, logPropertySet = false)] public Vector3 TestValueTypePropertyFieldGet { get { return valueTypeField; } set { valueTypeField = value; } }
            [LogCalled(logPropertyGet = false, logPropertySet = true)] public Vector3 TestValueTypePropertyFieldSet { get { return valueTypeField; } set { valueTypeField = value; } }
            public Vector3 TestValueTypePropertyFieldGetDirect { [LogCalled] get { return valueTypeField; } set { valueTypeField = value; } }
            public Vector3 TestValueTypePropertyFieldSetDirect { get { return valueTypeField; } [LogCalled] set { valueTypeField = value; } }
            [LogCalled] public Vector3 TestValueTypePropertyFieldGetOnly { get { return valueTypeField; } }
            [LogCalled] public Vector3 TestValueTypePropertyFieldSetOnly { set { valueTypeField = value; } }

            [LogCalled] public static Vector3 StaticTestValueTypeProperty { get; set; }
            [LogCalled(logPropertyGet = true, logPropertySet = false)] public static Vector3 StaticTestValueTypePropertyGet { get; set; }
            [LogCalled(logPropertyGet = false, logPropertySet = true)] public static Vector3 StaticTestValueTypePropertySet { get; set; }
            public static Vector3 StaticTestValueTypePropertyGetDirect { [LogCalled] get; set; }
            public static Vector3 StaticTestValueTypePropertySetDirect { get; [LogCalled] set; }
            [LogCalled] public static Vector3 StaticTestValueTypePropertyGetOnly { get; }

            private static Vector3 staticValueTypeField;

            [LogCalled] public static Vector3 StaticTestValueTypePropertyField { get { return staticValueTypeField; } set { staticValueTypeField = value; } }
            [LogCalled(logPropertyGet = true, logPropertySet = false)] public static Vector3 StaticTestValueTypePropertyFieldGet { get { return staticValueTypeField; } set { staticValueTypeField = value; } }
            [LogCalled(logPropertyGet = false, logPropertySet = true)] public static Vector3 StaticTestValueTypePropertyFieldSet { get { return staticValueTypeField; } set { staticValueTypeField = value; } }
            public static Vector3 StaticTestValueTypePropertyFieldGetDirect { [LogCalled] get { return staticValueTypeField; } set { staticValueTypeField = value; } }
            public static Vector3 StaticTestValueTypePropertyFieldSetDirect { get { return staticValueTypeField; } [LogCalled] set { staticValueTypeField = value; } }
            [LogCalled] public static Vector3 StaticTestValueTypePropertyFieldGetOnly { get { return staticValueTypeField; } }
            [LogCalled] public static Vector3 StaticTestValueTypePropertyFieldSetOnly { set { staticValueTypeField = value; } }

            [LogCalled] public string TestStringProperty { get; set; }
            [LogCalled(logPropertyGet = true, logPropertySet = false)] public string TestStringPropertyGet { get; set; }
            [LogCalled(logPropertyGet = false, logPropertySet = true)] public string TestStringPropertySet { get; set; }
            public string TestStringPropertyGetDirect { [LogCalled] get; set; }
            public string TestStringPropertySetDirect { get; [LogCalled] set; }
            [LogCalled] public string TestStringPropertyGetOnly { get; }

            private string stringField;

            [LogCalled] public string TestStringPropertyField { get { return stringField; } set { stringField = value; } }
            [LogCalled(logPropertyGet = true, logPropertySet = false)] public string TestStringPropertyFieldGet { get { return stringField; } set { stringField = value; } }
            [LogCalled(logPropertyGet = false, logPropertySet = true)] public string TestStringPropertyFieldSet { get { return stringField; } set { stringField = value; } }
            public string TestStringPropertyFieldGetDirect { [LogCalled] get { return stringField; } set { stringField = value; } }
            public string TestStringPropertyFieldSetDirect { get { return stringField; } [LogCalled] set { stringField = value; } }
            [LogCalled] public string TestStringPropertyFieldGetOnly { get { return stringField; } }
            [LogCalled] public string TestStringPropertyFieldSetOnly { set { stringField = value; } }

            [LogCalled] public static string StaticTestStringProperty { get; set; }
            [LogCalled(logPropertyGet = true, logPropertySet = false)] public static string StaticTestStringPropertyGet { get; set; }
            [LogCalled(logPropertyGet = false, logPropertySet = true)] public static string StaticTestStringPropertySet { get; set; }
            public static string StaticTestStringPropertyGetDirect { [LogCalled] get; set; }
            public static string StaticTestStringPropertySetDirect { get; [LogCalled] set; }
            [LogCalled] public static string StaticTestStringPropertyGetOnly { get; }

            private static string staticStringField;

            [LogCalled] public static string StaticTestStringPropertyField { get { return staticStringField; } set { staticStringField = value; } }
            [LogCalled(logPropertyGet = true, logPropertySet = false)] public static string StaticTestStringPropertyFieldGet { get { return staticStringField; } set { staticStringField = value; } }
            [LogCalled(logPropertyGet = false, logPropertySet = true)] public static string StaticTestStringPropertyFieldSet { get { return staticStringField; } set { staticStringField = value; } }
            public static string StaticTestStringPropertyFieldGetDirect { [LogCalled] get { return staticStringField; } set { staticStringField = value; } }
            public static string StaticTestStringPropertyFieldSetDirect { get { return staticStringField; } [LogCalled] set { staticStringField = value; } }
            [LogCalled] public static string StaticTestStringPropertyFieldGetOnly { get { return staticStringField; } }
            [LogCalled] public static string StaticTestStringPropertyFieldSetOnly { set { staticStringField = value; } }

            private static int templateField;

            public static int Template
            {
                get
                {
                    Debug.Log($"Get template field {templateField}");
                    return templateField;
                }
                set
                {
                    int oldValue = templateField;
                    Debug.Log($"Set template field. Old {oldValue} New {value}");

                    templateField = value;
                }
            }

            [LogCalled]
            public void NormalMethod() { }

            [LogCalled]
            public void MethodPara1(int para1) { }

            [LogCalled]
            public void MethodPara2(int para1, Vector3 para2) { }

            [LogCalled]
            public void MethodPara3(int para1, Vector3 para2, string para3) { }

            [LogCalled]
            public void MethodPara4(int para1, Vector3 para2, string para3, bool para4) { }

            [LogCalled]
            public void MethodOut(out int para)
            {
                para = 0;
            }
            
            [LogCalled]
            public void MethodOutPara1(out int para, int para1)
            {
                para = 0;
            }
        }
        
        private class TestGenericClass<T>
        {
            [LogCalled] public T TestProperty { get; set; }
            [LogCalled(logPropertyGet = true, logPropertySet = false)] public T TestPropertyGet { get; set; }
            [LogCalled(logPropertyGet = false, logPropertySet = true)] public T TestPropertySet { get; set; }
            public T TestPropertyGetDirect { [LogCalled] get; set; }
            public T TestPropertySetDirect { get; [LogCalled] set; }
            [LogCalled] public T TestPropertyGetOnly { get; }

            private T testField;

            [LogCalled] public T TestPropertyField { get { return testField; } set { testField = value; } }
            [LogCalled(logPropertyGet = true, logPropertySet = false)] public T TestPropertyFieldGet { get { return testField; } set { testField = value; } }
            [LogCalled(logPropertyGet = false, logPropertySet = true)] public T TestPropertyFieldSet { get { return testField; } set { testField = value; } }
            public T TestPropertyFieldGetDirect { [LogCalled] get { return testField; } set { testField = value; } }
            public T TestPropertyFieldSetDirect { get { return testField; } [LogCalled] set { testField = value; } }
            [LogCalled] public T TestPropertyFieldGetOnly { get { return testField; } }
            [LogCalled] public T TestPropertyFieldSetOnly { set { testField = value; } }

            [LogCalled] public static T StaticTestProperty { get; set; }
            [LogCalled(logPropertyGet = true, logPropertySet = false)] public static T StaticTestPropertyGet { get; set; }
            [LogCalled(logPropertyGet = false, logPropertySet = true)] public static T StaticTestPropertySet { get; set; }
            public static T StaticTestPropertyGetDirect { [LogCalled] get; set; }
            public static T StaticTestPropertySetDirect { get; [LogCalled] set; }
            [LogCalled] public static T StaticTestPropertyGetOnly { get; }

            private static T staticField;

            [LogCalled] public static T StaticTestPropertyField { get { return staticField; } set { staticField = value; } }
            [LogCalled(logPropertyGet = true, logPropertySet = false)] public static T StaticTestPropertyFieldGet { get { return staticField; } set { staticField = value; } }
            [LogCalled(logPropertyGet = false, logPropertySet = true)] public static T StaticTestPropertyFieldSet { get { return staticField; } set { staticField = value; } }
            public static T StaticTestPropertyFieldGetDirect { [LogCalled] get { return staticField; } set { staticField = value; } }
            public static T StaticTestPropertyFieldSetDirect { get { return staticField; } [LogCalled] set { staticField = value; } }
            [LogCalled] public static T StaticTestPropertyFieldGetOnly { get { return staticField; } }
            [LogCalled] public static T StaticTestPropertyFieldSetOnly { set { staticField = value; } }

            private T templateField;
            private static T staticTemplateField;

            public T Template
            {
                get
                {
                    Debug.Log($"Get template field {templateField}");
                    return templateField;
                }
                set
                {
                    T oldValue = templateField;
                    Debug.Log($"Set template field. Old {oldValue} New {value}");

                    templateField = value;
                }
            }
            
            public static T StaticTemplate
            {
                get
                {
                    Debug.Log($"Get template field {staticTemplateField}");
                    return staticTemplateField;
                }
                set
                {
                    T oldValue = staticTemplateField;
                    Debug.Log($"Set template field. Old {oldValue} New {value}");

                    staticTemplateField = value;
                }
            }

            public void TemplateMethod()
            {
                Debug.Log($"LogCalledTest<{typeof(T)}>");
            }

            public void TemplateMethod(T para1)
            {
                Debug.Log($"LogCalledTest<{typeof(T)}> (para1: {para1})");
            }
        }

        private TestClass test;
        private TestGenericClass<Vector3> testGeneric;

        [SetUp]
        public void SetUp()
        {
            test = new TestClass();
            testGeneric = new TestGenericClass<Vector3>();
        }

        #region Normal properties
        [Test]
        public void TestProperty()
        {
            int val = test.TestProperty;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestProperty)} Get {val}");
            test.TestProperty = 10;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestProperty)} Set (Old: {val}, New: {test.TestProperty})");
            test.TestProperty = default;
        }

        [Test]
        public void TestPropertyGet()
        {
            int val = test.TestPropertyGet;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestPropertyGet)} Get {val}");
        }

        [Test]
        public void TestPropertyGetOnly()
        {
            int val = test.TestPropertyGetOnly;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestPropertyGetOnly)} Get {val}");
        }

        [Test]
        public void TestPropertySet()
        {
            test.TestPropertySet = 10;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestPropertySet)} Set (Old: {default(int)}, New: {test.TestPropertySet})");
            test.TestPropertySet = default;
        }

        [Test]
        public void TestPropertyGetDirect()
        {
            int val = test.TestPropertyGetDirect;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} get_{nameof(test.TestPropertyGetDirect)} ()");
        }

        [Test]
        public void TestPropertySetDirect()
        {
            test.TestPropertySetDirect = 10;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} set_{nameof(test.TestPropertySetDirect)} (value: {test.TestPropertySetDirect})");
            test.TestPropertySetDirect = default;
        }

        [Test]
        public void TestPropertyField()
        {
            int val = test.TestPropertyField;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestPropertyField)} Get {val}");
            test.TestPropertyField = 10;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestPropertyField)} Set (Old: {val}, New: {test.TestPropertyField})");
            test.TestPropertyField = default;
        }

        [Test]
        public void TestPropertyFieldGet()
        {
            int val = test.TestPropertyFieldGet;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestPropertyFieldGet)} Get {val}");
        }

        [Test]
        public void TestPropertyFieldGetOnly()
        {
            int val = test.TestPropertyFieldGetOnly;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestPropertyFieldGetOnly)} Get {val}");
        }

        [Test]
        public void TestPropertyFieldSet()
        {
            test.TestPropertyFieldSet = 10;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestPropertyFieldSet)} Set (Old: {default(int)}, New: {test.TestPropertyFieldSet})");
            test.TestPropertyFieldSet = default;
        }

        [Test]
        public void TestPropertyFieldSetOnly()
        {
            test.TestPropertyFieldSetOnly = 10;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestPropertyFieldSetOnly)} Set (Old: {default(int)}, New: {10})");
            test.TestPropertyFieldSetOnly = default;
        }

        [Test]
        public void TestPropertyFieldGetDirect()
        {
            int val = test.TestPropertyFieldGetDirect;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} get_{nameof(test.TestPropertyFieldGetDirect)} ()");
        }

        [Test]
        public void TestPropertyFieldSetDirect()
        {
            test.TestPropertyFieldSetDirect = 10;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} set_{nameof(test.TestPropertyFieldSetDirect)} (value: {test.TestPropertyFieldSetDirect})");
            test.TestPropertyFieldSetDirect = default;
        }
        #endregion

        #region Static properties
        [Test]
        public void TestStaticProperty()
        {
            int val = TestClass.StaticTestProperty;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestProperty)} Get {val}");
            TestClass.StaticTestProperty = 10;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestProperty)} Set (Old: {val}, New: {TestClass.StaticTestProperty})");
            TestClass.StaticTestProperty = default;
        }

        [Test]
        public void TestStaticPropertyGet()
        {
            int val = TestClass.StaticTestPropertyGet;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestPropertyGet)} Get {val}");
        }

        [Test]
        public void TestStaticPropertyGetOnly()
        {
            int val = TestClass.StaticTestPropertyGetOnly;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestPropertyGetOnly)} Get {val}");
        }

        [Test]
        public void TestStaticPropertySet()
        {
            TestClass.StaticTestPropertySet = 10;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestPropertySet)} Set (Old: {default(int)}, New: {TestClass.StaticTestPropertySet})");
            TestClass.StaticTestPropertySet = default;
        }

        [Test]
        public void TestStaticPropertyGetDirect()
        {
            int val = TestClass.StaticTestPropertyGetDirect;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} get_{nameof(TestClass.StaticTestPropertyGetDirect)} ()");
        }

        [Test]
        public void TestStaticPropertySetDirect()
        {
            TestClass.StaticTestPropertySetDirect = 10;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} set_{nameof(TestClass.StaticTestPropertySetDirect)} (value: {TestClass.StaticTestPropertySetDirect})");
            TestClass.StaticTestPropertySetDirect = default;
        }

        [Test]
        public void TestStaticPropertyField()
        {
            int val = TestClass.StaticTestPropertyField;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestPropertyField)} Get {val}");
            TestClass.StaticTestPropertyField = 10;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestPropertyField)} Set (Old: {val}, New: {TestClass.StaticTestPropertyField})");
            TestClass.StaticTestPropertyField = default;
        }

        [Test]
        public void TestStaticPropertyFieldGet()
        {
            int val = TestClass.StaticTestPropertyFieldGet;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestPropertyFieldGet)} Get {val}");
        }

        [Test]
        public void TestStaticPropertyFieldGetOnly()
        {
            int val = TestClass.StaticTestPropertyFieldGetOnly;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestPropertyFieldGetOnly)} Get {val}");
        }

        [Test]
        public void TestStaticPropertyFieldSet()
        {
            TestClass.StaticTestPropertyFieldSet = 10;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestPropertyFieldSet)} Set (Old: {default(int)}, New: {TestClass.StaticTestPropertyFieldSet})");
            TestClass.StaticTestPropertyFieldSet = default;
        }

        [Test]
        public void TestStaticPropertyFieldSetOnly()
        {
            TestClass.StaticTestPropertyFieldSetOnly = 10;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestPropertyFieldSetOnly)} Set (Old: {default(int)}, New: {10})");
            TestClass.StaticTestPropertyFieldSetOnly = default;
        }

        [Test]
        public void TestStaticPropertyFieldGetDirect()
        {
            int val = TestClass.StaticTestPropertyFieldGetDirect;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} get_{nameof(TestClass.StaticTestPropertyFieldGetDirect)} ()");
        }

        [Test]
        public void TestStaticPropertyFieldSetDirect()
        {
            TestClass.StaticTestPropertyFieldSetDirect = 10;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} set_{nameof(TestClass.StaticTestPropertyFieldSetDirect)} (value: {TestClass.StaticTestPropertyFieldSetDirect})");
            TestClass.StaticTestPropertyFieldSetDirect = default;
        }
        #endregion

        #region Value type properties
        [Test]
        public void TestValueTypeProperty()
        {
            Vector3 val = test.TestValueTypeProperty;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestValueTypeProperty)} Get {val}");
            test.TestValueTypeProperty = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestValueTypeProperty)} Set (Old: {val}, New: {test.TestValueTypeProperty})");
            test.TestValueTypeProperty = default;
        }

        [Test]
        public void TestValueTypePropertyGet()
        {
            Vector3 val = test.TestValueTypePropertyGet;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestValueTypePropertyGet)} Get {val}");
        }

        [Test]
        public void TestValueTypePropertyGetOnly()
        {
            Vector3 val = test.TestValueTypePropertyGetOnly;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestValueTypePropertyGetOnly)} Get {val}");
        }

        [Test]
        public void TestValueTypePropertySet()
        {
            test.TestValueTypePropertySet = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestValueTypePropertySet)} Set (Old: {default(Vector3)}, New: {test.TestValueTypePropertySet})");
            test.TestValueTypePropertySet = default;
        }

        [Test]
        public void TestValueTypePropertyGetDirect()
        {
            Vector3 val = test.TestValueTypePropertyGetDirect;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} get_{nameof(test.TestValueTypePropertyGetDirect)} ()");
        }

        [Test]
        public void TestValueTypePropertySetDirect()
        {
            test.TestValueTypePropertySetDirect = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} set_{nameof(test.TestValueTypePropertySetDirect)} (value: {test.TestValueTypePropertySetDirect})");
            test.TestValueTypePropertySetDirect = default;
        }

        [Test]
        public void TestValueTypePropertyField()
        {
            Vector3 val = test.TestValueTypePropertyField;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestValueTypePropertyField)} Get {val}");
            test.TestValueTypePropertyField = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestValueTypePropertyField)} Set (Old: {val}, New: {test.TestValueTypePropertyField})");
            test.TestValueTypePropertyField = default;
        }

        [Test]
        public void TestValueTypePropertyFieldGet()
        {
            Vector3 val = test.TestValueTypePropertyFieldGet;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestValueTypePropertyFieldGet)} Get {val}");
        }

        [Test]
        public void TestValueTypePropertyFieldGetOnly()
        {
            Vector3 val = test.TestValueTypePropertyFieldGetOnly;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestValueTypePropertyFieldGetOnly)} Get {val}");
        }

        [Test]
        public void TestValueTypePropertyFieldSet()
        {
            test.TestValueTypePropertyFieldSet = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestValueTypePropertyFieldSet)} Set (Old: {default(Vector3)}, New: {test.TestValueTypePropertyFieldSet})");
            test.TestValueTypePropertyFieldSet = default;
        }

        [Test]
        public void TestValueTypePropertyFieldSetOnly()
        {
            test.TestValueTypePropertyFieldSetOnly = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestValueTypePropertyFieldSetOnly)} Set (Old: {default(Vector3)}, New: {new Vector3(1, 1, 1)})");
            test.TestValueTypePropertyFieldSetOnly = default;
        }

        [Test]
        public void TestValueTypePropertyFieldGetDirect()
        {
            Vector3 val = test.TestValueTypePropertyFieldGetDirect;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} get_{nameof(test.TestValueTypePropertyFieldGetDirect)} ()");
        }

        [Test]
        public void TestValueTypePropertyFieldSetDirect()
        {
            test.TestValueTypePropertyFieldSetDirect = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} set_{nameof(test.TestValueTypePropertyFieldSetDirect)} (value: {test.TestValueTypePropertyFieldSetDirect})");
            test.TestValueTypePropertyFieldSetDirect = default;
        }
        #endregion

        #region Static value type properties
        [Test]
        public void TestStaticValueTypeProperty()
        {
            Vector3 val = TestClass.StaticTestValueTypeProperty;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestValueTypeProperty)} Get {val}");
            TestClass.StaticTestValueTypeProperty = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestValueTypeProperty)} Set (Old: {val}, New: {TestClass.StaticTestValueTypeProperty})");
            TestClass.StaticTestValueTypeProperty = default;
        }

        [Test]
        public void TestStaticValueTypePropertyGet()
        {
            Vector3 val = TestClass.StaticTestValueTypePropertyGet;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestValueTypePropertyGet)} Get {val}");
        }

        [Test]
        public void TestStaticValueTypePropertyGetOnly()
        {
            Vector3 val = TestClass.StaticTestValueTypePropertyGetOnly;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestValueTypePropertyGetOnly)} Get {val}");
        }

        [Test]
        public void TestStaticValueTypePropertySet()
        {
            TestClass.StaticTestValueTypePropertySet = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestValueTypePropertySet)} Set (Old: {default(Vector3)}, New: {TestClass.StaticTestValueTypePropertySet})");
            TestClass.StaticTestValueTypePropertySet = default;
        }

        [Test]
        public void TestStaticValueTypePropertyGetDirect()
        {
            Vector3 val = TestClass.StaticTestValueTypePropertyGetDirect;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} get_{nameof(TestClass.StaticTestValueTypePropertyGetDirect)} ()");
        }

        [Test]
        public void TestStaticValueTypePropertySetDirect()
        {
            TestClass.StaticTestValueTypePropertySetDirect = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} set_{nameof(TestClass.StaticTestValueTypePropertySetDirect)} (value: {TestClass.StaticTestValueTypePropertySetDirect})");
            TestClass.StaticTestValueTypePropertySetDirect = default;
        }

        [Test]
        public void TestStaticValueTypePropertyField()
        {
            Vector3 val = TestClass.StaticTestValueTypePropertyField;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestValueTypePropertyField)} Get {val}");
            TestClass.StaticTestValueTypePropertyField = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestValueTypePropertyField)} Set (Old: {val}, New: {TestClass.StaticTestValueTypePropertyField})");
            TestClass.StaticTestValueTypePropertyField = default;
        }

        [Test]
        public void TestStaticValueTypePropertyFieldGet()
        {
            Vector3 val = TestClass.StaticTestValueTypePropertyFieldGet;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestValueTypePropertyFieldGet)} Get {val}");
        }

        [Test]
        public void TestStaticValueTypePropertyFieldGetOnly()
        {
            Vector3 val = TestClass.StaticTestValueTypePropertyFieldGetOnly;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestValueTypePropertyFieldGetOnly)} Get {val}");
        }

        [Test]
        public void TestStaticValueTypePropertyFieldSet()
        {
            TestClass.StaticTestValueTypePropertyFieldSet = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestValueTypePropertyFieldSet)} Set (Old: {default(Vector3)}, New: {TestClass.StaticTestValueTypePropertyFieldSet})");
            TestClass.StaticTestValueTypePropertyFieldSet = default;
        }

        [Test]
        public void TestStaticValueTypePropertyFieldSetOnly()
        {
            TestClass.StaticTestValueTypePropertyFieldSetOnly = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestValueTypePropertyFieldSetOnly)} Set (Old: {default(Vector3)}, New: {new Vector3(1, 1, 1)})");
            TestClass.StaticTestValueTypePropertyFieldSetOnly = default;
        }

        [Test]
        public void TestStaticValueTypePropertyFieldGetDirect()
        {
            Vector3 val = TestClass.StaticTestValueTypePropertyFieldGetDirect;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} get_{nameof(TestClass.StaticTestValueTypePropertyFieldGetDirect)} ()");
        }

        [Test]
        public void TestStaticValueTypePropertyFieldSetDirect()
        {
            TestClass.StaticTestValueTypePropertyFieldSetDirect = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} set_{nameof(TestClass.StaticTestValueTypePropertyFieldSetDirect)} (value: {TestClass.StaticTestValueTypePropertyFieldSetDirect})");
            TestClass.StaticTestValueTypePropertyFieldSetDirect = default;
        }
        #endregion

        #region Value type properties
        [Test]
        public void TestStringProperty()
        {
            string val = test.TestStringProperty;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestStringProperty)} Get {val}");
            test.TestStringProperty = "Hello world";
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestStringProperty)} Set (Old: {val}, New: {test.TestStringProperty})");
            test.TestStringProperty = default;
        }

        [Test]
        public void TestStringPropertyGet()
        {
            string val = test.TestStringPropertyGet;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestStringPropertyGet)} Get {val}");
        }

        [Test]
        public void TestStringPropertyGetOnly()
        {
            string val = test.TestStringPropertyGetOnly;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestStringPropertyGetOnly)} Get {val}");
        }

        [Test]
        public void TestStringPropertySet()
        {
            test.TestStringPropertySet = "Hello world";
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestStringPropertySet)} Set (Old: {default(string)}, New: {test.TestStringPropertySet})");
            test.TestStringPropertySet = default;
        }

        [Test]
        public void TestStringPropertyGetDirect()
        {
            string val = test.TestStringPropertyGetDirect;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} get_{nameof(test.TestStringPropertyGetDirect)} ()");
        }

        [Test]
        public void TestStringPropertySetDirect()
        {
            test.TestStringPropertySetDirect = "Hello world";
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} set_{nameof(test.TestStringPropertySetDirect)} (value: {test.TestStringPropertySetDirect})");
            test.TestStringPropertySetDirect = default;
        }

        [Test]
        public void TestStringPropertyField()
        {
            string val = test.TestStringPropertyField;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestStringPropertyField)} Get {val}");
            test.TestStringPropertyField = "Hello world";
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestStringPropertyField)} Set (Old: {val}, New: {test.TestStringPropertyField})");
            test.TestStringPropertyField = default;
        }

        [Test]
        public void TestStringPropertyFieldGet()
        {
            string val = test.TestStringPropertyFieldGet;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestStringPropertyFieldGet)} Get {val}");
        }

        [Test]
        public void TestStringPropertyFieldGetOnly()
        {
            string val = test.TestStringPropertyFieldGetOnly;
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestStringPropertyFieldGetOnly)} Get {val}");
        }

        [Test]
        public void TestStringPropertyFieldSet()
        {
            test.TestStringPropertyFieldSet = "Hello world";
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestStringPropertyFieldSet)} Set (Old: {default(string)}, New: {test.TestStringPropertyFieldSet})");
            test.TestStringPropertyFieldSet = default;
        }

        [Test]
        public void TestStringPropertyFieldSetOnly()
        {
            test.TestStringPropertyFieldSetOnly = "Hello world";
            LogAssert.Expect(LogType.Log, $"{nameof(test.TestStringPropertyFieldSetOnly)} Set (Old: {default(string)}, New: {"Hello world"})");
            test.TestStringPropertyFieldSetOnly = default;
        }

        [Test]
        public void TestStringPropertyFieldGetDirect()
        {
            string val = test.TestStringPropertyFieldGetDirect;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} get_{nameof(test.TestStringPropertyFieldGetDirect)} ()");
        }

        [Test]
        public void TestStringPropertyFieldSetDirect()
        {
            test.TestStringPropertyFieldSetDirect = "Hello world";
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} set_{nameof(test.TestStringPropertyFieldSetDirect)} (value: {test.TestStringPropertyFieldSetDirect})");
            test.TestStringPropertyFieldSetDirect = default;
        }
        #endregion

        #region Static value type properties
        [Test]
        public void TestStaticStringProperty()
        {
            string val = TestClass.StaticTestStringProperty;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestStringProperty)} Get {val}");
            TestClass.StaticTestStringProperty = "Hello world";
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestStringProperty)} Set (Old: {val}, New: {TestClass.StaticTestStringProperty})");
            TestClass.StaticTestStringProperty = default;
        }

        [Test]
        public void TestStaticStringPropertyGet()
        {
            string val = TestClass.StaticTestStringPropertyGet;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestStringPropertyGet)} Get {val}");
        }

        [Test]
        public void TestStaticStringPropertyGetOnly()
        {
            string val = TestClass.StaticTestStringPropertyGetOnly;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestStringPropertyGetOnly)} Get {val}");
        }

        [Test]
        public void TestStaticStringPropertySet()
        {
            TestClass.StaticTestStringPropertySet = "Hello world";
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestStringPropertySet)} Set (Old: {default(string)}, New: {TestClass.StaticTestStringPropertySet})");
            TestClass.StaticTestStringPropertySet = default;
        }

        [Test]
        public void TestStaticStringPropertyGetDirect()
        {
            string val = TestClass.StaticTestStringPropertyGetDirect;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} get_{nameof(TestClass.StaticTestStringPropertyGetDirect)} ()");
        }

        [Test]
        public void TestStaticStringPropertySetDirect()
        {
            TestClass.StaticTestStringPropertySetDirect = "Hello world";
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} set_{nameof(TestClass.StaticTestStringPropertySetDirect)} (value: {TestClass.StaticTestStringPropertySetDirect})");
            TestClass.StaticTestStringPropertySetDirect = default;
        }

        [Test]
        public void TestStaticStringPropertyField()
        {
            string val = TestClass.StaticTestStringPropertyField;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestStringPropertyField)} Get {val}");
            TestClass.StaticTestStringPropertyField = "Hello world";
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestStringPropertyField)} Set (Old: {val}, New: {TestClass.StaticTestStringPropertyField})");
            TestClass.StaticTestStringPropertyField = default;
        }

        [Test]
        public void TestStaticStringPropertyFieldGet()
        {
            string val = TestClass.StaticTestStringPropertyFieldGet;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestStringPropertyFieldGet)} Get {val}");
        }

        [Test]
        public void TestStaticStringPropertyFieldGetOnly()
        {
            string val = TestClass.StaticTestStringPropertyFieldGetOnly;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestStringPropertyFieldGetOnly)} Get {val}");
        }

        [Test]
        public void TestStaticStringPropertyFieldSet()
        {
            TestClass.StaticTestStringPropertyFieldSet = "Hello world";
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestStringPropertyFieldSet)} Set (Old: {default(string)}, New: {TestClass.StaticTestStringPropertyFieldSet})");
            TestClass.StaticTestStringPropertyFieldSet = default;
        }

        [Test]
        public void TestStaticStringPropertyFieldSetOnly()
        {
            TestClass.StaticTestStringPropertyFieldSetOnly = "Hello world";
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass.StaticTestStringPropertyFieldSetOnly)} Set (Old: {default(string)}, New: {"Hello world"})");
            TestClass.StaticTestStringPropertyFieldSetOnly = default;
        }

        [Test]
        public void TestStaticStringPropertyFieldGetDirect()
        {
            string val = TestClass.StaticTestStringPropertyFieldGetDirect;
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} get_{nameof(TestClass.StaticTestStringPropertyFieldGetDirect)} ()");
        }

        [Test]
        public void TestStaticStringPropertyFieldSetDirect()
        {
            TestClass.StaticTestStringPropertyFieldSetDirect = "Hello world";
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} set_{nameof(TestClass.StaticTestStringPropertyFieldSetDirect)} (value: {TestClass.StaticTestStringPropertyFieldSetDirect})");
            TestClass.StaticTestStringPropertyFieldSetDirect = default;
        }
        #endregion

        #region Normal generic properties
        [Test]
        public void TestGenericProperty()
        {
            Vector3 val = testGeneric.TestProperty;
            LogAssert.Expect(LogType.Log, $"{nameof(testGeneric.TestProperty)} Get {val}");
            testGeneric.TestProperty = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(testGeneric.TestProperty)} Set (Old: {val}, New: {testGeneric.TestProperty})");
            testGeneric.TestProperty = default;
        }

        [Test]
        public void TestGenericPropertyGet()
        {
            Vector3 val = testGeneric.TestPropertyGet;
            LogAssert.Expect(LogType.Log, $"{nameof(testGeneric.TestPropertyGet)} Get {val}");
        }

        [Test]
        public void TestGenericPropertyGetOnly()
        {
            Vector3 val = testGeneric.TestPropertyGetOnly;
            LogAssert.Expect(LogType.Log, $"{nameof(testGeneric.TestPropertyGetOnly)} Get {val}");
        }

        [Test]
        public void TestGenericPropertySet()
        {
            testGeneric.TestPropertySet = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(testGeneric.TestPropertySet)} Set (Old: {default(Vector3)}, New: {testGeneric.TestPropertySet})");
            testGeneric.TestPropertySet = default;
        }

        [Test]
        public void TestGenericPropertyGetDirect()
        {
            Vector3 val = testGeneric.TestPropertyGetDirect;
            LogAssert.Expect(LogType.Log, $"{nameof(TestGenericClass<Vector3>)}<UnityEngine.{nameof(Vector3)}> get_{nameof(testGeneric.TestPropertyGetDirect)} ()");
        }

        [Test]
        public void TestGenericPropertySetDirect()
        {
            testGeneric.TestPropertySetDirect = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(TestGenericClass<Vector3>)}<UnityEngine.{nameof(Vector3)}> set_{nameof(testGeneric.TestPropertySetDirect)} (value: {testGeneric.TestPropertySetDirect})");
            testGeneric.TestPropertySetDirect = default;
        }

        [Test]
        public void TestGenericPropertyField()
        {
            Vector3 val = testGeneric.TestPropertyField;
            LogAssert.Expect(LogType.Log, $"{nameof(testGeneric.TestPropertyField)} Get {val}");
            testGeneric.TestPropertyField = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(testGeneric.TestPropertyField)} Set (Old: {val}, New: {testGeneric.TestPropertyField})");
            testGeneric.TestPropertyField = default;
        }

        [Test]
        public void TestGenericPropertyFieldGet()
        {
            Vector3 val = testGeneric.TestPropertyFieldGet;
            LogAssert.Expect(LogType.Log, $"{nameof(testGeneric.TestPropertyFieldGet)} Get {val}");
        }

        [Test]
        public void TestGenericPropertyFieldGetOnly()
        {
            Vector3 val = testGeneric.TestPropertyFieldGetOnly;
            LogAssert.Expect(LogType.Log, $"{nameof(testGeneric.TestPropertyFieldGetOnly)} Get {val}");
        }

        [Test]
        public void TestGenericPropertyFieldSet()
        {
            testGeneric.TestPropertyFieldSet = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(testGeneric.TestPropertyFieldSet)} Set (Old: {default(Vector3)}, New: {testGeneric.TestPropertyFieldSet})");
            testGeneric.TestPropertyFieldSet = default;
        }

        [Test]
        public void TestGenericPropertyFieldSetOnly()
        {
            testGeneric.TestPropertyFieldSetOnly = new Vector3(1, 1 ,1);
            LogAssert.Expect(LogType.Log, $"{nameof(testGeneric.TestPropertyFieldSetOnly)} Set (Old: {default(Vector3)}, New: {new Vector3(1, 1, 1)})");
            testGeneric.TestPropertyFieldSetOnly = default;
        }

        [Test]
        public void TestGenericPropertyFieldGetDirect()
        {
            Vector3 val = testGeneric.TestPropertyFieldGetDirect;
            LogAssert.Expect(LogType.Log, $"{nameof(TestGenericClass<Vector3>)}<UnityEngine.{nameof(Vector3)}> get_{nameof(testGeneric.TestPropertyFieldGetDirect)} ()");
        }

        [Test]
        public void TestGenericPropertyFieldSetDirect()
        {
            testGeneric.TestPropertyFieldSetDirect = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(TestGenericClass<Vector3>)}<UnityEngine.{nameof(Vector3)}> set_{nameof(testGeneric.TestPropertyFieldSetDirect)} (value: {testGeneric.TestPropertyFieldSetDirect})");
            testGeneric.TestPropertyFieldSetDirect = default;
        }
        #endregion

        #region Static generic properties
        [Test]
        public void TestStaticGenericProperty()
        {
            Vector3 val = TestGenericClass<Vector3>.StaticTestProperty;
            LogAssert.Expect(LogType.Log, $"{nameof(TestGenericClass<Vector3>.StaticTestProperty)} Get {val}");
            TestGenericClass<Vector3>.StaticTestProperty = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(TestGenericClass<Vector3>.StaticTestProperty)} Set (Old: {val}, New: {TestGenericClass<Vector3>.StaticTestProperty})");
            TestGenericClass<Vector3>.StaticTestProperty = default;
        }

        [Test]
        public void TestStaticGenericPropertyGet()
        {
            Vector3 val = TestGenericClass<Vector3>.StaticTestPropertyGet;
            LogAssert.Expect(LogType.Log, $"{nameof(TestGenericClass<Vector3>.StaticTestPropertyGet)} Get {val}");
        }

        [Test]
        public void TestStaticGenericPropertyGetOnly()
        {
            Vector3 val = TestGenericClass<Vector3>.StaticTestPropertyGetOnly;
            LogAssert.Expect(LogType.Log, $"{nameof(TestGenericClass<Vector3>.StaticTestPropertyGetOnly)} Get {val}");
        }

        [Test]
        public void TestStaticGenericPropertySet()
        {
            TestGenericClass<Vector3>.StaticTestPropertySet = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(TestGenericClass<Vector3>.StaticTestPropertySet)} Set (Old: {default(Vector3)}, New: {TestGenericClass<Vector3>.StaticTestPropertySet})");
            TestGenericClass<Vector3>.StaticTestPropertySet = default;
        }

        [Test]
        public void TestStaticGenericPropertyGetDirect()
        {
            Vector3 val = TestGenericClass<Vector3>.StaticTestPropertyGetDirect;
            LogAssert.Expect(LogType.Log, $"{nameof(TestGenericClass<Vector3>)}<UnityEngine.{nameof(Vector3)}> get_{nameof(TestGenericClass<Vector3>.StaticTestPropertyGetDirect)} ()");
        }

        [Test]
        public void TestStaticGenericPropertySetDirect()
        {
            TestGenericClass<Vector3>.StaticTestPropertySetDirect = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(TestGenericClass<Vector3>)}<UnityEngine.{nameof(Vector3)}> set_{nameof(TestGenericClass<Vector3>.StaticTestPropertySetDirect)} (value: {TestGenericClass<Vector3>.StaticTestPropertySetDirect})");
            TestGenericClass<Vector3>.StaticTestPropertySetDirect = default;
        }

        [Test]
        public void TestStaticGenericPropertyField()
        {
            Vector3 val = TestGenericClass<Vector3>.StaticTestPropertyField;
            LogAssert.Expect(LogType.Log, $"{nameof(TestGenericClass<Vector3>.StaticTestPropertyField)} Get {val}");
            TestGenericClass<Vector3>.StaticTestPropertyField = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(TestGenericClass<Vector3>.StaticTestPropertyField)} Set (Old: {val}, New: {TestGenericClass<Vector3>.StaticTestPropertyField})");
            TestGenericClass<Vector3>.StaticTestPropertyField = default;
        }

        [Test]
        public void TestStaticGenericPropertyFieldGet()
        {
            Vector3 val = TestGenericClass<Vector3>.StaticTestPropertyFieldGet;
            LogAssert.Expect(LogType.Log, $"{nameof(TestGenericClass<Vector3>.StaticTestPropertyFieldGet)} Get {val}");
        }

        [Test]
        public void TestStaticGenericPropertyFieldGetOnly()
        {
            Vector3 val = TestGenericClass<Vector3>.StaticTestPropertyFieldGetOnly;
            LogAssert.Expect(LogType.Log, $"{nameof(TestGenericClass<Vector3>.StaticTestPropertyFieldGetOnly)} Get {val}");
        }

        [Test]
        public void TestStaticGenericPropertyFieldSet()
        {
            TestGenericClass<Vector3>.StaticTestPropertyFieldSet = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(TestGenericClass<Vector3>.StaticTestPropertyFieldSet)} Set (Old: {default(Vector3)}, New: {TestGenericClass<Vector3>.StaticTestPropertyFieldSet})");
            TestGenericClass<Vector3>.StaticTestPropertyFieldSet = default;
        }

        [Test]
        public void TestStaticGenericPropertyFieldSetOnly()
        {
            TestGenericClass<Vector3>.StaticTestPropertyFieldSetOnly = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(TestGenericClass<Vector3>.StaticTestPropertyFieldSetOnly)} Set (Old: {default(Vector3)}, New: {new Vector3(1, 1, 1)})");
            TestGenericClass<Vector3>.StaticTestPropertyFieldSetOnly = default;
        }

        [Test]
        public void TestStaticGenericPropertyFieldGetDirect()
        {
            Vector3 val = TestGenericClass<Vector3>.StaticTestPropertyFieldGetDirect;
            LogAssert.Expect(LogType.Log, $"{nameof(TestGenericClass<Vector3>)}<UnityEngine.{nameof(Vector3)}> get_{nameof(TestGenericClass<Vector3>.StaticTestPropertyFieldGetDirect)} ()");
        }

        [Test]
        public void TestStaticGenericPropertyFieldSetDirect()
        {
            TestGenericClass<Vector3>.StaticTestPropertyFieldSetDirect = new Vector3(1, 1, 1);
            LogAssert.Expect(LogType.Log, $"{nameof(TestGenericClass<Vector3>)}<UnityEngine.{nameof(Vector3)}> set_{nameof(TestGenericClass<Vector3>.StaticTestPropertyFieldSetDirect)} (value: {TestGenericClass<Vector3>.StaticTestPropertyFieldSetDirect})");
            TestGenericClass<Vector3>.StaticTestPropertyFieldSetDirect = default;
        }
        #endregion

        #region Normal methods
        [Test]
        public void TestNormalMethod()
        {
            test.NormalMethod();
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} {nameof(test.NormalMethod)} ()");
        }
        
        [Test]
        public void TestMethodWith1Parameter()
        {
            test.MethodPara1(10);
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} {nameof(test.MethodPara1)} (para1: 10)");
        }
        
        [Test]
        public void TestMethodWith2Parameter()
        {
            test.MethodPara2(10, Vector3.one);
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} {nameof(test.MethodPara2)} (para1: 10, para2: {Vector3.one})");
        }
        
        [Test]
        public void TestMethodWith3Parameter()
        {
            test.MethodPara3(10, Vector3.one, "Hello world");
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} {nameof(test.MethodPara3)} (para1: 10, para2: {Vector3.one}, para3: Hello world)");
        }
        
        [Test]
        public void TestMethodWith4Parameter()
        {
            test.MethodPara4(10, Vector3.one, "Hello world", false);
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} {nameof(test.MethodPara4)} (para1: 10, para2: {Vector3.one}, para3: Hello world, para4: False)");
        }
        
        [Test]
        public void TestMethodWithOut()
        {
            test.MethodOut(out _);
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} {nameof(test.MethodOut)} (out para)");
        }
        
        [Test]
        public void TestMethodWithOutAnd1Parameter()
        {
            test.MethodOutPara1(out _, 10);
            LogAssert.Expect(LogType.Log, $"{nameof(TestClass)} {nameof(test.MethodOutPara1)} (out para, para1: 10)");
        }
        #endregion
    }

}
