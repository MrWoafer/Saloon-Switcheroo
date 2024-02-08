using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class WeightedRand
{
    public static int WeightRand(int[] weights)
    {
        int total = Enumerable.Sum(weights);
        int random = Random.Range(0, total);

        int runningTotal = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            runningTotal += weights[i];
            if (random < runningTotal)
            {
                return i;
            }
        }
        return -1;
    }

    public static bool RandomBool(float probability)
    {
        if (probability == 0f)
        {
            return false;
        }
        else if (probability == 1f)
        {
            return true;
        }
        else
        {
            return Random.Range(0f, 1f) < probability;
        }
    }
}
