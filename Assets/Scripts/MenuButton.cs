using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuButton : MonoBehaviour
{
    [Header("Settings")]
    public UnityEvent command;

    [Header("Sprites")]
    public Sprite sprUnselected;
    public Sprite sprHover;

    private SpriteRenderer spr;

    private int originalSortingOrder;

    private Transform originalTransform;

    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        originalSortingOrder = spr.sortingOrder;

        originalTransform = transform;
    }

    private void OnMouseEnter()
    {
        spr.sprite = sprHover;
        transform.localScale *= 1.2f;
        transform.eulerAngles = new Vector3(0f, 0f, 10f);
        spr.sortingOrder = 100;
    }
    private void OnMouseExit()
    {
        spr.sprite = sprUnselected;

        transform.position = originalTransform.position;
        transform.eulerAngles = Vector3.zero;
        transform.localScale /= 1.2f;
        spr.sortingOrder = originalSortingOrder;
    }

    private void OnMouseDown()
    {
        command.Invoke();
    }
}
