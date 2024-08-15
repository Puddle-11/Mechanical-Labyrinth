using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMenuButton : MonoBehaviour
{

    private Vector3 scale;
    public MainSceneManager MSMref;
    private void Awake()
    {
        scale = transform.localScale;
    }
    // Update is called once per frame
    void Update()
    {
        if(MSMref.currentButton == this.GetComponent<IMenuButton>())
        {
            transform.localScale = scale * MSMref.buttonHoverScale;
        }
        else
        {
            transform.localScale = scale;

        }
    }
}
