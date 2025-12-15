using UnityEngine;
using UnityEngine.UI; // Required for Legacy Text

public class RolledNumberScript : MonoBehaviour
{
    DiceRollScript diceRollScript;

    [SerializeField]
    Text rolledNumberText;

    void Awake()
    {
        diceRollScript = FindFirstObjectByType<DiceRollScript>();

        // AUTO-FIX: If you forgot to drag the Text into the slot, this finds it for you.
        if (rolledNumberText == null)
        {
            rolledNumberText = GetComponent<Text>();
        }
    }

    void Update()
    {
        // Safety check: make sure we actually found the dice script
        if (diceRollScript != null && rolledNumberText != null)
        {
            if (diceRollScript.isLanded)
            {
                // Ensure diceFaceNum is actually a string value
                rolledNumberText.text = diceRollScript.diceFaceNum.ToString();
            }
            else
            {
                rolledNumberText.text = "?";
            }
        }
    }
}