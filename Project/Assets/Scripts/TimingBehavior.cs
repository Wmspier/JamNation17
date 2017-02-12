using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimingBehavior : MonoBehaviour {

    public string _TimerPreffix = "Time: ";

    private float mCurrentTime=0;
    private float mTimerStart;
    private Text mText;
    private bool mTiming = true;

    public int GetFinalTime() { return (int)mCurrentTime; }

    void Start()
    {
        mTimerStart = Time.time;
        mText = gameObject.GetComponent<Text>();
    }

	// Update is called once per frame
	void Update () {

        if (mTiming)
        {
            mCurrentTime = -(mTimerStart - Time.time);
            mCurrentTime = Mathf.Round(mCurrentTime * 10.0f) / 10.0f;
            mText.text = _TimerPreffix + mCurrentTime;
        }
	}

    public void StopTimer()
    {
        mTiming = false;
    }
}
