using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LeaderboardDisplay : MonoBehaviour
{
    public LeaderboardManager manager;
    public Text leaderboardText; // Drag a big Text object here

    void Start()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (manager == null || leaderboardText == null) return;

        List<PlayerEntry> topPlayers = manager.GetTopPlayers();
        leaderboardText.text = "<b>TOP PLAYERS</b>\n\n";

        // Show top 5
        int count = 0;
        foreach (PlayerEntry entry in topPlayers)
        {
            if (count >= 5) break;
            leaderboardText.text += $"{count + 1}. {entry.name} : {entry.wins} Wins\n";
            count++;
        }
    }
}