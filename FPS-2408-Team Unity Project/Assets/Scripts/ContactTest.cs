using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactTest : MonoBehaviour
{

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision");
        ContactPoint[] c = collision.contacts;
        for (int i = 0; i < c.Length; i++)
        {
            Debug.Log(c[i].normal);
        }
    }
}
