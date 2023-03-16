using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawProjectile : MonoBehaviour
{
    private PoolScript Pool;
    private GameObject m_Indicator;

    public LineRenderer m_Line;
    public int m_LineSegment = 10;
    public Vector3 m_MaxThrowDistance;
    public float m_Distance = 10f;
    public bool m_RopeHit = false;
    public LayerMask m_IndicatorMask;

    private void Start()
    {
        Pool = FindObjectOfType<PoolScript>();
    }

    public void ResetLine()
    {
        m_Line.positionCount = 0;
        m_Line.positionCount = m_LineSegment;
        IndicatorDeactivate();
    }

    public void DrawLine(Vector3 _Direction, float _Gravity, Vector3 _StartPos)
    {
        for (int i = 0; i < m_LineSegment; i++)
        {
            Vector3 pos = CalculatePosInTime(_Direction, _Gravity, i / (float)m_LineSegment * m_Distance, _StartPos);
            m_Line.SetPosition(i, pos);
        }

        RaycastHit hit;
        if (Physics.Raycast(_StartPos, _Direction, out hit, m_Distance, m_IndicatorMask))
        {
            IndicatorActivate(hit.point);
        }
        else if(!m_RopeHit)
        {
            IndicatorDeactivate();
        }
    }

    public void IndicatorActivate(Vector3 _Position)
    {
        if (m_Indicator == null)
        {
            m_Indicator = Pool.GetPooledObject("HitIndicator");
            m_Indicator.SetActive(true);
        }
        m_Indicator.transform.position = _Position;
    }

    public void IndicatorDeactivate()
    {
        if (m_Indicator != null)
        {
            m_Indicator.SetActive(false);
            m_Indicator = null;
        }
    }

    Vector3 CalculatePosInTime(Vector3 _Vo, float _Gravity ,float _Time, Vector3 _StartPos)
    {
        Vector3 result = _StartPos + _Vo * _Time;
        float sY = (-0.5f * Mathf.Abs(_Gravity) * (_Time * _Time)) + (_Vo.y * _Time) + _StartPos.y;

        result.y = sY;

        return result;
    }
}
