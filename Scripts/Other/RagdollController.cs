using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private Animator Animator;

    public Rigidbody[] m_RagRigids;
    public Collider[] m_Colliders;
    public Collider m_ParentCollider;
    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponent<Animator>();
        m_RagRigids = GetComponentsInChildren<Rigidbody>();
        m_Colliders = GetComponentsInChildren<Collider>();
        EnableRagdoll(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableRagdoll(bool _IsEnabled)
    {
        Animator.enabled = !_IsEnabled;
        if(m_ParentCollider) m_ParentCollider.enabled = !_IsEnabled;

        foreach (Rigidbody rb in m_RagRigids)
        {
            if (rb != null)
            {
                rb.isKinematic = !_IsEnabled;
            }
        }
        foreach (Collider col in m_Colliders)
        {
            if (col != null)
            {
                col.enabled = _IsEnabled;
            }
        }
    }
}
