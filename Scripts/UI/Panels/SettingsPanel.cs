using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;

namespace AnotherWorld.UI
{
    public class SettingsPanel : UIBase
    {
        public SimplePanel m_SettingsWindow;

        public UILever m_HapticsLever;

        public event Action<bool> OnHapticSet;
        public event Action<bool> OnPanelActive;

        protected override void Awake()
        {
            base.Awake();
        }
        public void SettingsToggle()
        {
            OnPanelActive?.Invoke(!m_SettingsWindow.gameObject.activeSelf);
            m_SettingsWindow.TogglePanel();
        }
        public void ToggleHaptic()
        {
            bool activeHaptic = MMVibrationManager._vibrationsActive;
            if (activeHaptic)
                TurnHapticsOff();
            else
                TurnHapticsOn();

        }

        public void TurnHapticsOn()
        {
            OnHapticSet?.Invoke(true);
            SetHapticsOn();
        }

        private void SetHapticsOn()
        {
            MMVibrationManager.SetHapticsActive(true);
            MMVibrationManager.Haptic(HapticTypes.Success, false, true, this);
        }

        public void TurnHapticsOff()
        {
            OnHapticSet?.Invoke(false);
            SetHapticsOff();
        }

        private void SetHapticsOff()
        {
            MMVibrationManager.Haptic(HapticTypes.Warning, false, true, this);
            MMVibrationManager.SetHapticsActive(false);
        }

        public void SetHaptics(bool enabled)
        {
            if(enabled)
            {
                SetHapticsOn();
            }
            else
            {
                SetHapticsOff();
            }

            m_HapticsLever.SetOnOff(enabled);
        }

        public void OpenURL(string _URL)
        {
            Application.OpenURL(_URL);
        }

        public void SetupPrivacyButton()
        {
            //if (m_PrivacyButton == null) return;
            //if (LionGDPR.Status == LionGDPR.UserStatus.Applies)
            //{
            //    m_PrivacyButton.gameObject.SetActive(true);
            //    if (Application.platform == RuntimePlatform.IPhonePlayer)
            //    {
            //        if (!LionGDPR.CanShowGdpr) m_PrivacyButton.gameObject.SetActive(false);
            //    }
            //
            //}
            //else
            //{
            //    m_PrivacyButton.gameObject.SetActive(false);
            //}
        }

        public void ShowGDPR()
        {
            //LionGDPR.Show();
        }
    }


}