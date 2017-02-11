using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehavior : MonoBehaviour {

    public GameObject _TilePrefab;
    public int _GridWidth = 10;
    public int _GridHeight = 10;

    public int _MaxTileWidth;
    public int _MaxTileHeight;

    private RectTransform mTransform;
    private GameObject [][] mGrid;

	// Use this for initialization
	void Start () {
        Initialize ();
	}

    void Initialize ()
    {
        mTransform = gameObject.GetComponent<RectTransform> ();
        _MaxTileWidth = (int)(mTransform.rect.width / _GridWidth);
        _MaxTileHeight = (int)(mTransform.rect.height / _GridHeight);
    }

    void CreateGrid ()
    {
        for (int i = 0; i < _GridWidth; i++) {
            for (int j = 0; j < _GridHeight; j++) {
                GameObject newTile = Instantiate (_TilePrefab);
                RectTransform newTileTransform = newTile.GetComponent<RectTransform> ();
                newTileTransform.SetParent (mTransform);

            }
        }
    }
}
