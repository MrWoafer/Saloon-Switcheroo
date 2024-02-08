using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleFullscreen : MonoBehaviour
{
    private bool ticked = true;

    [Header("References")]
    public Sprite sprUnticked;
    public Sprite sprHoverUnticked;
    public Sprite sprTicked;
    public Sprite sprHoverTicked;
    private SpriteRenderer spr;

    private bool mouseOver = false;

    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();

        ticked = Screen.fullScreen;
        SetTick(ticked);
    }

    // Update is called once per frame
    void Update()
    {
        ticked = Screen.fullScreen;
        if (mouseOver)
        {
            if (ticked)
            {
                spr.sprite = sprHoverTicked;
            }
            else
            {
                spr.sprite = sprHoverUnticked;
            }
        }
        else
        {
            if (ticked)
            {
                spr.sprite = sprTicked;
            }
            else
            {
                spr.sprite = sprUnticked;
            }
        }
    }

    public void SetTick(bool isTicked)
    {
        ticked = isTicked;

        if (ticked)
        {
            spr.sprite = sprTicked;
        }
        else
        {
            spr.sprite = sprUnticked;
        }
    }

    private void OnMouseOver()
    {
        mouseOver = true;
    }
    private void OnMouseExit()
    {
        mouseOver = false;
    }

    private void OnMouseDown()
    {
        ticked = !ticked;
        Screen.fullScreen = !Screen.fullScreen;
    }
}
