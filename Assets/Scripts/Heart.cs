using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    [Header("Settings")]
    public float startingY = 14f / 16f;
    public float endY = 0f;

    [Header("References")]
    public GameObject spriteMask;

    private void Start()
    {
        SetHealth(1f);
    }

    public void SetHealth(float percentage)
    {
        spriteMask.transform.localPosition = new Vector3(0f, Mathf.Round(Mathf.Lerp(startingY, endY, 1f - percentage) * 16f) / 16f, 0f);
    }
}
