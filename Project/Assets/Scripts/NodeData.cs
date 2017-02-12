using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class NodeData : MonoBehaviour {

    public GameObject _PaintingPrefab;
    public float _CollectionTime = 3.0f;
    public float _PaintingSpawnChance = 0.5f;
    public int _Score = 1;
    public bool _LastNode=false;

    private float mCollectionTimer = 0.0f;
    private int mFlashTimer = 0;
    private Transform mTransform;

    private bool mHasPainting = false;
    private bool mOccupied = false;

    private ScoringBehavior mScore;
    private Text mStealingText;
    private Slider mCollectionSlider;
    private GameObject mPainting;
    private MeshRenderer mMeshRenderer;

    void Awake()
    {
        mTransform = gameObject.GetComponent<Transform>();
    }
	// Use this for initialization
	void Start () {

        if (Random.Range(0, 10) <= (10 * _PaintingSpawnChance) && !_LastNode)
        {
            mHasPainting = true;
        }

        if (mHasPainting)
        {
            mPainting = Instantiate(_PaintingPrefab);
            mPainting.GetComponent<Transform>().SetParent(mTransform);
            Vector3 newPos = mTransform.position;
            newPos.z = 39;
            mPainting.GetComponent<Transform>().position = newPos;
            mMeshRenderer = mPainting.GetComponent<MeshRenderer>();
        }

        mScore = FindObjectOfType<ScoringBehavior>();
        mCollectionSlider = FindObjectOfType<Slider>();
        mStealingText = mCollectionSlider.gameObject.GetComponent<Transform>().GetChild(0).GetComponent<Text>();
        mStealingText.enabled = false;
	}

    void OnTriggerStay(Collider other)
    {
        if (mHasPainting) 
        {
            mStealingText.enabled = true;

            mCollectionTimer += Time.deltaTime;
            mFlashTimer ++;

            float frac = mCollectionTimer / _CollectionTime;
            mCollectionSlider.value = frac;

            if (mCollectionTimer >= _CollectionTime) 
            {
                mCollectionSlider.value = 0;
                mScore.IncrementScore(_Score);
                Destroy(mPainting);
                mHasPainting = false;
                mStealingText.enabled = false;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        mStealingText.enabled = false;
        if (mHasPainting)
        {
            mCollectionTimer = 0;
        }
    }
}
