using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRotator : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite leftRight;
    public Sprite up;
    public Sprite down;

    private SpriteRenderer spr;

    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    public void SetRotation(Direction direction)
    {
        if (spr != null)
        {
            if (direction == Direction.up)
            {
                spr.sprite = up;
            }
            else if (direction == Direction.down)
            {
                spr.sprite = down;
            }
            else
            {
                spr.sprite = leftRight;
            }
        }
    }
}
