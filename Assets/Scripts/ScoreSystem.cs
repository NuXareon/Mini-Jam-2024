using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public TextMeshProUGUI ScoreText;
    public BackgroundManager Background;
    public int ScoreBackgroundChange = 1;

    private int score = 0;

    private void Update()
    {
        ScoreText.text = score.ToString();
    }

    public void IncreaseScore(int increase)
    {
        int scoreMod = score % ScoreBackgroundChange;
        if (scoreMod + increase >= ScoreBackgroundChange)
        {
            Background.SetNextBackground();
        }
        score += increase;
    }
}
