using System.Collections.Generic;

[System.Serializable]
public class PlayerEntry
{
    public string name;
    public int wins;

    public PlayerEntry(string n, int w)
    {
        name = n;
        wins = w;
    }
}

[System.Serializable]
public class LeaderboardList
{
    public List<PlayerEntry> entries = new List<PlayerEntry>();
}