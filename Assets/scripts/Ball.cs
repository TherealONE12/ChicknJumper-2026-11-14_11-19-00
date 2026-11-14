using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private float startDirection = -45f;
    [SerializeField, Range(10f, 80f)] private float bounceDirection = 45f;
    private Vector2 lastSpeed;
    private SpriteRenderer spriteRenderer;

    public bool IsAlive { get; private set; }
    public int BounceCount { get; private set; }
    public event Action Bounced;
    public event Action Died;

    private void Awake()
    {
        if (body == null) body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        body.gravityScale = 0f;
        body.linearDamping = 0f;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    private void Start() => Launch(body.position);

    public void Launch(Vector2 position)
    {
        body.simulated = false;
        transform.SetPositionAndRotation(new Vector3(position.x, position.y, transform.position.z), Quaternion.identity);
        body.position = position;
        body.rotation = 0f;
        body.angularVelocity = 0f;
        body.simulated = true;
        BounceCount = 0;
        IsAlive = true;
        float radians = startDirection * Mathf.Deg2Rad;
        lastSpeed = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * speed;
        body.linearVelocity = lastSpeed;
    }

    private void FixedUpdate()
    {
        if (!IsAlive) return;
        lastSpeed = body.linearVelocity;
        if (spriteRenderer != null) spriteRenderer.flipX = lastSpeed.x < 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsAlive) return;
        // A component avoids relying on an undefined custom Unity tag.
        if (!collision.collider.TryGetComponent<BounceSurface>(out _))
        {
            Die();
            return;
        }
        Vector2 normal = collision.GetContact(0).normal;
        Vector2 tangent = new Vector2(-normal.y, normal.x);
        float side = Mathf.Sign(Vector2.Dot(lastSpeed, tangent));
        if (side == 0f) side = 1f;
        float radians = bounceDirection * Mathf.Deg2Rad;
        Vector2 direction = tangent * side * Mathf.Cos(radians) + normal * Mathf.Sin(radians);
        lastSpeed = direction.normalized * speed;
        body.linearVelocity = lastSpeed;
        BounceCount++;
        Bounced?.Invoke();
    }

    public void Die()
    {
        if (!IsAlive) return;
        IsAlive = false;
        body.linearVelocity = Vector2.zero;
        body.simulated = false;
        Died?.Invoke();
    }
}
