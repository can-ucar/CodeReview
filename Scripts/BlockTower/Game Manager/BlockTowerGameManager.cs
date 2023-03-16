using System.Collections;
using System.Collections.Generic;
using AnotherWorld.Core;
using AnotherWorld.InGameVFX;
using AnotherWorld.UI;
using UnityEngine;

public class BlockTowerGameManager : GameManager
{
    [SerializeField] private ColumnManager m_ColumnManager;
    [SerializeField] private DeckManager m_DeckManager;

    protected override void OnEnable()
    {
        base.OnEnable();
        ColumnManager.GetGameData += GetGameData;
        ShapeTemplate.GetGameData += GetGameData;
        ColumnManager.OnSaveColumns += SaveGameData;
        ColumnManager.OnSaveColumns += ColumnsUpdated;
        CoinManager.OnSetCoin += AddCoin;
        BlockTowerUIManager.OnTowerUnlock += NewTowerUnlockProcess;
        BlockTowerUIManager.OnTowerUpgrade += UpgradeTowerProcess;
        TowerUnlockUI.OnCheckIsBuyable += CheckPurchase;
        TowerUpgradeUI.OnCheckIsBuyable += CheckPurchase;
        DeckManager.OnSaveIndex += SaveTripleWaveIndex;
        StartPanel.OnTutorialDone += SetTutorialPassed;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ColumnManager.GetGameData -= GetGameData;
        ShapeTemplate.GetGameData -= GetGameData;
        ColumnManager.OnSaveColumns -= SaveGameData;
        ColumnManager.OnSaveColumns -= ColumnsUpdated;
        CoinManager.OnSetCoin -= AddCoin;
        BlockTowerUIManager.OnTowerUnlock -= NewTowerUnlockProcess;
        BlockTowerUIManager.OnTowerUpgrade -= UpgradeTowerProcess;
        TowerUnlockUI.OnCheckIsBuyable -= CheckPurchase;
        DeckManager.OnSaveIndex -= SaveTripleWaveIndex;
        TowerUpgradeUI.OnCheckIsBuyable -= CheckPurchase;
        StartPanel.OnTutorialDone -= SetTutorialPassed;
    }

    protected override void StartLevel()
    {
        base.StartLevel();
        m_ColumnManager.Initialize();
        m_DeckManager.Initialize(m_GameData.tripleWaveIndex);
        
    }

    GameData GetGameData()
    {
        return m_GameData;
    }

    void SaveTripleWaveIndex(int index)
    {
        m_GameData.tripleWaveIndex = index;
    }
    
    private void ColumnsUpdated()
    {
        m_SDKEventManager.LevelStarted(m_GameData.towerCurrentBlocks.Count, 1);
    }

    void NewTowerUnlockProcess(int amount, Column unlockedAttemptTo)
    {
        if (CheckPurchase(amount))
        {
            RemoveCoin(amount);
            unlockedAttemptTo.UnlockTower();
        }
        else
        {
            InGameVFXManager.Instance.ShowSimpleGameLog("Not enough coin!",2f,true);
        }
    }

    void UpgradeTowerProcess(int amount, Column upgradeAttemptTo)
    {
        if (CheckPurchase(amount))
        {
            RemoveCoin(amount);
            upgradeAttemptTo.UpgradeTower();
        }
        else
        {
            InGameVFXManager.Instance.ShowSimpleGameLog("Not enough coin!",2f,true);
        }
    }

    void SetTutorialPassed()
    {
        m_GameData.tutorial = true;
        SaveGameData();
    }
}
