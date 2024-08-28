using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapScript : MonoBehaviour
{
    public float AbsoluteHeight;
    private void LateUpdate()
    {
       transform.position = new Vector3(transform.position.x, AbsoluteHeight, transform.position.z);
    }
}
