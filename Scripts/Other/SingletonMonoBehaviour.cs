using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
#if UNITY_EDITOR
        if (Instance != null)
        {
            LogMultipleInstanceWarning();
            Instance.LogMultipleInstanceWarning();
        }
#endif

        Instance = this as T;
    }

#if UNITY_EDITOR
    private void LogMultipleInstanceWarning()
    {
        Debug.LogWarning("Multiple Instances of Sigleton " + typeof(T), this);
    }
#endif
}
