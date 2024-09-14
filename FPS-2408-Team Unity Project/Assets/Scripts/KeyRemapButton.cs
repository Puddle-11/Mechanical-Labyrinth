using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyRemapButton : MonoBehaviour
{
    public Button Button;
    public EventSystem EV;
    private bool selected;
    private KeyCode currKey;
    private void Update()
    {

        if(selected && Input.anyKeyDown)
        {
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(kcode))
                    currKey = kcode;
            }
        }
        selected = EV.currentSelectedGameObject == Button.gameObject;
    }
}
