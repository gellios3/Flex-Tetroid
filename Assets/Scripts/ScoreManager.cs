using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int[] ScoreValues = {50, 150, 300, 1200};

    public AudioClip CleranLineSound;

    public int RowsCount { get; set; }

    public int TotalScore { get; set; }

    private void Update()
    {
        UpdateScore();
    }

    private void UpdateScore()
    {
        if (RowsCount <= 0) return;
        var gameInstance = FindObjectOfType<Game>();
        gameInstance.LinesCleaned += RowsCount;
        TotalScore += ScoreValues[RowsCount - 1] + gameInstance.CurrentLevel * (10 + RowsCount * 10);
        RowsCount = 0;
        gameInstance.PlaySound(CleranLineSound);
    }
}