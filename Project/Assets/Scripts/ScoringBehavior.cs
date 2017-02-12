using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScoringBehavior : MonoBehaviour {

    private Text mText;
    private int mCurrentScore=0;
    private int mNumPlayers = 1;
    private int mNumPlayersAtTop = 0;
    private GameObject mEndGamePanel;
    private TimingBehavior mTimingBehavior;

    public string _ScorePrefix = "Score: ";
    public string _EndGamePrefix = "YOU REACHED THE TOP IN ";
    public string _EndGameMiddle = " SECONDS AND STOLE ";
    public string _EndGameSuffixSingular = " PAINTING! GOOD JOB!";
    public string _EndGameSuffixPlural = " PAINTINGS! GOOD JOB!";

	// Use this for initialization
	void Start () {
        mText = gameObject.GetComponent<Text>();
        mEndGamePanel = GameObject.FindGameObjectWithTag("EndGamePanel");
        mEndGamePanel.SetActive(false);
        mNumPlayers = GameObject.FindGameObjectsWithTag("Player").Length;
        mTimingBehavior = FindObjectOfType<TimingBehavior>();
	}

    public void IncrementScore(int amount)
    {
        mCurrentScore += amount;
        mText.text = _ScorePrefix + mCurrentScore;
    }

    public void EndGame(int finalScore)
    {
        int finalTime = mTimingBehavior.GetFinalTime();
        Text endGameText = mEndGamePanel.GetComponent<Transform>().GetChild(0).GetComponent<Text>();
        Debug.Log(_EndGamePrefix);
        if(finalScore==1)endGameText.text = _EndGamePrefix + finalTime + _EndGameMiddle + finalScore + _EndGameSuffixSingular;
        else endGameText.text =             _EndGamePrefix + finalTime + _EndGameMiddle + finalScore + _EndGameSuffixPlural;
        mEndGamePanel.SetActive(true);
    }

    public void PlayerReachedTop()
    {
        mNumPlayersAtTop++;
        if (mNumPlayersAtTop == mNumPlayers)
        {
            mTimingBehavior.StopTimer();
            EndGame(mCurrentScore);
        }
    }
}
