using UnityEngine;
using TMPro; // Needed for InputField

public class ChooseCharacterScript : MonoBehaviour
{
    public GameObject[] characters;
    public TMP_InputField inputField;
    public SceneChanger sceneChanger;

    // We removed 'playerCount' here so it doesn't overwrite your 2/3/4 player buttons.

    int characterIndex = 0;

    private void Awake()
    {
        characterIndex = 0;
        // Hide all characters, show the first one
        foreach (GameObject character in characters)
        {
            character.SetActive(false);
        }
        if (characters.Length > 0) characters[0].SetActive(true);
    }

    public void NextCharacter()
    {
        characters[characterIndex].SetActive(false);
        characterIndex++;
        if (characterIndex >= characters.Length) characterIndex = 0;
        characters[characterIndex].SetActive(true);
    }

    public void PreviousCharacter()
    {
        characters[characterIndex].SetActive(false);
        characterIndex--;
        if (characterIndex < 0) characterIndex = characters.Length - 1;
        characters[characterIndex].SetActive(true);
    }

    public void Play()
    {
        string nameToSave = inputField.text;

        // FIX 1: Allow short names (Your old code required > 3 letters, so "Bob" failed)
        if (nameToSave.Length > 0)
        {
            // FIX 2: Save the data explicitly
            PlayerPrefs.SetInt("SelectedCharacter", characterIndex);
            PlayerPrefs.SetString("PlayerName", nameToSave);

            // Force Unity to write this to disk immediately
            PlayerPrefs.Save();

            // Trigger your SceneChanger
            // (Keeping "play" assuming that is your animation trigger name)
            StartCoroutine(sceneChanger.Delay("play", characterIndex, nameToSave));
        }
        else
        {
            // If they didn't type a name, select the box so they know to type
            inputField.Select();
        }
    }
}