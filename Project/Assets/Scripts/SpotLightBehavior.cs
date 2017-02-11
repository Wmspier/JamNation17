using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightBehavior : MonoBehaviour {

    public float _LerpSpeed=1f;
    public float _Delay=5f;
    public float _DistanceFromWall = 30f;

    private Light mLight;
    private Transform mTransform;
    private GameObject[] mLeftSpawnPositions;
    private GameObject[] mRightSpawnPositions;
    private GameObject[] mPlayerList;

    private bool mLightEnabled = false;
    private float mLightDelayTimer=0f;

    private float mLerpDistance;
    private float mLerpStart;
    private Vector3 mLerpStartPosition;
    private Vector3 mLerpEndPosition;

    void Awake() {
        mLight = gameObject.GetComponent<Light>();
        mTransform = gameObject.GetComponent<Transform>();
    }

	// Use this for initialization
	void Start () {
        mLeftSpawnPositions = GameObject.FindGameObjectsWithTag("LeftLightSpawn");
        mRightSpawnPositions = GameObject.FindGameObjectsWithTag("RightLightSpawn");
        mPlayerList = GameObject.FindGameObjectsWithTag("Player");
        mLight.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

        //Light is not enable -- increment the delay timer
        if (!mLightEnabled)
        {
            mLightDelayTimer += Time.deltaTime;
            if (mLightDelayTimer >= _Delay)
            {
                mLightEnabled = true;
                EnableLight();
            }
        }
        else 
        { 
            float distCovered = (Time.time - mLerpStart) * _LerpSpeed;
            float frac = distCovered / mLerpDistance;
            mTransform.position = Vector3.Lerp(mLerpStartPosition, mLerpEndPosition, frac);

            foreach (GameObject player in mPlayerList)
            {
                player.GetComponent<PlayerSpotLightBehavior>().TestAttach();
            }

            //Light has reached destination
            if (frac >= 1.0)
            {
                mLightEnabled = false;
                mLightDelayTimer = 0f;
                mLight.enabled = false;
            }
        }
		
	}

    void EnableLight()
    { 
        GameObject randomStart = mLeftSpawnPositions[Random.Range(0, mLeftSpawnPositions.Length - 1)];
        GameObject randomEnd = mRightSpawnPositions[Random.Range(0, mRightSpawnPositions.Length - 1)];

        int direction = Random.Range(0, 2);
        Vector3 start;
        Vector3 end;
        if (direction == 0)
        {
            start = randomStart.GetComponent<Transform>().position;
            end = randomEnd.GetComponent<Transform>().position;
        }
        else 
        { 
            end = randomStart.GetComponent<Transform>().position;
            start = randomEnd.GetComponent<Transform>().position;
        }

        mLerpStartPosition = start;
        mLerpStartPosition.z = _DistanceFromWall;
        mLerpEndPosition = end;
        mLerpEndPosition.z = _DistanceFromWall;
        mLerpDistance = Vector3.Distance(mLerpStartPosition, mLerpEndPosition);

        mLerpStart = Time.time;

        mLight.enabled = true;
    }
}
