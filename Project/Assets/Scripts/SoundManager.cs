using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioClip _MainTheme;
    public AudioClip _StealSound;

    private AudioSource mMusicSource;
    private AudioSource mSoundSource;

	// Use this for initialization
	void Start () {

        mMusicSource = this.GetComponent<AudioSource>();
        mMusicSource.clip = _MainTheme;

        mSoundSource = GetComponent<Transform>().GetChild(0).GetComponent<AudioSource>();
        mSoundSource.clip = _StealSound;
        mSoundSource.loop = false;

        mMusicSource.Play();
        //_MainTheme.Play();
	}

    public void PlayStealSound()
    { 
        mSoundSource.Play();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
