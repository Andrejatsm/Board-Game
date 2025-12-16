using UnityEngine;
using System.IO;
using System.Linq; // Needed for sorting
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour
{
    private string filePath;

    void Awake()
    {
        // Saves to a persistent folder on your computer (e.g., AppData)
        filePath = Path.Combine(Application.persistentDataPath, "leaderboard.json");
    }

    public void AddWin(string playerName)
    {
        LeaderboardList data = LoadData();

        // Check if player exists
        PlayerEntry existingPlayer = data.entries.Find(x => x.name == playerName);

        if (existingPlayer != null)
        {
            existingPlayer.wins++; // Add win
        }
        else
        {
            data.entries.Add(new PlayerEntry(playerName, 1)); // Create new
        }

        SaveData(data);
    }

    public List<PlayerEntry> GetTopPlayers()
    {
        LeaderboardList data = LoadData();
        // Sort by wins (Highest to Lowest)
        return data.entries.OrderByDescending(x => x.wins).ToList();
    }

    private void SaveData(LeaderboardList data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }

    private LeaderboardList LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<LeaderboardList>(json);
        }
        return new LeaderboardList(); // Return empty list if no file exists
    }
}