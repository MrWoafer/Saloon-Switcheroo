using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPoint : MonoBehaviour
{
    [Header("Settings")]
    public float timer = 10f;
    public float randomnessRange = 1f;
    private float timerTime = 0f;

    [Header("References")]
    public GameObject[] items;
    public int[] weights;
    private GameObject item = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timerTime -= Time.deltaTime;

        if (timerTime <= 0f)
        {
            timerTime = Random.Range(timer - randomnessRange, timer + randomnessRange);

            if (item == null)
            {
                GameObject itemToSpawn = items[WeightedRand.WeightRand(weights)];
                item = Instantiate(itemToSpawn, transform);
                item.transform.parent = transform;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
}
