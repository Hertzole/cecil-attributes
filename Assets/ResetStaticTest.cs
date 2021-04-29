using Hertzole.CecilAttributes;
using UnityEngine;

#pragma warning disable CS0414

public class ResetStaticTest : MonoBehaviour
{
    private bool changeTester = true;

    private bool changeTest2 = false;

    [ResetStatic]
    private static int defaultTest;

    [ResetStatic]
    private static int myCustomDefault = 69;

    [ResetStatic]
    private static byte byteTest;
    [ResetStatic]
    private static byte sbyteTest;
    [ResetStatic]
    private static short shortTest;
    [ResetStatic]
    private static ushort ushortTest;
    [ResetStatic]
    private static int intTest;
    [ResetStatic]
    private static uint uintTest;
    [ResetStatic]
    private static long longTest;
    [ResetStatic]
    private static ulong ulongTest;
    [ResetStatic]
    private static float floatTest;
    [ResetStatic]
    private static double doubleTest;
    [ResetStatic]
    private static decimal decimalTest;
    [ResetStatic]
    private static bool boolTest;
    [ResetStatic]
    private static string stringTest;
    [ResetStatic]
    private static Vector3 vector3Test;
    [ResetStatic]
    private static Vector3 Vector3Test { get; set; }
    [ResetStatic]
    private static Vector2 Vector2Test { get; set; }
    [ResetStatic]
    private static Vector4 Vector4Test { get; set; }
    [ResetStatic]
    private static Vector2Int Vector2IntTest { get; set; }
    [ResetStatic]
    private static Vector3Int Vector3IntTest { get; set; }
    [ResetStatic]
    private static Matrix4x4 MatrixTestNeo { get; set; }
    [ResetStatic]
    private static ResetStaticTest classTest;
    [ResetStatic]
    private static ResetStaticTest ClassTest { get; set; }

    public static Vector3 vec = new Vector3(10, 10, 10);

    public static Vector3Int v3 = Vector3Int.zero;

    private static int propField = 666;

    private static string Foor = $"Foorbar {propField}";

    public static bool test1 = false;
    public static bool test2 = true;
    public static bool test3 = false;

    public static int StaticProp { get; set; } = 5;
    public static event System.Action OnTestEvent;

    public static int PropField
    {
        get
        {
            return propField;
        }
        set { propField = value; }
    }

    private bool changeTemplate = false;
    public bool ChangeTemplate
    {
        get { return changeTemplate; }
        set
        {
            if (!changeTemplate.Equals(value))
            {
                bool oldValue = changeTemplate;
                changeTemplate = value;
                OnChangeTemplateChanged(oldValue, changeTemplate);
            }
        }
    }

    public static ResetStaticTest Instance { get; private set; }

    private static void DummyMethod()
    {
        floatTest = 0;
        doubleTest = 0;
        decimalTest = 0;
        vector3Test = new Vector3();
        Vector3Test = new Vector3();
        Vector2Test = new Vector2();
        Vector4Test = new Vector4();
        Vector2IntTest = new Vector2Int();
        Vector3IntTest = new Vector3Int();
        MatrixTestNeo = new Matrix4x4();
        StaticProp = 0;
    }

    private void OnChangeTemplateChanged(bool oldValue, bool newValue)
    {
        Debug.Log("On Change Template changed " + oldValue + " | " + newValue);
    }

    private void Awake()
    {
        Instance = this;

        Debug.Log(defaultTest);
    }

    // Start is called before the first frame update
    void Start()
    {
        changeTester = changeTest2;

        OnTestEvent?.Invoke();

        vec = new Vector3(10, 10, 10);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTesterChange()
    {
        Debug.Log("OnTester Changed");
    }

    private void OnTest2Change()
    {
        float x = transform.position.x;
        x += transform.position.y;

        Debug.Log(x);
    }

    private static int Test()
    {
        return 5;
    }
}
#pragma warning restore CS0414
