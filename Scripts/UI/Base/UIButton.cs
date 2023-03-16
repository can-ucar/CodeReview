using System;
using System.Collections;
using System.Collections.Generic;
using AnotherWorld.Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AnotherWorld.UI
{

    public class UIButton : UIBase, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Change Sprites")]
        [SerializeField] private Sprite m_OpenSprite,m_ClosedSprite;
        [Space(10)]
        [Header("Events")]
        [SerializeField] private UnityEvent m_ButtonDownActionList;
        [SerializeField] private UnityEvent m_ButtonUpActionList;

        private Image m_ButtonImage;
        private RectTransform m_RectTransform;

        private bool Opened = true;

        protected override void Awake()
        {
            base.Awake();
            m_ButtonImage = GetComponent<Image>();
            m_RectTransform = GetComponent<RectTransform>();
            ChangeSprite();
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            ButtonInit(m_RectTransform);
        
                m_ButtonDownActionList?.Invoke();
            
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ButtonRelease(m_RectTransform);
            
           m_ButtonUpActionList?.Invoke();
            
        }

        

        #region UITools

        public virtual void ButtonInit(RectTransform button)
        {
            button.gameObject.SetActive(true);
            button.localScale = Vector3.zero;
            ChangeSprite();
            button.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }

        public void ButtonInitCustom(RectTransform button, AnimationCurve _Curve, float _Time)
        {
            button.gameObject.SetActive(true);
            button.localScale = Vector3.zero;
            button.DOScale(Vector3.one, _Time).SetEase(_Curve);
        }

        public void ButtonPress(RectTransform _Button)
        {
            ChangeSprite();
            if (_Button.gameObject.TryGetComponent(out UIPulseAnimation pulseAnimation))
            {
                pulseAnimation.StopPulse();
            }

            //MMVibrationManager.Haptic(HapticTypes.RigidImpact);
            _Button.localScale = Vector3.one * 0.8f;
            HapticManager.SetVibration(Haptics.RigidImpact);
        }

        public void ButtonDisable(RectTransform button)
        {
            button.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack);
        }

        public void ButtonRelease(RectTransform _Button)
        {
            //ChangeSprite();
            _Button.localScale = Vector3.one;
            if (_Button.gameObject.TryGetComponent(out UIPulseAnimation pulseAnimation))
            {
                pulseAnimation.ResumePulse();
            }
        }

        public virtual void ChangeSprite()
        {
            if (m_OpenSprite != null && Opened)
            {
                m_ButtonImage.sprite = m_OpenSprite;
                Opened = false;
            }
            else if (m_ClosedSprite != null && !Opened)
            {
                m_ButtonImage.sprite = m_ClosedSprite;
                Opened = true;
            }
        }
        #endregion
    }
}