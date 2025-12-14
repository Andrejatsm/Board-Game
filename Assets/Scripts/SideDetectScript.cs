using UnityEngine;

public class SideDetectScript : MonoBehaviour
{
    DiceRollScript diceRollScript;
    Rigidbody diceRb;

    void Awake()
    {
        diceRollScript = FindFirstObjectByType<DiceRollScript>();
        if (diceRollScript != null)
            diceRb = diceRollScript.GetComponent<Rigidbody>();
    }

    private void OnTriggerStay(Collider sideCollider)
    {
        if (diceRollScript == null || diceRb == null) return;

        // consider the dice "stationary" when both linear and angular velocities are very small
        if (diceRb.linearVelocity.sqrMagnitude < 0.01f && diceRb.angularVelocity.sqrMagnitude < 0.01f)
        {
            diceRollScript.isLanded = true;
            diceRollScript.diceFaceNum = sideCollider.name;
        }
        else
        {
            diceRollScript.isLanded = false;
        }
    }
}
