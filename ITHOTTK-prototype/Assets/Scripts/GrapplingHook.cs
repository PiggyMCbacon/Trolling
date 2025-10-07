using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GrapplingHook : MonoBehaviour
{
    [Header("Grapple Settings")]
    public float maxDistance = 5f;
    public float ropeExtendSpeed = 25f;
    public float pullSpeed = 10f;
    public float retractSpeed = 40f;
    public float stopDistance = 0.2f;
    public LayerMask grappleLayer;
    public KeyCode fireKey = KeyCode.Mouse1;

    [Header("Cooldown")]
    public float grappleCooldown = 0.5f;

    [Header("References")]
    public Rigidbody2D playerBody; // Assign this to the player's Rigidbody2D in the Inspector

    private LineRenderer lineRenderer;

    private bool isFiring = false;
    private bool isGrappling = false;
    private bool didHit = false;

    private Vector2 grapplePoint;
    private Vector2 currentRopeEnd;
    private float lastGrappleTime;

    private int playerLayer;
    private int grappleLayerNumber;

    void Start()
    {
        if (playerBody == null)
        {
            playerBody = GetComponentInParent<Rigidbody2D>(); // fallback
            if (playerBody == null)
            {
                Debug.LogError("GrapplingHook: No Rigidbody2D found! Assign playerBody in the Inspector.");
            }
        }

        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.sortingOrder = 10;
        lineRenderer.positionCount = 0;

        // Cache player layer and grapple layer number for collision ignoring
        playerLayer = playerBody.gameObject.layer;
        grappleLayerNumber = LayerMaskToLayer(grappleLayer);
    }

    void Update()
    {
        // Fire or cancel grapple
        if (Input.GetKeyDown(fireKey) && Time.time >= lastGrappleTime + grappleCooldown)
        {
            if (isFiring || isGrappling)
            {
                StopGrapple();
            }
            else
            {
                FireGrapple();
            }
        }

        if (isFiring)
        {
            float speed = didHit ? ropeExtendSpeed : retractSpeed;
            currentRopeEnd = Vector2.MoveTowards(currentRopeEnd, grapplePoint, speed * Time.deltaTime);
            UpdateRope();

            if (didHit)
            {
                if (Vector2.Distance(currentRopeEnd, grapplePoint) < 0.1f)
                {
                    isFiring = false;
                    isGrappling = true;

                    // Disable collisions between player and grapple layer so we can pull through walls
                    Physics2D.IgnoreLayerCollision(playerLayer, grappleLayerNumber, true);
                }
            }
            else
            {
                if (Vector2.Distance(currentRopeEnd, grapplePoint) < 0.1f)
                {
                    StopGrapple();
                }
            }
        }

        if (isGrappling && playerBody != null)
        {
            UpdateRope();
            Vector2 direction = (grapplePoint - playerBody.position).normalized;
            playerBody.MovePosition(playerBody.position + direction * pullSpeed * Time.deltaTime);

            if (Vector2.Distance(playerBody.position, grapplePoint) < stopDistance)
            {
                StopGrapple();
            }
        }
    }

    void FireGrapple()
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorld - (Vector2)transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grappleLayer);

        if (hit.collider != null)
        {
            Vector2 hitPoint = hit.point;
            Vector2 hitNormal = hit.normal;

            // Offset grapple point slightly outside the collider so player doesn't get stuck
            grapplePoint = hitPoint + hitNormal * 0.1f;

            didHit = true;
        }
        else
        {
            grapplePoint = (Vector2)transform.position + direction * maxDistance;
            didHit = false;
        }

        isFiring = true;
        isGrappling = false;
        currentRopeEnd = transform.position;

        lineRenderer.positionCount = 2;
        UpdateRope();

        lastGrappleTime = Time.time;
    }

    void UpdateRope()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, currentRopeEnd);
    }

    public void StopGrapple()
    {
        isFiring = false;
        isGrappling = false;
        didHit = false;
        lineRenderer.positionCount = 0;

        // Re-enable collisions when grapple stops
        Physics2D.IgnoreLayerCollision(playerLayer, grappleLayerNumber, false);
    }

    int LayerMaskToLayer(LayerMask layerMask)
    {
        int layer = layerMask.value;
        int layerNumber = 0;
        while (layer > 1)
        {
            layer = layer >> 1;
            layerNumber++;
        }
        return layerNumber;
    }
}