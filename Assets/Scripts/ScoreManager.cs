using UnityEngine;
using TMPro;

public class ScoreManager : GenericSingleton<ScoreManager>
{
    [SerializeField] TextMeshProUGUI scoreText;
    int score;

    public int Score
    {
        get { return score; }
        set { score = value; }
    }

    private void Update()
    {
        scoreText.text = score.ToString();
    }
}
