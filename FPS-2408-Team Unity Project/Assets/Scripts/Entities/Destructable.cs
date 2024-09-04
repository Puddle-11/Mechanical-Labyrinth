using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//====================================
//REWORKED
//====================================
public class Destructable : BaseEntity
{
    #region MonoBehavior Methods
    public override void Start()
    {
        base.Start();
    }
    #endregion

    #region Override Methods
    public override void SetHealth(int _amount)
    {

        if (_amount < maxHealth)
            healthBar?.gameObject?.SetActive(true);
        else
            healthBar?.gameObject?.SetActive(false);

        base.SetHealth(_amount);
    }
    #endregion

    // Start is called before the first frame update
}