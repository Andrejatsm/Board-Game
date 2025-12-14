using UnityEngine;

public class Board : MonoBehaviour
{
    public Transform[] tiles;

    void Awake()
    {
        tiles = new Transform[transform.childCount];
        for (int i = 0; i < tiles.Length; i++)
            tiles[i] = transform.GetChild(i);
    }

    public Transform GetTile(int index)
    {
        return tiles[Mathf.Clamp(index, 0, tiles.Length - 1)];
    }

    public int TileCount => tiles.Length;
}
