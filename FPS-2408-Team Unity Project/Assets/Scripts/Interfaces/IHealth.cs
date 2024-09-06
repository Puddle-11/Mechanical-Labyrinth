using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{

    public void SetHealthAfterDelay(int _newHealth, float _delay);
    public void UpdateHealthAfterDelay(int _newHealth, float _delay);
    public int GetCurrentHealth();
    public int GetMaxHealth();
    public void SetMaxHealth(int _val);
    public void UpdateHealth(int _amount);
    public void SetHealth(int _amount);
    public void ResetHealth();


}