using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPhysicsTest : MonoBehaviour
{
    public List<GameObject> m_BlockList = new List<GameObject>();
    
    public float power = 10.0F;
    public float radius = 1f;
    public float upwardsUp;
    public float upwardsDown;

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hix;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hix,1000f))
            {
                Vector3 explosionPos = new Vector3(0,hix.transform.position.y,0);

                for (int i = 0; i < m_BlockList.Count; i++)
                {
                    GameObject o = m_BlockList[i];
                    if (o == hix.collider.gameObject)
                    {
                        Rigidbody rbUP = m_BlockList[i+1].GetComponent<Rigidbody>();
                        rbUP.AddExplosionForce(power * (m_BlockList.Count - i+1), explosionPos, radius, upwardsUp,ForceMode.Impulse);
                        
                        //Rigidbody rbDOWN = m_BlockList[i-1].GetComponent<Rigidbody>();
                        //rbDOWN.AddExplosionForce(power, explosionPos, radius, -upwardsDown);
                    }
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hix;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hix,1000f))
            {
                Vector3 explosionPos = new Vector3(0,hix.transform.position.y,0);

                for (int i = 0; i < m_BlockList.Count; i++)
                {
                    GameObject o = m_BlockList[i];
                    if (o == hix.collider.gameObject)
                    {
                        Rigidbody rbUP = m_BlockList[i+1].GetComponent<Rigidbody>();
                        rbUP.AddExplosionForce(power * (m_BlockList.Count - i+1), explosionPos, radius, upwardsUp,ForceMode.Impulse);
                        m_BlockList.Remove(hix.collider.gameObject);
                        Destroy(hix.collider.gameObject);
                        //Rigidbody rbDOWN = m_BlockList[i-1].GetComponent<Rigidbody>();
                        //rbDOWN.AddExplosionForce(power, explosionPos, radius, -upwardsDown);
                    }
                }
            }
        }
    }
    
    
}
