using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIPulseAnimation : MonoBehaviour
{
    private RectTransform m_RectTransform;
    private Sequence m_PulseSequence;

    public float m_PulseTime = 1f;

    // Start is called before the first frame update
    void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        InitiatePulse();
    }

    public void InitiatePulse()
    {
        m_PulseSequence = DOTween.Sequence();

        m_PulseSequence.Append(
            m_RectTransform.DOScale(Vector3.one * 1.15f, m_PulseTime)
            .SetEase(Ease.InOutSine)
            ).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopPulse()
    {
        m_PulseSequence.Pause();
    }

    public void ResumePulse()
    {
        m_PulseSequence.Play();
    }

    private void OnDisable()
    {
        StopPulse();
    }
}
