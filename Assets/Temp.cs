using Hertzole.CecilAttributes;
using System;
using UnityEngine;

public class Temp : MonoBehaviour
{
#pragma warning disable CS0414
    [Serializable]
    public struct TestClass
    {
        public int lol;
        public string test;
    }

    [SerializeField]
    private string test = null;
    [SerializeField]
    private TestClass lel = new TestClass();
    [SerializeField]
    private TestClass cantRememberMe = new TestClass();
    [SerializeField]
    private int customName = 0;

    [SerializeField]
    private TestClass[] testList = null;

    [OnChange(hook = nameof(OnChangedTestChanged))]
    private int changedTest;
    [OnChange(hook = nameof(OnChangedTestChanged), equalCheck = false)]
    private int noEquals;
    [OnChange(hook = nameof(OnChangedTestPropertyChanged))]
    private int ChangedTest { get; set; }

    private int changeTemplate = 10;

    private int ChangeTemplate
    {
        get { return changeTemplate; }
        set
        {
            if (changeTemplate != value)
            {
                int oldValue = changeTemplate;
                changeTemplate = value;
                OnChangedTestChanged(oldValue, value);
            }
        }
    }

    public int ChangeTemplate2
    {
        set
        {
            int oldValue = changeTemplate;
            changeTemplate = value;
            OnChangedTestChanged(oldValue, value);
        }
    }

    private void Start()
    {
        Debug.Log("This is a message with " + test + " " + lel.lol + " " + cantRememberMe.lol + " " + customName + " " + (customName + 2).ToString());
        Debug.Log($"This is a message with {test} {lel.lol} {cantRememberMe.lol} {customName} {customName + 2}");

        changedTest = 10;
        noEquals = 30;
        ChangedTest = 40;
    }
#pragma warning restore CS0414

    private void OnChangedTestChanged(int oldVal, int newVal)
    {
        Debug.Log("OnChangedTest changed from " + oldVal + " to " + newVal);
    }

    private void OnChangedTestPropertyChanged(int oldVal, int newVal)
    {
        Debug.Log("OnChangedTestProperty changed from " + oldVal + " to " + newVal);
    }
}
