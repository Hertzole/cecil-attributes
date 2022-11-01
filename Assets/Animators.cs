using System.Collections;
using System.Collections.Generic;
using Hertzole.CecilAttributes;
using UnityEngine;
using UnityEngine.Profiling;

public class Animators : MonoBehaviour
{
    [SerializeField] 
    [GetComponent]
    private Renderer ren = default;
    [GetComponent(enableInEditor = false)]
    public Animator[] anims;
    
    protected virtual bool Template(bool dirty)
    {
        return true;
    }

    [Timed]
    public void ProfilerTest1()
    {
        Debug.Log("Hello world");
    }
    
    [Timed]
    public int ProfilerTest2()
    {
        int value = 1;
        for (int i = 0; i < 100; i++)
        {
            value += i;
        }
        return value;
    }

    public void ProfilerTemplate()
    {
        Profiler.BeginSample("Template", this);
        Template(true);
        Profiler.EndSample();
    }

    public class Nested
    {
        [Timed]
        public int ProfilerTest2()
        {
            int value = 1;
            for (int i = 0; i < 100; i++)
            {
                value += i;
            }
            return value;
        }
    }
}
