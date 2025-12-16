using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // REMOVED: public SaveLoadScript saveLoadScript; (This caused the error)

    public FadeScript fadeScript;

    public void CloseGame()
    {
        StartCoroutine(Delay("quit", -1, ""));
    }

    public IEnumerator Delay(string command, int characterIndex, string characterName)
    {
        if (string.Equals(command, "quit", System.StringComparison.OrdinalIgnoreCase))
        {
            if (fadeScript != null) yield return fadeScript.FadeOut(0.1f);

            Time.timeScale = 1f;

            // This resets the session data (like player count) so next time it starts fresh
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

            Time.timeScale = 1f;

            // REMOVED: saveLoadScript.SaveGame(...) 
            // We deleted this line because ChooseCharacterScript already saved the data to PlayerPrefs!

            SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
        else if (string.Equals(command, "menu", System.StringComparison.OrdinalIgnoreCase))
        {
            if (fadeScript != null) yield return fadeScript.FadeOut(0.1f);

            Time.timeScale = 1f;

            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }

    public void GoToMenu()
    {
        StartCoroutine(Delay("menu", -1, ""));
    }
}