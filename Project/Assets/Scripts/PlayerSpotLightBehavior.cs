﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpotLightBehavior : MonoBehaviour
{

    //Is the player holding on to the building
    private bool mAttached;
    private PlayerClimbBehavior mClimbBehavior;
    private MeshRenderer mMeshRenderer;
    private Light mSpotLight;
    private Transform mTransform;

    public Material _AttachedMaterial;
    public Material _DetachedMaterial;

    void Start()
    {
        mClimbBehavior = gameObject.GetComponent<PlayerClimbBehavior>();
        mMeshRenderer = gameObject.GetComponent<MeshRenderer>();
        mTransform = gameObject.GetComponent<Transform>();
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

            return (Vector3.Distance(hit.point, mTransform.position) <= 6);
        }
        else return false;
    }

    //Toggle attach state if climbing
    public void TestAttach()
    {
        if (IsWithinLight())
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
            mMeshRenderer.material = _AttachedMaterial;
        }
        else
        {
            mMeshRenderer.material = _DetachedMaterial;
            mClimbBehavior.FreezeClimbing();
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