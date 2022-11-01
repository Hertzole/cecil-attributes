using System.Collections;
using System.Collections.Generic;
using Hertzole.CecilAttributes;
using UnityEngine;

public class Boxes : Animators
{
    [GetComponent(target = GetComponentTarget.Children)]
    public List<BoxCollider> boxes;

    protected override bool Template(bool dirty)
    {
       
        
        return base.Template(dirty);
    }

    protected override void FetchComponentsTemplate()
    {
        if (boxes == null)
        {
            boxes = new List<BoxCollider>();
        }
        boxes.Clear();
        GetComponentsInChildren(true, boxes);
        
        base.FetchComponentsTemplate();
    }
}
