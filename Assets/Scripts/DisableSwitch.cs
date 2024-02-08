using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableSwitch : MonoBehaviour
{
    [Header("Settings")]
    public bool ticked = true;

    [Header("References")]
    public Sprite sprUnticked;
    public Sprite sprHoverUnticked;
    public Sprite sprTicked;
    public Sprite sprHoverTicked;
    private SpriteRenderer spr;

    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();

        ticked = PlayerPrefs.GetInt("EnableSwitcheroo", 1) == 1;
        SetTick(ticked);
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
        if (ticked)
        {
            spr.sprite = sprHoverTicked;
        }
        else
        {
            spr.sprite = sprHoverUnticked;
        }
    }
    private void OnMouseExit()
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

    private void OnMouseDown()
    {
        SetTick(!ticked);
        PlayerPrefs.SetInt("EnableSwitcheroo", ticked ? 1 : 0);
    }
}
