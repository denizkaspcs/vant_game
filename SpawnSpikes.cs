using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using System.Diagnostics;
using System.Security.Cryptography;
//using System.Security.Policy;
using UnityEngine.Events;

public class SpawnSpikes : MonoBehaviour
{
    public GameObject spikes; //grid olan spikes
    public Spike spikeComp; //spike kodu
    public float respawnTime = 1.0f;
    float timer = 0f;

    // Update is called once per frame
    void Update()
    {
        if (spikeComp.getDestroyed() == true)
        {
            timer += Time.deltaTime;
            if (timer >= respawnTime)
            {
                spikes.SetActive(true);
                timer = 0;
                spikeComp.setDestroyed(false);
            }
        }
    }
}
