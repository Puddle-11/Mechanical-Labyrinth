using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoInventory : MonoBehaviour
{
    public static AmmoInventory instance;

    [Header("Inventory Variables")]
    [Space]
    public int[] ammoCounts = new int[5];
    public enum bulletType
    {
        Pistol,
        Assualt,
        Shotgun,
        Sniper,
        Explosive
    }


    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateAmmoInventory(bulletType type, int amount)
    {
        ammoCounts[(int)type] += amount;
    }
}
