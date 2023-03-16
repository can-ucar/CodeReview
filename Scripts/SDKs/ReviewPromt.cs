using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
using Google.Play.Common;
using Google.Play.Review;
#endif


public class ReviewPromt : MonoBehaviour
{
    [HideInInspector] public static ReviewPromt Instance;

    private bool m_ShownInSession = false;

    public float m_MinShowTime = 20;
    
    private static Coroutine _currentReview;
#if UNITY_ANDROID
    private static ReviewManager _reviewManager;
    private static PlayReviewInfo _reviewInfo;
#endif
    private void Awake()
    {
        if (Instance != null)
            GameObject.Destroy(Instance);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void ShowReview()
    {
#if UNITY_ANDROID
        if (!m_ShownInSession && Time.time >= m_MinShowTime)
        {
            m_ShownInSession = true;
            TryGetReview();        // Attempt to show in-app review prompt
        }
#endif
    }
#if UNITY_ANDROID
    public void TryGetReview()
    {
        if (_currentReview != null)
        {
            StopCoroutine(_currentReview);
            _currentReview = (Coroutine) null;
        }
        if (_reviewManager == null)
            _reviewManager = new ReviewManager();
        _currentReview = StartCoroutine(AndroidReview());
    }

    private static IEnumerator AndroidReview()
    {
        PlayAsyncOperation<PlayReviewInfo, ReviewErrorCode> requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return (object) requestFlowOperation;
        _reviewInfo = requestFlowOperation.GetResult();
        
        PlayAsyncOperation<VoidResult, ReviewErrorCode> launchFlowOperation = _reviewManager.LaunchReviewFlow(_reviewInfo);
        yield return (object) launchFlowOperation;
        _reviewInfo = (PlayReviewInfo) null;
    }
#endif
}
