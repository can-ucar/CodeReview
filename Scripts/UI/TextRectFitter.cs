using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextRectFitter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private float m_Padding;
    [SerializeField] private float m_MinSize;

    private RectTransform m_RectTr;
    private float m_CharWidth;
    private int m_TextLength;

    private void Awake()
    {
        if (!m_Text)
        {
            m_Text = GetComponent<TextMeshProUGUI>();
        }
        m_RectTr = (RectTransform) transform;
    }

    void OnEnable()
    {
        m_Text.enableWordWrapping = false;
        m_Text.overflowMode = TextOverflowModes.Overflow;
        var tmp = m_Text.text;
        m_TextLength = tmp.Length;
        m_Text.text = "0";
        m_Text.ForceMeshUpdate();
        m_CharWidth = m_Text.GetRenderedValues().x + 0.001f;
        m_Text.text = tmp;
        Resize();
        m_Text.ForceMeshUpdate();
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
    }

    void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
    }
    
    private void Resize()
    {
        if (CanvasUpdateRegistry.IsRebuildingGraphics())
        {
            StartCoroutine(ResizeAfterGraphicsRebuild());
        }
        else
        {
            var size = Mathf.Max(m_MinSize, m_TextLength * m_CharWidth + m_Padding);
            m_RectTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
        }
    }

    private IEnumerator ResizeAfterGraphicsRebuild()
    {
        yield return new WaitUntil(() => CanvasUpdateRegistry.IsRebuildingGraphics());

        var size = Mathf.Max(m_MinSize, m_TextLength * m_CharWidth + m_Padding);
        m_RectTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
    }

    void ON_TEXT_CHANGED(Object obj)
    {
        if (obj == m_Text && m_Text.text.Length != m_TextLength)
        {
            m_TextLength = m_Text.text.Length;
            Resize();
        }
    }
}
