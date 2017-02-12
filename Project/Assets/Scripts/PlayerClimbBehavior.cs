using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveState 
{ 
    IDLE,
    CLIMB,
    FALL,
    TOTAL
}

public class PlayerClimbBehavior : MonoBehaviour {


    public float _MaxSpeed = 0;
    public float _ClimbDelay = 1.0f;
    public float _LerpSpeed = 1.0f;
    public float _RopeLength = 5.0f;
    public float _RopeMargin = 5.0f;
    public KeyCode _ClimbKey;
    public GameObject _AttachedPlayer;
    public bool _UseArduino=false;

    public float _MaxClimbFrequency = 10f;
    public float _FrequencyMod = 0.5f;
    public float _ClimbFrequencyDecayTick = 0.5f; //How often the climb frequncy decays
    public float _ClimbFrequencyDecay = 0.5f; //Decay of climb frequency per tick
    public float _ClimbFrequencyDecayTime = 1f; //Seconds of NO INPUT before climb freq decay

    public float _RecoveryDelay = 3f;

    public Sprite _IdleSprite;
    public Sprite _WinSprite;
    public Sprite _FallSprite;

    private bool mClimbFrequencyDecay = false;

    private float mClimbFrequency=0f;
    private float mClimbFrequencyDecayTickTimer = 0f;
    private float mClimbFrequencyDecayTimer = 0f;

    private Transform mTransform;
    private Rigidbody2D mRigidBody;
    private bool mCanClimb=true;
    private bool mClimbing = false;
    private bool mEnabled = true;
    private bool mReachedTop=false;
    private float mClimbDelayTimer = 0f;

    private float mRecoveryTimer = 0f;

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
    private ScoringBehavior mScoringBehavior;
    private StateMachineBehaviour mAnimationStateMachine;

    private SpriteRenderer mAnimSprite;
    private SpriteRenderer mFixedSprite;

    private bool mClimbTick=false;

    //Getters/Setters
    public bool isClimbing () { return mClimbing; }
    public void setEnabled(bool state) { mEnabled = state; }
    public bool isEnabled() { return mEnabled; }
    public void climbTick() { mClimbTick = true; }

    public Color _DebugRopeColor;

	// Use this for initialization
	void Awake () {
        gameObject.tag = "Player";
        mGroundY = GameObject.FindGameObjectWithTag("Ground").GetComponent<Transform>().position.y;
        mSpotLightBehavior = gameObject.GetComponent<PlayerSpotLightBehavior>();
        mTransform = gameObject.GetComponent<Transform> ();
        mRigidBody = gameObject.GetComponent<Rigidbody2D>();
        mAnimSprite = mTransform.GetChild(0).GetComponent<SpriteRenderer>();
        mFixedSprite = mTransform.GetChild(1).GetComponent<SpriteRenderer>();
	}

    void Start()
    {
        InitializeClimbingNodeList();
        mScoringBehavior = FindObjectOfType<ScoringBehavior>();
    }

    void InitializeClimbingNodeList()
    {
        Transform nodeList = GameObject.FindGameObjectWithTag("ClimbingNodes").GetComponent<Transform>();
        mClimbNodeList = new GameObject[nodeList.childCount];
        mTotalClimbNodes = nodeList.childCount - 1;
        //GameObject child = nodeList.GetChild(0).gameObject;
        //int childIncrement = 0;
        //while (child.GetComponent<Transform>().childCount > 0)
        //{
        //    mClimbNodeList.Add(child);
        //    childIncrement++;
        //    child = child.GetComponent<Transform>().GetChild(0).gameObject;
        //}
        //mTotalClimbNodes = childIncrement;
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
            mScoringBehavior.PlayerReachedTop();
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

    void UpdateClimbFrequency()
    {
        if (mClimbFrequency + _FrequencyMod <= _MaxClimbFrequency)
        {
            mClimbFrequency += _FrequencyMod;
        }
    }

	// Update is called once per frame
	void Update () {

        #region DEBUG
        if (!_UseArduino && Input.GetKeyDown(_ClimbKey)) climbTick();

        if(_AttachedPlayer) Debug.DrawLine(mTransform.position, _AttachedPlayer.GetComponent<Transform>().position, _DebugRopeColor);

        #endregion

        if (mEnabled && !mReachedTop)
        {
            #region TIMERS
            //Time before climb frequency beginds to decay
            mClimbFrequencyDecayTimer += Time.deltaTime;
            if (mClimbFrequencyDecayTimer >= _ClimbFrequencyDecayTime)
            {
                mClimbFrequencyDecay = true;
            }

            //Climb frequency is decaying
            if (mClimbFrequencyDecay && mCanClimb) 
            {
                mClimbFrequencyDecayTickTimer += Time.deltaTime;

                //Climb Decay Tick
                if (mClimbFrequencyDecayTickTimer >= _ClimbFrequencyDecayTick)
                {
                    mClimbFrequency -= _ClimbFrequencyDecay;
                    if (mClimbFrequency < 0) mClimbFrequency = 0;
                    mClimbFrequencyDecayTickTimer = 0f;

                    //_LerpSpeed = mClimbFrequency;
                    if (mClimbing)
                    {
                        mClimbLerpStart = Time.time;
                        mClimbLerpStartPosition = mTransform.position;
                        mClimbLerpDistance = Vector3.Distance(mTransform.position, mClimbLerpEndPosition);
                    }
                }

            }

            //Player is waiting to climb -- Increment the timer
            if (!mCanClimb)
            {
                mClimbDelayTimer += Time.deltaTime;
                if (mClimbDelayTimer >= _ClimbDelay)
                {
                    mCanClimb = true;
                }
            }
            #endregion

            if (mClimbTick && mCanClimb)
            {
                mClimbTick = false;
                UpdateClimbFrequency();


                //_LerpSpeed = mClimbFrequency;
                if (mClimbing)
                {
                    mClimbLerpStart = Time.time;
                    mClimbLerpStartPosition = mTransform.position;
                    mClimbLerpDistance = Vector3.Distance(mTransform.position, mClimbLerpEndPosition);

                }
            }


            //Climb input is held and the player can climb --- Set the end position and begin the Lerp Timer
            if (mClimbFrequency>0 && !mClimbing)
            {
                mClimbLerpEndPosition = FindClimbPosition();
                mClimbLerpDistance = Vector3.Distance(mTransform.position, mClimbLerpEndPosition);
                float distanceFromAttachedPlayer=0;
                if(_AttachedPlayer) distanceFromAttachedPlayer = Vector3.Distance(mTransform.position, _AttachedPlayer.GetComponent<Transform>().position);
                if ((mClimbLerpDistance <= _RopeLength + _RopeMargin && distanceFromAttachedPlayer <= _RopeLength + _RopeMargin && !(distanceFromAttachedPlayer >= _RopeLength && AboveAttachedObject())) || _AttachedPlayer == null)
                {
                    mClimbLerpStartPosition = mTransform.position;
                    mClimbLerpStart = Time.time;
                    mClimbing = true;
                    mCanClimb = false;
                    ToggleGravity(false);
                    ChangeState(MoveState.CLIMB);
                }
            }

            //Player is Climbing -- Lerp the position between the start and end points
            if (mClimbing)
            {
                float distCovered = (Time.time - mClimbLerpStart) * _LerpSpeed;
                //float distCovered = (Time.time - mClimbLerpStart) * mClimbFrequency;
                float fracClimb = distCovered / mClimbLerpDistance;
                mTransform.position = Vector3.Lerp(mClimbLerpStartPosition, mClimbLerpEndPosition, fracClimb);

                //Vector3 direction = (mFocusedClimbNode.GetComponent<Transform>().position - mTransform.position).normalized;
                //Quaternion lookRotation = Quaternion.LookRotation(direction);
                //mTransform.rotation = lookRotation;
                //mTransform.LookAt(mFocusedClimbNode.GetComponent<Transform>().position);
                //Quaternion rotation = mTransform.rotation;
                //rotation.x = 0;
                //mTransform.rotation = rotation;

                //Player has reached climb destination
                if (fracClimb >= 1.0)
                {
                    mClimbing = false;
                    mClimbDelayTimer = 0f;
                    mFocusedClimbNode = FindNextClimbNode();
                    mClimbFrequency = 0;
                    ChangeState(MoveState.IDLE);
                }
            }

            if (_AttachedPlayer) Debug.DrawLine(mTransform.position, _AttachedPlayer.GetComponent<Transform>().position, Color.red);

        }
        else
        {
            /// Anything within this else takes place when the Player is falling ///

            mRecoveryTimer += Time.deltaTime;
            if (mRecoveryTimer >= _RecoveryDelay)
            {
                FreezeClimbing();
                ToggleGravity(false);
                mSpotLightBehavior.AllowClimb();
                mFocusedClimbNode = FindClosestClimbNode(mTransform.position);
                mFocusedClimbNodeIndex = FindClimbNodeIndex(mFocusedClimbNode);
                mRecoveryTimer = 0f;
                Debug.Log("RESET");
            }

            //Stop the Player from falling through the ground plane
            if (mTransform.position.y <= mGroundY)
            {
                ToggleGravity(false);
                mSpotLightBehavior.AllowClimb();
                mFocusedClimbNode = mClimbNodeList[0];
                mFocusedClimbNodeIndex = FindClimbNodeIndex(mFocusedClimbNode);
                mRigidBody.velocity = Vector3.zero;
            }
            if (_AttachedPlayer)
            { 
                float distance = Vector3.Distance(mTransform.position, _AttachedPlayer.GetComponent<Transform>().position);
                if (mTransform.position.y < _AttachedPlayer.GetComponent<Transform>().position.y && mRigidBody.velocity.y < 0 && distance < _RopeLength)
                {
                    FakeRope();
                }
            }
         }
	}

    void FakeRope()
    {
        Vector3 newPos = _AttachedPlayer.GetComponent<Transform>().position;
        newPos.y -= (_RopeLength - _RopeMargin);
        if (newPos.y < mGroundY) newPos.y = mGroundY;
        mTransform.position = newPos;
        mFocusedClimbNode = FindClosestClimbNode(mTransform.position);
        mFocusedClimbNodeIndex = FindClimbNodeIndex(mFocusedClimbNode);
        mRigidBody.velocity = Vector3.zero;
    }

    bool AboveAttachedObject()
    {
        return mTransform.position.y > _AttachedPlayer.GetComponent<Transform>().position.y;    
    }

    //Reset the climbing state so when the player 're-attaches' it doesn't jump to the end of the lerp
    public void FreezeClimbing ()
    {
        mClimbing = false;
        mCanClimb = false;
        mClimbDelayTimer = 0f;
        mRigidBody.velocity = Vector3.zero;
    }

    public void ToggleGravity(bool state)
    {
        if (!state)
        { 
            mRigidBody.gravityScale = 0;
            mRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        }
        else { 
            mRigidBody.gravityScale = 1;
            mRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public void ChangeState(MoveState state)
    {
        switch (state)
        {
            case MoveState.FALL:
                mAnimSprite.enabled = false;
                mFixedSprite.enabled = true;
                mFixedSprite.sprite = _FallSprite;
                break;
            case MoveState.IDLE:
                mAnimSprite.enabled = false;
                mFixedSprite.enabled = true;
                mFixedSprite.sprite = _IdleSprite;
                break;
            case MoveState.CLIMB:
                mAnimSprite.enabled = true;
                mFixedSprite.enabled = false;
                break;
            default:
                break;
        }
    }
}
