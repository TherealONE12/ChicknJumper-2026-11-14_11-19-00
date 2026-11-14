using System.Collections.Generic;
using UnityEngine;

// Recycle mirrored scenery after the camera has followed the chicken.
[DefaultExecutionOrder(200)]
[RequireComponent(typeof(SpriteRenderer))]
public class EndlessBackground : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private readonly List<SpriteRenderer> tiles = new List<SpriteRenderer>();
    private SpriteRenderer source;
    private Vector3 origin;
    public float RepeatWidth => source.bounds.size.x * 2f;

    private void Awake()
    {
        source = GetComponent<SpriteRenderer>();
        origin = transform.position;
        tiles.Add(source);
    }

    private void LateUpdate()
    {
        if (cam == null) return;
        float width = source.bounds.size.x;
        int needed = Mathf.CeilToInt(cam.orthographicSize * cam.aspect * 2f / width) + 2;
        while (tiles.Count < needed)
        {
            var tile = new GameObject("Scenery tile", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
            tile.transform.SetParent(transform.parent, false);
            tile.transform.localScale = transform.localScale;
            tile.sprite = source.sprite;
            tile.sharedMaterial = source.sharedMaterial;
            tile.color = source.color;
            tile.sortingLayerID = source.sortingLayerID;
            tile.sortingOrder = source.sortingOrder;
            tiles.Add(tile);
        }
        int first = Mathf.FloorToInt((cam.transform.position.x - cam.orthographicSize * cam.aspect - origin.x) / width);
        for (int i = 0; i < tiles.Count; i++)
        {
            int index = first + i;
            tiles[i].transform.position = origin + Vector3.right * (index * width);
            tiles[i].flipX = (index & 1) != 0;
        }
    }

    private void OnDestroy()
    {
        for (int i = 1; i < tiles.Count; i++)
            if (tiles[i] != null) Destroy(tiles[i].gameObject);
    }
}
