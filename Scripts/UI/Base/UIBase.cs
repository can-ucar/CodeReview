using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

namespace AnotherWorld.UI
{

    public abstract class UIBase : MonoBehaviour
    {
        public event Action<bool> Activated;
        public bool IsActivated { get; protected set; }

        public virtual void OpenPanel()
        {
            SetActive(true);
            GetComponent<RectTransform>().localScale = Vector3.zero;
            GetComponent<RectTransform>().DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).OnComplete(() => GetComponent<RectTransform>().localScale = Vector3.one); ;
        }
        public virtual void ClosePanel()
        {
            GetComponent<RectTransform>().DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(()=> SetActive(false));
        }
        public virtual void SetActive(bool isActive)
        {
            switch (isActive)
            {
                case true:
                    Active();
                    break;
                case false:
                    Passive();
                    break;
            }
        }

        public virtual void Active()
        {

            if (!IsActivated)
            {
                IsActivated = true;
                Activated?.Invoke(true);
                gameObject.SetActive(true);
                OnActive();
            }
        }

        public virtual void Passive()
        {
            if (IsActivated)
            {
                IsActivated = false;
                Activated?.Invoke(false);
                gameObject.SetActive(false);
                OnPassive();
            }
        }

        protected virtual void OnActive()
        {

        }

        protected virtual void OnPassive()
        {

        }

        public virtual void SetEnabled(bool value)
        {
            enabled = value;
        }

        protected virtual void Awake()
        {
            IsActivated = gameObject.activeSelf;
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {

        }

        protected virtual void FixedUpdate()
        {

        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }
    }
}