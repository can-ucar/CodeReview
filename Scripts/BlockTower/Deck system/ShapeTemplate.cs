using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using AnotherWorld.Core;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;


[System.Serializable]
public class ShapeTemplate : MonoBehaviour
{
    [HideInInspector, SerializeField] public Vector2Int m_Size;
    [HideInInspector, SerializeField] public BlockType[] m_Block;
    [SerializeField] float m_CellSize = 1f;
    [SerializeField] float m_ColumnGap = 1f;
    public Transform[] m_BlockParents;
    public List<ShapeColumn> m_BlockShapeColumns = new List<ShapeColumn>();
    public Block[] m_BlockPrefabs;
    [HideInInspector] public Block[,] m_Blocks;
    private Transform m_BlocksRoot;
    private Vector3 m_Offset;
    public Vector3 m_InitialPosition;

    private int m_LastX=0;
    private Block m_LastBlockX0;
    private Block m_LastBlockX1;
    private Block m_LastBlockX2;

    public bool m_IsPlaceable = true;
    public bool m_IsMoving;

    public static event Action OnShapePlaced;
    public static event Func<GameData> GetGameData;
    public static event Func<int> GetTutorialStepIndex; 

    public void Initialize(BlockType[] BlockTypes,Vector2Int Size,Shape shapeParent,DeckSlot deckSlot)
    {
        m_Block = BlockTypes;
        m_Size = Size;
        m_Blocks = new Block[m_Size.x, m_Size.y];
        m_BlocksRoot = new GameObject("Blocks").transform;
        m_BlocksRoot.parent = shapeParent.m_Me.transform;
        m_BlocksRoot.localScale = Vector3.one;
        var halfSize = -m_CellSize * 0.5f;
        m_Offset = new Vector3 (m_Size.x * halfSize - halfSize, 0, 0f);
        m_BlocksRoot.localPosition = m_Offset;
        m_BlockParents = new Transform[m_Size.x];
        
        for (int x = 0; x < m_Size.x; x++)
        {
            m_BlockParents[x] = new GameObject(x + " Parent").transform;
            m_BlockParents[x].parent = m_BlocksRoot;
            m_BlockParents[x].localPosition = Vector3.zero;
            for (int y = 0; y < m_Size.y; y++)
            {
                if (this[x, y] != BlockType.Empty)
                {
                    m_Blocks[x, y] = GetNewBlock(x, y, shapeParent,m_BlockParents[x]);
                    if (m_BlockParents[x].GetComponent<ShapeColumn>())
                    {
                        m_BlockParents[x].GetComponent<ShapeColumn>().m_MyBlocks.Add(m_Blocks[x, y]);
                    }
                    else
                    {
                        m_BlockParents[x].gameObject.AddComponent<ShapeColumn>().m_MyBlocks.Add(m_Blocks[x, y]);
                    }
                    m_Blocks[x, y].m_MyColumn = m_BlockParents[x].GetComponent<ShapeColumn>();
                    if (!m_BlockShapeColumns.Contains(m_BlockParents[x].GetComponent<ShapeColumn>()))
                    {
                        m_BlockShapeColumns.Add(m_BlockParents[x].GetComponent<ShapeColumn>());
                    }
                    List<Block> childBlocks = new List<Block>();
                    for (int i = 0; i < m_BlockParents[x].childCount; i++)
                    {
                        if (m_BlockParents[x].GetChild(i).GetComponent<Block>())
                        {
                            Block childBlock = m_BlockParents[x].GetChild(i).GetComponent<Block>();
                            if (!childBlocks.Contains(childBlock))
                            {
                                childBlocks.Add(childBlock);
                            }
                        }
                    }

                    foreach (Block childBlock in childBlocks)
                    {
                        foreach (Block block in childBlocks)
                        {
                            if (!childBlock.m_MyCombinedBlocks.Contains(block))
                            {
                                childBlock.m_MyCombinedBlocks.Add(block);
                            }
                        }
                    }
                    
                    
                    m_Blocks[x, y].m_MyDeckSlot = deckSlot;
                    m_Blocks[x, y].OnOffPhysics(false);

                }
            }
        }
    }
    public static event Func<List<Block>> GetPossibleBlocks;

    public void AutoInitialize(BlockType[] BlockTypes,Vector2Int Size,Shape shapeParent,DeckSlot deckSlot)
    {
        m_Block = BlockTypes;
        m_Size = Size;
        m_Blocks = new Block[m_Size.x, m_Size.y];
        m_BlocksRoot = new GameObject("Blocks").transform;
        m_BlocksRoot.parent = shapeParent.m_Me.transform;
        m_BlocksRoot.localScale = Vector3.one;
        var halfSize = -m_CellSize * 0.5f;
        m_Offset = new Vector3 (m_Size.x * halfSize - halfSize, 0, 0f);
        m_BlocksRoot.localPosition = m_Offset;
        m_BlockParents = new Transform[m_Size.x];
        
        for (int x = 0; x < m_Size.x; x++)
        {
            m_BlockParents[x] = new GameObject(x + " Parent").transform;
            m_BlockParents[x].parent = m_BlocksRoot;
            m_BlockParents[x].localPosition = Vector3.zero;
            for (int y = 0; y < m_Size.y; y++)
            {
                if (this[x, y] != BlockType.Empty)
                {
                    if (this[x,y] == BlockType.FillAuto)
                    {
                        m_Blocks[x, y] = GetPossibleBlock(x, y, shapeParent,m_BlockParents[x],GetPossibleBlocks.Invoke());
                        if (m_BlockParents[x].GetComponent<ShapeColumn>())
                        {
                            m_BlockParents[x].GetComponent<ShapeColumn>().m_MyBlocks.Add(m_Blocks[x, y]);
                        }
                        else
                        {
                            m_BlockParents[x].gameObject.AddComponent<ShapeColumn>().m_MyBlocks.Add(m_Blocks[x, y]);
                        }
                        m_Blocks[x, y].m_MyColumn = m_BlockParents[x].GetComponent<ShapeColumn>();
                        if (!m_BlockShapeColumns.Contains(m_BlockParents[x].GetComponent<ShapeColumn>()))
                        {
                            m_BlockShapeColumns.Add(m_BlockParents[x].GetComponent<ShapeColumn>());
                        }
                        List<Block> childBlocks = new List<Block>();
                        for (int i = 0; i < m_BlockParents[x].childCount; i++)
                        {
                            if (m_BlockParents[x].GetChild(i).GetComponent<Block>())
                            {
                                Block childBlock = m_BlockParents[x].GetChild(i).GetComponent<Block>();
                                if (!childBlocks.Contains(childBlock))
                                {
                                    childBlocks.Add(childBlock);
                                }
                            }
                        }

                        foreach (Block childBlock in childBlocks)
                        {
                            foreach (Block block in childBlocks)
                            {
                                if (!childBlock.m_MyCombinedBlocks.Contains(block))
                                {
                                    childBlock.m_MyCombinedBlocks.Add(block);
                                }
                            }
                        }
                    
                    
                        m_Blocks[x, y].m_MyDeckSlot = deckSlot;
                        m_Blocks[x, y].OnOffPhysics(false);
                    }
                }
            }
            m_LastX = x;
        }
    }

    public Block GetNewBlock(int x, int y, Shape mainShapeParent,Transform local)
    {
        Block block = null;

        foreach (Block blockPrefab in m_BlockPrefabs)
        {
            if (blockPrefab.m_CurrentBlockType == this[x, y])
            {
                block = blockPrefab;
            }
        }
        var blockInitialized = PoolScript.Instance.GetPooledObject(block.name).GetComponent<Block>();
        blockInitialized.transform.parent = local;
        blockInitialized.m_MyParent = mainShapeParent;
        blockInitialized.transform.localPosition = new Vector3(x * m_CellSize, y * m_CellSize, 0f);
        return blockInitialized;
    }

    public Block GetPossibleBlock(int x, int y, Shape mainShapeParent, Transform local,List<Block> PossibleBlocks)
    {
        Block block=null;

        if (x == 0)
        {
            if (m_LastBlockX0)
            {
                block = m_LastBlockX0;
            }
            else
            {
                block = PossibleBlocks[Random.Range(0, PossibleBlocks.Count)];
                m_LastBlockX0 = block;
            }
        }
        else if (x == 1)
        {
            if (m_LastBlockX1)
            {
                block = m_LastBlockX1;
            }
            else
            {
                block = PossibleBlocks[Random.Range(0, PossibleBlocks.Count)];
                if (block.m_CurrentBlockType == m_LastBlockX0.m_CurrentBlockType)
                {
                    block = PossibleBlocks[Random.Range(0, PossibleBlocks.Count)];
                }
                m_LastBlockX1 = block;
            }
        }
        else if (x == 2)
        {
            if (m_LastBlockX2)
            {
                block = m_LastBlockX2;
            }
            else
            {
                block = PossibleBlocks[Random.Range(0, PossibleBlocks.Count)];
                if (block.m_CurrentBlockType == m_LastBlockX1.m_CurrentBlockType)
                {
                    block = PossibleBlocks[Random.Range(0, PossibleBlocks.Count)];
                }
                m_LastBlockX2 = block;
            }
        }

        var blockInitialized = PoolScript.Instance.GetPooledObject(block.name).GetComponent<Block>();
        blockInitialized.transform.parent = local;
        blockInitialized.m_MyParent = mainShapeParent;
        blockInitialized.transform.localPosition = new Vector3(x * m_CellSize, y * m_CellSize, 0f);
        return blockInitialized;
    }
    

    public BlockType this[int x, int y]
    {
        get => m_Block[y * m_Size.x + x];
        set => m_Block[y * m_Size.x + x] = value;
    }

    public void SeperateBlocks()
    {
        if(m_BlockParents.Length==1) return;
        
        if (m_BlockParents.Length % 2 == 0)
        {
            for (int i = 0; i < m_BlockParents.Length; i++)
            {
                if (i > m_BlockParents.Length / 2)
                {
                    //m_BlockParents[i].transform.localPosition = new Vector3(-(m_CellSize / 2) * i, 0, 0);
                    m_BlockParents[i].transform.DOLocalMove( new Vector3(-(m_ColumnGap / 2) * i, 0, 0),0.3f);
                }
                else
                {
                    //m_BlockParents[i].transform.localPosition = new Vector3((m_CellSize / 2) * i, 0, 0);
                    m_BlockParents[i].transform.DOLocalMove( new Vector3((m_ColumnGap / 2) * i, 0, 0),0.3f);
                }
            }
        }
        else
        {
            for (int i = 0; i < m_BlockParents.Length; i++)
            {
                if (i < m_BlockParents.Length / 2)
                {
                   // m_BlockParents[i].transform.localPosition = new Vector3(-(m_CellSize / 2), 0, 0);
                    m_BlockParents[i].transform.DOLocalMove( new Vector3(-(m_ColumnGap / 2), 0, 0),0.3f);
                }
                else if (i == m_BlockParents.Length / 2)
                {
                    continue;
                }
                else
                {
                    //m_BlockParents[i].transform.localPosition = new Vector3((m_CellSize / 2), 0, 0);
                    m_BlockParents[i].transform.DOLocalMove( new Vector3((m_ColumnGap / 2), 0, 0),0.3f);
                }
            }
        }
    }

    public void CombineBlocks()
    {
        for (int i = 0; i < m_BlockParents.Length; i++)
        {
            //m_BlockParents[i].transform.localPosition = Vector3.zero;
            m_BlockParents[i].transform.DOLocalMove(Vector3.zero, 0.3f);
        }
    }

    public void SetDraggingBlocks(bool dragging)
    {
        foreach (Block block in m_Blocks)
        {
            if (block)
            {
                if (dragging)
                {
                    block.m_IsDragging = true;
                }
                else
                {
                    block.m_IsDragging = false;
                    block.m_IsOnProcess = false;
                    
                }
            }
        }
    }

    private List<Column> m_AllInteractedTowerColumns = new List<Column>();

    public void DraggingBlocksGhostSetup()
    {
        List<Column> currentAllTowerColumns = new List<Column>();
        
        Column interactedColumn = null;

        for (int x = 0; x < m_Blocks.GetLength(0); x++)
        {
            interactedColumn = null;

            for (int y = 0; y < m_Blocks.GetLength(1); y++)
            {
                if (m_Blocks[x,y])
                {
                    m_Blocks[x, y].m_MyColumn.m_MyBlocksInteractedColumn = null;
                    if (m_Blocks[x,y].m_InteractedColumns.Count > 0)
                    {
                        //if (interactedColumn == null)
                        {
                            if (m_Blocks[x, y].m_InteractedColumns.Count > 1)
                            {
                                float distance1 = Mathf.Abs(m_Blocks[x, y].m_InteractedColumns[0].transform.position.x - m_Blocks[x, y].transform.position.x);
                                float distance2 = Mathf.Abs(m_Blocks[x, y].m_InteractedColumns[1].transform.position.x - m_Blocks[x, y].transform.position.x);
                
                                if (distance1 < distance2)
                                {
                                    interactedColumn = m_Blocks[x, y].m_InteractedColumns[1];
                                }
                                else
                                {
                                    interactedColumn = m_Blocks[x, y].m_InteractedColumns[0];
                                }
                            }
                            else
                            {
                                interactedColumn = m_Blocks[x, y].m_InteractedColumns[0];
                            }
                            /*
                            if (x != 0 && currentAllTowerColumns.Count > 1 &&  m_Blocks[x, y].m_InteractedColumns[0] == currentAllTowerColumns[x - 1])
                            {
                                interactedColumn = m_Blocks[x, y].m_InteractedColumns[1];
                            }
                            else
                            {
                                interactedColumn = m_Blocks[x, y].m_InteractedColumns[0];
                            }
                            */
                        }
                    }
                    else
                    {
                        m_Blocks[x,y].OnOffMyGhost(false);
                    }
                }
            }

            if(interactedColumn == null) continue;
            interactedColumn.m_IsPreview = true;
            if(!currentAllTowerColumns.Contains(interactedColumn)) currentAllTowerColumns.Add(interactedColumn);

            for (int y = 0; y < m_Blocks.GetLength(1); y++)
            {
                if (m_Blocks[x,y])
                {
                    m_Blocks[x, y].m_MyColumn.m_MyBlocksInteractedColumn = interactedColumn;
                    interactedColumn.AddMyListForPrePlacing(m_Blocks[x,y]);
                }
            }
        }

        if (m_AllInteractedTowerColumns.Count == 0)
        {
            m_AllInteractedTowerColumns = new List<Column>(currentAllTowerColumns);
        }
        else
        {
            List<Column> removedColums = new List<Column>();
            foreach (Column column in m_AllInteractedTowerColumns)
            {
                if (!currentAllTowerColumns.Contains(column))
                {
                    removedColums.Add(column);
                    column.m_LastInteractedBlocks.Clear();
                    column.m_MyGhostBlocksList.Clear();
                    column.m_IsPreview = false;
                    column.ClearPreviewLists();
                    column.m_LastInteractedShapeColumn = null;
                    
                    if (!column.m_IsOrganizingBlocks)
                    {
                        column.OrganizeBlocks();
                    }
                }
            }

            foreach (Column column in removedColums)
            {
                m_AllInteractedTowerColumns.Remove(column);
            }
        }
    }


    public void ReleaseBlockShape()
    {
        SetDraggingBlocks(false);

        if (!CheckForPlaceableBlocks())
        {
            CombineBlocks();
            StartCoroutine(MoveToPositionCoroutine(transform, m_InitialPosition));
        }
    }
    
    private IEnumerator MoveToPositionCoroutine(Transform tr, Vector3 pos)
    {
        float t = 0;
        var startPos = tr.position;
        var speed = 5f / (pos - startPos).magnitude;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;

            tr.position = Vector3.Lerp(startPos, pos, t);
            tr.localScale = Vector3.Lerp(tr.localScale,Vector3.one,t);

            yield return 0;
        }
        tr.position = pos;
        m_IsMoving = false;
    }

    public bool CheckForPlaceableBlocks()
    {
        int placedAmount = 0;
        List<ShapeColumn> notPlacedColumns = new List<ShapeColumn>();
        List<Column> PlacedTowerColumns = new List<Column>();

        /*foreach (ShapeColumn shapeColumn in m_BlockShapeColumns)
        {
            if (shapeColumn.m_MyBlocksInteractedColumn)
            {
                Debug.Log("shapeColumn.m_MyBlocksInteractedColumn = " + shapeColumn.m_MyBlocksInteractedColumn.gameObject.name);
                Debug.Log("Placed blocks = " + shapeColumn.m_MyBlocks.Count);
                shapeColumn.m_MyBlocksInteractedColumn.PlaceBlocks(shapeColumn.m_MyBlocks);
                PlacedTowerColumns.Add(shapeColumn.m_MyBlocksInteractedColumn);
                placedAmount++;
            }
            else
            {
                notPlacedColumns.Add(shapeColumn);
            }
        }
        
        //Rearrange all remaining columns
        for (int i = 0; i < m_AllTowerColumns.Count; i++)
        {
            if (!PlacedTowerColumns.Contains(m_AllTowerColumns[i]))
            {
                m_AllTowerColumns[i].OrganizeBlocks();
            }
        }

        if (placedAmount != 0)
        {
            foreach (ShapeColumn notPlacedColumn in notPlacedColumns)
            {
                foreach (Block block in notPlacedColumn.m_MyBlocks)
                {
                    block.OnOffPhysics(true);
                    block.ResetBlock(1,true);
                }
            }
            OnShapePlaced?.Invoke();
            return true;
        }*/
        
        if (GetGameData.Invoke().tutorial)
        {
            foreach (ShapeColumn shapeColumn in m_BlockShapeColumns)
            {
                if (shapeColumn.m_MyBlocksInteractedColumn)
                {
                    Debug.Log("shapeColumn.m_MyBlocksInteractedColumn = " + shapeColumn.m_MyBlocksInteractedColumn.gameObject.name);
                    Debug.Log("Placed blocks = " + shapeColumn.m_MyBlocks.Count);
                    shapeColumn.m_MyBlocksInteractedColumn.PlaceBlocks(shapeColumn.m_MyBlocks);
                    placedAmount++;
                }
                else
                {
                    notPlacedColumns.Add(shapeColumn);
                }
            }

            if (placedAmount != 0)
            {
                foreach (ShapeColumn notPlacedColumn in notPlacedColumns)
                {
                    foreach (Block block in notPlacedColumn.m_MyBlocks)
                    {
                        block.OnOffPhysics(true);
                        block.ResetBlock(1,true);
                    }
                }
                OnShapePlaced?.Invoke();
                return true;
            }
        }
        else
        {
            foreach (ShapeColumn shapeColumn in m_BlockShapeColumns)
            {
                if (shapeColumn.m_MyBlocksInteractedColumn)
                {
                    if (GetTutorialStepIndex.Invoke() == 0)
                    {
                        if (transform.position.y > 0.48f && transform.position.y < 0.667f)
                        {
                            shapeColumn.m_MyBlocksInteractedColumn.PlaceBlocks(shapeColumn.m_MyBlocks);
                            placedAmount++;
                        }
                        else
                        {
                            shapeColumn.m_MyBlocksInteractedColumn.m_MyGhostBlocksList.Clear();
                            shapeColumn.m_MyBlocksInteractedColumn.m_IsPreview = false;
                            shapeColumn.m_MyBlocksInteractedColumn.OrganizeBlocks();
                            shapeColumn.m_MyBlocks[0].OnOffMyGhost(false);
                            return false;
                        }
                    }
                    else if (GetTutorialStepIndex.Invoke() == 1)
                    {
                        if (transform.position.y > 0.69f && transform.position.y < 1.12f)
                        {
                            shapeColumn.m_MyBlocksInteractedColumn.PlaceBlocks(shapeColumn.m_MyBlocks);
                            placedAmount++;
                        }
                        else
                        {
                            shapeColumn.m_MyBlocksInteractedColumn.m_MyGhostBlocksList.Clear();
                            shapeColumn.m_MyBlocksInteractedColumn.m_IsPreview = false;
                            shapeColumn.m_MyBlocksInteractedColumn.OrganizeBlocks();
                            shapeColumn.m_MyBlocks[0].OnOffMyGhost(false);
                            return false;
                        }
                    }
                }
                else
                {
                    notPlacedColumns.Add(shapeColumn);
                }
            }

            if (placedAmount != 0)
            {
                foreach (ShapeColumn notPlacedColumn in notPlacedColumns)
                {
                    foreach (Block block in notPlacedColumn.m_MyBlocks)
                    {
                        block.OnOffPhysics(true);
                        block.ResetBlock(1,true);
                    }
                }
                OnShapePlaced?.Invoke();
                return true;
            }
        }

        return false;
    }

    public void RemoveBlockFromList(Block block)
    {
        for (int x = 0; x < m_Size.x; x++)
        {
            for (int y = 0; y < m_Size.y; y++)
            {
                if (m_Blocks[x,y] == block)
                {
                    m_Blocks[x, y] = null;
                }
            }
        }
    }


    //public void SetGrayOut(bool grayOut)
    //{
    //    for (int y = 0; y < m_Size.y; y++)
    //    {
    //        for (int x = 0; x < m_Size.x; x++)
    //        {
    //            //m_Blocks[x, y]?.SetGrayout(grayOut);
    //        }
    //    }
    //}


    //public void SetBlock(bool[,] input)
    //{
    //    m_Size = new Vector2Int(input.GetLength(0), input.GetLength(1));

    //    m_Block = new bool[m_Size.x * m_Size.y];

    //    for (int y = 0; y < m_Size.y; y++)
    //    {
    //        for (int x = 0; x < m_Size.x; x++)
    //        {
    //            m_Block[y * m_Size.x + x] = input[x, y];
    //        }
    //    }
    //}


    //public bool[,] GetBlock ()
    //{
    //    bool[,] block = new bool[m_Size.x, m_Size.y];

    //    for (int y = 0; y < m_Size.y; y++)
    //    {
    //        for (int x = 0; x < m_Size.x; x++)
    //        {
    //            block[x, y] = m_Block[y * m_Size.x + x];
    //        }
    //    }
    //    return block;
    //}
}

[Serializable]
public enum BlockType
{
    Empty,
    Stone,
    Coal,
    Copper,
    Iron,
    Gold,
    Emerald,
    Sapphire,
    Redstone,
    Diamond,
    Obsidian,
    Chest,
    FillAuto,
    Dirt
}

