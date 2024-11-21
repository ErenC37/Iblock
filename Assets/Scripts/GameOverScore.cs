using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScore : MonoBehaviour
{
    public Text LastScoreText;
    private int LastScore;

    void Update() 
    {
        LastScore += Scores.currentScores_;
        UpdateLastScoreText();

    }

    private void UpdateLastScoreText()
    {
        LastScoreText.text = Scores.currentScores_.ToString();
    }
}
