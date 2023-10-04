using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameStartButton : MonoBehaviour
{
    private bool isGamePaused = false;
    public Button button;
    public TextMeshProUGUI buttonText;

    private void Start()
    {
        isGamePaused = Time.timeScale == 0f;
        if (isGamePaused)
        {
            buttonText.text = "Resume";
        }
    }
    public void PauseGame()
    {
        if (isGamePaused)
        {
            // Resume the game
            Time.timeScale = 1f;
            buttonText.text = "Pause";
        }
        else
        {
            // Pause the game
            Time.timeScale = 0f;
            buttonText.text = "Resume";
        }

        isGamePaused = !isGamePaused;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("CarsScene");
    }

    public void GoToDriveSCene()
    {
        SceneManager.LoadScene("NewScene");
    }
}
