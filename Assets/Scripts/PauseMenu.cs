using UnityEngine;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject settingsPanel; // optional
    public SceneChanger sceneChanger; // drag in inspector

    bool isPaused = false;

    void Update()
    {
        // Support both new Input System and legacy Input Manager gracefully.
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        // New Input System path
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
#else
        // Legacy Input Manager path
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
#endif
    }

    void TogglePause()
    {
        if (!isPaused) PauseGame();
        else ResumeGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pausePanel != null) pausePanel.SetActive(true);
        // Freeze game time (UI code should use unscaled time if animated)
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenSettings()
    {
        // optional separate settings UI
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            if (pausePanel != null) pausePanel.SetActive(false);
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(true);
        }
    }

    public void BackToMenu()
    {
        // Ensure normal time before transitioning
        Time.timeScale = 1f;
        if (sceneChanger != null) sceneChanger.GoToMenu();
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        if (sceneChanger != null) sceneChanger.CloseGame();
    }
}
