using UnityEngine;

public enum TileType
{
    Normal,
    Start,
    Win,
    Trap,        // Moves you BACK
    Boost,       // Moves you FORWARD
    BackToStart  // The "Death" tile
}

public class TileScript : MonoBehaviour
{
    [Header("Tile Settings")]
    public TileType type = TileType.Normal;

    [Tooltip("How many steps to move (Negative for Trap, Positive for Boost)")]
    public int effectAmount = 0;

    // Optional: Change color in Editor so you can see them easily
    void OnDrawGizmos()
    {
        if (type == TileType.Win) { Gizmos.color = Color.green; Gizmos.DrawSphere(transform.position + Vector3.up, 0.5f); }
        if (type == TileType.Trap) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position + Vector3.up, 0.5f); }
        if (type == TileType.Boost) { Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(transform.position + Vector3.up, 0.5f); }
    }
}