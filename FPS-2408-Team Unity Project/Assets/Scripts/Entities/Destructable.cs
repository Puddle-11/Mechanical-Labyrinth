using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : BaseEntity
{
    public override void Start()
    {
        healthBar.gameObject.SetActive(false);
        base.Start();
    }
    public override void SetHealth(int _amount)
    {
        if(_amount < maxHealth)
            healthBar.gameObject.SetActive(true);
        else
            healthBar.gameObject.SetActive(false);
        base.SetHealth(_amount);
    }


    // Start is called before the first frame update
}
