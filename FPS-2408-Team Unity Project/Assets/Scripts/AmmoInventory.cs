using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoInventory : MonoBehaviour
{
    public static AmmoInventory instance;

    [Header("Inventory Variables")]
    [Space]
    public int[] ammoCounts = new int[5];
    public Sprite[] typeIcons;
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
    public int GetAmmoAmount(bulletType type)
    {
        return ammoCounts[(int)type];
    }
    public Sprite GetAmmoIcon(bulletType type)
    {
        if ((int)type < typeIcons.Length)
        {
            return typeIcons[(int)type];
        }
        else
        {
            return null;
        }
    }
    public void UpdateAmmoInventory(bulletType type, int amount)
    {
        ammoCounts[(int)type] += amount;
    }
}
