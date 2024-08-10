using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth
{
    public void UpdateHealth(int _amount);
    public void SetHealth(int _amount);
    public void ResetHealth();
}
