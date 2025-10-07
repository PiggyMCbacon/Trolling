using UnityEngine;

public class Slingshot : MonoBehaviour
{
    public GameObject pebblePrefab;
    public float shootForce = 10f;
    public float cooldown = 1f;
    public KeyCode fireKey = KeyCode.Mouse0;

    private float lastShotTime = -999f;

    void Update()
    {
        if (Input.GetKeyDown(fireKey) && Time.time >= lastShotTime + cooldown)
        {
            ShootPebble();
            lastShotTime = Time.time;
        }
    }

    void ShootPebble()
    {
        Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        GameObject pebble = Instantiate(pebblePrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = pebble.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * shootForce, ForceMode2D.Impulse);
        Destroy(pebble, 2f); // Destroy after 2 seconds
    }
}