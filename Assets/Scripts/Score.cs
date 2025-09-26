using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    private int score = 0;
    private bool scoreGoalReached = false;
    [SerializeField] public TextMeshProUGUI scoreText;


    public void AddScore(int _score)
    {
        score += _score;
        scoreText.text = $"{score}";

        if (score > 1000 && !scoreGoalReached)
        {
            scoreGoalReached = true;
            int currLevel = GameData.LoadPlayerLevel();
            GameData.SavePlayerLevel(currLevel + 1);
        }
    }
}
