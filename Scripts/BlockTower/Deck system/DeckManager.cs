using System;
using System.Collections;
using System.Collections.Generic;
using AnotherWorld.UI;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeckManager : MonoBehaviour
{
    [Header("Implements")] 
    private PoolScript m_Pool;
    [SerializeField] private List<DeckSlot> m_DeckSlots = new List<DeckSlot>();
    [SerializeField] private ShapeTemplate m_ShapeTemplatePrefab;
    public List<ShapeList> m_AutoShapeListsByIndex = new List<ShapeList>();
    private AutoShape m_LastAutoShape;
    
    [Space(5)]
    public List<Shape> m_ManualShapes = new List<Shape>();

    [Header("Checks")] 
    [SerializeField] public bool m_AutoEnabled;

    [Header("Settings")] 
    public int m_TripleWaveIndex;
    private int m_HowManyShapePlacedInWave = 0;
    private int m_HowManyShapePlacedInTotal = 0;

    
    public static event Action<int> OnSaveIndex;
    public static event Func<int> OnGetUnlockedColumnsAmount; 

    private void OnEnable()
    {
        ShapeTemplate.OnShapePlaced += SpawnNewShapeWave;
        StartPanel.OnGetMiddleSlot += GetDeckSlotPos;
    }

    private void OnDisable()
    {
        ShapeTemplate.OnShapePlaced -= SpawnNewShapeWave;
        StartPanel.OnGetMiddleSlot -= GetDeckSlotPos;
    }

    private void Awake()
    {
        m_Pool = PoolScript.Instance;
    }

    [ContextMenu("Initialize")]
    public void Initialize(int tripleWaveIndexSave)
    {
        m_TripleWaveIndex = tripleWaveIndexSave;
        int startIndex = m_TripleWaveIndex * 3;
        int endIndex = startIndex + 3;

        if (startIndex >= m_ManualShapes.Count)
        {
            m_AutoEnabled = true;
        }
        
        if (!m_AutoEnabled)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                DeckSlot deckSlot = m_DeckSlots[i % 3];
                ShapeTemplate shapeTemplate = Instantiate(m_ShapeTemplatePrefab,deckSlot.transform);
                //shapeTemplate.transform.parent = deckSlot.transform;
                Transform shape = m_ManualShapes[i].Initialize(shapeTemplate.transform, deckSlot, shapeTemplate).transform;
                shape.transform.parent = shapeTemplate.transform;
                shape.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
                shape.transform.localPosition = Vector3.zero;
            }
        }
        else
        {
            if (m_LastAutoShape)
            {
                for (int i = 0; i < 3; i++)
                {
                    AutoShape nextAutoShape=m_AutoShapeListsByIndex[OnGetUnlockedColumnsAmount.Invoke()].AutoShapes[0];
                    foreach (AutoShape autoShape in m_AutoShapeListsByIndex[OnGetUnlockedColumnsAmount.Invoke()].AutoShapes)
                    {
                        if (autoShape.name != m_LastAutoShape.name)
                        {
                            nextAutoShape = autoShape;
                            m_LastAutoShape = autoShape;
                        }
                    }
                    
                    DeckSlot deckSlot = m_DeckSlots[i];
                    ShapeTemplate shapeTemplate = Instantiate(m_ShapeTemplatePrefab,deckSlot.transform);
                    //shapeTemplate.transform.parent = deckSlot.transform;
                    Transform shape = m_ManualShapes[i].AutoInitialize(shapeTemplate.transform, deckSlot, shapeTemplate,nextAutoShape.m_BlockType,nextAutoShape.m_Size).transform;
                    shape.transform.parent = shapeTemplate.transform;
                    shape.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
                    shape.transform.localPosition = Vector3.zero;
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    AutoShape nextAutoShape= m_AutoShapeListsByIndex[OnGetUnlockedColumnsAmount.Invoke()].AutoShapes[Random.Range(0,m_AutoShapeListsByIndex[OnGetUnlockedColumnsAmount.Invoke()].AutoShapes.Count)];
                    DeckSlot deckSlot = m_DeckSlots[i];
                    ShapeTemplate shapeTemplate = Instantiate(m_ShapeTemplatePrefab,deckSlot.transform);
                    //shapeTemplate.transform.parent = deckSlot.transform;
                    Transform shape = m_ManualShapes[i].AutoInitialize(shapeTemplate.transform, deckSlot, shapeTemplate,nextAutoShape.m_BlockType,nextAutoShape.m_Size).transform;
                    shape.transform.parent = shapeTemplate.transform;
                    shape.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
                    shape.transform.localPosition = Vector3.zero;
                }
            }
        }
    }
    
    void SpawnNewShapeWave()
    {
        m_HowManyShapePlacedInWave++;
        m_HowManyShapePlacedInTotal++;
        if (m_HowManyShapePlacedInWave == 3)
        {
            if (!m_AutoEnabled)
            {
                m_TripleWaveIndex++;
                OnSaveIndex?.Invoke(m_TripleWaveIndex);
            }
            m_HowManyShapePlacedInWave = 0;
            Initialize(m_TripleWaveIndex);
        }
    }
    
    Vector3 GetDeckSlotPos()
    {
        foreach (DeckSlot deckSlot in m_DeckSlots)
        {
            if (deckSlot.name.Equals("MiddleSlot"))
            {
                return transform.position;
            }
        }
        return Vector3.zero;
    }
}

[Serializable]
public class ShapeList
{
    public List<AutoShape> AutoShapes = new List<AutoShape>();
}
