using UnityEngine;

[DefaultExecutionOrder(100)]
public class CameraFollow : MonoBehaviour
{
    // the object the camera follows
    [SerializeField] private Transform target;

    // horizontal offset from the target
    [SerializeField] private float offsetX = 0f;

    // fixed camera height
    [SerializeField] private float fixedY = 0f;

    public void SnapToTarget()
    {
        if (target == null) return;

        // only x follows the target, y is always fixed
        transform.position = new Vector3(target.position.x + offsetX, fixedY, transform.position.z);
    }

    private void LateUpdate() => SnapToTarget();
}
