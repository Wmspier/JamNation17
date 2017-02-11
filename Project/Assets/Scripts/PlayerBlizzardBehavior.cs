using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlizzardBehavior : MonoBehaviour {

    //Is the player holding on to the building
    private bool mAttached;
    private PlayerClimbBehavior mClimbBehavior;
    private MeshRenderer mMeshRenderer;
    private BlizzardBehavior mBlizzard;

    public Material _AttachedMaterial;
    public Material _DetachedMaterial;

    void Start ()
    {
        mClimbBehavior = gameObject.GetComponent<PlayerClimbBehavior> ();
        mMeshRenderer = gameObject.GetComponent<MeshRenderer> ();
        mBlizzard = GameObject.FindGameObjectWithTag("Blizzard").GetComponent<BlizzardBehavior>();
    }

    void Update ()
    {
        //DEBUG//
        if (Input.GetKeyDown (KeyCode.B))
        {
            ToggleAttach ();
        }
    }

    public void TestAttach ()
    {
        if (mClimbBehavior.isClimbing ())
        {
            ToggleAttach (); 
        }
    }

    //Toggle the attach state and the material of the Player
    public void ToggleAttach ()
    {
        mAttached = !mAttached;

        if (mAttached) 
        {
            mMeshRenderer.material = _AttachedMaterial;
        } 
        else 
        { 
            mMeshRenderer.material = _DetachedMaterial;
            mClimbBehavior.FreezeClimbing ();
        }

        mClimbBehavior.enabled = mAttached;
    }

    public void AllowClimb()
    { 
            mMeshRenderer.material = _AttachedMaterial;
        mClimbBehavior.enabled = true;
    }
}
