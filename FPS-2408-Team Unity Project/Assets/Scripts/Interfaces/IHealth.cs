using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    public int GetCurrentHealth();
    public int GetMaxHealth();
    public void SetMaxHealth(int _val);
    public void UpdateHealth(int _amount);
    public void SetHealth(int _amount);
    public void ResetHealth();
}
