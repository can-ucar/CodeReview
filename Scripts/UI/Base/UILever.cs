using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace AnotherWorld.UI {
    public class UILever : UIButton
    {
        [SerializeField] private Color m_ActiveColor, m_DeActiveColor;
        public RectTransform m_ToggleLever;
        public Image m_ToggleBackground;
        protected override void Awake()
        {
            base.Awake();
            m_ToggleLever = transform.GetChild(1).GetComponent<RectTransform>();
            m_ToggleBackground = transform.GetChild(0).GetComponent<Image>();
        }
        public override void ButtonInit(RectTransform button)
        {
            //base.ButtonInit(button);
            button.gameObject.SetActive(true);
            button.localScale = Vector3.zero;
            button.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

            float currentX = m_ToggleLever.anchoredPosition.x;
            float targetX = -currentX;

            if (currentX > 0)
            {
                 m_ToggleBackground.color = m_DeActiveColor;
            }
            else
            {
                 m_ToggleBackground.color = m_ActiveColor;
            }

            m_ToggleLever.anchoredPosition = new Vector2(targetX, m_ToggleLever.anchoredPosition.y);
        }

       public void SetOnOff(bool turnOn)
        {
            float currentX = m_ToggleLever.anchoredPosition.x;
            float targetX = currentX;

            if (turnOn)
            {
                m_ToggleBackground.color = m_ActiveColor;
                targetX = currentX > 0 ? currentX : -currentX;
            }
            else
            {
                m_ToggleBackground.color = m_DeActiveColor;
                targetX = currentX > 0 ? -currentX : currentX;
            }

            m_ToggleLever.anchoredPosition = new Vector2(targetX, m_ToggleLever.anchoredPosition.y);
        }
    }
}