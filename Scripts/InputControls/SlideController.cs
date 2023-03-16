using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideController : MonoBehaviour
{
    private Vector2 m_InitialTouchPosition;

    public static event Action<Vector2> OnSlide;
    public static event Action OnStartTouch;
    public static event Action OnFinishTouch;
    public float m_MoveSensitivity = 200;
    public float m_TouchMultiplier = 3;

    private void Update()
    {
        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            Vector2 touchPosCorrected = new Vector2(t.position.x / (float)Screen.width, t.position.y / (float)Screen.width);
            if (t.phase == TouchPhase.Began)
            {
                m_InitialTouchPosition = touchPosCorrected;
                SetStartTouch();
            }
            if(t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
            {
                Vector2 displacement = touchPosCorrected - m_InitialTouchPosition;
                SlideDisplacement(displacement * m_TouchMultiplier);
            }
            if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) //could be wrong
            {
                SetFinishTouch();
            }
        }
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                m_InitialTouchPosition = Input.mousePosition;
                SetStartTouch();
            }
            else if (Input.GetMouseButton(0))
            {
                Vector2 displacement = ((Vector2)Input.mousePosition - m_InitialTouchPosition) / m_MoveSensitivity;
                SlideDisplacement(displacement);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                SetFinishTouch();
            }
#endif
    }

    private void SetStartTouch()
    {
        OnStartTouch?.Invoke();
    }

    private void SlideDisplacement(Vector2 _Displacement)
    {
        OnSlide?.Invoke(_Displacement);
    }

    private void SetFinishTouch()
    {
        OnFinishTouch?.Invoke();
    }
}
