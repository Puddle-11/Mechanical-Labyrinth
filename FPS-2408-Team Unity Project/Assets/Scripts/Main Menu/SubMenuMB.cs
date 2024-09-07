using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SubMenuMB : BaseMenuButton
{
     
    [SerializeField] private GameObject nextMenu;
    private MainMenuManager instance;
    private void Awake()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("MainMenuManager");
       if(temp != null) instance = temp.GetOrAddComponent<MainMenuManager>();
    }
    public override void Click()
    {
        if(instance != null)
        {
            instance.ChangeMenu(nextMenu);
        }

    }

}
