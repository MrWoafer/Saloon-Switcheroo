using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    pistol,
    jug,
    bottle
}

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    public BulletType bulletType;
    public int damage;
    public float speed;
    public float rotationSpeed = 0f;
    private float height;
    public Direction direction = Direction.right;
    public bool rotateShadow = false;

    [Header("References")]
    public GameObject sprite;
    public GameObject shadow;
    private Config config;
    private Rigidbody2D rb;
    public Collider2D collider;
    public SpriteOrder sprOrder;
    private ParticleManager partMan;
    private SoundManager snd;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<Config>();
        rb = GetComponent<Rigidbody2D>();
        partMan = GameObject.Find("ParticleManager").GetComponent<ParticleManager>();
        snd = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = rb.GetRelativeVector(Vector2.right) * speed;

        if (direction == Direction.left)
        {
            sprite.transform.Rotate(new Vector3(0f, 0f, -rotationSpeed * Time.deltaTime));
        }
        else
        {
            sprite.transform.Rotate(new Vector3(0f, 0f, rotationSpeed * Time.deltaTime));
        }

        sprOrder.height = height + 0.2f;

        if (direction == Direction.right)
        {
            shadow.transform.rotation = Quaternion.identity;
            shadow.transform.localPosition = new Vector3(shadow.transform.localPosition.x, -height, shadow.transform.localPosition.z);
            collider.offset = new Vector2(collider.offset.x, -height);
        }
        else if (direction == Direction.left)
        {
            shadow.transform.rotation = Quaternion.identity;
            shadow.transform.localPosition = new Vector3(shadow.transform.localPosition.x, height, shadow.transform.localPosition.z);
            collider.offset = new Vector2(collider.offset.x, height);
        }
        else if (direction == Direction.down)
        {
            if (rotateShadow)
            {
                shadow.transform.eulerAngles = new Vector3(0f, 0f, 90f);
            }
            else
            {
                shadow.transform.rotation = Quaternion.identity;
            }
            shadow.transform.localPosition = new Vector3(height, shadow.transform.localPosition.z);
            collider.offset = new Vector2(height, 0f);
        }
        else if (direction == Direction.up)
        {
            if (rotateShadow)
            {
                shadow.transform.eulerAngles = new Vector3(0f, 0f, 90f);
            }
            else
            {
                shadow.transform.rotation = Quaternion.identity;
            }
            shadow.transform.localPosition = new Vector3(-height, shadow.transform.localPosition.z);
            collider.offset = new Vector2(-height, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        /*if (col.gameObject.tag == "Player")
        {
            Player player = col.gameObject.GetComponent<Player>();
        }*/

        //Debug.Log("Hit!");

        if (bulletType == BulletType.pistol)
        {
            snd.Play("Impact");
            partMan.Play("BulletHit", collider.ClosestPoint(col.transform.position - new Vector3(0f, height, 0f)) + new Vector2(0f, height), new Vector3(0f, 0f, 0f));
        }
        else if (bulletType == BulletType.jug)
        {
            snd.Play("GlassBreak");
            partMan.Play("JugHit", collider.ClosestPoint(col.transform.position - new Vector3(0f, height, 0f)) + new Vector2(0f, height), new Vector3(0f, 0f, 0f));
        }
        else if (bulletType == BulletType.bottle)
        {
            snd.Play("GlassBreak");
            partMan.Play("BottleHit", collider.ClosestPoint(col.transform.position - new Vector3(0f, height, 0f)) + new Vector2(0f, height), new Vector3(0f, 0f, 0f));
        }

        Destroy(gameObject);
    }

    public void SetHeight(float newHeight)
    {
        height = newHeight;
    }
}
