using System.Collections;
using System.Collections.Generic;
using Hertzole.CecilAttributes;
using UnityEngine;

public class Animators : MonoBehaviour
{
    [GetComponent(showInInspector = true)]
    public Animator[] anims;
    
    protected virtual bool Template(bool dirty)
    {
        return true;
    }
}
