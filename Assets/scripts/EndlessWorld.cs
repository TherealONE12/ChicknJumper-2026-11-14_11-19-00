using UnityEngine;

[DefaultExecutionOrder(150)]
public class EndlessWorld : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Ball chicken;
    [SerializeField] private TrampolinePlacement placement;
    [SerializeField] private BoxCollider2D ceiling;
    [SerializeField] private EndlessBackground scenery;
    [SerializeField] private SpriteRenderer shade;
    private Rigidbody2D body;

    private void Awake() => body = chicken.GetComponent<Rigidbody2D>();

    private void FixedUpdate()
    {
        // Rebase by complete mirror periods to preserve precision during long runs.
        if (Mathf.Abs(body.position.x) > 2048f)
        {
            float shift = Mathf.Round(body.position.x / scenery.RepeatWidth) * scenery.RepeatWidth;
            body.position -= Vector2.right * shift;
            chicken.transform.position = new Vector3(body.position.x, body.position.y, chicken.transform.position.z);
            cam.transform.position -= Vector3.right * shift;
            if (placement.PlacedTrampoline != null)
                placement.PlacedTrampoline.transform.position -= Vector3.right * shift;
        }
        ceiling.transform.position = new Vector3(body.position.x, 5.4f, 0f);
        ceiling.size = new Vector2(cam.orthographicSize * cam.aspect * 2f + 20f, 0.3f);
    }

    private void LateUpdate()
    {
        shade.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
        shade.transform.localScale = new Vector3(cam.orthographicSize * cam.aspect * 2f / shade.sprite.bounds.size.x,
            cam.orthographicSize * 2f / shade.sprite.bounds.size.y, 1f);
    }
}
