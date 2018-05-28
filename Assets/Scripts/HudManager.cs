using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    public Text TextScore;
    public Text TextLevel;
    public Text TextLines;

    // Update is called once per frame
    private void Update()
    {
        TextScore.text = FindObjectOfType<ScoreManager>().TotalScore.ToString();
        TextLevel.text = FindObjectOfType<Game>().CurrentLevel.ToString();
        TextLines.text = FindObjectOfType<Game>().LinesCleaned.ToString();
    }
}