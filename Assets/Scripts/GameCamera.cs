using UnityEngine;

public class GameCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 normalOffset = new Vector3(0, 12, -12);
    public Vector3 zoomOffset = new Vector3(0, 6, -6);

    public float moveSpeed = 4f;
    private Vector3 currentOffset;

    void Start()
    {
        currentOffset = normalOffset;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = target.position + currentOffset;
        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * moveSpeed);
        transform.LookAt(target);
    }

    public void SetTarget(Transform t)
    {
        target = t;
        currentOffset = normalOffset;
    }

    public void FocusOnPlayer(Transform t)
    {
        target = t;
        currentOffset = zoomOffset;
    }
}
