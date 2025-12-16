using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for changing Button colors

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Settings")]
    public string gameSceneName = "Level1"; // Make sure this matches your Scene name exactly

    [Header("UI References")]
    public Image characterPreviewImage; // Drag your UI Image here to show which char is selected
    public Sprite[] characterSprites;   // Drag the sprites of your characters here (Warrior, Orc, etc.)

    [Header("Buttons")]
    public Button[] playerCountButtons; // Drag your 2, 3, and 4 player buttons here
    public Color selectedColor = Color.green;
    public Color normalColor = Color.white;

    private int currentCharacterIndex = 0;
    private int playerCount = 2; // Default to 2

    void Start()
    {
        // Load saved values or defaults
        playerCount = PlayerPrefs.GetInt("PlayerCount", 2);
        currentCharacterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);

        UpdatePlayerCountUI();
        UpdateCharacterUI();
    }

    // --- PLAYER COUNT FUNCTIONS ---

    public void SetPlayerCount(int count)
    {
        playerCount = count;
        PlayerPrefs.SetInt("PlayerCount", playerCount);
        UpdatePlayerCountUI();
    }

    void UpdatePlayerCountUI()
    {
        // Reset all buttons to white
        foreach (Button btn in playerCountButtons)
        {
            btn.GetComponent<Image>().color = normalColor;
        }

        // Highlight the selected one based on count (2->index 0, 3->index 1, 4->index 2)
        // Assumption: Button 0 is "2 Players", Button 1 is "3 Players", etc.
        int buttonIndex = playerCount - 2;
        if (buttonIndex >= 0 && buttonIndex < playerCountButtons.Length)
        {
            playerCountButtons[buttonIndex].GetComponent<Image>().color = selectedColor;
        }
    }

    // --- CHARACTER SELECTION FUNCTIONS ---

    public void NextCharacter()
    {
        currentCharacterIndex++;
        if (currentCharacterIndex >= characterSprites.Length) currentCharacterIndex = 0;

        SaveCharacter();
        UpdateCharacterUI();
    }

    public void PrevCharacter()
    {
        currentCharacterIndex--;
        if (currentCharacterIndex < 0) currentCharacterIndex = characterSprites.Length - 1;

        SaveCharacter();
        UpdateCharacterUI();
    }

    void SaveCharacter()
    {
        PlayerPrefs.SetInt("SelectedCharacter", currentCharacterIndex);
    }

    void UpdateCharacterUI()
    {
        if (characterSprites.Length > 0 && characterPreviewImage != null)
        {
            characterPreviewImage.sprite = characterSprites[currentCharacterIndex];
            // Preserve aspect ratio so the sprite doesn't look squashed
            characterPreviewImage.preserveAspect = true;
        }
    }

    // --- START GAME ---

    public void PlayGame()
    {
        // Save everything one last time just to be safe
        PlayerPrefs.SetInt("PlayerCount", playerCount);
        PlayerPrefs.SetInt("SelectedCharacter", currentCharacterIndex);
        PlayerPrefs.Save();

        SceneManager.LoadScene(gameSceneName);
    }
}