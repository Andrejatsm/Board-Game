using UnityEngine;

public class DiceRollScript : MonoBehaviour
{
    Rigidbody rBody;
    Animator anim; // Reference to the Animator
    Vector3 startPosition;

    [Header("Settings")]
    // Lowered these defaults significantly for Impulse mode
    [SerializeField] private float maxRandForcVal = 50f;
    [SerializeField] private float startRollingForce = 15f;

    float forceX, forceY, forceZ;

    public string diceFaceNum;
    public bool isLanded = false;
    public bool firstThrow = false;

    public int RolledValue
    {
        get
        {
            if (int.TryParse(diceFaceNum, out int result))
                return result;
            return 1;
        }
    }

    void Awake()
    {
        startPosition = transform.position;
        Initialize();
    }

    private void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>(); // Find the animator

        rBody.isKinematic = true;

        // When not rolling, we can enable the animator (if you have an idle animation)
        if (anim != null) anim.enabled = true;

        transform.rotation = new Quaternion(
            Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360), 0);
    }



    // ... (rest of variables)

    public void Roll()
    {
        // 1. DISABLE ANIMATOR
        if (anim != null) anim.enabled = false;

        // 2. Reset state
        isLanded = false;
        diceFaceNum = "";

        // 3. Enable Physics
        rBody.isKinematic = false;

        // 4. Calculate Forces
        // For torque, 50-100 is usually plenty for a spin
        forceX = Random.Range(0, maxRandForcVal);
        forceY = Random.Range(0, maxRandForcVal);
        forceZ = Random.Range(0, maxRandForcVal);

        // 5. Apply Forces
        // FIX: Removed the hardcoded '500' and used smaller numbers suitable for Impulse
        float jumpForce = Random.Range(5f, startRollingForce);

        rBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        rBody.AddTorque(new Vector3(forceX, forceY, forceZ), ForceMode.Impulse);
    }

    public void ResetDice()
    {
        transform.position = startPosition;
        firstThrow = false;
        isLanded = false;
        Initialize(); // This re-enables the Animator and IsKinematic
    }

    void Update()
    {
        // Debug Click-to-Roll Logic
        if (rBody != null)
        {
            if (Input.GetMouseButtonDown(0) && (isLanded || !firstThrow))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                    {
                        if (!firstThrow) firstThrow = true;
                        Roll();
                    }
                }
            }
        }
    }
}