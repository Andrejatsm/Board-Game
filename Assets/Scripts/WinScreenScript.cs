using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScreenScript : MonoBehaviour
{
    public Text winnerNameText;
    public GameObject winPanel;

    public void ShowWin(string winnerName)
    {
        winPanel.SetActive(true);
        winnerNameText.text = winnerName + " Wins!";

        // Save the win instantly
        FindObjectOfType<LeaderboardManager>().AddWin(winnerName);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}