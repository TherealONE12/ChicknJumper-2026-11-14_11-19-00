using UnityEngine;

[DefaultExecutionOrder(150)]
public class EndlessWorld : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Ball chicken;
    [SerializeField] private TrampolinePlacement placement;
    [SerializeField] private float deathHeight = 10f;
    [SerializeField] private EndlessBackground scenery;
    [SerializeField] private SpriteRenderer shade;
    private Rigidbody2D body;

    private void Awake() => body = chicken.GetComponent<Rigidbody2D>();

    private void FixedUpdate()
    {
        // Rebase by complete mirror periods to preserve precision during long runs.
        if (Mathf.Abs(body.position.x) > 2000f)
        {
            float shift = Mathf.Round(body.position.x / scenery.RepeatWidth) * scenery.RepeatWidth;
            body.position -= Vector2.right * shift;
            chicken.transform.position = new Vector3(body.position.x, body.position.y, chicken.transform.position.z);
            cam.transform.position -= Vector3.right * shift;
            if (placement.PlacedTrampoline != null)
                placement.PlacedTrampoline.transform.position -= Vector3.right * shift;
        }
        // No ceiling collider anymore: the chicken dies when it leaves through the top.
        if (body.position.y > deathHeight) chicken.Die();
    }

    private void LateUpdate()
    {
        shade.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
        shade.transform.localScale = new Vector3(cam.orthographicSize * cam.aspect * 2f / shade.sprite.bounds.size.x,
            cam.orthographicSize * 2f / shade.sprite.bounds.size.y, 1f);
    }
}