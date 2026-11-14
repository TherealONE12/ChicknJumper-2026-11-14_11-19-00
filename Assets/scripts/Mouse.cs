using UnityEngine;
using UnityEngine.InputSystem;

public class Placer : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    // tag the placed object gets (must exist in the Tag Manager)
    [SerializeField] private string placeTag = "bounce";

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            worldPos.z = 0f;

            GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity);
            obj.tag = placeTag;
        }
    }
}