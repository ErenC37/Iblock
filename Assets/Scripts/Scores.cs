using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scores : MonoBehaviour
{
    public Text socreText;

    public static int currentScores_;

    void Start()
    {
        currentScores_ = 0;

        UpdateScoreText();

    }

    private void OnEnable()
    {
        GameEvents.AddScores += AddScores;
    }

    private void OnDisable()
    {
        GameEvents.AddScores -= AddScores;
    }

    private void AddScores(int socres)
    {
        currentScores_ += socres;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        socreText.text = currentScores_.ToString();
    }

}
