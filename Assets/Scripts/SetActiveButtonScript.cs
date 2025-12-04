using System.Collections;
using UnityEngine;

public class SetActiveButtonScript : MonoBehaviour
{
    public GameObject targetObject;
    
    public void ToggleActiveAfterDelay(float delay)
    {
        StartCoroutine(ToggleActiveCaroutine(delay));
    }

    private IEnumerator ToggleActiveCaroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        targetObject.SetActive(!targetObject.activeSelf);
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
