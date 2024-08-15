using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneManager : MonoBehaviour
{

    public IMenuButton currentButton;
    public float buttonHoverScale;
    private void Awake()
    {
      
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && currentButton != null)
        {
            currentButton.Click();
        }
    }
}
