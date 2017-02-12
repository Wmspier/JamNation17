using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehavior : MonoBehaviour {

    public GameObject _LeftNode;
    public GameObject _RightNode;
    public float _Delay;
    public float _Duration;

    private bool mLaserOn = false;
    private LineRenderer mLineRenderer;
    private float mLaserTimer = 0f;
    private float mLaserDelayTimer = 0f;

    void Awake() {
        mLineRenderer = GetComponent<LineRenderer>();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (mLaserOn)
        {
            mLaserTimer += Time.deltaTime;
            if (mLaserTimer >= _Duration)
            {
                ToggleLaser(false);
                mLaserTimer = 0f;
            }
        }
        else
        {
            mLaserDelayTimer += Time.deltaTime;
            if (mLaserDelayTimer >= _Delay)
            {
                ToggleLaser(true);
                mLaserDelayTimer = 0f;
            }
        }
		
	}

    void ToggleLaser(bool toggle) 
    { 
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerClimbBehavior climb = other.gameObject.GetComponent<PlayerClimbBehavior>();
        climb.ToggleGravity(true);
        climb.FreezeClimbing();
        climb.ChangeState(MoveState.FALL);
    }
}
