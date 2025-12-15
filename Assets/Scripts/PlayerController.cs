using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public int currentTileIndex = 0;
    public float moveSpeed = 3f;

    // How high ABOVE the tile surface to float
    public float heightOffset = 1.0f;

    [Header("Animation Settings")]
    public string moveAnimationParameter = "Walk";

    private Animator anim;
    private Rigidbody rb;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Disable physics gravity so code has 100% control
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    public IEnumerator MoveSteps(Board board, int steps)
    {
        if (anim != null) anim.SetBool(moveAnimationParameter, true);

        for (int i = 0; i < steps; i++)
        {
            currentTileIndex++;

            // Safety Check
            if (currentTileIndex >= board.TileCount)
                currentTileIndex = board.TileCount - 1;

            // 1. Get the Tile
            Transform targetTile = board.GetTile(currentTileIndex);

            // 2. Calculate Target Position
            // We take the tile's position and ADD the offset to the Y axis.
            Vector3 targetPos = targetTile.position;
            targetPos.y += heightOffset;

            // 3. Move Loop
            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            yield return new WaitForSeconds(0.15f);
        }

        if (anim != null) anim.SetBool(moveAnimationParameter, false);
    }


    // NEW FUNCTION: Moves player directly to a specific tile (Sliding effect)
    public IEnumerator SlideToTile(Board board, int targetIndex)
    {
        // --- START ANIMATION ---
        if (anim != null) anim.SetBool(moveAnimationParameter, true);

        // 1. Update Logic
        currentTileIndex = targetIndex;

        // Clamp to ensure we don't go out of bounds
        if (currentTileIndex < 0) currentTileIndex = 0;
        if (currentTileIndex >= board.TileCount) currentTileIndex = board.TileCount - 1;

        // 2. Get Visual Target
        Transform targetTile = board.GetTile(currentTileIndex);

        // Use your relative height offset logic
        Vector3 targetPos = targetTile.position;
        targetPos.y += heightOffset;

        // 3. Move Smoothly (Faster than walking)
        float slideSpeed = moveSpeed * 2f;

        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                slideSpeed * Time.deltaTime
            );
            yield return null;
        }

        // --- STOP ANIMATION ---
        if (anim != null) anim.SetBool(moveAnimationParameter, false);
    }
}