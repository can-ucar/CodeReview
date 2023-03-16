using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldAndDrag : MonoBehaviour
{
    private bool m_CanTranslate = true;
    private bool m_Holding = true;
    private float m_ZCoord;
    private Vector3 m_Offset;
    private float m_InitialZ;
    private Vector3 m_InitialPosition;

    public bool m_ReturnInitialPos = false;
    public float m_ReturnLerpTime = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        m_InitialZ = transform.position.z;
        m_InitialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_ReturnInitialPos && !m_Holding)
        {
            transform.position = Vector3.Lerp(transform.position, m_InitialPosition, m_ReturnLerpTime);
        }
    }


    private void OnMouseDown()
    {
        if (m_CanTranslate)
        {
            m_Holding = true;
            m_ZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            m_Offset = gameObject.transform.position - GetMouseAsWorldPoint();
        }
    }

    private void OnMouseDrag()
    {
        if (m_CanTranslate)
        {
            transform.position = GetMouseAsWorldPoint() + m_Offset;
            transform.position = new Vector3(transform.position.x, transform.position.y, m_InitialZ);
        }
    }

    private void OnMouseUp()
    {
        if (m_CanTranslate)
        {
            m_Holding = false;
        }
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = m_ZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
