using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera m_FollowCamera;
    [SerializeField] private CinemachineVirtualCamera m_StartCamera;
    public GameObject m_Camera;

    public void Initialize()
    {
        m_FollowCamera.Priority = 0;
        m_StartCamera.Priority = 1;
    }
    
    public void StartFollowing(Transform _FollowTarget)
    {
        m_FollowCamera.Follow = _FollowTarget;

        m_FollowCamera.Priority = 1;
        m_StartCamera.Priority = 0;
    }
}
