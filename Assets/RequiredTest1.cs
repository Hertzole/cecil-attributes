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
    [Required]
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
        bool error = CheckRequired();

        if (error)
        {
            return;
        }

        Debug.Log("Awake");
        anim = GetComponent<Animator>();
    }

    private bool CheckRequired()
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

        return error;
    }
}

public class RequiredBaseTemplate : MonoBehaviour
{
    [SerializeField] 
    private Animator anim = default;
    
    protected virtual void Awake()
    {
        bool error = CheckRequired();

        if (error)
        {
            return;
        }

        Debug.Log("Awake");
        anim = GetComponent<Animator>();
    }

    protected virtual bool CheckRequired()
    {
        bool error = false;
        
        if (anim == null)
        {
            Debug.LogError("Animator is null", this);
            error = true;
        }

        return error;
    }
}

public class RequiredChildTemplate : RequiredBaseTemplate
{
    [SerializeField] 
    private Animator anim = default;

    protected override void Awake()
    {
        base.Awake();

        Debug.Log("child Awake");
    }

    protected override bool CheckRequired()
    {
        bool error = base.CheckRequired();
        
        if (anim == null)
        {
            Debug.LogError("Animator is null", this);
            error = true;
        }
        
        return error;
    }
}