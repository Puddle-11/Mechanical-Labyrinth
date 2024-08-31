using UnityEngine;
using UnityEngine.UI;

public class EntityHealthBar : MonoBehaviour
{
    [SerializeField] private Image HealthBar;
    public void UpdateHealthBar(float curr, float max)
    {
        HealthBar.fillAmount = curr / max;
    }
    private void Update()
    {
        if(GameManager.instance != null) transform.LookAt(new Vector3(GameManager.instance.playerRef.transform.position.x, transform.position.y, GameManager.instance.playerRef.transform.position.z));
    }
}
