using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerListener : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }
    public void UpdatePosition()
    {

        if (GameManager.instance == null || GameManager.instance.playerRef == null) return;
        transform.position = GameManager.instance.playerRef.transform.position;
    }
}
