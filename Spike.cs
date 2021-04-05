using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
//using System.Media;
using UnityEngine;
using System.Collections.Specialized;
using System.Security.Cryptography;
//using System.Security.Policy;
using UnityEngine.Events;

public class Spike : MonoBehaviour
{
    bool isDestroyed = false;
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Vector3 hitPosition = Vector3.zero;
            foreach (ContactPoint2D hit in col.contacts)
            {
                //Destroy(gameObject);
                gameObject.SetActive(false);
                isDestroyed = true;
            }
        }
    }

    public void setDestroyed(bool a)
    {
        isDestroyed = a;
    }

    public bool getDestroyed()
    {
        return isDestroyed;
    }
}
