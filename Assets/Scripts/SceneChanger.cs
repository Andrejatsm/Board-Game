using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public SaveLoadScript saveLoadScript;
    public FadeScript fadeScript;

    public void CloseGame()
    {
        StartCoroutine(Delay("quit", -1, ""));
    }

    public IEnumerator Delay(string command, int characterIndex, string characterName)
    {
        if (string.Equals(command, "quit", System.StringComparison.OrdinalIgnoreCase))
        {
            // Make sure fade works even if timeScale is 0 (FadeScript should use unscaled time).
            if (fadeScript != null) yield return fadeScript.FadeOut(0.1f);
            // Restore timescale so editor/build isn't stuck
            Time.timeScale = 1f;

            PlayerPrefs.DeleteAll();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        else if (string.Equals(command, "play", System.StringComparison.OrdinalIgnoreCase))
        {
            if (fadeScript != null) yield return fadeScript.FadeOut(0.1f);

            // Ensure game time is normal before loading
            Time.timeScale = 1f;

            if (saveLoadScript != null) saveLoadScript.SaveGame(characterIndex, characterName);
            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
        else if (string.Equals(command, "menu", System.StringComparison.OrdinalIgnoreCase))
        {
            if (fadeScript != null) yield return fadeScript.FadeOut(0.1f);

            // Restore timescale before switching to menu
            Time.timeScale = 1f;

            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }

    public void GoToMenu()
    {
        StartCoroutine(Delay("menu", -1, ""));
    }

    IEnumerator Quit()
    {
        yield return new WaitForSeconds(0.8f);
        Application.Quit();
    }
}
