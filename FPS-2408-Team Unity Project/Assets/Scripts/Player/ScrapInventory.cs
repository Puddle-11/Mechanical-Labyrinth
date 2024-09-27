using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapInventory : MonoBehaviour
{

    public static ScrapInventory instance;

    public int currentScrap;

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
        currentScrap = GameManager.instance.GetScrap();
        UIManager.instance.UpdateScrapCount(currentScrap);

    }

    public void AddScrap(int _amount)
    {
        currentScrap += _amount;
        UIManager.instance.UpdateScrapCount(currentScrap);
        GameManager.instance.SetScrap(currentScrap);

    }

    public void RemoveScrap(int _amount)
    { 
        currentScrap -= _amount;
        UIManager.instance.UpdateScrapCount(currentScrap);
        GameManager.instance.SetScrap(currentScrap);

    }
}
