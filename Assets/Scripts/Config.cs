using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Config : MonoBehaviour
{
    [Header("Development Settings")]
    public bool devMode = false;

    [Header("Player Settings")]
    public float defaultSpeed = 1f;
    public int startingHealth = 10;

    [Header("Weapon Settings")]
    public float bulletSpeed = 3f;
    public int bulletDamage = 2;
    public float pistolRecoil = 3f;
    public float pistolCooldown = 0.4f;
    public float jugSpeed = 2.5f;
    public int jugDamage = 1;
    public float jugCooldown = 0.1f;
    public float bottleSpeed = 2.6f;
    public int bottleDamage = 1;
    public float bottleCooldown = 0.1f;
    public float shotgunSpeed = 3f;
    public int shotgunDamage = 2;
    public float shotgunRecoil = 3f;
    public float shotgunCooldown = 1.3f;
    public float shotgunSpreadAngle = 25f;

    [Header("Event Settings")]
    public bool switcherooEnabled = true;
    public float minChangeTime = 20f;
    public float maxChangeTime = 30f;
    public float changeTimer = 5f;
    private string currentEvent = "";
    public float eventTextDuration = 2f;
    private string oldEvent = "";

    [Header("References")]
    public Text centreText;
    public Text centreTextDropShadow;
    public GameObject centreTextBackground;
    private float centreTextTimer = 0f;
    private UnityEvent centreTextEvent;
    public GameObject centreTextCanvas;
    public Player p1;
    public Player p2;

    private SoundManager snd;

    private bool flippingScreen = false;
    private float flipScreenTimer = 0f;
    private float flipScreenDuration;
    private bool flippingBack = false;

    private bool endedGame = false;

    public int GetAmmo(Item item)
    {
        switch (item)
        {
            case Item.jug: return 1;
            case Item.pistol: return 3;
            default: return 1;
        }
    }

    void Start()
    {
        snd = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        snd.Play("Music");

        //Screen.fullScreen = false;

        SetCentreText("FIGHT!", 2f);
        
        if (PlayerPrefs.GetInt("P2ControlSet", 1) == 1)
        {
            p2.up = KeyCode.Keypad8;
            p2.down = KeyCode.Keypad5;
            p2.left = KeyCode.Keypad4;
            p2.right = KeyCode.Keypad6;
            p2.shoot = KeyCode.Keypad9;
        }
        else
        {
            p2.up = KeyCode.P;
            p2.down = KeyCode.Semicolon;
            p2.left = KeyCode.L;
            p2.right = KeyCode.BackQuote;
            p2.shoot = KeyCode.LeftBracket;
        }

        p1.SetOriginalControls();
        p2.SetOriginalControls();

        //FlipScreen(3f);

        switcherooEnabled = PlayerPrefs.GetInt("EnableSwitcheroo", 1) == 1;
        if (devMode)
        {
            switcherooEnabled = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FlipScreen();
        }

        centreTextTimer -= Time.deltaTime;

        if (centreTextTimer <= 0f && centreText.text != "")
        {
            SetCentreText("");

            if (centreTextEvent != null)
            {
                centreTextEvent.Invoke();
                centreTextEvent = null;
            }
        }

        changeTimer -= Time.deltaTime;

        if (changeTimer <= 0f && switcherooEnabled && !endedGame)
        {
            PickEvent();
            changeTimer = Random.Range(minChangeTime, maxChangeTime);
        }

        if (flippingScreen)
        {
            flipScreenTimer -= Time.deltaTime;

            if (flippingBack)
            {
                Camera.main.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(0f, 180f, flipScreenTimer / flipScreenDuration));
            }
            else
            {
                Camera.main.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(180f, 0f, flipScreenTimer / flipScreenDuration));
            }

            if (flipScreenTimer <= 0f)
            {
                flippingScreen = false;
                
                if (flippingBack)
                {
                    Camera.main.transform.eulerAngles = new Vector3(0f, 0f, 0f);
                }
                else
                {
                    Camera.main.transform.eulerAngles = new Vector3(0f, 0f, 180f);
                }
            }
        }

        centreTextCanvas.transform.eulerAngles = Camera.main.transform.eulerAngles;

        if ((p1.health <= 0 || p2.health <= 0) && !endedGame && !devMode)
        {
            endedGame = true;
            UnityEvent endEvent = new UnityEvent();
            endEvent.AddListener(ReturnToMenu);

            if (p1.health <= 0)
            {
                SetCentreText("Player 2 wins!", 5f, endEvent);
            }
            else if (p2.health <= 0)
            {
                SetCentreText("Player 1 wins!", 5f, endEvent);
            }
        }
    }

    public void FlipScreen()
    {
        Camera.main.transform.eulerAngles += new Vector3(0f, 0f, 180f);
    }
    public void FlipScreen(float duration)
    {
        flippingScreen = true;
        flipScreenTimer = duration;
        flipScreenDuration = duration;

        flippingBack = Camera.main.transform.eulerAngles.z != 0f;
    }

    public void SetCentreText(string text, float duration, UnityEvent endEvent)
    {
        centreTextTimer = duration;
        centreTextEvent = endEvent;
        SetCentreText(text);
    }
    public void SetCentreText(string text, float duration)
    {
        centreTextTimer = duration;
        SetCentreText(text);
    }

    public void SetCentreText(string text)
    {
        centreText.text = text;
        centreTextDropShadow.text = text;

        centreText.gameObject.SetActive(text != "");
        centreTextDropShadow.gameObject.SetActive(text != "");
        centreTextBackground.SetActive(text != "");
    }

    private void PickEvent()
    {
        bool noEvent = true;
        int nextEvent;
        string nextEventName = "";
        string message = "";

        while (noEvent)
        {

            nextEvent = Random.Range(0, 4);

            // Scramble controls
            if (nextEvent == 0)
            {
                nextEventName = "Scramble Controls";
                message = "Drunk!";
                noEvent = false;
            }
            // Swap controls
            else if (nextEvent == 1)
            {
                nextEventName = "Swap Controls";
                message = "Swap Controls!";
                noEvent = false;
            }
            // Swap legs
            else if (nextEvent == 2)
            {
                nextEventName = "Swap Legs";
                message = "Swap Legs!";
                noEvent = false;
            }
            // Upside down
            else if (nextEvent == 3 && Camera.main.transform.eulerAngles.z == 0f)
            {
                nextEventName = "Upside Down";
                message = "Flip!";
                noEvent = false;
            }

            if (currentEvent == nextEventName)
            {
                noEvent = true;
            }
        }

        currentEvent = nextEventName;
        UnityEvent changeEvent = new UnityEvent();
        changeEvent.AddListener(SetEvent);

        SetCentreText(message, eventTextDuration, changeEvent);
    }

    private void SetEvent()
    {
        Debug.Log("Event!");

        p1.AutoPickup(false);
        p2.AutoPickup(false);

        if (Camera.main.transform.eulerAngles.z != 0f && currentEvent != "Upside Down")
        {
            FlipScreen(0.5f);
        }

        // Scramble controls
        if (currentEvent == "Scramble Controls")
        {
            p1.SelectOriginalControls();
            p2.SelectOriginalControls();
            p1.shoot = p1.originalShoot;
            p2.shoot = p2.originalShoot;

            p1.ScrambleControls();
            p2.ScrambleControls();
        }
        // Swap controls
        else if (currentEvent == "Swap Controls")
        {
            //p1.SelectOriginalControls();
            //p2.SelectOriginalControls();

            //p1.ScrambleControls();
            //p2.ScrambleControls();

            if (p1.AreOriginalControls())
            {
                p1.ScrambleControls();
                p2.ScrambleControls();

                p1.shoot = p1.originalShoot;
                p2.shoot = p2.originalShoot;
            }
            if (oldEvent == "Swap Legs")
            {
                p1.SelectOriginalControls();
                p2.SelectOriginalControls();

                p1.ScrambleControls();
                p2.ScrambleControls();

                p1.shoot = p1.originalShoot;
                p2.shoot = p2.originalShoot;
            }

            KeyCode[] temp = new KeyCode[] { p1.up, p1.down, p1.left, p1.right, p1.shoot };

            p1.up = p2.up;
            p1.down = p2.down;
            p1.left = p2.left;
            p1.right = p2.right;
            p1.shoot = p2.shoot;

            p2.up = temp[0];
            p2.down = temp[1];
            p2.left = temp[2];
            p2.right = temp[3];
            p2.shoot = temp[4];
        }
        // Swap legs
        else if (currentEvent == "Swap Legs")
        {
            p1.SelectOriginalControls();
            p2.SelectOriginalControls();

            KeyCode[] temp = new KeyCode[] { p1.up, p1.down, p1.left, p1.right, p1.shoot };

            p1.up = p2.up;
            p1.down = p2.down;
            p1.left = p2.left;
            p1.right = p2.right;

            p2.up = temp[0];
            p2.down = temp[1];
            p2.left = temp[2];
            p2.right = temp[3];

            p1.shoot = p1.originalShoot;
            p2.shoot = p2.originalShoot;

            p1.AutoPickup(true);
            p2.AutoPickup(true);
        }
        // Upside down
        else if (currentEvent == "Upside Down")
        {
            //p1.SelectOriginalControls();
            //p2.SelectOriginalControls();
            FlipScreen(0.5f);

            p1.SelectOriginalControls();
            p2.SelectOriginalControls();
            p1.shoot = p1.originalShoot;
            p2.shoot = p2.originalShoot;
        }

        p1.UpdateKeyDisplay();
        p2.UpdateKeyDisplay();

        oldEvent = currentEvent;
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
