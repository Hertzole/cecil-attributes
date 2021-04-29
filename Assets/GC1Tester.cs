using UnityEngine;

public class GC1Tester : MonoBehaviour
{
    private void Reset()
    {
        Debug.Log("Reset");
    }

    private void OnValidate()
    {
        Debug.Log("OnValidate");
    }
}
