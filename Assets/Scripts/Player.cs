using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Direction
{
    up,
    down,
    left,
    right
}

public enum Item
{
    none,
    pistol,
    jug,
    bottle,
    shotgun
}

public class Player : MonoBehaviour
{
    [Header("Settings")]
    public string playerName = "Player 1";
    public int health;
    private float speed;
    private int ammo;
    private float cooldown = 0f;
    public Color shirtColour;

    [Header("Controls")]
    public KeyCode up = KeyCode.W;
    public KeyCode down = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode shoot = KeyCode.E;

    private KeyCode originalUp;
    private KeyCode originalDown;
    private KeyCode originalLeft;
    private KeyCode originalRight;
    [HideInInspector]
    public KeyCode originalShoot;

    public Direction direction = Direction.right;
    public Item holding = Item.none;
    [SerializeField]
    public GameObject closestItem;
    [SerializeField]
    private float minItemDistance = 0f;
    [SerializeField]
    public bool itemInRange = false;
    [SerializeField]
    private float itemCalculationTimer = 0f;

    [Header("References")]
    public GameObject sprite;
    public GameObject gunHoldPoint;
    public GameObject activeGunTip;
    private GameObject gunTipLeftRight;
    private GameObject gunTipUp;
    private GameObject gunTipDown;
    public SpriteOrder activeWeaponSprOrder;
    public ItemRotator activeItemRotator;
    public GameObject bulletPrefab;
    public GameObject thrownJugPrefab;
    public GameObject thrownBottlePrefab;
    public Heart heart;
    public GameObject objPistol;
    public GameObject objPistolTipLeftRight;
    public GameObject objPistolTipUp;
    public GameObject objPistolTipDown;
    public GameObject objJug;
    public GameObject objJugTipLeftRight;
    public GameObject objJugTipUp;
    public GameObject objJugTipDown;
    public GameObject objBottle;
    public GameObject objobjBottleTipLeftRight;
    public GameObject objobjBottleTipUp;
    public GameObject objobjBottleTipDown;
    public GameObject objShotgun;
    public GameObject objShotgunTipLeftRight;
    public GameObject objShotgunTipUp;
    public GameObject objShotgunTipDown;
    public Text upKeyText;
    public Text downKeyText;
    public Text leftKeyText;
    public Text rightKeyText;
    private ParticleManager partMan;
    private CameraShake camShake;
    public GameObject ammo1;
    public GameObject ammo2;
    public GameObject ammo3;
    public GameObject ammoExtra1;
    public GameObject ammoExtra2;
    private SoundManager snd;

    private Config config;
    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D boxCol;

    private bool setRunTrigger = false;
    private bool holdingButton = false;

    private bool freeze = false;
    private int flashCount = 0;
    private bool flashing = true;
    private float flashTimer = 1f;
    private float flashFrequency;

    private bool autoPickup = false;

    // Start is called before the first frame update
    void Start()
    {
        sprite.GetComponent<SpriteRenderer>().material.SetColor("ShirtColour", shirtColour);

        config = GameObject.Find("Config").GetComponent<Config>();
        speed = config.defaultSpeed;
        SetHealth(config.startingHealth);

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCol = GetComponent<BoxCollider2D>();

        partMan = GameObject.Find("ParticleManager").GetComponent<ParticleManager>();

        camShake = Camera.main.GetComponent<CameraShake>();

        snd = GameObject.Find("SoundManager").GetComponent<SoundManager>();

        UpdateKeyDisplay();
        //ScrambleControls();
        SelectItem(null);

        UpdateAmmoDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        cooldown -= Time.deltaTime;

        rb.velocity = Vector2.zero;
        holdingButton = false;

        if (!freeze)
        {
            if (Input.GetKey(up))
            {
                holdingButton = true;
                rb.velocity += Vector2.up * speed;
                direction = Direction.up;
                sprite.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            if (Input.GetKey(down))
            {
                holdingButton = true;
                rb.velocity += Vector2.down * speed;
                direction = Direction.down;
                sprite.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            if (Input.GetKey(left))
            {
                holdingButton = true;
                rb.velocity += Vector2.left * speed;
                direction = Direction.left;
                sprite.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            if (Input.GetKey(right))
            {
                holdingButton = true;
                rb.velocity += Vector2.right * speed;
                direction = Direction.right;
                sprite.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            if (Input.GetKeyDown(shoot))
            {
                if (holding == Item.none && !autoPickup)
                {
                    if (itemInRange)
                    {
                        SelectItem(closestItem);
                    }
                }
                else if (cooldown <= 0f)
                {
                    if (holding == Item.pistol)
                    {
                        Shoot(Item.pistol, direction);
                    }
                    else if (holding == Item.jug)
                    {
                        Shoot(Item.jug, direction);
                        //HideAllItems();
                    }
                    else if (holding == Item.bottle)
                    {
                        Shoot(Item.bottle, direction);
                        //HideAllItems();
                    }
                    if (holding == Item.shotgun)
                    {
                        Shoot(Item.shotgun, direction);
                    }
                }
            }

            if (holding == Item.none && autoPickup)
            {
                if (itemInRange)
                {
                    SelectItem(closestItem);
                }
            }
        }

        if (rb.velocity.magnitude > 0f)
        {
            rb.velocity = rb.velocity / rb.velocity.magnitude * speed;
        }

        if (rb.velocity.magnitude > 0f && !setRunTrigger)
        {
            setRunTrigger = true;
            anim.ResetTrigger("RunEnd");
            anim.SetTrigger("RunStart");
        }
        else if (rb.velocity.magnitude == 0f && setRunTrigger)
        {
            setRunTrigger = false;
            anim.ResetTrigger("RunStart");
            anim.SetTrigger("RunEnd");
        }

        SetHealth(health);

        if (itemCalculationTimer <= 0f)
        {
            itemCalculationTimer = 0.1f;
            minItemDistance = 10000f;
        }
        else
        {
            itemCalculationTimer -= Time.deltaTime;
        }

        if (activeWeaponSprOrder != null)
        {
            if (direction == Direction.up)
            {
                activeWeaponSprOrder.offset = -1;
                activeWeaponSprOrder.height = gunHoldPoint.transform.position.y - transform.position.y;
            }
            else if (direction == Direction.down)
            {
                activeWeaponSprOrder.offset = 1;
                activeWeaponSprOrder.height = 0.4375f;
            }
            else if (direction == Direction.left)
            {
                activeWeaponSprOrder.offset = 1;
                activeWeaponSprOrder.height = 0.4375f;
            }
            else if (direction == Direction.right)
            {
                activeWeaponSprOrder.offset = 1;
                activeWeaponSprOrder.height = 0.4375f;
            }
        }

        anim.ResetTrigger("IdleUp");
        anim.ResetTrigger("IdleDown");
        if (!holdingButton)
        {
            if (direction == Direction.up)
            {
                anim.SetTrigger("IdleUp");
            }
            else if (direction == Direction.down)
            {
                anim.SetTrigger("IdleDown");
            }
        }

        anim.ResetTrigger("RunUp");
        anim.ResetTrigger("RunDown");
        if (holdingButton)
        {
            if (direction == Direction.up)
            {
                anim.SetTrigger("RunUp");
            }
            else if (direction == Direction.down)
            {
                anim.SetTrigger("RunDown");
            }
            else
            {
                anim.SetTrigger("NormalRun");
            }
        }

        if (activeItemRotator != null)
        {
            activeItemRotator.SetRotation(direction);
        }

        if (direction == Direction.left || direction == Direction.right)
        {
            activeGunTip = gunTipLeftRight;
        }
        else if (direction == Direction.down)
        {
            activeGunTip = gunTipDown;
        }
        else if (direction == Direction.up)
        {
            activeGunTip = gunTipUp;
        }

        if (freeze)
        {
            rb.velocity = Vector2.zero;
        }

        if (flashing)
        {
            flashTimer -= Time.deltaTime;

            if (flashTimer <= 0f)
            {
                flashTimer = flashFrequency;
                flashCount--;
                FlashColour(flashCount % 2  == 0);
                if (flashCount <= 0)
                {
                    flashing = false;
                    FlashColour(false);
                }
            }
        }
    }

    public void ScrambleControls()
    {
        do
        {
            KeyCode[] controls = new KeyCode[] { up, down, left, right };

            for (int i = 0; i < 100; i++)
            {
                int index1 = Random.Range(0, controls.Length);
                int index2 = Random.Range(0, controls.Length);

                KeyCode temp = controls[index1];
                controls[index1] = controls[index2];
                controls[index2] = temp;
            }

            up = controls[0];
            down = controls[1];
            left = controls[2];
            right = controls[3];
        }
        while (AreOriginalControls());

        UpdateKeyDisplay();
    }

    private void Shoot(Item item, Direction fireDirection)
    {
        ammo--;
        UpdateAmmoDisplay();

        if (ammo <= 0)
        {
            if (holding == Item.pistol || holding == Item.shotgun)
            {
                snd.Play("Break");
                partMan.Play("GunBreak", gunHoldPoint.transform.position + new Vector3(0f, 1f / 16f, 0f), Vector3.zero);
            }

            HideAllItems();
            holding = Item.none;
        }

        Bullet bullet = null;
        Bullet bullet2 = null;
        Bullet bullet3 = null;

        if (item == Item.pistol)
        {
            snd.Play("PistolBang");
            snd.Play("PistolRattle");
            ShootAnim();

            cooldown = config.pistolCooldown;

            bullet = Instantiate(bulletPrefab, activeGunTip.transform).GetComponent<Bullet>();
            bullet.damage = config.bulletDamage;
            bullet.speed = config.bulletSpeed;
            bullet.direction = fireDirection;
            bullet.transform.parent = null;
            bullet.transform.localScale = new Vector3(1f, 1f, 1f);
            bullet.SetHeight(activeGunTip.transform.position.y - transform.position.y);
            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), boxCol, true);

            if (direction == Direction.right)
            {
                partMan.Play("GunSmoke", activeGunTip.transform.position, new Vector3(0f, 60f, 0f));
            }
            else if (direction == Direction.up)
            {
                partMan.Play("GunSmoke", activeGunTip.transform.position, new Vector3(-60f, 0f, 90f));
            }
            else if (direction == Direction.left)
            {
                partMan.Play("GunSmoke", activeGunTip.transform.position, new Vector3(0f, -60f, 180f));
            }
            else if (direction == Direction.down)
            {
                partMan.Play("GunSmoke", activeGunTip.transform.position, new Vector3(60f, 0f, 90f));
            }

            camShake.Shake(0.1f, 0.3f);
            transform.position += Vector2ToVector3(-GetDirectionVector(direction) * config.pistolRecoil);
        }
        else if (item == Item.jug)
        {
            snd.Play("Whoosh");
            cooldown = config.jugCooldown;

            bullet = Instantiate(thrownJugPrefab, activeGunTip.transform).GetComponent<Bullet>();
            bullet.damage = config.jugDamage;
            bullet.speed = config.jugSpeed;
            camShake.Shake(0.1f, 0.1f);
            bullet.direction = fireDirection;
            bullet.transform.parent = null;
            bullet.transform.localScale = new Vector3(1f, 1f, 1f);
            bullet.SetHeight(activeGunTip.transform.position.y - transform.position.y);
            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), boxCol, true);

            ThrowAnim();
        }
        else if (item == Item.bottle)
        {
            snd.Play("Whoosh");
            cooldown = config.bottleCooldown;

            bullet = Instantiate(thrownBottlePrefab, activeGunTip.transform).GetComponent<Bullet>();
            bullet.damage = config.bottleDamage;
            bullet.speed = config.bottleSpeed;
            camShake.Shake(0.1f, 0.1f);
            bullet.direction = fireDirection;
            bullet.transform.parent = null;
            bullet.transform.localScale = new Vector3(1f, 1f, 1f);
            bullet.SetHeight(activeGunTip.transform.position.y - transform.position.y);
            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), boxCol, true);

            ThrowAnim();
        }
        else if (item == Item.shotgun)
        {
            snd.Play("ShotgunBang");
            snd.Play("PistolRattle");
            ShootAnim();

            cooldown = config.shotgunCooldown;

            bullet = Instantiate(bulletPrefab, activeGunTip.transform).GetComponent<Bullet>();
            bullet.damage = config.shotgunDamage;
            bullet.speed = config.shotgunSpeed;
            bullet.direction = fireDirection;
            bullet.transform.parent = null;
            bullet.transform.localScale = new Vector3(1f, 1f, 1f);
            bullet.SetHeight(activeGunTip.transform.position.y - transform.position.y);
            Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), boxCol, true);

            bullet2 = Instantiate(bulletPrefab, activeGunTip.transform).GetComponent<Bullet>();
            bullet2.damage = config.shotgunDamage;
            bullet2.speed = config.shotgunSpeed;
            bullet2.direction = fireDirection;
            bullet2.transform.parent = null;
            bullet2.transform.Rotate(new Vector3(0f, 0f, DirectionToRotation(fireDirection) + config.shotgunSpreadAngle));
            bullet2.transform.localScale = new Vector3(1f, 1f, 1f);
            bullet2.SetHeight(activeGunTip.transform.position.y - transform.position.y);
            Physics2D.IgnoreCollision(bullet2.GetComponent<Collider2D>(), boxCol, true);

            //Debug.Log("Shotgun");

            bullet3 = Instantiate(bulletPrefab, activeGunTip.transform).GetComponent<Bullet>();
            bullet3.damage = config.shotgunDamage;
            bullet3.speed = config.shotgunSpeed;
            bullet3.direction = fireDirection;
            bullet3.transform.parent = null;
            bullet3.transform.Rotate(new Vector3(0f, 0f, DirectionToRotation(fireDirection) - config.shotgunSpreadAngle));
            bullet3.transform.localScale = new Vector3(1f, 1f, 1f);
            bullet3.SetHeight(activeGunTip.transform.position.y - transform.position.y);
            Physics2D.IgnoreCollision(bullet3.GetComponent<Collider2D>(), boxCol, true);

            if (direction == Direction.right)
            {
                partMan.Play("GunSmoke", activeGunTip.transform.position, new Vector3(0f, 60f, 0f));
            }
            else if (direction == Direction.up)
            {
                partMan.Play("GunSmoke", activeGunTip.transform.position, new Vector3(-60f, 0f, 90f));
            }
            else if (direction == Direction.left)
            {
                partMan.Play("GunSmoke", activeGunTip.transform.position, new Vector3(0f, -60f, 180f));
            }
            else if (direction == Direction.down)
            {
                partMan.Play("GunSmoke", activeGunTip.transform.position, new Vector3(60f, 0f, 90f));
            }

            camShake.Shake(0.1f, 0.4f);
            transform.position += Vector2ToVector3(-GetDirectionVector(direction) * config.shotgunRecoil);
        }

        if (direction == Direction.right)
        {
            bullet.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else if (direction == Direction.left)
        {
            bullet.transform.eulerAngles = new Vector3(0f, 0f, 180f);
        }
        else if (direction == Direction.up)
        {
            bullet.transform.eulerAngles = new Vector3(0f, 0f, 90f);
        }
        else if (direction == Direction.down)
        {
            bullet.transform.eulerAngles = new Vector3(0f, 0f, 270f);
        }
    }

    private void SetHealth(int newHealth)
    {
        health = newHealth;
        if (health < 0)
        {
            health = 0;
        }

        heart.SetHealth((float)health / (float)config.startingHealth);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Bullet")
        {
            camShake.Shake(0.1f, 0.3f);
            SetHealth(health - col.gameObject.GetComponent<Bullet>().damage);
            //Flash(4, 0.2f);
            Flash(3, 0.2f);
            GruntSFX();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Pickupable>() != null)
        {
            itemInRange = true;

            float distance = Vector2.Distance(collision.transform.position, transform.position);

            if (distance < minItemDistance)
            {
                minItemDistance = distance;
                closestItem = collision.gameObject;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        itemInRange = false;
    }

    private void SelectItem(GameObject obj)
    {
        HideAllItems();

        if (obj != null)
        {
            Pickupable pickup = obj.GetComponent<Pickupable>();

            if (pickup.objectName == "Jug")
            {
                holding = Item.jug;
                objJug.SetActive(true);
                activeWeaponSprOrder = objJug.GetComponent<SpriteOrder>();
                activeItemRotator = objJug.GetComponent<ItemRotator>();
                gunTipLeftRight = objJugTipLeftRight;
                gunTipUp = objJugTipUp;
                gunTipDown = objJugTipDown;
            }
            else if (pickup.objectName == "Pistol")
            {
                holding = Item.pistol;
                objPistol.SetActive(true);
                activeWeaponSprOrder = objPistol.GetComponent<SpriteOrder>();
                activeItemRotator = objPistol.GetComponent<ItemRotator>();
                gunTipLeftRight = objPistolTipLeftRight;
                gunTipUp = objPistolTipUp;
                gunTipDown = objPistolTipDown;
            }
            else if (pickup.objectName == "Bottle")
            {
                holding = Item.bottle;
                objBottle.SetActive(true);
                activeWeaponSprOrder = objBottle.GetComponent<SpriteOrder>();
                activeItemRotator = objBottle.GetComponent<ItemRotator>();
                gunTipLeftRight = objobjBottleTipLeftRight;
                gunTipUp = objobjBottleTipUp;
                gunTipDown = objobjBottleTipDown;
            }
            else if (pickup.objectName == "Shotgun")
            {
                holding = Item.shotgun;
                objShotgun.SetActive(true);
                activeWeaponSprOrder = objShotgun.GetComponent<SpriteOrder>();
                activeItemRotator = objShotgun.GetComponent<ItemRotator>();
                gunTipLeftRight = objShotgunTipLeftRight;
                gunTipUp = objShotgunTipUp;
                gunTipDown = objShotgunTipDown;
            }

            ammo = config.GetAmmo(holding);
            UpdateAmmoDisplay();

            Destroy(obj);
        }
    }

    private void HideAllItems()
    {
        objJug.SetActive(false);
        objPistol.SetActive(false);
        objBottle.SetActive(false);
        objShotgun.SetActive(false);
    }

    private string KeyCodeToString(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.Keypad0: return "0";
            case KeyCode.Keypad1: return "1";
            case KeyCode.Keypad2: return "2";
            case KeyCode.Keypad3: return "3";
            case KeyCode.Keypad4: return "4";
            case KeyCode.Keypad5: return "5";
            case KeyCode.Keypad6: return "6";
            case KeyCode.Keypad7: return "7";
            case KeyCode.Keypad8: return "8";
            case KeyCode.Keypad9: return "9";
            default: return keyCode.ToString();
        }
    }

    public void UpdateKeyDisplay()
    {
        upKeyText.text = KeyCodeToString(up);
        downKeyText.text = KeyCodeToString(down);
        leftKeyText.text = KeyCodeToString(left);
        rightKeyText.text = KeyCodeToString(right);
    }

    private Vector2 GetDirectionVector(Direction dir)
    {
        if (dir == Direction.up)
        {
            return Vector2.up;
        }
        else if (dir == Direction.down)
        {
            return Vector2.down;
        }
        else if (dir == Direction.left)
        {
            return Vector2.left;
        }
        else if (dir == Direction.right)
        {
            return Vector2.right;
        }
        else
        {
            throw new System.ArgumentException("Unknown direction: " + dir);
        }
    }
    private float DirectionToRotation(Direction dir)
    {
        if (dir == Direction.right)
        {
            return 0f;
        }
        else if (dir == Direction.up)
        {
            return 90f;
        }
        else if (dir == Direction.left)
        {
            return 180f;
        }
        else if (dir == Direction.down)
        {
            return 270f;
        }
        else
        {
            throw new System.ArgumentException("Unknown direction: " + dir);
        }
    }

    private Vector3 Vector2ToVector3(Vector2 vector)
    {
        return new Vector3(vector.x, vector.y, 0f);
    }

    /*public void MoveSpriteX(int pixels)
    {
        sprite.transform.position += new Vector3(((float)pixels) / 16f * (direction != Direction.left ? 1f : -1f), 0f, 0f);
    }*/

    public void Freeze()
    {
        freeze = true;
    }
    public void UnFreeze()
    {
        freeze = false;
    }

    private void ThrowAnim()
    {
        if (direction == Direction.right)
        {
            anim.SetTrigger("ThrowRight");
        }
        else if (direction == Direction.left)
        {
            anim.SetTrigger("ThrowLeft");
        }
        if (direction == Direction.up)
        {
            anim.SetTrigger("ThrowUp");
        }
        if (direction == Direction.down)
        {
            anim.SetTrigger("ThrowDown");
        }
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
            sprite.GetComponent<SpriteRenderer>().material.SetFloat("IsSolidColour", 1f);
        }
        else
        {
            sprite.GetComponent<SpriteRenderer>().material.SetFloat("IsSolidColour", 0f);
        }
    }

    public void DustParticles()
    {
        partMan.Play("Dust", transform.position, Vector3.zero, sprite.GetComponent<SpriteRenderer>().sortingOrder);
    }

    private void UpdateAmmoDisplay()
    {
        ammo1.SetActive(false);
        ammo2.SetActive(false);
        ammo3.SetActive(false);
        ammoExtra1.SetActive(false);
        ammoExtra2.SetActive(false);

        if (ammo == 1)
        {
            ammo1.SetActive(true);

            if (holding == Item.shotgun)
            {
                ammoExtra1.SetActive(true);
                ammoExtra2.SetActive(true);
            }
        }
        else if (ammo == 2)
        {
            ammo1.SetActive(true);
            ammo2.SetActive(true);
        }
        else if (ammo == 3)
        {
            ammo1.SetActive(true);
            ammo2.SetActive(true);
            ammo3.SetActive(true);
        }
    }

    private void ShootAnim()
    {
        if (direction == Direction.up)
        {
            anim.SetTrigger("ShootUp");
        }
        else if (direction == Direction.down)
        {
            anim.SetTrigger("ShootDown");
        }
        else
        {
            anim.SetTrigger("Shoot");
        }
    }

    public void SetOriginalControls()
    {
        originalUp = up;
        originalDown = down;
        originalLeft = left;
        originalRight = right;
        originalShoot = shoot;
    }

    public void SelectOriginalControls()
    {
        up = originalUp;
        down = originalDown;
        left = originalLeft;
        right = originalRight;
        shoot = originalShoot;
    }

    public void AutoPickup(bool on)
    {
        autoPickup = on;
    }

    public bool AreOriginalControls()
    {
        return up == originalUp && down == originalDown && left == originalLeft && right == originalRight;
    }

    public void ClothSFX()
    {
        snd.Play("Cloth" + Random.Range(1, 4).ToString());
    }

    public void GruntSFX()
    {
        snd.Play("Grunt" + Random.Range(1, 4).ToString());
    }

    public void ReloadSFX()
    {
        if (ammo > 0)
        {
            snd.Play("Reload");
        }
    }
}
