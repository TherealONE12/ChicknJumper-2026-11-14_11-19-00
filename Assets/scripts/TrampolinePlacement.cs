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
    [SerializeField] private Vector2 arenaMin = new Vector2(-7.2f, -3.2f);
    [SerializeField] private Vector2 arenaMax = new Vector2(7.2f, 3.2f);
    // size of the free area needed to place a trampoline
    [SerializeField] private Vector2 trampolineSize = new Vector2(2.4f, 0.8f);
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
        if (prefab == null) return false;
        position.x = Mathf.Clamp(position.x, arenaMin.x, arenaMax.x);
        position.y = Mathf.Clamp(position.y, arenaMin.y, arenaMax.y);
        arenaMax.y = arenaMax.y + 1;
        // Do not create a collider inside the chicken or an arena wall.
        // The old trampoline and triggers are ignored, the old one gets replaced anyway.
        foreach (Collider2D hit in Physics2D.OverlapBoxAll(position, trampolineSize, 0f))
        {
            if (hit.isTrigger) continue;
            if (placedTrampoline != null && hit.transform.IsChildOf(placedTrampoline.transform)) continue;
            return false;
        }
        if (placedTrampoline != null)
        {
            placedTrampoline.SetActive(false);
            Destroy(placedTrampoline);
        }
        // Never overwrite the prefab reference with a scene instance.
        placedTrampoline = Instantiate(prefab, position, Quaternion.identity);
        // Only trampolines bounce: every collider of the trampoline becomes a solid bounce surface.
        foreach (Collider2D col in placedTrampoline.GetComponentsInChildren<Collider2D>())
        {
            col.isTrigger = false;
            if (!col.TryGetComponent<BounceSurface>(out _)) col.gameObject.AddComponent<BounceSurface>();
        }
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