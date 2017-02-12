using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioClip _MainTheme;

    private AudioSource mSource;

	// Use this for initialization
	void Start () {

        mSource = this.GetComponent<AudioSource>();
        mSource.clip = _MainTheme;
        mSource.Play();
        //_MainTheme.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
