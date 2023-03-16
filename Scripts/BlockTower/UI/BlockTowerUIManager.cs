using System;
using System.Collections;
using System.Collections.Generic;
using AnotherWorld.Core;
using UnityEngine;

public class BlockTowerUIManager : UIManager
{
    [SerializeField] private ComboPanel m_ComboPanel;
    public static event Action<int, Column> OnTowerUpgrade; 
    public static event Action<int, Column> OnTowerUnlock; 

    private void OnEnable()
    {
        TowerUnlockUI.OnTowerUnlock += TowerUnlockAttempt;
        TowerUpgradeUI.OnTowerUpgrade += TowerUpgradeAttempt;
        CoinManager.OnCombo += CallCombo;
    }

    private void OnDisable()
    {
        TowerUnlockUI.OnTowerUnlock -= TowerUnlockAttempt;
        TowerUpgradeUI.OnTowerUpgrade -= TowerUpgradeAttempt;
        CoinManager.OnCombo -= CallCombo;
    }

    public override void Initialize(bool showTutorial, GameData gameData)
    {
        base.Initialize(showTutorial, gameData);
        if (showTutorial)
        {
            m_TutorialPanel.Initialize(this,gameData);
        }
    }

    void TowerUnlockAttempt(Column column)
    {
        int amount = GameEconomySingleton.Instance.GetGameEconomy().GetNewTowerCostCost(column.m_TowerLevel);
        OnTowerUnlock?.Invoke(amount,column);
    }
    void TowerUpgradeAttempt(Column column)
    {
        int amount = GameEconomySingleton.Instance.GetGameEconomy().GetTowerUpgradeCost(column.m_TowerLevel);
        OnTowerUpgrade?.Invoke(amount,column);
    }

    void CallCombo(Vector3 position, int combo, int reward, int multi)
    {
        m_ComboPanel.EmitComboUI(position,combo,reward,multi);
    }
    
    public void TutorialEnd()
    {
        ShowHideSettingsPanel(true);
        //ShowHideRestartPanel(true);
        ShowHideLevelWinPanel(false);
        ShowHideLevelFailPanel(false);
    }
}
