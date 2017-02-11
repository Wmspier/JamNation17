using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BlizzardState
{
    SAFE,
    WARNING,
    DANGER,
    TOTAL
}

public class BlizzardBehavior : MonoBehaviour {

    public GameObject _WarningCube;
    public Material _SafeMaterial;
    public Material _WarningMaterial;
    public Material _DangerMaterial;

    public float _BlizzardDelay = 5f;
    public float _BlizzardDurration = 5f;
    public float _WarningTime = 0.5f;
    private float mBlizzardDelayTimer = 0f;
    private float mBlizzardTimer = 0f;

    private GameObject [] mPlayerList;
    private bool mEnabled;

	// Use this for initialization
	void Start () {
        mPlayerList = GameObject.FindGameObjectsWithTag ("Player");
        _WarningCube.GetComponent<MeshRenderer>().material = _SafeMaterial;
	}
	
	// Update is called once per frame
	void Update () {

        if (!mEnabled)
        {
            mBlizzardDelayTimer += Time.deltaTime;
            if (mBlizzardDelayTimer >= _BlizzardDelay)
            {
                mEnabled = true;
                ToggleWarningCube(BlizzardState.DANGER);
                mBlizzardDelayTimer = 0;
            }
            else if (mBlizzardDelayTimer >= _BlizzardDelay * _WarningTime && mBlizzardDelayTimer < _BlizzardDelay)
            {
                ToggleWarningCube(BlizzardState.WARNING);
                ResolvePlayerAttach();
            }
        }
        else
        {
            mBlizzardTimer += Time.deltaTime;
            if (mBlizzardTimer >= _BlizzardDurration)
            {
                mEnabled = false;
                ToggleWarningCube(BlizzardState.SAFE);
                mBlizzardTimer = 0;
                ResolvePlayerAttach();
            }
            else 
            {  
                TestPlayerAttach();
            }
        }
	}

    void ToggleWarningCube (BlizzardState state)
    {
        MeshRenderer cubeMeshRenderer = _WarningCube.GetComponent<MeshRenderer> ();
        switch (state) 
        {
        case BlizzardState.SAFE:
            cubeMeshRenderer.material = _SafeMaterial;
            break;
        case BlizzardState.WARNING:
            cubeMeshRenderer.material = _WarningMaterial;
            break;
        case BlizzardState.DANGER:
            cubeMeshRenderer.material = _DangerMaterial;
            break;
        default:
            break;
        }
    }

    void TestPlayerAttach ()
    {
        foreach (GameObject player in mPlayerList) 
        {
            PlayerBlizzardBehavior behavior = player.GetComponent<PlayerBlizzardBehavior> ();
            behavior.TestAttach ();
        }
    }

    void ResolvePlayerAttach()
    { 
        foreach (GameObject player in mPlayerList)
        {
            PlayerBlizzardBehavior behavior = player.GetComponent<PlayerBlizzardBehavior>();
            behavior.AllowClimb();
        }
    }
}
