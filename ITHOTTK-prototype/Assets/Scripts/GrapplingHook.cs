using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GrapplingHook : MonoBehaviour
{
    [Header("Grapple Settings")]
    public float maxDistance = 5f;
    public float ropeExtendSpeed = 20f;
    public float pullSpeed = 8f;
    public float stopDistance = 0.2f;
    public LayerMask grappleLayer;
    public KeyCode fireKey = KeyCode.Mouse1;

    [Header("Optional")]
    public float grappleCooldown = 0.5f;

    private Rigidbody2D rb;
    private LineRenderer lineRenderer;

    private bool isFiring = false;
    private bool isGrappling = false;
    private Vector2 grapplePoint;
    private Vector2 currentRopeEnd;
    private float lastGrappleTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.sortingOrder = 10;
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(fireKey) && Time.time >= lastGrappleTime + grappleCooldown)
        {
            if (isGrappling || isFiring)
            {
                StopGrapple();
                return;
            }

            TryFireGrapple();
        }

        if (isFiring)
        {
            // Animate rope extending toward grapple point
            currentRopeEnd = Vector2.MoveTowards(currentRopeEnd, grapplePoint, ropeExtendSpeed * Time.deltaTime);
            UpdateRope();

            if (Vector2.Distance(currentRopeEnd, grapplePoint) < 0.05f)
            {
                isFiring = false;
                isGrappling = true;
            }
        }

        if (isGrappling)
        {
            UpdateRope();

            Vector2 direction = (grapplePoint - rb.position).normalized;
            rb.MovePosition(rb.position + direction * pullSpeed * Time.deltaTime);

            if (Vector2.Distance(rb.position, grapplePoint) < stopDistance)
            {
                StopGrapple();
            }
        }
    }

    void TryFireGrapple()
    {
        Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grappleLayer);

        if (hit.collider != null)
        {
            grapplePoint = hit.point;
            currentRopeEnd = transform.position;
            isFiring = true;
            lineRenderer.positionCount = 2;
            UpdateRope();
            lastGrappleTime = Time.time;
        }
        else
        {
            // Optional: play "miss" feedback
            lastGrappleTime = Time.time;
        }
    }

    void UpdateRope()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, currentRopeEnd);
    }

    public void StopGrapple()
    {
        isGrappling = false;
        isFiring = false;
        lineRenderer.positionCount = 0;
    }
}