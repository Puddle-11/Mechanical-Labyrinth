using System.Collections;
using UnityEngine;

public class FlasherDrone : BaseEnemy
{
    [SerializeField] private float coolDown;
    [SerializeField] private float flashDuration;
    private float timeToWait;
    private bool playerInRange = false;
    private bool hasFlashed = false;
    #region Monobehavior Methods

    public override void Update()
    {
            if (playerInRange == true && rendRef[0].currRenderer.isVisible == true && IsEnemyInPlayerView())
            {
                if (!hasFlashed)
                {
                    StartCoroutine(FlashEffect());
                }
            }

        base.Update();
    }

    private IEnumerator FlashEffect()
    {
        hasFlashed = true;
        timeToWait = Time.deltaTime + flashDuration;
        //set UI flashscrrena and activate here
        yield return StartCoroutine(FlashScreen());


        yield return new WaitForSeconds(flashDuration);

        hasFlashed = false;
    }


    private IEnumerator FlashScreen()
    {
        UIManager.instance.flashScreen.SetActive(true);

        //flashScreenImage
        Color initialColor = UIManager.instance.flashScreenImage.color;

        //fully transparent
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);

        //fade out completely
        float timePassed = 0f;
        while (timePassed < flashDuration)
        {
            timePassed += Time.deltaTime;
            //fade out to transparent
            float alpha = Mathf.Lerp(1f, 0f, timePassed / flashDuration);

            UIManager.instance.flashScreenImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        //set to zero opacity to make sure
        UIManager.instance.flashScreenImage.color = targetColor;

        UIManager.instance.flashScreen.SetActive(false);
    }


    private bool IsEnemyInPlayerView()
    {
        Vector3 directionToEnemy = (transform.position - target.transform.position).normalized;
        float angle = Vector3.Angle(target.transform.forward, directionToEnemy);
        return angle < sightAngle / 2f;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef)
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef)
        {
            playerInRange = false;
        }
    }



    #endregion
}
