using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float duration;
    private float magnitude;
    private bool shaking = false;
    private float elapsed;
    private Vector3 originalPos;

    void Update()
    {
        if (shaking)
        {
            if (elapsed < duration)
            {
                if (WeightedRand.RandomBool(0.5f))
                {
                    float x = Random.Range(0f, 1f) * magnitude;
                    float y = Random.Range(0f, 1f) * magnitude;

                    transform.localPosition = new Vector3(x, y, originalPos.z);
                }

                elapsed += Time.deltaTime;
            }
            else
            {
                shaking = false;
                transform.localPosition = originalPos;
            }
        }
    }

    public void Shake(float duration, float magnitude)
    {
        if (shaking)
        {
            elapsed = 0f;
            this.duration = duration;
        }
        else
        {
            originalPos = transform.localPosition;
            elapsed = 0f;
            this.duration = duration;
            this.magnitude = magnitude;
            shaking = true;
        }
    }
}
