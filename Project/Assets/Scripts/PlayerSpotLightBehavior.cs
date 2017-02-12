using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpotLightBehavior : MonoBehaviour
{

    //Is the player holding on to the building
    private bool mAttached=true;
    private PlayerClimbBehavior mClimbBehavior;
    private Rigidbody mRigidBody;
    private MeshRenderer mMeshRenderer;
    private Light mSpotLight;
    private Transform mTransform;

    public Material _AttachedMaterial;
    public Material _DetachedMaterial;

    public KeyCode _DropKey;

    void Update()
    {
        if (Input.GetKeyDown(_DropKey))
        {
            ToggleAttach();
        }    
    }

    void Start()
    {
        mClimbBehavior = gameObject.GetComponent<PlayerClimbBehavior>();
        mMeshRenderer = gameObject.GetComponent<MeshRenderer>();
        mTransform = gameObject.GetComponent<Transform>();
        mRigidBody = gameObject.GetComponent<Rigidbody>();
        mSpotLight = GameObject.FindGameObjectWithTag("SpotLight").GetComponent<Light>();
    }

    bool IsWithinLight()
    {
        float range = mSpotLight.range;
        RaycastHit hit;
        Debug.DrawRay(mSpotLight.gameObject.GetComponent<Transform>().position, Vector3.forward * 100);
        if (Physics.Linecast(mSpotLight.gameObject.GetComponent<Transform>().position, Vector3.forward * 100, out hit))
        {
            //Debug.Log(Vector3.Distance(hit.point, mTransform.position));

            return (Vector3.Distance(hit.point, mTransform.position) <= 12);
        }
        else return false;
    }

    //Toggle attach state if climbing
    public void TestAttach()
    {
        if (IsWithinLight() && mClimbBehavior.isClimbing())
        {
            ToggleAttach();
        }
    }

    //Toggle the attach state and the material of the Player
    public void ToggleAttach()
    {
        mAttached = !mAttached;

        if (mAttached)
        {
            //mMeshRenderer.material = _AttachedMaterial;
            mClimbBehavior.ToggleGravity(false);
        }
        else
        {
            //mMeshRenderer.material = _DetachedMaterial;
            mClimbBehavior.ToggleGravity(true);
            mClimbBehavior.FreezeClimbing();
            mClimbBehavior.ChangeState(MoveState.FALL);
        }
        mClimbBehavior.setEnabled(mAttached);
    }

    //Similar to ToggleAttach but explicitly ENABLES
    public void AllowClimb()
    {
        //mMeshRenderer.material = _AttachedMaterial;
        mClimbBehavior.setEnabled(true);
        mClimbBehavior.ToggleGravity(false);
    }
}
