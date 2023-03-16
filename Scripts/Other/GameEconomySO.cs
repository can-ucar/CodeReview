using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameEconomy")]
public class GameEconomySO : ScriptableObject
{
    public List<int> m_TowerUpgradeCosts = new List<int>();
    [Space(5)] 
    public List<int> m_NewTowerUnlockCosts = new List<int>();

    [Space(5)] 
    public List<int> m_NewTowerLevels = new List<int>();
    
    [Space(5)] 
    public List<OreCoins> m_OreCoins = new List<OreCoins>();

    [Space(10)] public float m_Friction;
    public float m_BaseCost;


    public int GetTowerUpgradeCost(int _TowerLevel) //Tower level index starts from 0
    {
        int upgradeCost;
        if (_TowerLevel < m_TowerUpgradeCosts.Count)
        {
            upgradeCost = m_TowerUpgradeCosts[_TowerLevel];
        }
        else
        {
            upgradeCost = GetCostByFormula(_TowerLevel);
        }

        return upgradeCost;
    }
    
    public int GetNewTowerCostCost(int _TowerLevel) //New tower index starts from 0
    {
        int upgradeCost;
        if (_TowerLevel < m_NewTowerUnlockCosts.Count)
        {
            upgradeCost = m_NewTowerUnlockCosts[_TowerLevel];
        }
        else
        {
            upgradeCost = GetCostByFormula(_TowerLevel);
        }

        return upgradeCost;
    }

    public int GetOreCoinAmount(BlockType _BlockType)
    {
        int coin = 0;
        foreach (OreCoins oreCoin in m_OreCoins)
        {
            if (oreCoin._BlockType.Equals(_BlockType))
            {
                coin = oreCoin._Coin;
            }
        }

        return coin;
    }

    public int GetTowerCompleteCoinAmount(int _TowerLevel)
    {
        int completeCost;
        
        completeCost = GetCostByFormula(_TowerLevel);

        return completeCost;
    }


    private int GetCostByFormula(int _Index)
    {
        return Mathf.RoundToInt(m_BaseCost * Mathf.Exp(m_Friction * _Index));
    }
    
    [System.Serializable]
    public struct OreCoins
    {
        public BlockType _BlockType;
        public int _Coin;
    }
    

#if UNITY_EDITOR
    [ContextMenu("Setup Tower Upgrade Cost By Formula")]
    public void SetupTowerUpgradeByFormula()
    {
        if(m_TowerUpgradeCosts.Count == 0) return;
        for (int i = 0; i < m_TowerUpgradeCosts.Capacity; i++)
        {
            int cost = GetCostByFormula(i);
            m_TowerUpgradeCosts[i] = cost;
        }
    }
    
    [ContextMenu("Setup New Tower Unlock Cost By Formula")]
    public void SetupNewTowerUnlockByFormula()
    {
        if(m_NewTowerUnlockCosts.Count == 0) return;
        for (int i = 0; i < m_NewTowerUnlockCosts.Capacity; i++)
        {
            int cost = GetCostByFormula(i);
            m_NewTowerUnlockCosts[i] = cost;
        }
    }
#endif
}
