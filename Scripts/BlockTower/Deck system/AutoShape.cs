using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoShape : MonoBehaviour
{
    [HideInInspector,SerializeField]public BlockType[] m_BlockType;
    [HideInInspector,SerializeField]public Vector2Int m_Size;
    
    

    public BlockType this[int x, int y]
    {
        get => m_BlockType[y * m_Size.x + x];
        set => m_BlockType[y * m_Size.x + x] = value;
    }
}
