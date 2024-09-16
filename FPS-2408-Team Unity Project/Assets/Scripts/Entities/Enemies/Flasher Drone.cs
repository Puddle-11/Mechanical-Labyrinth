using System.Collections;
using UnityEngine;

public class FlasherDrone : BaseEnemy
{
    [SerializeField] private float coolDown;
    [SerializeField] private float flashDuration;
    [SerializeField] private EntityHealthBar flashProgress;
    private bool hasFlashed = false;
    #region Monobehavior Methods
    public override  void Start()
    {
        flashProgress.UpdateShieldBar(0);
        base.Start();
    }
    public override void Update()
    {
            if (IsInRange())
            {
            
                    StartCoroutine(FlashEffect());
            }

        base.Update();
    }

    private IEnumerator FlashEffect()
    {
        if (hasFlashed == true) yield break;
        hasFlashed = true;

        float timer = 0;
        while (timer < coolDown)
        {
            flashProgress.UpdateHealthBar(timer/coolDown);

            timer += Time.deltaTime;
            yield return null;
        }
        if(IsEnemyInPlayerView() ) UIManager.instance.FlashScreen(flashDuration);
        hasFlashed = false;
    }
    private bool IsEnemyInPlayerView()
    {
        Vector3 viewportPos = CameraController.instance.mainCamera.WorldToViewportPoint(transform.position);
        if(viewportPos.x > 0 && viewportPos.x <= 1 && viewportPos.y > 0 && viewportPos.y <= 1 && viewportPos.z > 0)
        {
            return true;
        }
        return false;

    }
    #endregion
}
