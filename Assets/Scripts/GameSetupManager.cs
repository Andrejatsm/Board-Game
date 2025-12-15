using UnityEngine;
using System.Collections.Generic;

public class GameSetupManager : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject[] playerPrefabs;
    public Transform spawnPoint;

    [Header("Adjustments")]
    [Tooltip("Size of the player (0.5 = 50% size)")]
    public float playerScale = 0.5f;
    [Tooltip("How high to lift player so feet aren't in ground")]
    public float heightOffset = 0.5f;

    [Header("File IO")]
    private const string textFileName = "PlayerNames";

    void Start()
    {
        List<PlayerController> activePlayers = new List<PlayerController>();

        // --- 1. SPAWN MAIN PLAYER ---
        int characterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        if (characterIndex >= playerPrefabs.Length) characterIndex = 0;

        // Calculate spawn position with the Height Offset
        Vector3 startPos = spawnPoint.position + (Vector3.up * heightOffset);

        GameObject mainCharObj = Instantiate(playerPrefabs[characterIndex], startPos, Quaternion.identity);

        // APPLY SCALING
        mainCharObj.transform.localScale = Vector3.one * playerScale;

        // Set Name & Add to list
        mainCharObj.GetComponent<NameScript>().SetName(PlayerPrefs.GetString("PlayerName", "Player"));
        activePlayers.Add(mainCharObj.GetComponent<PlayerController>());


        // --- 2. SPAWN OTHER PLAYERS (BOTS) ---
        int playerCount = PlayerPrefs.GetInt("PlayerCount", 1);
        string[] nameArray = ReadLinesFromFile(textFileName);

        for (int i = 0; i < playerCount - 1; i++)
        {
            // Move spawn point to the right for the next player
            spawnPoint.position += new Vector3(1.2f, 0, 0.08f);

            // Calculate new position with Height Offset
            Vector3 botPos = spawnPoint.position + (Vector3.up * heightOffset);

            int randomIndex = Random.Range(0, playerPrefabs.Length);
            GameObject otherObj = Instantiate(playerPrefabs[randomIndex], botPos, Quaternion.identity);

            // APPLY SCALING
            otherObj.transform.localScale = Vector3.one * playerScale;

            // Set Name & Add to list
            if (nameArray.Length > 0)
                otherObj.GetComponent<NameScript>().SetName(nameArray[Random.Range(0, nameArray.Length)]);

            activePlayers.Add(otherObj.GetComponent<PlayerController>());
        }

        // --- 3. CONNECT TO TURN MANAGER ---
        TurnManager turnManager = FindFirstObjectByType<TurnManager>();
        if (turnManager != null)
        {
            turnManager.InitializePlayers(activePlayers.ToArray());
        }
        else
        {
            Debug.LogError("TurnManager not found in scene!");
        }
    }

    string[] ReadLinesFromFile(string fileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);
        if (textAsset != null)
        {
            return textAsset.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        }
        return new string[0];
    }
}