using Hertzole.CecilAttributes;
using System.Collections.Generic;
using UnityEngine;

public class LogTester : MonoBehaviour
{
    private enum TestEnum { Value1, Value3 };

    private string testField;

    [LogCalled]
    public int PropertyTest { get; set; }

    private int temp = 5;

    [LogCalled]
    public int PropertyTest2 { get { return temp; } set { temp = value; } }

    [LogCalled(logPropertyGet: true, logPropertySet: false)]
    public int OnlyGetMessage { get; set; } = 10;

    [LogCalled(logPropertyGet: false, logPropertySet: true)]
    public int OnlySetMessage { get; set; } = 5;

    [LogCalled]
    public string PropertyString { get; set; } = "Hello";

    public int TemplateProperty
    {
        get
        {
            Debug.Log($"TemplateProperty Get (temp: {temp})");
            return temp;
        }
        set
        {
            int old = temp;
            Debug.Log($"TemplateProperty Set (Old: {old}, New: {value})");
            temp = value;
        }
    }

    int x;
    string y = "Y is for why";
    Vector3 z;

    // Start is called before the first frame update
    [LogCalled]
    void Start()
    {
        int x = OnlyGetMessage;
        OnlySetMessage = 5;

        PropertyTest = 10;
        PropertyTest2 = PropertyTest2 + 15;
        PropertyString = "World";

        Test1("This is test", TestEnum.Value1);
        Test2(69, 6.66f, "lel");
        ExtremeTest(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 10000);
        OutTest(10, out x, 20);
        InTest(42, "hello world", Vector3.up, 69, 666, this, TestEnum.Value3);
        RefTest(ref x, ref y, ref z);
    }

    [LogCalled]
    private void Test1(string param1, TestEnum e)
    {
    }

    [LogCalled]
    private static void StaticTest(Animator temp)
    {
        temps.Add(temp);
    }

    private static void StaticTestTemplate(Animator temp, Animator test2, Animator test3, Animator test4)
    {
        Debug.Log($"StaticTestTemplate {temp} {test2} {test3} {test4}");
        temps.Add(temp);
    }

    private static List<Animator> temps = new List<Animator>();

    private void Test1Template(string param1, int x)
    {
        Debug.Log($"Test1Template {param1}, {x}");
    }

    [LogCalled]
    private void Test2(int p1, float p3, string lol)
    {
    }

    private void Test2Template(int p1, float p3, string lol, LogTester teser)
    {
        Debug.Log($"Test2Template ({p1}, {lol}, {p3}, {teser})");
    }

    [LogCalled]
    private void ExtremeTest(int p1, int p2, int p3, int p4, int p5, int p6, int p7, int p8, int p9, int p10, int p11, int p12, int p13, int p14, int p15)
    {
    }

    [LogCalled]
    private void OutTest(int y, out int x, int z)
    {
        x = 10;
    }

    [LogCalled]
    private void InTest(in int x, in string temp, in Vector3 lul, in int y, in int z, in LogTester tes, in TestEnum e)
    {
    }

    [LogCalled]
    private void RefTest(ref int x, ref string y, ref Vector3 z)
    {

    }

    private void RefTestTemplate(ref int x, ref string y, ref Vector3 z)
    {
        Debug.Log($"RefTestTemplate {x}, {y}, {z}");
    }

    private void InTemplate(in int x, in string temp, in Vector3 lul, in int y, in int z, in LogTester tes, in TestEnum e)
    {
        Debug.Log($"InTemplate {x} {temp} {lul} {y} {z} {tes} {e}");
    }

    private void OutTemplate(out int x)
    {
        //Debug.Log(string.Format("OutTemplate {0}", x));
        x = 10;
        Debug.Log($"OutTemplate {x}");
    }

    private void FormatTest()
    {
        int x = 15;

        Debug.Log($"{x}");
        Debug.Log($"{x} {x}");
        Debug.Log($"{x} {x} {x}");
        Debug.Log($"{x} {x} {x} {x}");
        Debug.Log($"{x} {x} {x} {x} {x}");
        Debug.Log($"{x} {x} {x} {x} {x} {x}");
    }
}
