using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {

    public static CameraShake Instance;

    private Vector3 _originalPos;
    private float _timeAtCurrentFrame;
    private float _timeAtLastFrame;
    private float _fakeDelta;
    private Vector3 velocity = Vector3.one;

    void Awake()
    {
        Instance = this;
    }

    void Update() 
    {
        _timeAtCurrentFrame = Time.realtimeSinceStartup;
        _fakeDelta = _timeAtCurrentFrame - _timeAtLastFrame;
        _timeAtLastFrame = _timeAtCurrentFrame; 
    }

    public static void Shake (float duration, float amount) 
    {
        Instance._originalPos = Instance.gameObject.transform.localPosition;
        Instance.StopAllCoroutines();
        Instance.StartCoroutine(Instance.CShake(duration, amount));
    }

    public IEnumerator CShake (float duration, float amount) 
    {
        float endTime = Time.deltaTime + duration;

        while (duration > 0) {
            transform.localPosition = Vector3.Lerp(_originalPos,_originalPos + (Random.insideUnitSphere / 2) * amount,2f);// _originalPos + Random.insideUnitSphere * amount;

            duration -= _fakeDelta;

            yield return null;
        }

        transform.localPosition = _originalPos;
    }
}