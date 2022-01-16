using System.Collections;
using System.Collections.Generic;
using Hertzole.CecilAttributes;
using UnityEngine;

public class Boxes : MonoBehaviour
{
    [GetComponent(target = GetComponentTarget.Children)]
    public List<BoxCollider> boxes;
}
