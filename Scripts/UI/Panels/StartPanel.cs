using System;
using System.Collections;
using System.Collections.Generic;
using AnotherWorld.Core;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnotherWorld.UI
{
    public class StartPanel : UIBase
    {
       // [SerializeField] private Button noAdsButton;

        //------------ActionEvents--------------
        public event Action PlayAction;
        public event Action NoAdsAction;

        private BlockTowerUIManager m_UIManager;
        private GameData m_GameData;
        [SerializeField] private bool m_Active;
        public int m_StepIndex = 0;
        
        [SerializeField] private GameObject m_HandClickSprite;
        [SerializeField] private GameObject m_HandMoveSprite;
        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private GameObject m_FirstUpgradeUIPanel;

        private int m_Placed=0;
        private bool m_Upgradeable;

        public static event Func<Vector3> OnGetMiddleSlot;

        public static event Action OnTutorialDone;
        
        /// Start with the tower out.
        /// Hide the "Tower Upgrade" button before the player buys the next tower, and when they do buy the new tower, add the upgrade buttons
        /// Make the finger drag from the dirt block on the bottom to the dirt blocks on the tower
        /// Make the finger drag from the stone block on the bottom to the stone blocks on the tower
        /// After the player clears the tower, they get the reward, and a little finger teaches them to unlock a new tower
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Column.OnBlockAdded += OnPlaced;
            TowerUpgradeUI.OnUpgradeable += OnUpgradeable;
            Column.OnUnlock += OnInteracted;
            ShapeTemplate.GetTutorialStepIndex += () => m_StepIndex;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            Column.OnBlockAdded -= OnPlaced;
            TowerUpgradeUI.OnUpgradeable -= OnUpgradeable;
            Column.OnUnlock -= OnInteracted;
            ShapeTemplate.GetTutorialStepIndex -= () => m_StepIndex;
        }

        private void OnInteracted()
        {
            if (!m_Active) return;
            if (m_StepIndex >= 3) return;
            m_StepIndex++;
            HandClickAnimation(false);
            HandMoveAnimation(false);
            DOTween.Kill(78);
            TextAnimation(false);
            CallTutorialStepController();
        }

        private void OnPlaced()
        {
            if (m_Placed < 2)
            {
                if (!m_Active) return;
                m_Placed++;
                if (m_StepIndex >= 3) return;
                m_StepIndex++;
                HandClickAnimation(false);
                HandMoveAnimation(false);
                DOTween.Kill(78);
                TextAnimation(false);
                CallTutorialStepController();
            }
        }

        private void OnUpgradeable()
        {
            if (!m_Upgradeable)
            {
                if (!m_Active) return;
                m_Upgradeable = true;
                if (m_StepIndex >= 3) return;
                m_StepIndex++;
                HandClickAnimation(false);
                HandMoveAnimation(false);
                DOTween.Kill(78);
                TextAnimation(false);
                CallTutorialStepController();
            }
        }

        public void Initialize(BlockTowerUIManager blockTowerUIManager,GameData gameData)
        {
            m_GameData = gameData;
            m_UIManager = blockTowerUIManager;
            m_Active = true;
            StartCoroutine(TutorialStepController());
        }

        public void PlayGame()
        {
            PlayAction?.Invoke();
            SetActive(false);
        }
        
        public void NoAds()
        {
            NoAdsAction?.Invoke();
        }

        public void CallTutorialStepController()
        {
            StartCoroutine(TutorialStepController());
        }

        IEnumerator TutorialStepController()
        {
            if (!m_Active) yield break;
            switch (m_StepIndex)
            {
                case 0:
                    HandMoveAnimation(true,new Vector3(-0.5f,-0.3f,0),new Vector3(0,0.5f,0));
                    TextAnimation(true, "Drag blocks to place!");
                    break;
                case 1:
                    HandMoveAnimation(true,new Vector3(0,-0.3f,0),new Vector3(0,0.8f,0));
                    TextAnimation(true, "Drag blocks to place!");
                    break;
                case 2:
                    HandClickAnimation(true,new Vector3(m_FirstUpgradeUIPanel.transform.position.x,m_FirstUpgradeUIPanel.transform.position.y - 0.05f,m_FirstUpgradeUIPanel.transform.position.z));
                    TextAnimation(true, "Tap to unlock new tower!");
                    break;
                case 3:
                    TutorialEnd();
                    break;
            }
        }

        void HandClickAnimation(bool show, Vector3 pos = default)
        {
            if (show)
            {
                m_HandClickSprite.SetActive(true);
                m_HandClickSprite.transform.position = Camera.main.WorldToScreenPoint(pos);
            }
            else
            {
                m_HandClickSprite.SetActive(false);
            }
        }
        void HandClickUIAnimation(bool show, Vector3 pos = default)
        {
            if (show)
            {
                m_HandClickSprite.SetActive(true);
                m_HandClickSprite.transform.position = pos;
            }
            else
            {
                m_HandClickSprite.SetActive(false);
            }
        }

        void HandMoveAnimation(bool show, Vector3 startPos = default, Vector3 endPos = default)
        {
            if (show)
            {
                m_HandMoveSprite.transform.DOMove(Camera.main.WorldToScreenPoint(startPos),0);
                m_HandMoveSprite.SetActive(true);
                m_HandMoveSprite.transform.DOScale(0.9f, 0.5f)
                    .OnComplete((() =>
                    {
                        m_HandMoveSprite.transform.DOMove(Camera.main.WorldToScreenPoint(endPos), 1f)
                            .OnComplete((() =>
                            {
                                m_HandMoveSprite.transform.DOScale(1.61f, 0.5f)
                                    .OnComplete((() =>
                                    {
                                        HandMoveAnimation(true,startPos,endPos);
                                    })).SetId(78);
                            })).SetId(78);
                    })).SetId(78);

            }
            else
            {
                m_HandMoveSprite.SetActive(false);
            }
        }

        void TextAnimation(bool show, string text="")
        {
            if (show)
            {
                m_Text.gameObject.SetActive(true);
                m_Text.text = text;
            }
            else
            {
                m_Text.gameObject.SetActive(false);
            }
        }

        void TutorialEnd()
        {
            m_UIManager.TutorialEnd();
            OnTutorialDone?.Invoke();
        }
        
    }
}