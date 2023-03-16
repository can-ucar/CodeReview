using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
namespace AnotherWorld.UI
{
    public class FailPanel : UIBase
    {
        [SerializeField] private RectTransform m_FailTextHolder;


        public void InitTextHolder()
        {
            m_FailTextHolder.gameObject.SetActive(true);
            m_FailTextHolder.localScale = Vector3.zero;
            m_FailTextHolder.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }
    }
}