using Hertzole.CecilAttributes;
using UnityEngine;

public class GetComponentTester : MonoBehaviour
{
    [SerializeField]
    [GetComponent(showInInspector = true)]
    private Transform trs = null;
    [SerializeField]
    [GetComponent(target = GetComponentTarget.Children)]
    private BoxCollider childBox = null;
    [SerializeField]
    [GetComponent(target = GetComponentTarget.Parent)]
    private MeshRenderer parentRenderer = null;
    [SerializeField]
    private int test = 12;

    private void Template()
    {
        if (ReferenceEquals(trs, null))
        {
            trs = GetComponent<Transform>();
        }

        if (ReferenceEquals(childBox, null))
        {
            childBox = GetComponentInChildren<BoxCollider>();
        }
    }

    private void Test()
    {
        Template();
    }
}
