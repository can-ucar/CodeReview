using System;
using AnotherWorld.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerUnlockUI : MonoBehaviour
{
    [SerializeField] private Column m_Tower;
    [SerializeField] private NumberCounter m_CoinText;
    [SerializeField] private Button m_Button;
    [SerializeField] private CanvasGroup m_Opacity;
    
    public static event Action<Column> OnTowerUnlock;
    public static event Func<int, bool> OnCheckIsBuyable;

    private void OnEnable()
    {
        UIManager.OnCoinChanged += SetCoinText;
    }

    private void OnDisable()
    {
        UIManager.OnCoinChanged -= SetCoinText;
    }

    public void SetCoinText()
    {
        int amount = GameEconomySingleton.Instance.GetGameEconomy().GetNewTowerCostCost(m_Tower.UnlockedStartIndex);
        m_CoinText.Value = amount;
        if (OnCheckIsBuyable.Invoke(amount))
        {
            m_Button.interactable = true;
            m_Opacity.alpha = 1;
        }
        else
        {
            m_Button.interactable = false;
            m_Opacity.alpha = .4f;
        }
    }

    public void UnlockAttempt()
    {
        OnTowerUnlock?.Invoke(m_Tower);
    }
}
