using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbNodeGeneration : MonoBehaviour {


    public GameObject _NodePrefab;
    public GameObject _Building;
    public Vector3 _BuildingRoot;
    public int _NumNodes;
    public int _HorizontalDisp;

    private Transform mTransform;
    private float mVerticalDispStep;

    void Awake() 
    {
        //mTransform = gameObject.GetComponent<Transform>();
        //GenerateNodes();
    }

    void GenerateNodes() 
    {
        float buildingHeight = _Building.GetComponent<MeshFilter>().mesh.bounds.size.y * _Building.GetComponent<Transform>().lossyScale.y;
        mVerticalDispStep = buildingHeight / (_NumNodes+1);

        for (int i = 0; i < _NumNodes; i++)
        {
            float randDisp = Random.Range(0, _HorizontalDisp / 2);
            int posNeg = Random.Range(0, 2);
            int sign = 0;
            if (posNeg == 0) sign = -1;
            else sign = 1;

            float horizontalDisplacement = randDisp * sign;

            GameObject newNode = Instantiate(_NodePrefab);
            newNode.name = "ClimbNode_" + i;
            Transform newNodeTransform = newNode.GetComponent<Transform>();
            Vector3 newNodePos = _BuildingRoot;
            newNodePos.y += mVerticalDispStep * i;
            newNodePos.x += horizontalDisplacement;
            newNodeTransform.position = newNodePos;
            newNodeTransform.SetParent(mTransform);
        }

    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
