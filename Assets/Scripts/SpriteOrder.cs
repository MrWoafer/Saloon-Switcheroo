using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOrder : MonoBehaviour
{
    [Header("Settings")]
    public int offset = 0;
    public float height = 0f;

    private SpriteRenderer spr;

    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        spr.sortingOrder = (int)(-(transform.parent.position.y - height) * 10000f) + offset;
    }
}
