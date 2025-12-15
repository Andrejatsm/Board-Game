using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public Transform target;

    [Header("Camera Offsets")]
    public Vector3 playerOffset = new Vector3(0, 18, -18); // High angle for player
    public Vector3 diceOffset = new Vector3(0, 10, -10);   // Closer angle for dice

    public float moveSpeed = 5f;
    private Vector3 currentOffset;

    void Start()
    {
        currentOffset = playerOffset;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Smoothly move to position
        Vector3 desiredPosition = target.position + currentOffset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * moveSpeed);

        // Always look at the target
        transform.LookAt(target);
    }

    // Call this when it's a player's turn
    public void FocusOnPlayer(Transform playerTransform)
    {
        target = playerTransform;
        currentOffset = playerOffset;
    }

    // Call this when the dice is rolling
    public void FocusOnDice(Transform diceTransform)
    {
        target = diceTransform;
        currentOffset = diceOffset;
    }
}