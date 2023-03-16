using System;
using UnityEngine;
using AnotherWorld.UI;
using UnityEngine.SceneManagement;

namespace AnotherWorld.Core
{
    public interface IUIManager
    {
        public event Action OnRestart;

        public event Action<bool> OnSettingsActive;
        public event Action<SettingsData> OnSettignsChanged;
        public void Initialize(bool showTutorial, GameData gameData);
        public void SetSettingsData(SettingsData settings);
        public void Restart();
        public void ShowHideSettingsPanel(bool show);
        public void ShowHideTutorialPanel(bool show);
    }

    public interface IUIManagerLevelBased : IUIManager
    {
        public event Action OnNextLevel;
        public void ShowHideLevelWinPanel(bool show);
    }

    public interface IUIManagerFailableLevelBased : IUIManagerLevelBased
    {
        public void ShowHideLevelFailPanel(bool show);
    }

    public class UIManager : Manager<UIManager>, IUIManagerFailableLevelBased
    {
        [SerializeField] private LevelPanel m_LevelPanel;
        [SerializeField] private CoinPanel m_CoinPanel;
        [SerializeField] private SettingsPanel m_SettingsPanel;
        [SerializeField] private SimplePanel m_RestartPanel;
        [SerializeField] private WinPanel m_WinPanel;
        [SerializeField] private FailPanel m_FailPanel;
        public StartPanel m_TutorialPanel;

        private SettingsData m_SettingsData;

        public event Action OnRestart;
        public event Action<bool> OnSettingsActive;
        public event Action<SettingsData> OnSettignsChanged;
        public event Action OnNextLevel;

        public static event Action OnCoinChanged;

        private void Start()
        {
            m_SettingsPanel.OnHapticSet += SetHaptics;
            m_SettingsPanel.OnPanelActive += OnSettingsActivated;
        }

        private void OnDestroy()
        {
            m_SettingsPanel.OnHapticSet -= SetHaptics;
            m_SettingsPanel.OnPanelActive -= OnSettingsActivated;
        }

        public void SetSettingsData(SettingsData settingsData)
        {
            m_SettingsData = settingsData;
            m_SettingsPanel.SetHaptics(m_SettingsData.hapticsEnabled);
        }

        public virtual void Initialize(bool showTutorial, GameData gameData)
        {
            SetLevelCoin(gameData.coin);
            //m_RestartPanel?.OpenPanel();
            //ShowHideSettingsPanel(true);
            ShowHideLevelWinPanel(false);
            ShowHideLevelFailPanel(false);
            if (showTutorial)
            {
                //ShowHideRestartPanel(false);
                ShowHideSettingsPanel(false);
            }
            else
            {
                //ShowHideRestartPanel(true);
                ShowHideSettingsPanel(true);
            }
            ShowHideTutorialPanel(showTutorial);
        }

        /// <summary>
        /// Sets the LevelUI text to 'index + 1'
        /// </summary>
        /// <param name="index">Index of the level (zero based)</param>
        public void SetLevelText(int index)
        {
            m_LevelPanel?.SetLevelText(index);
        }

        public void SetLevelCoin(int coin)
        {
            m_CoinPanel?.SetLevelCoin(coin);
            OnCoinChanged?.Invoke();
        }

        public void ShowHideLevelPanel(bool show)
        {
            if (show)
            {
                m_LevelPanel?.OpenPanel();
            }
            else
            {
                m_LevelPanel?.ClosePanel();
            }
        }

        public void ShowHideRestartPanel(bool show)
        {
            if (show)
            {
                m_RestartPanel?.OpenPanel();
            }
            else
            {
                m_RestartPanel?.ClosePanel();
            }
        }

        public void ShowHideCoinPanel(bool show)
        {
            if (show)
            {
                m_CoinPanel?.OpenPanel();
            }
            else
            {
                m_CoinPanel?.ClosePanel();
            }
        }

        private void OnSettingsActivated(bool activated)
        {
            OnSettingsActive?.Invoke(activated);
        }

        public void ToggleSettingsPanel()
        {
            m_SettingsPanel.SettingsToggle();
        }

        public void ShowHideSettingsPanel(bool show)
        {
            if(show)
            {
                m_SettingsPanel?.OpenPanel();
            }
            else
            {
                m_SettingsPanel?.ClosePanel();
            }
        }

        public virtual void ShowHideLevelWinPanel(bool show)
        {
            if (show)
            {
                m_WinPanel?.OpenPanel();
            }
            else
            {
                m_WinPanel?.ClosePanel();
            }
            //ShowHideLevelPanel(!show);
            ShowHideCoinPanel(!show);
        }

        public virtual void ShowHideLevelFailPanel(bool show)
        {
            if (show)
            {
                m_FailPanel?.OpenPanel();
            }
            else
            {
                m_FailPanel?.ClosePanel();
            }
            //ShowHideLevelPanel(!show);
            ShowHideCoinPanel(!show);
        }
        
        public void ShowHideTutorialPanel(bool show)
        {
            if (show)
            {
                m_TutorialPanel?.OpenPanel();
            }
            else
            {
                m_TutorialPanel?.ClosePanel();
            }
        }

        public void Restart()
        {
            //ShowHideLevelFailPanel(false);
            //OnRestart?.Invoke();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void NextLevel()
        {
            OnNextLevel?.Invoke();
        }

        public void SetHaptics(bool enabled)
        {
            m_SettingsData.hapticsEnabled = enabled;
            OnSettignsChanged?.Invoke(m_SettingsData);
        }
    }


    [System.Serializable]
    public class SettingsData
    {
        public bool hapticsEnabled = true;
    }
}

