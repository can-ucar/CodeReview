using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AnotherWorld.Core
{
    [DefaultExecutionOrder(-1)]
    public abstract class GameManager : Manager<GameManager>
    {
        // MANAGERS
        [SerializeField] protected LevelManager m_LevelManager;
        [SerializeField] protected UIManager m_UIManager;
        [SerializeField] protected SaveLoadManager m_SaveLoadManager;
        [SerializeField] protected InputManager m_InputManager;
        [SerializeField] protected SDKEventManager m_SDKEventManager;

        // Save/Load
        protected GameData m_GameData;
        private float m_FocusTime;
        private int m_LevelStartCoin;

        // UI
        public int m_TutorialLevel = 0;

        // State
        public enum GameState
        {
            PreStart,
            Started,
            Fail,
            Win,
            Paused
        }
        protected GameState m_GameState;

        protected virtual void OnEnable()
        {
            m_UIManager.OnRestart += RestartLevel;
            m_UIManager.OnSettignsChanged += SaveSettings;
            m_UIManager.OnNextLevel += NextLevel;
            m_UIManager.OnSettingsActive += OnSettingsActive;
        }

        protected virtual void OnDisable()
        {
            m_UIManager.OnRestart -= RestartLevel;
            m_UIManager.OnSettignsChanged -= SaveSettings;
            m_UIManager.OnNextLevel -= NextLevel;
            m_UIManager.OnSettingsActive -= OnSettingsActive;
        }

        private void Start()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            LoadGameData();
            if(m_GameData.settingsData == null)
            {
                m_GameData.settingsData = new SettingsData();
                SaveGameData();
            }
            m_UIManager.SetSettingsData(m_GameData.settingsData);
            m_UIManager.Initialize(!m_GameData.tutorial, m_GameData);
            StartLevel();
        }

        protected void LoadGameData()
        {
            m_GameData = m_SaveLoadManager.LoadGameData();
        }

        protected void SaveGameData()
        {
            if (m_GameData.tutorial)
            {
                m_SaveLoadManager.SaveGameData(m_GameData);
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                m_FocusTime = Time.time;
            }
            else
            {
                m_GameData.playTime += Time.time - m_FocusTime;
                SaveGameData();
            }
        }
        private float GetPlayTime()
        {
            return m_GameData.playTime + Time.time - m_FocusTime;
        }

        protected virtual void StartLevel()
        {
            //m_LevelManager?.LoadLevel(m_GameData.level);
            m_InputManager?.SetEnabled(true);
            //m_UIManager?.SetLevelText(m_GameData.level);
            m_UIManager?.SetLevelCoin(m_GameData.coin);
            m_GameState = GameState.Started;
            m_SDKEventManager?.LevelStarted(m_GameData.level, 10);//add playtime here
        }

        protected virtual void RestartLevel()
        {
            m_GameData.attemptNumber++;
            m_GameData.coin = m_LevelStartCoin;
            m_UIManager.SetLevelCoin(m_LevelStartCoin);
            SaveGameData();
            StartLevel();
        }

        protected virtual void NextLevel()
        {
            m_UIManager.ShowHideLevelWinPanel(false);
            m_GameData.level++;
            m_GameData.attemptNumber = 0;
            m_LevelStartCoin = m_GameData.coin;
            SaveGameData();
            StartLevel();
        }

        protected virtual void LevelWin()
        {
            if(m_GameState == GameState.Started)
            {
                m_GameState = GameState.Win;
                m_UIManager.ShowHideLevelWinPanel(true);
                m_InputManager.SetEnabled(false);
            }
        }

        protected virtual void LevelFail()
        {
            if (m_GameState == GameState.Started)
            {
                m_GameState = GameState.Fail;
                m_UIManager.ShowHideLevelFailPanel(true);
                m_InputManager.SetEnabled(false);
            }
        }

        private void OnSettingsActive(bool active)
        {
            m_GameState = active ? GameState.Paused : GameState.Started;
            m_InputManager.SetEnabled(!active);
        }

        public void SaveSettings(SettingsData settingsData)
        {
            m_GameData.settingsData = settingsData;
            SaveGameData();
        }

        protected virtual void AddCoin(int amount)
        {
            m_GameData.coin += amount;
            m_UIManager.SetLevelCoin(m_GameData.coin);
            SaveGameData();
        }
        protected virtual void RemoveCoin(int amount)
        {
            m_GameData.coin -= amount;
            m_UIManager.SetLevelCoin(m_GameData.coin);
            SaveGameData();
        }

        protected virtual bool CheckPurchase(int amount)
        {
            if (m_GameData.coin >= amount)
            {
                return true;
            }
            return false;
        }
    }
}

