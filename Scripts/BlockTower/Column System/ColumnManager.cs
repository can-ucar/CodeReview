using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnotherWorld.Core;
using AnotherWorld.UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class ColumnManager : MonoBehaviour
{
    [Header(("Implements"))]
    [SerializeField] private List<Column> m_ColumnsOnScene = new List<Column>();
    [SerializeField] private List<BlockList> m_BlockTypesLevels = new List<BlockList>();
    [SerializeField] private List<Block> m_AllBlockTypes = new List<Block>();
    private Block m_LastCreatedBlockType;
    [SerializeField] private ParticleSystem m_DustParticle;
    [SerializeField] private List<GameObject> m_BlockPrefabs = new List<GameObject>();
    
    [Header("Checks")]
    private bool m_IsLastBlocksCreatedOverAndOver;

    [Header("Settings")] 
    [Range(0,100)]public float m_OverAndOverSameBlockCreatePercentage;
    public int m_HowManyBlockOverAndOverNeededToBlastAtLeast = 3;

    public static event Func<GameData> GetGameData;
    public static event Action OnSaveColumns;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        Column.OnGetHowManyBlockOverAndOverNeededToBlastAtLeast += GetOnGetHowManyBlockOverAndOverNeededToBlastAtLeast;
        Column.OnGetNewBlocks += InitializeNewBlocksToColumn;
        ShapeTemplate.GetPossibleBlocks += GetPossibleBlockTypes;
        Column.OnNewBlocksAdded += SaveColumnsData;
        Column.OnUpgrade += SaveColumnsData;
        Column.OnUnlock += SaveColumnsData;
        Column.OnUpgradeBlocks += UpgradeColumn;
        DeckManager.OnGetUnlockedColumnsAmount += GetUnlockedColumnsAmount;
        StartPanel.OnTutorialDone += TutorialEndJob;
    }

    private void OnDisable()
    {
        Column.OnGetHowManyBlockOverAndOverNeededToBlastAtLeast -= GetOnGetHowManyBlockOverAndOverNeededToBlastAtLeast;
        Column.OnGetNewBlocks -= InitializeNewBlocksToColumn;
        ShapeTemplate.GetPossibleBlocks -= GetPossibleBlockTypes;
        Column.OnNewBlocksAdded -= SaveColumnsData;
        Column.OnUpgrade -= SaveColumnsData;
        Column.OnUnlock -= SaveColumnsData;
        Column.OnUpgradeBlocks -= UpgradeColumn;
        DeckManager.OnGetUnlockedColumnsAmount -= GetUnlockedColumnsAmount;
        StartPanel.OnTutorialDone -= TutorialEndJob;
    }
    
    public void Initialize()
    {
        if (LoadColumnsData())
        {
            InitializeNewBlocksToAllColumnsFromSave();
        }
        else
        {
           AutoInitializeNewBlocksToAllColumns();
           SaveColumnsData();
        }
    }

    void InitializeNewBlocksToAllColumnsFromSave()
    {
        GameData gameData = GetGameData.Invoke();

        for (int i = 0; i < m_ColumnsOnScene.Count; i++)
        {
            m_ColumnsOnScene[i].m_TowerLevel = gameData.towerLevels[i];
            if (m_ColumnsOnScene[i].m_TowerLevel > 0)
            {
                if(gameData.towerCurrentBlocks[i].Count == 0) continue;
                foreach (BlockType blockName in gameData.towerCurrentBlocks[i])
                {
                    if(blockName == BlockType.Chest) continue;
                    for (var index = 0;
                         index < m_BlockTypesLevels[m_ColumnsOnScene[i].m_TowerLevel].Blocks.Count;
                         index++)
                    {
                        var blockType = m_BlockTypesLevels[m_ColumnsOnScene[i].m_TowerLevel].Blocks[index];
                        if (blockType.m_CurrentBlockType == blockName)
                        {
                            Block createdBlock = Instantiate(GetBlockPrefab(blockType)).GetComponent<Block>();
                            m_ColumnsOnScene[i].AddMyListForCreation(createdBlock);
                        }
                    }
                }

                int chestIndex = m_ColumnsOnScene[i].m_TowerLevel >= 4 ? 4 : m_ColumnsOnScene[i].m_TowerLevel;
                Block createdChest = PoolScript.Instance.GetPooledObject("Block_Chest_"+chestIndex.ToString()).GetComponent<Block>();
                m_ColumnsOnScene[i].AddMyListForCreation(createdChest);
                m_ColumnsOnScene[i].UpgradePanelOnOff(true);
                m_ColumnsOnScene[i].UnlockPanelOnOff(false);
                m_ColumnsOnScene[i].m_IsUnlocked = true;
                m_ColumnsOnScene[i].RiseColumn();
                m_DustParticle.Play();
            }
            else
            {
                m_ColumnsOnScene[i].UpgradePanelOnOff(false);
                m_ColumnsOnScene[i].UnlockPanelOnOff(true);
            }
        }
    }
    void AutoInitializeNewBlocksToAllColumns()
    {
         foreach (Column targetColumn in m_ColumnsOnScene)
            {
                if (!targetColumn.m_IsUnlocked)
                {
                    if (!GetGameData.Invoke().tutorial)
                    {
                        targetColumn.UpgradePanelOnOff(false);
                        if (targetColumn == m_ColumnsOnScene[1])
                        {
                            targetColumn.UnlockPanelOnOff(true);
                        }
                        else
                        {
                            targetColumn.UnlockPanelOnOff(false);
                        }
                    }
                    else
                    {
                        targetColumn.UpgradePanelOnOff(false);
                        targetColumn.UnlockPanelOnOff(true);
                    }
                    continue;
                }
                if (targetColumn.m_ManualBlockCreationList.Count == 0)
                {
                    for (int i = 0; i < targetColumn.m_HowManyBlockItWillHave; i++)
                    {
                        if (i < targetColumn.m_HowManyBlockItWillHave - 1)
                        {
                            int OverAndOverSameBlockCreatePercentage = Random.Range(0, 100);
                            for (int j = 0; j < 1; j++)
                            {
                                if (OverAndOverSameBlockCreatePercentage <= m_OverAndOverSameBlockCreatePercentage)
                                {
                                    if (!m_IsLastBlocksCreatedOverAndOver)
                                    {
                                        Block nextBlock;
                                        if (m_LastCreatedBlockType == null)
                                        {
                                            nextBlock = Instantiate(GetBlockPrefab(m_BlockTypesLevels[targetColumn.m_TowerLevel].Blocks[Random.Range(0, m_BlockTypesLevels[targetColumn.m_TowerLevel].Blocks.Count)]
                                                    )).GetComponent<Block>();
                                        }
                                        else
                                        {
                                            string blockName = "";
                                            Block block = m_BlockTypesLevels[targetColumn.m_TowerLevel].Blocks[0];
                                            foreach (var blockType in m_BlockTypesLevels[targetColumn.m_TowerLevel].Blocks)
                                            {
                                                if (m_LastCreatedBlockType.m_CurrentBlockType ==
                                                    blockType.m_CurrentBlockType)
                                                {
                                                    blockName = blockType.gameObject.name;
                                                    block = blockType;
                                                }
                                            }

                                            nextBlock = Instantiate(GetBlockPrefab(block)).GetComponent<Block>();
                                        }

                                        m_LastCreatedBlockType = nextBlock;
                                        targetColumn.AddMyListForCreation(nextBlock);
                                        m_IsLastBlocksCreatedOverAndOver = true;
                                    }
                                    else
                                    {
                                        Block nextBlock = Instantiate(GetBlockPrefab(m_BlockTypesLevels[targetColumn.m_TowerLevel].Blocks[Random.Range(0, m_BlockTypesLevels[targetColumn.m_TowerLevel].Blocks.Count)]
                                                )).GetComponent<Block>();
                                        if (nextBlock.m_CurrentBlockType != m_LastCreatedBlockType.m_CurrentBlockType)
                                        {
                                            m_LastCreatedBlockType = nextBlock;
                                            targetColumn.AddMyListForCreation(nextBlock);
                                            m_IsLastBlocksCreatedOverAndOver = false;
                                        }
                                        else
                                        {
                                            j--;
                                            nextBlock.gameObject.SetActive(false);
                                        }
                                    }
                                }
                                else
                                {
                                    Block nextBlock = Instantiate(GetBlockPrefab(m_BlockTypesLevels[targetColumn.m_TowerLevel].Blocks[Random.Range(0, m_BlockTypesLevels[targetColumn.m_TowerLevel].Blocks.Count)])
                                            ).GetComponent<Block>();
                                    if (m_LastCreatedBlockType)
                                    {
                                        if (nextBlock.m_CurrentBlockType != m_LastCreatedBlockType.m_CurrentBlockType)
                                        {
                                            m_LastCreatedBlockType = nextBlock;
                                            targetColumn.AddMyListForCreation(nextBlock);
                                            m_IsLastBlocksCreatedOverAndOver = false;
                                        }
                                        else
                                        {
                                            j--;
                                            nextBlock.gameObject.SetActive(false);
                                        }
                                    }
                                    else
                                    {
                                        m_LastCreatedBlockType = nextBlock;
                                        targetColumn.AddMyListForCreation(nextBlock);
                                        m_IsLastBlocksCreatedOverAndOver = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            int chestIndex = targetColumn.m_TowerLevel >= 4 ? 4 : targetColumn.m_TowerLevel;
                            Block nextBlock = PoolScript.Instance.GetPooledObject("Block_Chest_"+chestIndex.ToString()).GetComponent<Block>();
                            targetColumn.AddMyListForCreation(nextBlock);
                            m_IsLastBlocksCreatedOverAndOver = false;
                        }
                    }
                }
                else
                {
                    targetColumn.m_ManualBlockCreationList.Reverse();
                    foreach (BlockType blockType in targetColumn.m_ManualBlockCreationList)
                    {
                        Block nextBlock = null;
                        
                        foreach (Block block in m_BlockTypesLevels[targetColumn.m_TowerLevel].Blocks)
                        {

                            if (block.m_CurrentBlockType == blockType)
                            {
                                nextBlock = Instantiate(GetBlockPrefab(block)).GetComponent<Block>();

                            }
                            else if (blockType == BlockType.Chest)
                            {
                                int chestIndex = targetColumn.m_TowerLevel >= 4 ? 4 : targetColumn.m_TowerLevel;
                                nextBlock = PoolScript.Instance.GetPooledObject("Block_Chest_"+chestIndex.ToString()).GetComponent<Block>();
                            }
                        }

                        targetColumn.AddMyListForCreation(nextBlock);
                    }
                }
                targetColumn.m_ManualBlockCreationList.Clear();
                targetColumn.m_IsUnlocked = true;
                if (targetColumn == m_ColumnsOnScene[0])
                {
                    targetColumn.UpgradePanelOnOff(false);
                    targetColumn.UnlockPanelOnOff(false);
                    targetColumn.RiseColumn(true);
                    //m_DustParticle.Play();
                }
                else
                {
                    targetColumn.UpgradePanelOnOff(true);
                    targetColumn.UnlockPanelOnOff(false);
                    targetColumn.RiseColumn();
                    m_DustParticle.Play();
                }
            }
    }
    void InitializeNewBlocksToColumn(Column column)
    {
        if (column.m_ManualBlockCreationList.Count == 0)
        {
            column.SendColumnUnderground();
            for (int i = 0; i < column.m_HowManyBlockItWillHave; i++)
            {
                if (i < column.m_HowManyBlockItWillHave - 1)
                {
                    int OverAndOverSameBlockCreatePercentage = Random.Range(0, 100);
                    for (int j = 0; j < 1; j++)
                    {
                        if (OverAndOverSameBlockCreatePercentage <= m_OverAndOverSameBlockCreatePercentage)
                        {
                            if (!m_IsLastBlocksCreatedOverAndOver)
                            {
                                Block nextBlock;
                                if (m_LastCreatedBlockType == null)
                                {
                                    nextBlock = Instantiate(GetBlockPrefab(m_BlockTypesLevels[column.m_TowerLevel].Blocks[Random.Range(0, m_BlockTypesLevels[column.m_TowerLevel].Blocks.Count)])).GetComponent<Block>();
                                }
                                else
                                {
                                    string blockName = "";
                                    Block block = m_BlockTypesLevels[column.m_TowerLevel].Blocks[0];
                                    foreach (var blockType in m_BlockTypesLevels[column.m_TowerLevel].Blocks)
                                    {
                                        if (m_LastCreatedBlockType.m_CurrentBlockType == blockType.m_CurrentBlockType)
                                        {
                                            blockName = blockType.gameObject.name;
                                            block = blockType;
                                        }
                                    }

                                    nextBlock = Instantiate(GetBlockPrefab(block)).GetComponent<Block>();
                                }

                                m_LastCreatedBlockType = nextBlock;
                                column.AddMyListForCreation(nextBlock);
                                m_IsLastBlocksCreatedOverAndOver = true;
                            }
                            else
                            {
                                Block nextBlock = Instantiate(GetBlockPrefab(m_BlockTypesLevels[column.m_TowerLevel].Blocks[Random.Range(0, m_BlockTypesLevels[column.m_TowerLevel].Blocks.Count)]))
                                    .GetComponent<Block>();
                                if (nextBlock.m_CurrentBlockType != m_LastCreatedBlockType.m_CurrentBlockType)
                                {
                                    m_LastCreatedBlockType = nextBlock;
                                    column.AddMyListForCreation(nextBlock);
                                    m_IsLastBlocksCreatedOverAndOver = false;
                                }
                                else
                                {
                                    j--;
                                    nextBlock.gameObject.SetActive(false);
                                }
                            }
                        }
                        else
                        {
                            Block nextBlock = Instantiate(GetBlockPrefab(m_BlockTypesLevels[column.m_TowerLevel].Blocks[Random.Range(0, m_BlockTypesLevels[column.m_TowerLevel].Blocks.Count)]))
                                .GetComponent<Block>();
                            if (m_LastCreatedBlockType)
                            {
                                if (nextBlock.m_CurrentBlockType != m_LastCreatedBlockType.m_CurrentBlockType)
                                {
                                    m_LastCreatedBlockType = nextBlock;
                                    column.AddMyListForCreation(nextBlock);
                                    m_IsLastBlocksCreatedOverAndOver = false;
                                }
                                else
                                {
                                    j--;
                                    nextBlock.gameObject.SetActive(false);
                                }
                            }
                            else
                            {
                                m_LastCreatedBlockType = nextBlock;
                                column.AddMyListForCreation(nextBlock);
                                m_IsLastBlocksCreatedOverAndOver = false;
                            }
                        }
                    }
                }
                else
                {
                    int chestIndex = column.m_TowerLevel >= 4 ? 4 : column.m_TowerLevel;
                    Block nextBlock = PoolScript.Instance.GetPooledObject("Block_Chest_"+chestIndex.ToString()).GetComponent<Block>();
                    column.AddMyListForCreation(nextBlock);
                    m_IsLastBlocksCreatedOverAndOver = false;
                }
            }
            column.RiseColumn();
            m_DustParticle.Play();
        }
        else
        {
            column.SendColumnUnderground();
            foreach (BlockType blockType in column.m_ManualBlockCreationList)
            {
                Block nextBlock = null;
                foreach (Block block in m_BlockTypesLevels[column.m_TowerLevel].Blocks)
                {

                    if (block.m_CurrentBlockType == blockType)
                    {
                        nextBlock = Instantiate(GetBlockPrefab(block)).GetComponent<Block>();

                    }
                    else if (blockType == BlockType.Chest)
                    {
                        int chestIndex = column.m_TowerLevel >= 4 ? 4 : column.m_TowerLevel;
                        nextBlock = PoolScript.Instance.GetPooledObject("Block_Chest_"+chestIndex.ToString()).GetComponent<Block>();
                    }
                }

                column.AddMyListForCreation(nextBlock);
            }
        }
        
        column.m_MyCollider.enabled = true;
        column.m_IsCompleted = false;
        SaveColumnsData();
        column.RiseColumn();
        m_DustParticle.Play();
    }

    void UpgradeColumn(Column column)
    {
        List<Block> changeBlocks = new List<Block>();

        for (int i = 0; i < column.m_MyBlockList.Count/2; i++)
        {
            int index = Random.Range(0, column.m_MyBlockList.Count - 1);
            if (column.m_MyBlockList[index].m_CurrentBlockType == BlockType.Chest)
            {
                i--;
                continue;
            }
            if (!changeBlocks.Contains(column.m_MyBlockList[index]))
            {
                changeBlocks.Add(column.m_MyBlockList[index]);
            }
        }
        
        for (int i = 0; i < column.m_MyBlockList.Count; i++)
        {
            Block block = column.m_MyBlockList[i];
            if (block.m_CurrentBlockType == BlockType.Chest)
            {
                Block chest = block;
                int chestIndex = column.m_TowerLevel >= 4 ? 4 : column.m_TowerLevel;
                Block nextBlock = PoolScript.Instance.GetPooledObject("Block_Chest_"+chestIndex.ToString()).GetComponent<Block>();
                nextBlock.transform.position = chest.transform.position;
                nextBlock.m_MyParent = chest.m_MyParent;
                nextBlock.m_MyColumn = chest.m_MyColumn;
                nextBlock.SetPlaced();
                column.InsertToMyBlockList(column.m_MyBlockList.IndexOf(chest),nextBlock);
                column.RemoveFromMyBlockList(chest);
                nextBlock.m_ParticleController.PlayRenewParticle(chest.transform.position);
                chest.ResetBlock();
            }
        }

        foreach (Block block in column.m_MyBlockList)
        {
            block.OnOffPhysics(false);
        }

        List<Block> newUpdatedBlocks = new List<Block>();
        for (int i = 0; i < changeBlocks.Count; i++)
        {
            Block newBlock = Instantiate(GetBlockPrefab(m_BlockTypesLevels[column.m_TowerLevel]
                    .Blocks[^1]))
                .GetComponent<Block>();
            newUpdatedBlocks.Add(newBlock);
        }

        for (int i = 0; i < changeBlocks.Count; i++)
        {
            newUpdatedBlocks[i].transform.position = changeBlocks[i].transform.position;
            newUpdatedBlocks[i].m_MyParent = changeBlocks[i].m_MyParent;
            newUpdatedBlocks[i].m_MyColumn = changeBlocks[i].m_MyColumn;
            newUpdatedBlocks[i].SetPlaced();
            column.InsertToMyBlockList(column.m_MyBlockList.IndexOf(changeBlocks[i]),newUpdatedBlocks[i]);
            column.RemoveFromMyBlockList(changeBlocks[i]);
            changeBlocks[i].m_ParticleController.PlayRenewParticle(changeBlocks[i].transform.position);
            changeBlocks[i].ResetBlock();
        }
        
        foreach (Block block in column.m_MyBlockList)
        {
            block.OnOffPhysics(true);
        }
        
        SaveColumnsData();
    }
    
    int GetOnGetHowManyBlockOverAndOverNeededToBlastAtLeast()
    {
        return m_HowManyBlockOverAndOverNeededToBlastAtLeast;
    }
    public List<Block> GetPossibleBlockTypes()
    {
        List<Block> possibleBlocks = new List<Block>();
        List<BlockType> unlockedBlockTypes = new List<BlockType>();
        foreach (Column column in m_ColumnsOnScene)
        {
            foreach (Block block in m_BlockTypesLevels[column.m_TowerLevel].Blocks)
            {
                if (!unlockedBlockTypes.Contains(block.m_CurrentBlockType))
                {
                    unlockedBlockTypes.Add(block.m_CurrentBlockType);
                }
            }
        }

        foreach (Block block in m_AllBlockTypes)
        {
            foreach (BlockType unlockedBlockType in unlockedBlockTypes)
            {
                if (block.m_CurrentBlockType == unlockedBlockType)
                {
                    if (!possibleBlocks.Contains(block))
                    {
                        possibleBlocks.Add(block);
                    }
                }
            }
        }

        return possibleBlocks;
    }

    int GetUnlockedColumnsAmount()
    {
        int amount = 0;
        foreach (var column in m_ColumnsOnScene)
        {
            if (column.m_IsUnlocked)
            {
                amount++;
            }
        }

        if (amount != 0)
        {
            amount--;
        }

        return amount;
    }

    void SaveColumnsData()
    {
        GameData gameData = GetGameData.Invoke();
        gameData.towerLevels.Clear();
        gameData.towerCurrentBlocks.Clear();
        for (int i = 0; i < m_ColumnsOnScene.Count; i++)
        {
            List<BlockType> blockNames = new List<BlockType>();
            foreach (Block block in m_ColumnsOnScene[i].m_MyBlockList)
            {
                blockNames.Add(block.m_CurrentBlockType);
            }
            gameData.towerCurrentBlocks.Add(blockNames);
            gameData.towerLevels.Add(m_ColumnsOnScene[i].m_TowerLevel);
        }
        OnSaveColumns?.Invoke();
    }

    bool LoadColumnsData()
    {
        GameData gameData = GetGameData.Invoke();
        if (gameData.towerCurrentBlocks.Count != 0 && gameData.towerLevels.Count != 0)
        {
            return true;
        }

        return false;
    }

    private GameObject GetBlockPrefab(Block _Block)
    {
        foreach (GameObject prefab in m_BlockPrefabs)
        {
            if (prefab.name == _Block.name)
            {
                return prefab;
            }
        }

        return null;
    }
    
    void TutorialEndJob()
    {
        foreach (Column column in m_ColumnsOnScene)
        {
            if (!column.m_IsUnlocked)
            {
                column.UnlockPanelOnOff(true);
                column.UpgradePanelOnOff(false);
            }
            else
            {
                column.UnlockPanelOnOff(false);
                column.UpgradePanelOnOff(true);
            }
        }
    }
}

[Serializable]
public class BlockList
{
    public List<Block> Blocks = new List<Block>();
}
