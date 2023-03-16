using System;
using AnotherWorld.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerUpgradeUI : MonoBehaviour
{
    [SerializeField] private Column m_Tower;
    [SerializeField] private TextMeshProUGUI m_LevelText;
    [SerializeField] private NumberCounter m_CoinText;
    [SerializeField] private Button m_Button;
    [SerializeField] private CanvasGroup m_Opacity;
    
    public static event Action<Column> OnTowerUpgrade;
    public static event Func<int, bool> OnCheckIsBuyable;

    public static event Action OnUpgradeable;

    private void OnEnable()
    {
        UIManager.OnCoinChanged += SetCoinLevelText;
    }

    private void OnDisable()
    {
        UIManager.OnCoinChanged -= SetCoinLevelText;
    }

    public void SetCoinLevelText()
    {
        int amount = GameEconomySingleton.Instance.GetGameEconomy().GetTowerUpgradeCost(m_Tower.m_TowerLevel);
        m_CoinText.Value = amount;
        m_LevelText.text = "lv " + (m_Tower.m_TowerLevel+1);
        if (OnCheckIsBuyable.Invoke(amount))
        {
            m_Button.interactable = true;
            m_Opacity.alpha = 1;
            OnUpgradeable?.Invoke();
        }
        else
        {
            m_Button.interactable = false;
            m_Opacity.alpha = .4f;
        }
    }
    

    public void UpgradeAttempt()
    {
        OnTowerUpgrade?.Invoke(m_Tower);
    }
}
