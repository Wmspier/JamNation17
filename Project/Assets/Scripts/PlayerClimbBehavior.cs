using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbBehavior : MonoBehaviour {


    public float _MaxSpeed = 0;
    public float _ClimbDelay = 1.0f;
    public float _LerpSpeed = 1.0f;

    private Transform mTransform;
    private Rigidbody mRigidBody;
    private bool mCanClimb=true;
    private bool mClimbing = false;
    private bool mEnabled = true;
    private bool mReachedTop=false;
    private float mClimbDelayTimer = 0f;

    private float mClimbLerpDistance;
    private float mClimbLerpStart;
    private Vector3 mClimbLerpStartPosition;
    private Vector3 mClimbLerpEndPosition;

    private GameObject[] mClimbNodeList;
    private GameObject mFocusedClimbNode;
    private int mFocusedClimbNodeIndex;
    private int mTotalClimbNodes;

    private float mGroundY;

    private PlayerSpotLightBehavior mSpotLightBehavior;
    private SpringJoint mJoint;

    //Getters/Setters
    public bool isClimbing () { return mClimbing; }
    public void setEnabled(bool state) { mEnabled = state; }

	// Use this for initialization
	void Awake () {
        gameObject.tag = "Player";
        mGroundY = GameObject.FindGameObjectWithTag("Ground").GetComponent<Transform>().position.y;
        mSpotLightBehavior = gameObject.GetComponent<PlayerSpotLightBehavior>();
        mJoint = gameObject.GetComponent<SpringJoint>();
        mTransform = gameObject.GetComponent<Transform> ();
        mRigidBody = gameObject.GetComponent<Rigidbody>();
	}

    void Start()
    {
        InitializeClimbingNodeList(); 
    }

    void InitializeClimbingNodeList()
    {
        Transform nodeList = GameObject.FindGameObjectWithTag("ClimbingNodes").GetComponent<Transform>();
        mClimbNodeList = new GameObject[nodeList.childCount];
        mTotalClimbNodes = nodeList.childCount-1;
        for (int i = 0; i < nodeList.childCount; i++)
        {
            mClimbNodeList[i] = nodeList.GetChild(i).gameObject;
        }
        mFocusedClimbNode = mClimbNodeList[0];
        mFocusedClimbNodeIndex = 0;
    }

    Vector3 FindClimbPosition()
    {
        return mFocusedClimbNode.GetComponent<Transform>().position;
    }

    GameObject FindNextClimbNode()
    {
        if (mFocusedClimbNodeIndex == mTotalClimbNodes)
        {
            mReachedTop = true;
            return mClimbNodeList[mTotalClimbNodes];
        }
        else 
        { 
            return mClimbNodeList[++mFocusedClimbNodeIndex];
        }
    }

    int FindClimbNodeIndex(GameObject node)
    { 
        int indexIter = 0;
        foreach (GameObject n in mClimbNodeList)
        {
            if (n == node) return indexIter;
            indexIter++;
        }
        return -1;
    }

    GameObject FindClosestClimbNode(Vector3 position)
    {
        GameObject closestNode = mFocusedClimbNode;
        float minDistance = Vector3.Distance(mTransform.position, mFocusedClimbNode.GetComponent<Transform>().position);
        int indexIter = 0;
        foreach (GameObject node in mClimbNodeList) 
        {
            float distanceFromNode = Vector3.Distance(mTransform.position, node.GetComponent<Transform>().position);
            if (distanceFromNode < minDistance) 
            {
                closestNode = node;
                mFocusedClimbNodeIndex = indexIter;
            }
            indexIter++;
        }

        return closestNode;
    }

	// Update is called once per frame
	void Update () {

        if (mEnabled && !mReachedTop) 
        {  
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
            if (Input.GetKey(KeyCode.Space) && mCanClimb && !mClimbing)
            {
                mClimbLerpEndPosition = FindClimbPosition();
                mClimbLerpDistance = Vector3.Distance(mTransform.position, mClimbLerpEndPosition);
                mClimbLerpStartPosition = mTransform.position;
                mClimbLerpStart = Time.time;
                mClimbing = true;
            }

            //Player is Climbing -- Lerp the position between the start and end points
            if (mClimbing)
            {
                float distCovered = (Time.time - mClimbLerpStart) * _LerpSpeed;
                float fracClimb = distCovered / mClimbLerpDistance;
                mTransform.position = Vector3.Lerp(mClimbLerpStartPosition, mClimbLerpEndPosition, fracClimb);

                //Player has reached climb destination
                if (fracClimb >= 1.0)
                {
                    mClimbing = false;
                    mClimbDelayTimer = 0f;
                    mCanClimb = false;
                    mFocusedClimbNode = FindNextClimbNode();
                    //mJoint.connectedBody = mFocusedClimbNode.GetComponent<Rigidbody>();
                    mJoint.anchor = mFocusedClimbNode.GetComponent<Transform>().position;
                }
            }

            Debug.DrawLine(mTransform.position, mFocusedClimbNode.GetComponent<Transform>().position,Color.red);

        }
      
        //Stop the Player from following through the ground plane
        if (mTransform.position.y <= mGroundY) 
        {
            mRigidBody.velocity = Vector3.zero;
            ToggleGravity(false);
            mSpotLightBehavior.AllowClimb();
            //mFocusedClimbNode = FindClosestClimbNode(mTransform.position);
            //mFocusedClimbNodeIndex = FindClimbNodeIndex(mFocusedClimbNode);
            //mJoint.connectedBody = mFocusedClimbNode.GetComponent<Rigidbody>();
            //mJoint.anchor = mFocusedClimbNode.GetComponent<Transform>().position;
        }
	}

    //Reset the climbing state so when the player 're-attaches' it doesn't jump to the end of the lerp
    public void FreezeClimbing ()
    {
        mClimbing = false;
        mClimbDelayTimer = 0f;
        mCanClimb = false;
    }

    public void ToggleGravity(bool state)
    {
        mRigidBody.useGravity = state;
    }
}
