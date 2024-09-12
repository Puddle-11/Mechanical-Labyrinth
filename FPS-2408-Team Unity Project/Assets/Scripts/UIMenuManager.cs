using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UIMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject[] Menus;
    private int activeMenu;
    
    public void ReturnToHome()
    {
        ActivateMenu(0);
    }
    public void ActivateMenu(int _index)
    {
        if(_index < 0 || _index >= Menus.Length)
        {
            Debug.LogWarning("failed to set menu. given index was outside bounds");
            return;
        }
                activeMenu = _index;
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
    public void ActivateMenu(GameObject _menu)
    {
        for (int i = 0; i < Menus.Length; i++)
        {
            if (Menus[i] == _menu)
            {
                ActivateMenu(i);
                return;
            }
        }

        Debug.LogWarning("failed to find gameobject " + _menu + " In menu listing, please assign to array on " +gameObject.name);
    }
    public void OnEnable()
    {
        ReturnToHome();
    }
}
