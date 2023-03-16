using AnotherWorld.UI;
using System.Collections;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] private RectTransform m_Hand;
    private float m_Speed = 1f;
    private float m_Delay = 0.3f;
    private float m_Scale = 1.2f;
    [SerializeField] private AnimationCurve m_MoveCurve;

    private IEnumerator DragHandCoroutine;
    private bool m_IsRunning;

    private void Awake()
    {
        gameObject.SetActive(false);
    }


    public void StartTutorial(Vector3 from, Vector3 to)
    {
        m_Hand.gameObject.SetActive(false);
        gameObject.SetActive(true);
        m_IsRunning = false;
        StopAllCoroutines();
        var fromScreen = Camera.main.WorldToScreenPoint(from);
        var toScreen = Camera.main.WorldToScreenPoint(to);
        DragHandCoroutine = HandDragCoroutine(fromScreen, toScreen);
        StartCoroutine(StartStopCoroutine());
    }

    public void StopTutorial()
    {
        m_IsRunning = false;
        m_Hand.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private IEnumerator StartStopCoroutine()
    {
        while (true)
        {
            if (m_IsRunning)
            {
                if (Input.GetMouseButton(0))
                {
                    m_IsRunning = false;
                    StopCoroutine(DragHandCoroutine);
                    m_Hand.gameObject.SetActive(false);
                }
            }
            else if (!Input.GetMouseButton(0))
            {
                m_IsRunning = true;
                StartCoroutine(DragHandCoroutine);
            }
            yield return 0;
        }
    }

    private IEnumerator HandDragCoroutine(Vector3 from, Vector3 to)
    {
        float t;
        while (m_IsRunning)
        {
            m_Hand.position = from;
            m_Hand.gameObject.SetActive(true);
            t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * m_Speed * 3f;
                m_Hand.localScale = Vector3.one * Mathf.Lerp(m_Scale, 1f, t);
                yield return 0;
            }
            yield return new WaitForSeconds(m_Delay);

            t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * m_Speed;
                m_Hand.position = Vector3.Lerp(from, to, m_MoveCurve.Evaluate(t));
                yield return 0;
            }
            yield return new WaitForSeconds(m_Delay);

            t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * m_Speed * 3f;
                m_Hand.localScale = Vector3.one * Mathf.Lerp(1f, m_Scale, t);
                yield return 0;
            }
            m_Hand.gameObject.SetActive(false);
            yield return new WaitForSeconds(m_Delay);
        }
    }
}