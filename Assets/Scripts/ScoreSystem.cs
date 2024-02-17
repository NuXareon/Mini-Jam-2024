using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public TextMeshProUGUI ScoreText;

    public int score = 0;

    private void Update()
    {
        ScoreText.text = score.ToString();
    }
}
