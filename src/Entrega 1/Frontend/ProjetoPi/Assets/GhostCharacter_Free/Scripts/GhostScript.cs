using UnityEngine;

public class GhostScript : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody rb;
    private Camera cam;
    private Animator animator;

    private Vector3 target;
    private bool isMoving = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        animator = GetComponent<Animator>();

        target = transform.position;

        // Verificações (evita NullReference)
        if (rb == null)
            Debug.LogError("Rigidbody não encontrado!");

        if (cam == null)
            Debug.LogError("Camera principal não encontrada!");

        if (animator == null)
            Debug.LogWarning("Animator não encontrado (animação será ignorada)");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                target = hit.point;
                target.y = transform.position.y;
                isMoving = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (!isMoving || rb == null) return;

        Vector3 direction = target - transform.position;

        if (direction.magnitude < 0.2f)
        {
            isMoving = false;

            // animação parar
            if (animator != null)
                animator.SetBool("isWalking", false);

            return;
        }

        direction = direction.normalized;

        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);

        // rotação
        if (direction != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(direction);
            rb.rotation = Quaternion.Slerp(rb.rotation, rot, 10f * Time.deltaTime);
        }

        // animação andar
        if (animator != null)
            animator.SetBool("isWalking", true);
    }
}