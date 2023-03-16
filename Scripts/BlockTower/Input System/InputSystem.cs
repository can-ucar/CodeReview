using System;
using System.Collections;
using System.Collections.Generic;
using AnotherWorld.Core;
using UnityEngine;

public class InputSystem : InputManager
{
    [SerializeField] private ShapeTemplate m_SelectedParent;
    [SerializeField] private Vector3 m_SelectedStackableLastPos;
    [SerializeField] private LayerMask m_SelectableMasks;
    [SerializeField] private LayerMask m_YPlane;
    [SerializeField] private float m_VerticalOffset;
    [SerializeField] private float m_ZOffset;
    [SerializeField] private float m_scaleOffset;
    [SerializeField] private AnimationCurve m_SelectedRiseCurve;
    [SerializeField] private AnimationCurve m_ZCurve;
    private float tRise;

    [SerializeField] private Camera Camera;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnButtonDown();
        }

        if (Input.GetMouseButton(0))
        {
            OnButtonHold();
        }
        
        if(Input.GetMouseButtonUp(0))
        {
            OnButtonUp();
        }
    }

    void OnButtonDown()
    {
        m_SelectedParent = null;
        Block selected=null;
        if (SendRayTo(m_SelectableMasks).normal != Vector3.zero)
        {
            selected = SendRayTo(m_SelectableMasks).collider.GetComponentInParent<Block>();
        }
        if(selected == null) return;
        if(selected.m_IsPlaced) return;

        m_SelectedParent = selected.m_MyParent.m_ShapeTemplate;
        if (m_SelectedParent.m_IsMoving)
        {
            m_SelectedParent = null;
            return;
        }
        m_SelectedStackableLastPos = m_SelectedParent.transform.position;
        
        m_SelectedParent.SeperateBlocks();
        tRise = 0f;
    }

    void OnButtonHold()
    {
        if (m_SelectedParent == null) return;

        RaycastHit hit = SendRayTo(m_YPlane);
        
        //m_SelectedParent.m_Me.transform.position = Vector3.Lerp(m_SelectedParent.m_Me.transform.position, hit.point + new Vector3(0,m_VerticalOffset,-0.05f), 2f * Time.deltaTime);
        tRise += Time.deltaTime * 2f;
        var pos = hit.point;
        pos.y += tRise < 2f ? Mathf.LerpUnclamped(0f, m_VerticalOffset, m_SelectedRiseCurve.Evaluate(tRise)) : m_VerticalOffset;
        //pos.z += tRise < 2f ? Mathf.LerpUnclamped(0f, m_ZOffset, m_SelectedRiseCurve.Evaluate(tRise)) : m_ZOffset;
        pos.z = -0.8f + m_ZCurve.Evaluate(pos.y / 2f) * m_ZOffset;
        m_SelectedParent.transform.position = pos;
        m_SelectedParent.transform.localScale = tRise < 2f ? Vector3.LerpUnclamped(m_SelectedParent.transform.localScale, Vector3.one*m_scaleOffset, m_SelectedRiseCurve.Evaluate(tRise)) : Vector3.one*m_scaleOffset;
        //m_SelectedBlock.m_IsDragging = true;
        m_SelectedParent.SetDraggingBlocks(true);
        m_SelectedParent.DraggingBlocksGhostSetup();
        m_SelectedParent.m_IsMoving = true;
    }

    void OnButtonUp()
    {
        if (m_SelectedParent == null) return;
        m_SelectedParent.transform.localScale = Vector3.one * m_scaleOffset;
        m_SelectedParent.ReleaseBlockShape();
        
        m_SelectedParent = null;
    }

    RaycastHit SendRayTo(LayerMask targetLayerMask)
    {
        var ray = Camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit,1000f,targetLayerMask))
        {
            //hit
        }

        return hit;
    }
}
