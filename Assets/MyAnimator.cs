using System.Collections;
using System.Collections.Generic;
using Hertzole.CecilAttributes;
using UnityEngine;

public class MyAnimator : MonoBehaviour
{
    [GetComponent(showInInspector = true)]
    public Animator anim;

    bool ITemplate()
    {
        return Template(false);
    }
    
    protected virtual bool Template(bool dirty)
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
            dirty = true;
        }
        
        return dirty;
    }
}
