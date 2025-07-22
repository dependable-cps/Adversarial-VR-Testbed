using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int score = 0;
    [SerializeField] private Text scoreText;

    void Awake()
    {
        scoreText.text = "0";
        Instance = this;
    }

    public void AddScore(int value)
    {
        score += value;
        scoreText.text = score.ToString();
        Debug.Log("Score: " + score);
    }
}