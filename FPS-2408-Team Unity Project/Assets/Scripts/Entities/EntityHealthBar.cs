using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class EntityHealthBar : MonoBehaviour
{
    [SerializeField] private Image HealthBar;
    public void UpdateHealthBar(float curr, float max)
    {
        HealthBar.fillAmount = curr / max;
    }
 
    private void Update()
    {
       transform.LookAt(new Vector3(GameManager.instance.playerRef.transform.position.x, transform.position.y, GameManager.instance.playerRef.transform.position.z));
    }
}
