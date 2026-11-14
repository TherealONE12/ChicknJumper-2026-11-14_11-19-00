using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody2D body;

    // Start direction in degrees: 0 = right, 90 = up, -45 = down right, -135 = down left
    [SerializeField] private float startDirection = -45f;

    // Bounce angle relative to the hit surface in degrees (45 = diagonal)
    [SerializeField] private float bounceDirection = 45f;

    // every bouncy obj needs a special tag to be bouncy
    [SerializeField] private string bounceTag = "bounce";

    private Vector2 lastSpeed;

    void Awake()
    {
        if (body == null) body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f;
        body.linearDamping = 0f;
    }

    void Start()
    {
        // starting direction
        float rad = startDirection * Mathf.Deg2Rad;
        Vector2 startDir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        body.linearVelocity = startDir * speed;
    }

    void FixedUpdate()
    {
        // get the speed before the collision
        lastSpeed = body.linearVelocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag(bounceTag))
        {
            // die
            Debug.Log("Player collided with " + collision.collider.name);
            return;
        }

        Vector2 normal = collision.GetContact(0).normal;
        Vector2 tangent = new Vector2(-normal.y, normal.x);

        // keep moving along the surface in the same direction as before
        float side = Mathf.Sign(Vector2.Dot(lastSpeed, tangent));
        side = side == 0 ? 1f : side;

        // mix tangent and normal to get the bounce angle
        float rad = bounceDirection * Mathf.Deg2Rad;
        Vector2 dir = tangent * side * Mathf.Cos(rad) + normal * Mathf.Sin(rad);

        body.linearVelocity = dir.normalized * speed;
    }
}