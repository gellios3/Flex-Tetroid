using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSystem : MonoBehaviour
{
    public Text TextLevel;

    public void PlayAgain()
    {
        Game.IsStartDefault = Game.StartingLevel == 0;
        SceneManager.LoadScene("ModernGame");
    }

    public void OnSelectLevel(float value)
    {
        Game.StartingLevel = (int) value;
        TextLevel.text = value.ToString();
    }
}