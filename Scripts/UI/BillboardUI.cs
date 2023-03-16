using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    Quaternion originalRotation;

    public string m_CameraTag;

    private Transform m_CamTransform;

    void Start()
    {
        originalRotation = transform.rotation;

        if (m_CameraTag == null || m_CameraTag == "")
            m_CamTransform = Camera.main.transform; 
        else
        {
            var cams = Camera.allCameras;

            for (int i = 0; i < cams.Length; i++)
            {
                if (cams[i].tag == m_CameraTag)
                {
                    m_CamTransform = cams[i].transform;
                    break;
                }
            }
        }

    }

    void Update()
    {
        //transform.rotation = m_CamTransform.rotation * originalRotation;
        //transform.LookAt(m_CamTransform, Vector3.up);
        transform.forward = -m_CamTransform.forward;
    }
}
