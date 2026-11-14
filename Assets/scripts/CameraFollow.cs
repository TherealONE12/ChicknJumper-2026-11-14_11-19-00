using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // the object the camera follows
    [SerializeField] private Transform target;

    // horizontal offset from the target
    [SerializeField] private float offsetX = 0f;

    // fixed camera height
    [SerializeField] private float fixedY = 0f;

    void LateUpdate()
    {
        if (target == null) return;

        // only x follows the target, y is always fixed
        transform.position = new Vector3(target.position.x + offsetX, fixedY, transform.position.z);
    }
}