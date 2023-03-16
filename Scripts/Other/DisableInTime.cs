using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableInTime : MonoBehaviour {
    public float disableTime;
	// Use this for initialization
	void Start () {
	
	}
    private void OnEnable()
    {
        Invoke("Disable", disableTime);
    }
    // Update is called once per frame
    void Update () {
		
	}

    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
