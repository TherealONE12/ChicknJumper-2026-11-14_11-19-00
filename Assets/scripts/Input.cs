using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input : MonoBehaviour
{
    public Vector2 mousePos = new Vector2(0, 0);
    [SerializeField] private GameObject prefab;
    [SerializeField] private Camera cam;
    void Start()
    {
        
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
            worldPos.z = 0f;
            prefab = Instantiate(prefab, worldPos, Quaternion.identity);
        }
    }
}