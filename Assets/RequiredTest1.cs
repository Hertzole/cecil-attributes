using System;
using System.Collections;
using System.Collections.Generic;
using Hertzole.CecilAttributes;
using UnityEngine;

public class RequiredTest1 : MonoBehaviour
{
    [SerializeField] 
    [Required]
    private Animator anim = default;
    [SerializeField] 
    private SkinnedMeshRenderer ren = default;
    
    private void Awake()
    {
        Debug.Log("Awake");
    }
}

public class RequiredTest2 : RequiredTest1
{
    
}

public class RequiredTest3 : RequiredTest2
{
    [SerializeField]
    [Required]
    private BoxCollider box = default;
}

public class RequiredTemplate : MonoBehaviour
{
    [SerializeField] 
    private Animator anim = default;
    [SerializeField] 
    private BoxCollider box = default;
    
    private void Awake()
    {
        bool error = false;
        
        if (anim == null)
        {
            Debug.LogError("Animator is null", this);
            error = true;
        }

        if (box == null)
        {
            Debug.LogError("Box is null", this);
            error = true;
        }

        if (error)
        {
            return;
        }

        Debug.Log("Awake");
        anim = GetComponent<Animator>();
    }
}