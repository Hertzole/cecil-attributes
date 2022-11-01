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
    [GetComponent(target = GetComponentTarget.Children, enableInEditor = false)]
    public Animator[] anims;
    [SerializeField] 
    [GetComponent]
    private BoxCollider col = default;
    
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

    protected virtual void FetchComponentsTemplate()
    {
        
        
        if (ren == null)
        {
            ren = GetComponent<Renderer>();
        }
        
        anims = GetComponentsInChildren<Animator>(true);
        
        if (ren == null)
        {
            ren = GetComponentInParent<Renderer>();
        }
    }
}
