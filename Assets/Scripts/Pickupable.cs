using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : MonoBehaviour
{
    [Header("Settings")]
    public string objectName = "Object";

    [Header("References")]
    public SpriteRenderer spr;
    private Collider2D collider;
    public SpriteOrder sprOrder;

    GameObject[] playerObjs;
    Player[] players;

    private bool freeze = false;
    private int flashCount = 0;
    private bool flashing = true;
    private float flashTimer = 1f;
    private float flashFrequency;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider2D>();

        playerObjs = GameObject.FindGameObjectsWithTag("Player");
        players = new Player[playerObjs.Length];
        for (int i = 0; i < playerObjs.Length; i++)
        {
            players[i] = playerObjs[i].GetComponent<Player>();
        }

        if (objectName == "Jug")
        {
            spr.material.SetFloat("Alpha", 170f / 255f);
        }

        Flash(3, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        collider.offset = new Vector2(collider.offset.x, -sprOrder.height * 0.5f);

        spr.material.SetFloat("Outlined", 0f);
        foreach (Player i in players)
        {
            if (i.closestItem == gameObject && i.itemInRange)
            {
                spr.material.SetFloat("Outlined", 1f);
            }
        }

        if (flashing)
        {
            flashTimer -= Time.deltaTime;

            if (flashTimer <= 0f)
            {
                flashTimer = flashFrequency;
                flashCount--;
                FlashColour(flashCount % 2 == 0);
                if (flashCount <= 0)
                {
                    flashing = false;
                    FlashColour(false);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        /*Player player = collision.GetComponent<Player>();
        if (player != null && player.closestItem == gameObject)
        {
            spr.material.SetFloat("Outlined", 1f);
        }
        else
        {
            spr.material.SetFloat("Outlined", 0f);
        }*/
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        spr.material.SetFloat("Outlined", 0f);
    }

    public void Flash(int count, float frequency)
    {
        flashing = true;
        flashCount = count * 2;
        flashFrequency = frequency;
        flashTimer = flashFrequency;
        FlashColour(true);
    }

    private void FlashColour(bool flash)
    {
        if (flash)
        {
            spr.material.SetFloat("IsSolidColour", 1f);
        }
        else
        {
            spr.material.SetFloat("IsSolidColour", 0f);
        }
    }
}
