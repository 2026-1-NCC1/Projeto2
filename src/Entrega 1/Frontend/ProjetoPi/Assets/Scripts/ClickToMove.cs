using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PointClickMovement : MonoBehaviour
{
    public static PointClickMovement instance;

    public float speed = 5f;
    public float rotationSpeed = 10f;
    public float stoppingDistance = 0.2f;
    public float interactionDistance = 1.5f;

    public Transform holdPoint;

    [Header("Layer do Item")]
    public LayerMask itemLayer;

    private Rigidbody rb;
    private Vector3 target;
    private bool isMoving = false;

    private PickableItem targetItem = null;
    private PickableItem heldItem = null;
    private Panela panelaAlvo = null;

    private PickableItem currentHoverItem;

    void Start()
    {
        instance = this;

        rb = GetComponent<Rigidbody>();
        target = transform.position;

        rb.useGravity = false;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;

        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ |
                         RigidbodyConstraints.FreezePositionY;

        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        HandleHover();

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 👉 ITEM
                PickableItem item = hit.collider.GetComponent<PickableItem>();
                if (item == null)
                    item = hit.collider.GetComponentInParent<PickableItem>();

                if (item != null && heldItem == null)
                {
                    targetItem = item;

                    Vector3 dir = (item.transform.position - transform.position).normalized;
                    target = item.transform.position - dir * interactionDistance;

                    target.y = transform.position.y;
                    isMoving = true;
                    return;
                }

                // 👉 PANELA
                Panela p = hit.collider.GetComponentInParent<Panela>();

                if (p != null && heldItem != null)
                {
                    panelaAlvo = p;

                    Vector3 dir = (p.transform.position - transform.position).normalized;
                    target = p.transform.position - dir * interactionDistance;

                    target.y = transform.position.y;
                    isMoving = true;
                    return;
                }

                // 👉 CHÃO
                targetItem = null;
                panelaAlvo = null;

                target = hit.point;
                target.y = transform.position.y;
                isMoving = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (!isMoving)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 direction = (target - transform.position);
        direction.y = 0;

        float distance = direction.magnitude;

        if (distance <= stoppingDistance)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = target;

            isMoving = false;

            // 👉 PEGAR ITEM
            if (targetItem != null && heldItem == null)
            {
                targetItem.PickUp(holdPoint);
                heldItem = targetItem;
                targetItem = null;
            }

            // 👉 COLOCAR NA PANELA
            if (panelaAlvo != null && heldItem != null)
            {
                panelaAlvo.ColocarItem(heldItem);
                heldItem = null;
                panelaAlvo = null;
            }

            return;
        }

        direction.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        rb.linearVelocity = new Vector3(direction.x, 0, direction.z) * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        rb.linearVelocity = Vector3.zero;
        isMoving = false;
    }

    void HandleHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        PickableItem newItem = null;

        // 👉 IMPORTANTE: usa Layer
        if (Physics.Raycast(ray, out hit, 100f, itemLayer))
        {
            newItem = hit.collider.GetComponent<PickableItem>();

            if (newItem == null)
                newItem = hit.collider.GetComponentInParent<PickableItem>();
        }

        if (currentHoverItem != newItem)
        {
            if (currentHoverItem != null)
                currentHoverItem.SetHighlight(false);

            if (newItem != null && !EstaSegurandoItem())
                newItem.SetHighlight(true);

            currentHoverItem = newItem;
        }
    }

    public bool EstaSegurandoItem()
    {
        return heldItem != null;
    }
}