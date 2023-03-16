using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Column : MonoBehaviour
{
    [Header("Implements")] 
    public List<BlockType> m_ManualBlockCreationList = new List<BlockType>(); // its for manual block creation if its size 0 it will be auto created
    public List<Block> m_MyBlockList = new List<Block>(); //its actual blocks list all placed ones
    public List<Block> m_MyGhostBlocksList = new List<Block>(); //its for preview ghost block order list
    public List<Block> blocksToBlast = new List<Block>();
    [SerializeField] private Transform m_MySpawnPos; //column spawn pos transform
    [SerializeField] private Transform m_MyBlocksParent; //column blocks parent
    public List<Block> m_LastInteractedBlocks = new List<Block>(); //column's last interacted block while dragging blocks
    private Block m_LastCheckedBlockForBlast; //as its own name
    public ShapeColumn m_LastInteractedShapeColumn;
    private List<Vector3> m_InitialBlocksOrder = new List<Vector3>();

    [Space(5)] 
    [SerializeField] private GameObject m_UnlockUI;
    [SerializeField] private GameObject m_UpgradeUI;
    
    [Space(5)] 
    public Collider m_MyCollider;

    [Header("Checks")]
    public bool m_IsPreview; //is column shows preview that like how its gonna be look like its checkbox
    public int m_BlastComboCount;
    public bool m_IsCompleted;
    public bool m_IsUnlocked;
    public bool m_IsColumnBusy;
    private bool m_IsColumnExploding;
    public bool m_IsOrganizingBlocks;
    private bool m_IsColumnOrganized;
    
    

    [Header("Settings")] 
    public int m_HowManyBlockItWillHave; //its column's max block limit
    public float power = 10.0F;
    public float radius = 1f;
    public float upwardsUp;

    [Space(5)] 
    public int m_TowerLevel = 0;
    public int UnlockedStartIndex = 0;

    // / Actions
    public static Action<Column> OnGetNewBlocks;
    public static Action OnNewBlocksAdded;
    public static Action<int,Vector3> OnColumnCompleted;
    public static event Action<List<Block>, int> OnBlast;
    public static event Action OnUpgrade;// tutorial
    public static event Action<Column> OnUpgradeBlocks;
    public static event Action OnUnlock;// tutorial
    public static event Action OnBlockAdded; // tutorial

    

    // / Functions
    public static Func<int> OnGetHowManyBlockOverAndOverNeededToBlastAtLeast;

    private void OnEnable()
    {
        ShapeTemplate.OnShapePlaced += BlockPlaced;
    }

    private void OnDisable()
    {
        ShapeTemplate.OnShapePlaced -= BlockPlaced;
    }

    private void BlockPlaced()
    {
        m_IsPreview = false;
        ClearPreviewLists();
        m_LastInteractedBlocks.Clear();
        if (!m_IsOrganizingBlocks)
        {
            OrganizeBlocks();
        }

        m_LastInteractedShapeColumn = null;
    }

    private void Update()
    {
        PreviewPlacingMode();
    }

    public void AddMyListForCreation(Block block)
    {
        StartCoroutine(AddMyListForCreationIE(block));
    }

    IEnumerator AddMyListForCreationIE(Block block)
    {
        m_IsColumnBusy = true;
        AddToMyBlockList(block);
        m_InitialBlocksOrder.Add(block.transform.position);
        
        float blockGap = m_MyBlockList[0].m_BlockGap;
        float spawnPosY = m_MySpawnPos.position.y;

        for (int i = 0; i < m_MyBlockList.Count; i++)
        {
            if (!m_MyBlockList[i].m_IsPlaced)
            {
                m_MyBlockList[i].m_IsPlaced = true;
            }
            if (i == 0)
            {
                m_MyBlockList[i].transform.position = new Vector3(m_MySpawnPos.transform.position.x, spawnPosY, m_MySpawnPos.transform.position.z);
            }
            else
            {
                m_MyBlockList[i].transform.position = new Vector3(m_MySpawnPos.transform.position.x, spawnPosY + (i * blockGap), m_MySpawnPos.transform.position.z);
            }
            m_MyBlockList[i].OnOffPhysics(true);
            if (m_MyBlockList[i].m_ParticleController.m_ChestAnimator)
            {
                m_MyBlockList[i].m_ParticleController.m_ChestAnimator.Play("idle");
            }
            //yield return new WaitForSeconds(0.1f);
        }

        m_IsColumnBusy = false;
        block.transform.parent = m_MyBlocksParent;
        yield break;
    }
    
    public void AddMyListForPrePlacing(Block interactedBlock)
    {
        if (!m_LastInteractedShapeColumn)
        {
            m_LastInteractedShapeColumn = interactedBlock.m_MyColumn;
        }

        if (interactedBlock.m_MyColumn != m_LastInteractedShapeColumn)
        {
            return;
        }
        
        if (!m_LastInteractedBlocks.Contains(interactedBlock))
        {
            m_LastInteractedBlocks.Add(interactedBlock);
        }
    }

    public void ClearPreviewLists()
    {
        m_MyGhostBlocksList.Clear();
        //m_NewGhostBlocks.Clear();
    }

    public List<Block> GetLastInteractedBlocksList()
    {
        return m_LastInteractedBlocks;
    }

    public void OrganizeBlocks()
    {
        if (!m_IsColumnOrganized)
        {
            StartCoroutine(OrganizeBlocksIE());
        }
    }

    IEnumerator OrganizeBlocksIE()
    {
        yield return new WaitUntil(CheckColumnIsStillBusy);
        Debug.Log(name + " Organizing");
        m_IsColumnBusy = true;
        m_IsOrganizingBlocks = true;
        if (m_MyBlockList.Count <= 1)
        {
            m_IsColumnBusy = false;
            m_IsOrganizingBlocks = false;
            yield break;
        }
        //m_MyBlockList = m_MyBlockList.OrderBy(gameObject => gameObject.transform.position.y).ToList();
        m_MyBlockList.Sort(((block, block1) => block.transform.position.y.CompareTo(block1.transform.position.y)));

        Block starBlock=null;
        foreach (Block block in m_MyBlockList)
        {
            if (block.m_CurrentBlockType == BlockType.Chest)
            {
                starBlock = block;
            }
        }

        RemoveFromMyBlockList(starBlock);
        AddToMyBlockList(starBlock);
        
        foreach (var block in m_MyBlockList)
        {
            block.m_IsOnProcess = true;
        }
        
        float blockGap = m_MyBlockList[0].m_BlockGap;
        float spawnPosY = m_MySpawnPos.position.y;

        for (int i = 0; i < m_MyBlockList.Count; i++)
        {
            Block block = m_MyBlockList[i];
            if (!block.m_IsPlaced)
            {
                block.m_IsPlaced = true;
            }
            if (i == 0)
            {
                //block.transform.position = new Vector3(m_MySpawnPos.transform.position.x, spawnPosY, m_MySpawnPos.transform.position.z);
                block.transform.DOMove(new Vector3(m_MySpawnPos.transform.position.x, spawnPosY, m_MySpawnPos.transform.position.z), 0.08f).SetId(777);
            }
            else
            {
                //block.transform.position = new Vector3(m_MySpawnPos.transform.position.x, spawnPosY + (i * blockGap), m_MySpawnPos.transform.position.z);
                block.transform.DOMove(new Vector3(m_MySpawnPos.transform.position.x, spawnPosY + (i * blockGap), m_MySpawnPos.transform.position.z), 0.08f).SetId(777);
            }
            block.transform.parent = m_MyBlocksParent;
            block.OnOffMyGhost(false);
            //yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.2f);
        foreach (var block in m_MyBlockList)
        {
            block.m_IsOnProcess = false;
            block.OnOffPhysics(true);
        }
        DOTween.Complete(777);
        m_IsColumnOrganized = true;
        m_IsColumnBusy = false;
        m_IsOrganizingBlocks = false;
        
    }

    public void PlaceBlocks(List<Block> _BlocksToPlace)
    {
        //if (m_LastInteractedBlocks.Count != 0)
        //{
            m_IsColumnBusy = true;
            m_BlastComboCount = 0;
            List<Block> toBeRemoved = new List<Block>();
            foreach (Block lastInteractedBlock in m_LastInteractedBlocks)
            {
                //lastInteractedBlock.SetPlaced();
                //toBeRemoved.Add(lastInteractedBlock);
                //BlastSequenceCall();
            }
            foreach (Block block in toBeRemoved)
            {
                //m_LastInteractedBlocks.Remove(block);
            }

            m_LastInteractedBlocks = new List<Block>();
            //m_MyBlockList.Clear();
            foreach (Block block in _BlocksToPlace)
            {
                AddToMyBlockList(block);
            }
            m_IsPreview = false;
            ClearPreviewLists();
            m_LastInteractedShapeColumn = null;
            m_IsColumnBusy = false;
            BlastSequenceCall(_BlocksToPlace, false);
            OnNewBlocksAdded?.Invoke();
            OnBlockAdded?.Invoke();
            SetBlockPositionOrder();
        //}
    }

    private void SetBlockPositionOrder()
    {
        if(m_MyBlockList.Count == 0) return;
        float blockGap = m_MyBlockList[0].m_BlockGap;
        float spawnPosY = m_MySpawnPos.position.y;
        
        m_InitialBlocksOrder = new List<Vector3>();
        for (int i = 0; i < m_MyBlockList.Count; i++)
        {
            m_InitialBlocksOrder.Add(new Vector3(m_MySpawnPos.transform.position.x, spawnPosY + blockGap * i, m_MySpawnPos.transform.position.z));
        }
    }

    private Coroutine m_BlastCoroutine;
    
    public void BlastSequenceCall(List<Block> _BlocksToPlace, bool _IsCombo)
    {
        Debug.Log(name + " blast called");
        m_BlastCoroutine = StartCoroutine(BlastSequence(_BlocksToPlace, _IsCombo));
    }

    public void StopBlastSequence()
    {
        StopCoroutine(m_BlastCoroutine);
    }

    IEnumerator BlastSequence(List<Block> _BlocksToPlace, bool _IsCombo)
    {
        OrganizeBlocks();
        
        yield return new WaitUntil(CheckColumnIsStillBusy);
        
        if(!_IsCombo) StartCoroutine(CheckBlastIE(_BlocksToPlace));
        else StartCoroutine(CheckComboBlastIE(_BlocksToPlace, 1));


        yield return new WaitUntil(CheckColumnIsStillBusy);
        
        /*GetNewBlocksAfterBlast();
        
        yield return new WaitUntil(CheckColumnIsStillBusy);*/

        CheckColumnIsCompletedCall();

        yield return new WaitUntil(CheckColumnIsStillBusy);
        
    }

    IEnumerator CheckBlastIE(List<Block> _BlocksToPlace)
    {
        blocksToBlast.Clear();
        m_IsColumnBusy = true;
        Debug.Log(name + " Checking Blast");
        int HowManyBlockOverAndOverNeededToBlastAtLeast = OnGetHowManyBlockOverAndOverNeededToBlastAtLeast.Invoke();
        
        m_LastCheckedBlockForBlast = m_MyBlockList[0];
        blocksToBlast.Add(m_MyBlockList[0]);
        for (int i = 1; i < m_MyBlockList.Count; i++) //Instead of looping all elements, go down and up from placed index
        {
            Block block = m_MyBlockList[i];

            if (m_LastCheckedBlockForBlast.m_CurrentBlockType == block.m_CurrentBlockType)
            {
                blocksToBlast.Add(block);
            }
            else
            {
                if (blocksToBlast.Count >= HowManyBlockOverAndOverNeededToBlastAtLeast)
                {
                    m_BlastComboCount++;
                    OnBlast?.Invoke(blocksToBlast,m_BlastComboCount);
                    foreach (Block blastBlock in blocksToBlast)
                    {
                        RemoveFromMyBlockList(blastBlock);
                        blastBlock.m_ParticleController.PlayBlastParticle(blastBlock.transform.position);
                        blastBlock.ResetBlock();
                        //yield return new WaitForSeconds(0.1f);
                    }
                    
                    StartCoroutine(BlastAfterExplosion(block));
                    
                    yield return new WaitUntil(CheckBlastExplosionIsFinished);
                    
                    if (m_MyBlockList.Count > 1)
                    {
                        StopBlastSequence();
                        m_IsColumnBusy = false;
                        BlastSequenceCall(_BlocksToPlace,true); //Send last blast index and check from there
                        yield break;
                    }
                    m_IsColumnBusy = false;
                    yield break;
                }
                
                blocksToBlast.Clear();
                blocksToBlast.Add(block);
                m_LastCheckedBlockForBlast = block;
            }
        }

        yield return new WaitForEndOfFrame();
        m_LastCheckedBlockForBlast = null;
        OnNewBlocksAdded?.Invoke();
        m_IsColumnBusy = false;
    }
    
    
    IEnumerator CheckComboBlastIE(List<Block> _BlocksToPlace, int _BlockIndex)
    {
        blocksToBlast.Clear();
        m_IsColumnBusy = true;
        Debug.Log(name + " Checking Blast");
        int HowManyBlockOverAndOverNeededToBlastAtLeast = OnGetHowManyBlockOverAndOverNeededToBlastAtLeast.Invoke();
        
        m_LastCheckedBlockForBlast = m_MyBlockList[0];
        blocksToBlast.Add(m_MyBlockList[0]);
        for (int i = 1; i < m_MyBlockList.Count; i++) //Instead of looping all elements, go down and up from placed index
        {
            Block block = m_MyBlockList[i];

            if (m_LastCheckedBlockForBlast.m_CurrentBlockType == block.m_CurrentBlockType)
            {
                blocksToBlast.Add(block);
            }
            else
            {
                if (blocksToBlast.Count >= HowManyBlockOverAndOverNeededToBlastAtLeast)
                {
                    yield return StartCoroutine(HighlightBlocks(blocksToBlast));

                    m_BlastComboCount++;
                    OnBlast?.Invoke(blocksToBlast,m_BlastComboCount);
                    foreach (Block blastBlock in blocksToBlast)
                    {
                        RemoveFromMyBlockList(blastBlock);
                        blastBlock.m_ParticleController.PlayBlastParticle(blastBlock.transform.position);
                        blastBlock.ResetBlock();
                        //yield return new WaitForSeconds(0.1f);
                    }
                    
                    StartCoroutine(BlastAfterExplosion(block));
                    
                    yield return new WaitUntil(CheckBlastExplosionIsFinished);
                    
                    if (m_MyBlockList.Count > 1)
                    {
                        StopBlastSequence();
                        m_IsColumnBusy = false;
                        BlastSequenceCall(_BlocksToPlace, true); //Send last blast index and check from there
                        yield break;
                    }
                    m_IsColumnBusy = false;
                    yield break;
                }
                
                blocksToBlast.Clear();
                blocksToBlast.Add(block);
                m_LastCheckedBlockForBlast = block;
            }
        }

        yield return new WaitForEndOfFrame();
        m_LastCheckedBlockForBlast = null;
        OnNewBlocksAdded?.Invoke();
        m_IsColumnBusy = false;
    }

    IEnumerator HighlightBlocks(List<Block> _BlocksToBlast)
    {
        float highlightDuration = 0.05f;
        int highlightAmount = 2;
        float unitDuration = 0.1f;

        for (int i = 0; i < highlightAmount; i++)
        {
            foreach (Block block in _BlocksToBlast)
            {
                block.Highlight(true);
            }
            yield return new WaitForSeconds(unitDuration);
            
            foreach (Block block in _BlocksToBlast)
            {
                block.Highlight(false);
            }
            yield return new WaitForSeconds(highlightDuration);

            unitDuration *= 0.8f;
            highlightDuration *= 0.8f;
            //transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    IEnumerator BlastAfterExplosion(Block lastBlock)
    {
        m_IsColumnExploding = true;
        Vector3 explosionPos = lastBlock.transform.position;
        float blastMultipler = 1;
        foreach (Block block in m_MyBlockList)
        {
            block.OnOffPhysics(true);
        }
        for (int i = 0; i < m_MyBlockList.Count; i++)
        {
            Block block = m_MyBlockList[i];

            if (block == lastBlock)
            {
                Rigidbody rbUP = m_MyBlockList[i].m_Rigidbody;

                switch (m_BlastComboCount)
                {
                    case 1:
                        foreach (Block blockq in m_MyBlockList)
                        {
                            blockq.m_Rigidbody.drag = 0.2f;
                            blastMultipler = 1;
                        }
                        break;
                    case 2:
                        foreach (Block blockq in m_MyBlockList)
                        {
                            blockq.m_Rigidbody.drag = 0.4f;
                            blastMultipler = 1.3f;
                        }
                        break;
                    case 3:
                        foreach (Block blockq in m_MyBlockList)
                        {
                            blockq.m_Rigidbody.drag = 0.5f;
                            blastMultipler = 1.5f;
                        }
                        break;
                }
                
                rbUP.AddExplosionForce((power * blastMultipler) * (m_MyBlockList.Count - i), explosionPos, radius, upwardsUp,ForceMode.Impulse);
            }
        }

        yield return new WaitUntil(CheckAllBlocksFalling);
        
        foreach (Block block in m_MyBlockList)
        {
            block.m_Rigidbody.drag = 0f;
            block.m_Rigidbody.velocity *= blastMultipler;
        }
        
        //yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(CheckAllBlocksFellv1);
        m_IsColumnExploding = false;
    }

    bool CheckColumnIsStillBusy()
    {
        return !m_IsColumnBusy;
    }

    bool CheckAllBlocksFellv1()
    {
        int blockCount = m_MyBlockList.Count;
        int fellBlocksAmount = 0;
        foreach (Block block in m_MyBlockList)
        {
            if (block.m_Rigidbody.velocity.y > -0.1f)
            {
                fellBlocksAmount++;
            }
        }
        if (fellBlocksAmount == blockCount)
        {
            return true;
        }

        return false;
    }
    
    bool CheckAllBlocksFellv2()
    {
        int blockCount = m_MyBlockList.Count;
        int fellBlocksAmount = 0;
        foreach (Block block in m_MyBlockList)
        {
            if (block.m_Rigidbody.IsSleeping())
            {
                fellBlocksAmount++;
            }
        }
        if (fellBlocksAmount == blockCount)
        {
            return true;
        }

        return false;
    }
    
    bool CheckAllBlocksFalling()
    {
        int blockCount = m_MyBlockList.Count;
        int fellBlocksAmount = 0;
        foreach (Block block in m_MyBlockList)
        {
            if (block.m_Rigidbody.velocity.y < -2f | block.m_Rigidbody.IsSleeping())
            {
                fellBlocksAmount++;
            }
        }
        if (fellBlocksAmount == blockCount)
        {
            return true;
        }

        return false;
    }

    bool CheckBlastExplosionIsFinished()
    {
        return !m_IsColumnExploding;
    }

    void CheckColumnIsCompletedCall()
    {
        StartCoroutine(CheckColumnIsCompleted());
    }

    IEnumerator CheckColumnIsCompleted()
    {
        m_IsColumnBusy = true;
        yield return new WaitUntil(CheckAllBlocksFellv2);
        if (m_BlastComboCount >= 3)
        {
            FeverModeCall();
        }
        else
        {
            if (m_MyBlockList[0].m_CurrentBlockType == BlockType.Chest)
            {
                yield return new WaitForSeconds(.1f);
                if(m_MyBlockList.Count == 0) yield break;
                Block block = m_MyBlockList[0];
                RemoveFromMyBlockList(block);

                //chest anim start
                block.m_ParticleController.PlayChestParticleAndAnimation(block.transform.position);
                OnColumnCompleted?.Invoke(m_TowerLevel,block.transform.position);
                

                m_MyCollider.enabled = false;
                m_IsCompleted = true;
                
                
                yield return new WaitForSeconds(0.7f);
                block.gameObject.SetActive(false);
                block.ResetBlock();
                yield return new WaitForSeconds(.3f);
                OnGetNewBlocks?.Invoke(this);
            }
        }
        m_IsColumnBusy = false;
    }

    [ContextMenu("Fever Mode")]
    void FeverModeCall()
    {
        StartCoroutine(FeverMode());
    }

    IEnumerator FeverMode()
    {
        m_IsColumnBusy = true;
        Debug.Log(name + " in Fever Mode");
        int blocksAmount = m_MyBlockList.Count;
        foreach (Block block in m_MyBlockList)
        {
            block.OnOffPhysics(false);
            block.m_Rigidbody.drag = 0;
        }
        for (int i = 0; i < blocksAmount; i++)
        {
            if(m_MyBlockList.Count == 0) break;
            Block item = m_MyBlockList[0];
            if (item.m_CurrentBlockType == BlockType.Chest)
            {
                item.OnOffPhysics(true);
                yield return null;
                yield return new WaitUntil(CheckAllBlocksFellv2);
                item.m_ParticleController.PlayChestParticleAndAnimation(item.transform.position);
                OnColumnCompleted?.Invoke(m_TowerLevel,item.transform.position);
                yield return new WaitForSeconds(0.7f);
            }
            item.m_IsOnProcess = true;
            if (item.m_CurrentBlockType != BlockType.Chest)
            {
                List<Block> blastedBlock = new List<Block>();
                blastedBlock.Add(item);
                OnBlast?.Invoke(blastedBlock,m_BlastComboCount);
                item.m_ParticleController.PlayBlastParticle(item.transform.position);
            }
            item.ResetBlock();
            RemoveFromMyBlockList(item);
            
            if(m_MyBlockList.Count == 0) break;

            yield return new WaitForSeconds(.05f);
        }
        m_MyCollider.enabled = false;
        m_IsCompleted = true;
        yield return new WaitForSeconds(.3f);
        OnGetNewBlocks?.Invoke(this);
        m_IsColumnBusy = false;
    }

    public void FixColumn()
    {
        m_IsPreview = false;
        ClearPreviewLists();
        m_LastInteractedBlocks.Clear();
        if (!m_IsOrganizingBlocks)
        {
            OrganizeBlocks();
        }
    }

    void PreviewPlacingMode()
    {
        if (m_LastInteractedBlocks.Count ==0)
        {
            m_IsPreview = false;
            return;
        }
        
        if (m_IsPreview && !m_IsColumnBusy && m_LastInteractedBlocks[0].m_MyParent.m_ShapeTemplate.m_IsPlaceable)
        {
            //m_MyGhostBlocksList = m_MyGhostBlocksList.OrderBy(gameObject => gameObject.transform.position.magnitude).ToList();
            m_IsColumnOrganized = false;
            m_MyGhostBlocksList = new List<Block>();
            foreach (Block block in m_MyBlockList)
            {
                if (!m_MyGhostBlocksList.Contains(block))
                {
                    m_MyGhostBlocksList.Add(block);
                }
            }
            
            m_LastInteractedBlocks = m_LastInteractedBlocks.OrderBy(gameObject => gameObject.transform.position.y).ToList();


            int placeAfterThisIndex = m_InitialBlocksOrder.Count;
            for (int i = 0; i < m_InitialBlocksOrder.Count; i++)
            {
                if (m_InitialBlocksOrder[i].y > m_LastInteractedBlocks[0].transform.position.y)
                {
                    placeAfterThisIndex = i;
                    break;
                }
            }

            foreach (Block lastInteractedBlock in m_LastInteractedBlocks)
            {
                if (!m_MyGhostBlocksList.Contains(lastInteractedBlock))
                {
                    m_MyGhostBlocksList.Insert(Mathf.Clamp(placeAfterThisIndex, 0, m_MyGhostBlocksList.Count - 1),lastInteractedBlock);
                }
            }
            
            //m_MyGhostBlocksList.InsertRange(placeAfterThisIndex,m_LastInteractedBlocks);

            float blockGap = m_MyGhostBlocksList[0].m_BlockGap;
            float spawnPosY = m_MySpawnPos.position.y;
            
            
            for (int i = 0; i < m_MyGhostBlocksList.Count; i++)
            {
                Block block = m_MyGhostBlocksList[i];
                block.m_IsOnProcess = true;
                if (i==0)
                {
                    if (m_LastInteractedBlocks.Contains(block))
                    {
                        Vector3 pos = new Vector3(m_MySpawnPos.transform.position.x, spawnPosY, m_MySpawnPos.transform.position.z);
                        block.OnOffMyGhost(true,pos);
                        continue;
                    }

                    //block.transform.position = new Vector3(m_MySpawnPos.transform.position.x, spawnPosY, m_MySpawnPos.transform.position.z);
                    block.transform.position = Vector3.LerpUnclamped(block.transform.position, new Vector3(m_MySpawnPos.transform.position.x, spawnPosY, m_MySpawnPos.transform.position.z), 15f * Time.deltaTime);
                }
                else
                {
                    if (m_LastInteractedBlocks.Contains(block))
                    {
                        Vector3 pos = new Vector3(m_MySpawnPos.transform.position.x, spawnPosY + (i * blockGap), m_MySpawnPos.transform.position.z);
                        block.OnOffMyGhost(true,pos);
                        continue;
                    }
                    
                    //block.transform.position = new Vector3(m_MySpawnPos.transform.position.x, spawnPosY + (i * blockGap), m_MySpawnPos.transform.position.z);
                    block.transform.position = Vector3.LerpUnclamped(block.transform.position, new Vector3(m_MySpawnPos.transform.position.x, spawnPosY + (i * blockGap), m_MySpawnPos.transform.position.z), 15f * Time.deltaTime);
                }
                block.OnOffPhysics(false);
            }
        }
        /*else if (m_IsPreview && !m_IsColumnBusy && !m_LastInteractedBlocks[0].m_MyParent.m_ShapeTemplate.m_IsPlaceable)
        {
            FixColumn();
            m_IsPreview = false;
        }*/
    }

    public void InsertToMyBlockList(int _Index, Block _BlockToAdd)
    {
        m_MyBlockList.Insert(_Index, _BlockToAdd);
        SetBlockPositionOrder();
    }
    
    public void AddToMyBlockList(Block _BlockToAdd)
    {
        m_MyBlockList.Add(_BlockToAdd);
        SetBlockPositionOrder();
    }
    
    public void RemoveFromMyBlockList(Block _BlockToRemove)
    {
        m_MyBlockList.Remove(_BlockToRemove);
        SetBlockPositionOrder();
    }

    [ContextMenu("Unlock Tower")]
    public void UnlockTower()
    {
        m_TowerLevel = GameEconomySingleton.Instance.GetGameEconomy().m_NewTowerLevels[UnlockedStartIndex];
        UnlockPanelOnOff(false);
        UpgradePanelOnOff(true);
        m_IsUnlocked = true;
        OnGetNewBlocks?.Invoke(this);
        OnUnlock?.Invoke();
    }

    public void UpgradeTower()
    {
        StartCoroutine(UpgradeTowerIe());
    }

    IEnumerator UpgradeTowerIe()
    {
        yield return new WaitUntil(CheckColumnIsStillBusy);
        m_TowerLevel++;
        UnlockPanelOnOff(false);
        if (m_TowerLevel >= 13)
        {
            UpgradePanelOnOff(false);
        }
        else
        {
            UpgradePanelOnOff(true);
        }
        OnUpgradeBlocks?.Invoke(this);
        OnUpgrade?.Invoke();
    }

    public void UpgradePanelOnOff(bool on)
    {
        if (m_TowerLevel >= 13)
        {
            m_UpgradeUI.SetActive(false);
            return;
        }
        m_UpgradeUI.SetActive(on);
        if(on)m_UpgradeUI.GetComponent<TowerUpgradeUI>().SetCoinLevelText();
    }

    public void UnlockPanelOnOff(bool on)
    {
        if (m_UnlockUI)
        {
            m_UnlockUI.SetActive(on);
            if(on)m_UnlockUI.GetComponent<TowerUnlockUI>().SetCoinText();
        }
    }

    [ContextMenu("Rise")]
    public void RiseColumn(bool riseQuick = false)
    {
        m_IsColumnBusy = true;
        if (!riseQuick)
        {
            foreach (var block in m_MyBlockList)
            {
                block.OnOffPhysics(false);
            }
            transform.DOLocalMoveY(0, 0.6f).SetEase(Ease.Linear).OnComplete((() =>
            {
                foreach (var block in m_MyBlockList)
                {
                    block.OnOffPhysics(true);
                }
                m_IsColumnBusy = false;
                SetBlockPositionOrder();
            }));
            CameraShake.Shake(1f,0.005f);
        }
        else
        {
            foreach (var block in m_MyBlockList)
            {
                block.OnOffPhysics(false);
            }
            transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
            foreach (var block in m_MyBlockList)
            {
                block.OnOffPhysics(true);
            }
            m_IsColumnBusy = false;
        }
    }

    public void SendColumnUnderground()
    {
        m_IsColumnBusy = true;
        transform.DOLocalMoveY(-0.81f, 0f).SetEase(Ease.Linear).OnComplete((() =>
        {
            m_IsColumnBusy = false;
        }));
    }
    
}
