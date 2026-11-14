using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    public Vector2 mousePos = new Vector2(0, 0);
    [SerializeField] private bool reference;
    void Start()
    {
        
    }

    void Update()
    {
        mousePos = Mouse.current.position.ReadValue();
        
        
    }
}