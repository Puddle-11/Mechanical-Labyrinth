using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject[] Menus;
    public void ReturnToHome()
    {
        ActivateMenu(0);
    }
    public void ActivateMenu(int _index)
    {
        if(_index < 0 || _index >= Menus.Length)
        {
            _index = 0;
        }
        for (int i = 0; i < Menus.Length; i++)
        {
            if(i == _index)
            {
                Menus[i].SetActive(true);
            }
            else
            {
                Menus[i].SetActive(false);

            }
        }
    }
}
