using System.Collections.Generic;
using UnityEngine;

public class MoverOfTheClouds : MonoBehaviour
{
    public GameObject clouds;

    void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(clouds.transform.position);
    }
}
