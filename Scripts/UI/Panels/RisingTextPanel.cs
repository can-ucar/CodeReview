using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingTextPanel : MonoBehaviour
{
    [SerializeField] private RisingTextUI m_RisingTextUIPrefab;
    private string m_RisingTextUIName;
    private PoolScript m_Pool => PoolScript.Instance;

    private void Awake()
    {
        m_RisingTextUIName = m_RisingTextUIPrefab.name;
    }

    public void EmitText(int value, Vector3 position)
    {
        m_Pool.GetPooledObject(m_RisingTextUIName).GetComponent<RisingTextUI>().Initialize(value, position);
    }
}
