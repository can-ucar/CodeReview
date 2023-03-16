using System;
using UnityEngine;

public class ShooterGameTarget : MonoBehaviour
{
    public static event Action<GameObject> OnInitialized;
    public static event Action<GameObject> OnHit;

    private bool m_IsHit;

    private void Start()
    {
        var material = GetComponent<Renderer>().material;

        material.color = UnityEngine.Random.ColorHSV();

        OnInitialized?.Invoke(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!m_IsHit && collision.collider.gameObject.layer == 1)
        {
            OnHit?.Invoke(gameObject);
            m_IsHit = true;
        }
    }


}
