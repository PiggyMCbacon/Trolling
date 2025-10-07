using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GrapplingHook : MonoBehaviour
{
    public float maxDistance = 5f;
    public float pullSpeed = 10f;
    public LayerMask grappleLayer;
    public KeyCode fireKey = KeyCode.Mouse1;

    private LineRenderer lineRenderer;
    private Rigidbody2D rb;
    private Vector2 grapplePoint;
    private bool isGrappling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();

        // Setup LineRenderer
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.sortingOrder = 10;
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(fireKey))
        {
            if (isGrappling)
            {
                StopGrapple();
                return;
            }

            Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grappleLayer);

            if (hit.collider != null)
            {
                grapplePoint = hit.point;
                isGrappling = true;
                lineRenderer.positionCount = 2;
            }
        }

        if (isGrappling)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, grapplePoint);

            Vector2 direction = (grapplePoint - rb.position).normalized;
            rb.MovePosition(rb.position + direction * pullSpeed * Time.deltaTime);

            if (Vector2.Distance(rb.position, grapplePoint) < 0.2f)
            {
                StopGrapple();
            }
        }
    }

    public void StopGrapple()
    {
        isGrappling = false;
        lineRenderer.positionCount = 0;
    }
}