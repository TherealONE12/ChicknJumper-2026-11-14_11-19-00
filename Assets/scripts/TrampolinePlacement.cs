using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Keep the imported script GUID while avoiding UnityEngine.Input name collisions.
public class TrampolinePlacement : MonoBehaviour
{
    public Vector2 mousePos;
    [SerializeField] private GameObject prefab;
    [SerializeField] private Camera cam;
    [SerializeField] private Ball chicken;
    [SerializeField] private Vector2 arenaMin = new Vector2(-8.2f, -3.2f);
    [SerializeField] private Vector2 arenaMax = new Vector2(8.2f, 3.2f);
    private GameObject placedTrampoline;
    public GameObject PlacedTrampoline => placedTrampoline;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            TryPlace(Mouse.current.position.ReadValue());
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            TryPlace(Touchscreen.current.primaryTouch.position.ReadValue());
    }

    private void TryPlace(Vector2 screenPosition)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        if (prefab == null || cam == null || (chicken != null && !chicken.IsAlive)) return;
        mousePos = screenPosition;
        Vector3 world = cam.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, -cam.transform.position.z));
        TryPlaceWorld(world);
    }

    public bool TryPlaceWorld(Vector2 position)
    {
        if (prefab == null || cam == null || Time.timeScale == 0f || (chicken != null && !chicken.IsAlive)) return false;
        // The placement area travels with the view; clicks never change its limits.
        float halfWidth = cam.orthographicSize * cam.aspect;
        position.x = Mathf.Clamp(position.x, cam.transform.position.x - halfWidth + 1.2f,
            cam.transform.position.x + halfWidth - 1.2f);
        position.y = Mathf.Clamp(position.y, arenaMin.y, arenaMax.y);
        // Do not create a collider inside the chicken or an arena wall.
        if (Physics2D.OverlapBox(position, new Vector2(2.4f, 0.8f), 0f) != null) return false;
        if (placedTrampoline != null)
        {
            placedTrampoline.SetActive(false);
            Destroy(placedTrampoline);
        }
        // Never overwrite the prefab reference with a scene instance.
        placedTrampoline = Instantiate(prefab, position, Quaternion.identity);
        return true;
    }

    public void ClearTrampoline()
    {
        if (placedTrampoline == null) return;
        placedTrampoline.SetActive(false);
        Destroy(placedTrampoline);
        placedTrampoline = null;
    }
}
