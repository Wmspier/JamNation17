using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class RopeBehavior : MonoBehaviour {

    public GameObject target1;
    public GameObject target2;
    
    public int numberOfNodes = 10;
    public GameObject nodePrefab;
    
    List<GameObject> nodeList = new List<GameObject>();
    
    
    void Start()
    {
        nodeList.Add(target1);
        GameObject previousNode = target1;
        for(int i = 0; i < numberOfNodes; i++)
        {
            GameObject newNode = (GameObject) Instantiate(nodePrefab, target1.transform.position, Quaternion.identity);
            newNode.GetComponent<HingeJoint2D>().connectedBody = previousNode.GetComponent<Rigidbody2D>();
            nodeList.Add(newNode);
            Vector3 test = target1.transform.position + new Vector3((i) * 2, 0, 0);
            previousNode = newNode;
        }
        target2.GetComponent<HingeJoint2D>().connectedBody = previousNode.GetComponent<Rigidbody2D>();
        numberOfNodes ++; //To add the target1 node !
        this.GetComponent<LineRenderer>().SetVertexCount( numberOfNodes);
    }
    
    void Update() {
        for(int i = 0; i < numberOfNodes; i++)
        {
            this.GetComponent<LineRenderer>().SetPosition(i, nodeList[i].transform.position);
        }
    
    }
    
}
