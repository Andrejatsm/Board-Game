using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int currentTileIndex = 0;
    public float moveSpeed = 3f;

    public IEnumerator MoveSteps(Board board, int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            currentTileIndex++;

            if (currentTileIndex >= board.TileCount)
                currentTileIndex = board.TileCount - 1;

            Vector3 target = board.GetTile(currentTileIndex).position;

            while (Vector3.Distance(transform.position, target) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            yield return new WaitForSeconds(0.15f);
        }
    }
}
