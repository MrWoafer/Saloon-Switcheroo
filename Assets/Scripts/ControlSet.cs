using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSet : MonoBehaviour
{
    [Header("Settings")]
    public bool ticked = true;

    [Header("References")]
    public Sprite sprUnticked;
    public Sprite sprHoverUnticked;
    public Sprite sprTicked;
    public Sprite sprHoverTicked;
    private SpriteRenderer spr;

    private bool mouseOver = false;
    public bool setting = true;

    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();

        ticked = PlayerPrefs.GetInt("P2ControlSet", 1) == 1;
        SetTick(ticked);
    }

    // Update is called once per frame
    void Update()
    {
        ticked = (PlayerPrefs.GetInt("P2ControlSet", 1) == 1) == setting;

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
        if (PlayerPrefs.GetInt("P2ControlSet", 1) == 1)
        {
            PlayerPrefs.SetInt("P2ControlSet", 0);
        }
        else
        {
            PlayerPrefs.SetInt("P2ControlSet", 1);
        }
    }
}
