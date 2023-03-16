using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
namespace AnotherWorld.UI
{
    public class WinPanel : UIBase
    {
        [SerializeField] private RectTransform m_WinTextHolder;



        public void InitTextHolder()
        {
            m_WinTextHolder.gameObject.SetActive(true);
            m_WinTextHolder.localScale = Vector3.zero;
            m_WinTextHolder.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }
    }
}