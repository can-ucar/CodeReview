using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("Implements")] 
    public List<Block> m_MyCombinedBlocks = new List<Block>(); //other combined block like tetris blocks L, T, I shapes
    public Shape m_MyParent; //combined blocks's parent to get mid point of them for drag move
    public ShapeColumn m_MyColumn; //combined blocks's parent's shape column
    public DeckSlot m_MyDeckSlot; //spawned deck slot in to
    public List<Column> m_InteractedColumns = new List<Column>();
    [SerializeField] private Renderer m_Renderer;
    private Material m_DefaultMat;
    [SerializeField] private Material m_LowlightMat;
    public Rigidbody m_Rigidbody;
    [SerializeField] private Collider m_Collider;
    [SerializeField] private JellyMesh m_Jelly;
    [SerializeField] private GameObject m_MyGhostBlock;
    public ParticleController m_ParticleController;
    private bool m_GhostBlockActive;
    private Vector3 m_GhostBlockPosition = Vector3.zero;
    private MeshRenderer[] m_MeshRenderers;
    public GameObject m_Model;

    [Header("Checks")] 
    public bool m_IsPlaced;
    public bool m_IsDragging;
    public bool m_IsOnProcess;
    public bool m_IsBlockPlaceable = true;

    [Header("Settings")] 
    public BlockType m_CurrentBlockType;
    public float m_BlockGap;
    public float m_BlockCoinValue=5;

    private void Awake()
    {
        //m_DefaultMat = m_Renderer.material;
        m_MeshRenderers = m_Model.GetComponentsInChildren<MeshRenderer>();
    }
    public void OnOffPhysics(bool on)
    {
        if (on)
        {
            m_Rigidbody.useGravity = true;
            m_Collider.enabled = true;
            //m_Jelly.enabled = true;
        }
        else
        {
            m_Rigidbody.useGravity = false;
            m_Rigidbody.velocity = Vector3.zero;
            m_Collider.enabled = false;
            //m_Jelly.enabled = false;
        }
    }

    public void Highlight(bool highlight)
    {
        if (highlight)
        {
            m_MeshRenderers[0].material.SetColor("_EmissionColor", Color.white);
        }
        else
        {
            m_MeshRenderers[0].material.SetColor("_EmissionColor", Color.black);
        }
    }
    public void Lowlight(bool lowlight)
    {
        if (lowlight)
        {
            m_Renderer.material.SetColor("_BaseColor", new Color(m_Renderer.material.color.r,m_Renderer.material.color.g,m_Renderer.material.color.b,0.4f));
            m_IsBlockPlaceable = false;
        }
        else
        {
            m_Renderer.material.SetColor("_BaseColor", new Color(m_Renderer.material.color.r,m_Renderer.material.color.g,m_Renderer.material.color.b,1f));
            m_IsBlockPlaceable = true;
        }
    }

    public void OnOffMyGhost(bool on,Vector3 targetPos = default)
    {
        m_GhostBlockActive = on;
        
        if (on)
        {
            m_MyGhostBlock.transform.position = targetPos;
            m_GhostBlockPosition = targetPos;
            m_MyGhostBlock.SetActive(true);
        }
        else
        {
            m_MyGhostBlock.SetActive(false);
            m_MyGhostBlock.transform.localPosition = Vector3.zero;
        }
    }

    private void Update()
    {
        if (m_GhostBlockActive)
        {
            m_MyGhostBlock.transform.position = m_GhostBlockPosition;
        }
    }

    public void SetPlaced()
    {
        m_IsPlaced = true;
        if (m_MyParent != null)
        {
            if (m_MyParent.m_ShapeTemplate)
            {
                m_MyParent?.m_ShapeTemplate.RemoveBlockFromList(this);
            }
        }
        if (m_MyColumn)
        {
            m_MyColumn.m_MyBlocks.Remove(this);
        }
        m_MyColumn = null;
        m_MyCombinedBlocks.Clear();
        m_MyParent = null;
    }

    public void ResetBlock(float duration=0,bool fallen=false)
    {
        if (duration==0)
        {
            m_MyCombinedBlocks.Clear();
            m_MyParent = null;
            m_MyDeckSlot = null;
            m_IsPlaced = false;
            m_IsDragging = false;
            m_IsOnProcess = false;
            transform.parent = PoolScript.Instance.transform;
            transform.localPosition = Vector3.zero;
            gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(ResetAfterDuration(duration,fallen));
        }
    }

    IEnumerator ResetAfterDuration(float duration,bool fallen)
    {
        yield return new WaitForSeconds(duration);
        
        float time = 0;
        Vector3 startValue = transform.localScale;
        Vector3 endValue = Vector3.zero;
        float _Duration = 0.5f;
    
        while (time < _Duration)
        {
            float t = time / _Duration;
            t = t * t * (3f - 2f * t);
        
            Vector3 value = Vector3.Lerp(startValue, endValue, t);
            transform.localScale = value;
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = endValue;
        

        if(fallen) m_ParticleController.PlayFallenParticle(transform.position);
        m_MyCombinedBlocks.Clear();
        m_MyParent = null;
        m_MyDeckSlot = null;
        m_IsPlaced = false;
        m_IsDragging = false;
        m_IsOnProcess = false;
        transform.parent = PoolScript.Instance.transform;
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
    }
}
