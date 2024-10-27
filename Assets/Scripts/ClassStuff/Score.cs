using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public TextMeshProUGUI ScoreText;
    private int currentScore = 0;

    public void Increment()
    {
        currentScore++;
        ScoreText.text = currentScore.ToString();
    }
}
