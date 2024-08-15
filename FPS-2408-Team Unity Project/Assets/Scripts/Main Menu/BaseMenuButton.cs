using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMenuButton : MonoBehaviour
{

    private Vector3 scale;
    private void Awake()
    {
        scale = transform.localScale;
    }
    // Update is called once per frame
    void Update()
    {
        if(MainSceneManager.instance.currentButton == this.GetComponent<IMenuButton>())
        {
            transform.localScale = scale * MainSceneManager.instance.buttonHoverScale;
        }
        else
        {
            transform.localScale = scale;

        }
    }
}
