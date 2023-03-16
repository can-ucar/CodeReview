using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private GameEconomySO m_GameEconomy;

    public static event Action<int> OnSetCoin; 
    
    public static event Action<Vector3, int, int, int> OnCombo; 
    private void OnEnable()
    {
        Column.OnBlast += CalculateBlastEarn;
        Column.OnColumnCompleted += CalculateTowerCompleteEarn;
    }

    private void OnDisable()
    {
        Column.OnBlast += CalculateBlastEarn;
        Column.OnColumnCompleted += CalculateTowerCompleteEarn;
    }

    void CalculateBlastEarn(List<Block> blastedBlocks, int blastComboCount)
    {
        int rawCalculatedValue = 0;
        foreach (Block blastedBlock in blastedBlocks)
        {
            int blockCoinValue = m_GameEconomy.GetOreCoinAmount(blastedBlock.m_CurrentBlockType);
            FloatingMoney3D.Instance.Create3DFloatingMoneyCall( (int)(Mathf.Pow(blockCoinValue, 0.1f) * 2f), blastedBlock.transform.position);
            rawCalculatedValue += blockCoinValue;
        }
        int finalCalculatedValue = rawCalculatedValue * blastComboCount;

        Vector3 comboPos = blastedBlocks[blastedBlocks.Count / 2 > 0 ? blastedBlocks.Count / 2 : 0].transform.position;

        if (blastComboCount > 1)
        {
            OnCombo?.Invoke(comboPos,blastComboCount,finalCalculatedValue,blastComboCount);
        }
        
        SetValueToCoinSystemAndSave(finalCalculatedValue);
    }

    void CalculateTowerCompleteEarn(int _TowerLevel,Vector3 pos)
    {
        int finalCalculatedValue = m_GameEconomy.GetTowerCompleteCoinAmount(_TowerLevel);
        FloatingMoney3D.Instance.Create3DFloatingMoneyCall(20, pos);
        SetValueToCoinSystemAndSave(finalCalculatedValue);
        //OnCombo?.Invoke(pos,1,finalCalculatedValue,3);
    }

    void SetValueToCoinSystemAndSave(int value)
    {
        OnSetCoin?.Invoke(value);
    }
}
