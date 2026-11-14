using UnityEngine;

// Put this on the same GameObject as a collider to make the chicken bounce.
[RequireComponent(typeof(Collider2D))]
public class BounceSurface : MonoBehaviour
{
    [SerializeField] private bool awardsPoint = true;
    public bool AwardsPoint => awardsPoint;
}
