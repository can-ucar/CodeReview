using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public bool m_EqualWidth;
    private Camera Cam;
    // Use this for initialization
    void Start ()
    {
        Cam = GetComponent<Camera>();
        if(m_EqualWidth) EqualWidth(); //if ortho
    }

    void EqualWidth ()
    {
        Cam.orthographicSize = Cam.orthographicSize / Cam.aspect;
    }
}
