using System.Collections;
using UnityEngine;

public class DiceRollScript : MonoBehaviour
{
    private Rigidbody rBody;
    private Vector3 startPosition;

    [Header("Roll Forces")]
    [SerializeField] private float maxRandForceVal = 10f;
    [SerializeField] private float startRollingForce = 8f; // reduced to reasonable impulse magnitude

    [Header("State")]
    public bool isLanded = false;
    public int rolledValue = 0;
    public string diceFaceNum = null; // set by SideDetectScript (collider name)

    private bool isRolling = false;

    void Awake()
    {
        rBody = GetComponent<Rigidbody>();
        startPosition = transform.position;
        ResetDice();
    }

    // Called by TurnManager
    public void Roll()
    {
        if (isRolling) return;

        ResetDice();

        isRolling = true;
        isLanded = false;
        rolledValue = 0;
        diceFaceNum = null;

        rBody.isKinematic = false;

        Vector3 torque = new Vector3(
            Random.Range(-maxRandForceVal, maxRandForceVal),
            Random.Range(-maxRandForceVal, maxRandForceVal),
            Random.Range(-maxRandForceVal, maxRandForceVal)
        );

        // Apply an upward impulse plus torque
        rBody.AddForce(Vector3.up * Random.Range(startRollingForce * 0.8f, startRollingForce), ForceMode.Impulse);
        rBody.AddTorque(torque, ForceMode.Impulse);
    }

    void FixedUpdate()
    {
        if (!isRolling) return;

        // Use Rigidbody.velocity / angularVelocity (Unity API)
        if (rBody.linearVelocity.sqrMagnitude < 0.05f && rBody.angularVelocity.sqrMagnitude < 0.05f)
        {
            // Let SideDetectScript assign diceFaceNum if present; otherwise fall back
            DetermineFaceValue();
            isRolling = false;
            isLanded = true;
            rBody.isKinematic = true;
        }
    }

    void DetermineFaceValue()
    {
        // If a side detector has set a collider name that can be parsed to an int, prefer it.
        if (!string.IsNullOrEmpty(diceFaceNum))
        {
            // Try to parse numeric names (e.g. "1", "3"), otherwise strip non-digits
            if (int.TryParse(diceFaceNum, out int parsed))
            {
                rolledValue = Mathf.Clamp(parsed, 1, 6);
                Debug.Log("Dice landed on (from collider name): " + rolledValue);
                return;
            }

            // Try to extract digits from the name (e.g. "Side_4")
            string digits = System.Text.RegularExpressions.Regex.Match(diceFaceNum, @"\d+").Value;
            if (!string.IsNullOrEmpty(digits) && int.TryParse(digits, out parsed))
            {
                rolledValue = Mathf.Clamp(parsed, 1, 6);
                Debug.Log("Dice landed on (from collider name digits): " + rolledValue);
                return;
            }

            // If the collider name is non-numeric, leave rolledValue determination to fallback
            Debug.Log("Collider name provided but not numeric: " + diceFaceNum);
        }

        // Fallback: simple random result
        rolledValue = Random.Range(1, 7);
        Debug.Log("Dice landed on (fallback random): " + rolledValue);
    }

    public void ResetDice()
    {
        rBody.isKinematic = true;
        transform.position = startPosition;
        transform.rotation = Random.rotation;

        isLanded = false;
        isRolling = false;
        rolledValue = 0;
        diceFaceNum = null;
    }
}
