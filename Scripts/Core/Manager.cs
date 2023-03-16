using System;
using UnityEngine;

namespace AnotherWorld.Core
{
    public class Manager<T> : MonoBehaviour where T : Manager<T>
    {
        private static T m_Instance;
        
        protected virtual void Awake()
        {
            if (m_Instance != null && m_Instance != this)
            {
                Destroy(this);
            }
            else
            {
                m_Instance = this as T;
            }
        }

//#if UNITY_EDITOR
//        private void OnValidate()
//        {
//            if(!Application.isPlaying && !String.IsNullOrEmpty(gameObject.scene.path) && !String.IsNullOrEmpty(gameObject.scene.name))
//            {
//                if (m_Instance == null || m_Instance == this)
//                {
//                    m_Instance = this as T;
//                }
//                else
//                {
//                    Debug.LogError(this.GetType() + " already had an instance on scene. Destroying duplicate");

//                    UnityEditor.EditorApplication.delayCall += () =>
//                    {
//                        DestroyImmediate(this);
//                    };
//                }
//            }
//        }
//#endif
    }
}

