using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Shape
{
    [HideInInspector] public GameObject m_Me;

    [HideInInspector,SerializeField]public BlockType[] m_BlockType;
    [HideInInspector,SerializeField]public Vector2Int m_Size;

    [HideInInspector]public ShapeTemplate m_ShapeTemplate;
    
    public Transform Initialize(Transform _Parent, DeckSlot deckSlot,ShapeTemplate template,List<Block> possibleBlocks = default,BlockType[] BlockType = default,Vector2Int Size = default)
    {
        m_Me = new GameObject("Shape");
        m_ShapeTemplate = template;
        
        m_ShapeTemplate.Initialize(m_BlockType,m_Size,this,deckSlot);
        m_ShapeTemplate.m_InitialPosition = _Parent.position;
        
        return m_Me.transform;
    }
    
    public Transform AutoInitialize(Transform _Parent, DeckSlot deckSlot,ShapeTemplate template,BlockType[] BlockType,Vector2Int Size)
    {
        m_Me = new GameObject("Shape");
        m_ShapeTemplate = template;
        
        m_ShapeTemplate.AutoInitialize(BlockType,Size,this,deckSlot);
        m_ShapeTemplate.m_InitialPosition = _Parent.position;
        
        return m_Me.transform;
    }
    public BlockType this[int x, int y]
    {
        get => m_BlockType[y * m_Size.x + x];
        set => m_BlockType[y * m_Size.x + x] = value;
    }
}
