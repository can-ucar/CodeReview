using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class RisingTextUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private Gradient m_TextColor;
    [SerializeField] private int m_ValueMin = 5;
    [SerializeField] private int m_ValueMax = 50;
    [SerializeField] private float m_Scale = 0.5f;
    [SerializeField] private float m_RiseUpPosition = 0.15f;
    [SerializeField] private float m_Duration = 1f;
    [SerializeField] private AnimationCurve m_FadeCurve;

   

    public void Initialize(int value, Vector3 position)
    {
        var pos = Camera.main.WorldToScreenPoint(position);
        transform.position = pos;
        float t = Mathf.InverseLerp(m_ValueMin, m_ValueMax, value);
        var scaleMul = m_Scale * t + 1f; 
        transform.localScale = Vector3.one * scaleMul;
        pos.y += Screen.height * m_RiseUpPosition * scaleMul;
        m_Text.text = "+" + value;
        m_Text.color = m_TextColor.Evaluate(t);
        StartCoroutine(RiseUpCoroutine(pos, m_Duration * scaleMul));
    }

    private IEnumerator RiseUpCoroutine(Vector3 position, float duration)
    {
        var speed = 1f / duration;
        var start = transform.position;
        float t = 0;
        var color = m_Text.color;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;

            color.a = m_FadeCurve.Evaluate(t);
            m_Text.color = color;

            transform.position = Vector3.LerpUnclamped(start, position, t);

            yield return 0;
        }
        gameObject.SetActive(false);
    }
}
