using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    private int score = 0;
    [SerializeField] public TextMeshProUGUI scoreText;


    public void AddScore(int _score)
    {
        score += _score;
        scoreText.text = $"{score}";
    }
}
