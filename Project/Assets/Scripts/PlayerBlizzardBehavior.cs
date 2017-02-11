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
    }

    //Toggle attach state if climbing
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

        mClimbBehavior.ToggleGravity(true);
        mClimbBehavior.setEnabled(mAttached);
    }

    //Similar to ToggleAttach but explicitly ENABLES
    public void AllowClimb()
    { 
        mMeshRenderer.material = _AttachedMaterial;
        mClimbBehavior.setEnabled(true);
        mClimbBehavior.ToggleGravity(false);
    }
}
