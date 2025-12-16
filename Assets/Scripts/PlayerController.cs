using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public int currentTileIndex = 0;
    public float moveSpeed = 3f;
    public float heightOffset = 1.0f;

    [Header("Animation Settings")]
    public string moveAnimationParameter = "Walk";
    public string deathAnimationParameter = "Die"; 
    
    // NEW: Add the name of your Idle bool here (e.g. "Idle" or "OrcIdle")
    public string idleAnimationParameter = "Idle"; 

    private Animator anim;
    private Rigidbody rb;
    private SpriteRenderer spriteRenderer; 

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>(); 

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    public IEnumerator MoveSteps(Board board, int steps)
    {
        // Ensure Idle is false when we start walking
        if (anim != null) 
        {
            anim.SetBool(idleAnimationParameter, false);
            anim.SetBool(moveAnimationParameter, true);
        }

        for (int i = 0; i < steps; i++)
        {
            currentTileIndex++;
            if (currentTileIndex >= board.TileCount) currentTileIndex = board.TileCount - 1;

            Transform targetTile = board.GetTile(currentTileIndex);
            Vector3 targetPos = targetTile.position;
            targetPos.y += heightOffset;

            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(0.15f);
        }

        if (anim != null) 
        {
            anim.SetBool(moveAnimationParameter, false);
            // Return to Idle
            anim.SetBool(idleAnimationParameter, true);
        }
    }

    public IEnumerator SlideToTile(Board board, int targetIndex)
    {
        if (anim != null) 
        {
            anim.SetBool(idleAnimationParameter, false);
            anim.SetBool(moveAnimationParameter, true);
        }

        currentTileIndex = targetIndex;
        if (currentTileIndex < 0) currentTileIndex = 0;
        if (currentTileIndex >= board.TileCount) currentTileIndex = board.TileCount - 1;

        Transform targetTile = board.GetTile(currentTileIndex);
        Vector3 targetPos = targetTile.position;
        targetPos.y += heightOffset;

        float slideSpeed = moveSpeed * 2f;

        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, slideSpeed * Time.deltaTime);
            yield return null;
        }

        if (anim != null) 
        {
            anim.SetBool(moveAnimationParameter, false);
            anim.SetBool(idleAnimationParameter, true);
        }
    }

    // --- UPDATED DEATH SEQUENCE ---
    public IEnumerator DeathAndRespawn(Board board)
    {
        if (anim != null) 
        {
            // 1. Slow down the animation so it's not too fast (0.5 = Half Speed)
            anim.speed = 0.5f;
            
            // Turn off other states to be safe
            anim.SetBool(idleAnimationParameter, false);
            anim.SetBool(moveAnimationParameter, false);
            
            // Trigger the Death
            anim.SetTrigger(deathAnimationParameter);
        }

        // 2. Wait LONGER so the animation has time to finish (2 seconds)
        yield return new WaitForSeconds(2.0f);

        // 3. Hide Player
        if (spriteRenderer != null) spriteRenderer.enabled = false;

        // Reset speed to normal while invisible
        if (anim != null) anim.speed = 1.0f;

        // 4. Teleport to Start
        currentTileIndex = 0; 
        Vector3 startPos = board.GetTile(0).position;
        startPos.y += heightOffset;
        transform.position = startPos;

        // 5. Wait invisible (Respawn time)
        yield return new WaitForSeconds(1.0f);

        // 6. Show Player
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        
        // 7. FIX: Force the "Idle" bool to True so it transitions out of Death
        if (anim != null) 
        {
            // Reset the Die trigger just in case
            anim.ResetTrigger(deathAnimationParameter);
            
            // This is the key line to fix your issue:
            anim.SetBool(idleAnimationParameter, true); 
            
            // Force the state machine to jump to "Idle" immediately
            anim.Play("Idle", 0, 0f); 
        }
    }
}