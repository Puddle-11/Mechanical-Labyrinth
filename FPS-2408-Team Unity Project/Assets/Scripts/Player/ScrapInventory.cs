using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapInventory : MonoBehaviour
{

    public static ScrapInventory instance;

    private int currentScrap;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
    void Start()
    {
        currentScrap = 0;
        UIManager.instance.UpdateScrapCount(currentScrap);
    }

    public void AddScrap(int _amount)
    {
        currentScrap += _amount;
        UIManager.instance.UpdateScrapCount(currentScrap);
    }
}
