using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{

    public void SetHealthAfterDelay(int _newHealth, float _delay);
    public void UpdateHealthAfterDelay(int _newHealth, float _delay, float _shieldPen = 1);
    public int GetCurrentHealth();
    public int GetMaxHealth();
    public void SetMaxHealth(int _val);
    public void UpdateHealth(int _amount, float _shieldPen = 1);
    public void SetHealth(int _amount);
    public void ResetHealth();

    public void SetCurrentShield(int _amount);
    public int GetCurrentShield();
    public int GetMaxShield();
    public void SetMaxShield(int _amount);
    public void UpdateShield(int _amount, float _shieldPen);
    public void ResetShield();

}