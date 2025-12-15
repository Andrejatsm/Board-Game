using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Board : MonoBehaviour
{
    [Header("Randomization Settings")]
    [Range(0f, 1f)] public float trapChance = 0.15f;      // 15% chance
    [Range(0f, 1f)] public float boostChance = 0.10f;     // 10% chance
    [Range(0f, 1f)] public float backToStartChance = 0.05f; // 5% chance

    // Hide from Inspector so it doesn't get broken manually
    private Transform[] tiles;

    public int TileCount => tiles != null ? tiles.Length : 0;

    void Awake()
    {
        // 1. FIND TILES
        List<Transform> tileList = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Tile"))
            {
                tileList.Add(child);
            }
        }

        // Sort by hierarchy order
        tiles = tileList.OrderBy(t => t.GetSiblingIndex()).ToArray();

        Debug.Log($"Board Setup: Found {tiles.Length} tiles.");

        // 2. RANDOMIZE TILES
        RandomizeBoard();
    }

    void RandomizeBoard()
    {
        // We start at index 1 (skip Start) and end at Length-1 (skip Win)
        for (int i = 1; i < tiles.Length - 1; i++)
        {
            TileScript ts = tiles[i].GetComponent<TileScript>();

            // Safety check: Does the tile have the script?
            if (ts == null)
            {
                // Auto-add the script if it's missing
                ts = tiles[i].gameObject.AddComponent<TileScript>();
            }

            // IMPORTANT: Only randomize if you haven't manually set it to something else
            if (ts.type == TileType.Normal)
            {
                float roll = Random.value; // Returns 0.0 to 1.0

                // Check for "Death" Tile first (rarest)
                if (roll < backToStartChance)
                {
                    ts.type = TileType.BackToStart;
                }
                // Check for Trap
                else if (roll < backToStartChance + trapChance)
                {
                    ts.type = TileType.Trap;
                    ts.effectAmount = Random.Range(1, 4); // Push back 1 to 3 steps
                }
                // Check for Boost
                else if (roll < backToStartChance + trapChance + boostChance)
                {
                    ts.type = TileType.Boost;
                    ts.effectAmount = Random.Range(1, 4); // Boost forward 1 to 3 steps
                }
            }
        }
    }

    public Transform GetTile(int index)
    {
        if (tiles == null || tiles.Length == 0) return transform;
        if (index >= tiles.Length) index = tiles.Length - 1;
        if (index < 0) index = 0;
        return tiles[index];
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }
    }
}