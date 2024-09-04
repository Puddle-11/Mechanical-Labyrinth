using UnityEngine;
using UnityEngine.UI;
//====================================
//REWORKED
//====================================
public class EntityHealthBar : MonoBehaviour
{
    [SerializeField] private Image HealthBar;

    public void UpdateHealthBar(float curr, float max)
    {
        HealthBar.fillAmount = curr / max;
    }


}