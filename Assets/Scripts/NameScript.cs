using UnityEngine;
using TMPro; // 1. CHANGE THIS LINE (was using UnityEngine.UI;)

public class NameScript : MonoBehaviour
{
    // 2. CHANGE THE TYPE HERE
    // Was 'public Text nameText;'
    public TextMeshPro nameText;

    public string playerName;

    public void SetName(string name)
    {
        playerName = name;

        if (nameText != null)
        {
            nameText.text = name;
        }
    }
}