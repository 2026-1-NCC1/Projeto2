using UnityEngine;

public class PickableItem : MonoBehaviour
{
    private bool isHeld = false;

    private Renderer rend;

    public Color highlightColor = Color.yellow;

    // 🔥 PREFABS DAS PRÓXIMAS FASES
    public GameObject carneAssadaPrefab;
    public GameObject carneQueimadaPrefab;

    void Start()
    {
        rend = GetComponent<Renderer>();

        if (rend != null)
        {
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", Color.black);
        }
    }

    public void SetHighlight(bool state)
    {
        if (isHeld || rend == null) return;

        if (state)
        {
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", highlightColor * 2f);
        }
        else
        {
            rend.material.SetColor("_EmissionColor", Color.black);
        }
    }

    public void PickUp(Transform holdPoint)
    {
        SetHighlight(false);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        isHeld = true;
    }

    public void DropToPosition(Vector3 pos)
    {
        isHeld = false;

        transform.SetParent(null);
        transform.position = pos;
        transform.rotation = Quaternion.identity;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        SetHighlight(false);
    }

    void Update()
    {
        if (isHeld)
        {
            float floatY = Mathf.Sin(Time.time * 2f) * 0.1f;
            transform.localPosition = new Vector3(0, floatY, 0);
        }
    }
}