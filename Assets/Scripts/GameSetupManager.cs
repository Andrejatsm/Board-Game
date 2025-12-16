using UnityEngine;
using System.Collections.Generic;

public class GameSetupManager : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject[] playerPrefabs;
    public Transform spawnPoint;

    [Header("Adjustments")]
    public float playerScale = 0.5f;
    public float heightOffset = 0.5f;

    [Header("File IO")]
    private const string textFileName = "PlayerNames";

    void Start()
    {
        // --- 1. THE GHOST HUNTER ---
        // This checks if another script ran before us and created players.
        var existingPlayers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        if (existingPlayers.Length > 0)
        {
            Debug.LogError("⚠️ GHOSTS DETECTED! Found " + existingPlayers.Length + " players already in the scene. Destroying them...");
            foreach (var p in existingPlayers)
            {
                Destroy(p.gameObject);
            }
        }

        Debug.Log(">>> GAME SETUP STARTED ON: " + gameObject.name);

        List<PlayerController> activePlayers = new List<PlayerController>();

        // --- 2. CHECK THE NAME ---
        string savedName = PlayerPrefs.GetString("PlayerName", "Player");
        Debug.Log(">>> ATTEMPTING TO SPAWN HUMAN WITH NAME: " + savedName);

        // If this prints "Hjustons" or a random name, your Main Menu is saving wrong.
        // If this prints "Player", your Main Menu isn't saving at all.
        // If this prints your name (e.g. "Jeff"), but you SEE "Hjustons", you have a Ghost Script running AFTER this one.

        // --- 3. SPAWN HUMAN ---
        int characterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        if (characterIndex >= playerPrefabs.Length) characterIndex = 0;

        Vector3 startPos = spawnPoint.position + (Vector3.up * heightOffset);
        GameObject mainCharObj = Instantiate(playerPrefabs[characterIndex], startPos, Quaternion.identity);
        mainCharObj.transform.localScale = Vector3.one * playerScale;

        mainCharObj.GetComponent<NameScript>().SetName(savedName);
        activePlayers.Add(mainCharObj.GetComponent<PlayerController>());

        // --- 4. SPAWN BOTS ---
        int totalPlayers = PlayerPrefs.GetInt("PlayerCount", 2);
        string[] nameArray = ReadLinesFromFile(textFileName);

        for (int i = 0; i < totalPlayers - 1; i++)
        {
            spawnPoint.position += new Vector3(1.2f, 0, 0.08f);
            int randomIndex = Random.Range(0, playerPrefabs.Length);

            GameObject botObj = Instantiate(playerPrefabs[randomIndex], spawnPoint.position + (Vector3.up * heightOffset), Quaternion.identity);
            botObj.transform.localScale = Vector3.one * playerScale;

            string randomName = "Bot " + (i + 1);
            if (nameArray.Length > 0) randomName = nameArray[Random.Range(0, nameArray.Length)];

            botObj.GetComponent<NameScript>().SetName(randomName);
            activePlayers.Add(botObj.GetComponent<PlayerController>());
        }

        // --- 5. INITIALIZE TURN MANAGER ---
        TurnManager turnManager = FindFirstObjectByType<TurnManager>();
        if (turnManager != null)
        {
            turnManager.InitializePlayers(activePlayers.ToArray());
        }
    }

    string[] ReadLinesFromFile(string fileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);
        if (textAsset != null)
            return textAsset.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        return new string[0];
    }
}