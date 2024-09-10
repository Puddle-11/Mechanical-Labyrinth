using UnityEngine;
using UnityEngine.UI;
//====================================
//REWORKED
//====================================
public class EntityHealthBar : MonoBehaviour
{
    [SerializeField] private Image HealthBar;
    [SerializeField] private Image ShieldBar;

    public void UpdateShieldBar(float _val)
    {
        if (ShieldBar != null) ShieldBar.fillAmount = _val;
    }
    public void UpdateHealthBar(float _val)
    {
        HealthBar.fillAmount = _val;
    }


}