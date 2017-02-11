using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbBehavior : MonoBehaviour {


    public float _MaxSpeed = 0;
    public float _ClimbDelay = 1.0f;
    public float _LerpSpeed = 1.0f;

    private Transform mTransform;
    private bool mCanClimb=true;
    private bool mClimbing = false;
    private float mClimbDelayTimer = 0f;

    private float mClimbLerpDistance;
    private float mClimbLerpStart;
    private Vector3 mClimbLerpStartPosition;
    private Vector3 mClimbLerpEndPosition;

    //Getters/Setters
    public bool isClimbing () { return mClimbing; }

	// Use this for initialization
	void Awake () {
        gameObject.tag = "Player";
        mTransform = gameObject.GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {

        //Player is waiting to climb -- Increment the timer
        if (!mCanClimb) 
        { 
            mClimbDelayTimer += Time.deltaTime;
            if (mClimbDelayTimer >= _ClimbDelay) 
            {
                mCanClimb = true;
            }
        }

        //Climb input is held and the play can climb --- Set the end position and begin the Lerp Timer
        if (Input.GetKey (KeyCode.Space) && mCanClimb && !mClimbing){
            mClimbLerpEndPosition = mTransform.position + new Vector3(0,_MaxSpeed,0);
            mClimbLerpDistance = Vector3.Distance (mTransform.position, mClimbLerpEndPosition);
            mClimbLerpStartPosition = mTransform.position;
            mClimbLerpStart = Time.time;
            mClimbing = true;
        }

        //Player is Climbing -- Lerp the position between the start and end points
        if (mClimbing) 
        {
            float distCovered = (Time.time - mClimbLerpStart) * _LerpSpeed;
            float fracClimb = distCovered / mClimbLerpDistance;
            mTransform.position = Vector3.Lerp (mClimbLerpStartPosition, mClimbLerpEndPosition, fracClimb);

            if (fracClimb >= 1.0) 
            {
                mClimbing = false;
                mClimbDelayTimer = 0f;
                mCanClimb = false;
            }
        }

	}

    //Reset the climbing state so when the player 're-attaches' it doesn't jump to the end of the lerp
    public void FreezeClimbing ()
    {
        mClimbing = false;
        mClimbDelayTimer = 0f;
        mCanClimb = false;
    }
}
