using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class FlasherDrone : BaseEnemy
{
    [SerializeField] private GameObject flashScreen;
    [SerializeField] private Image flashScreenImage;
    [SerializeField] private float coolDown;
    [SerializeField] private float flashDuration;
    private float timeToWait;
    private bool playerInRange = false;
    private bool hasFlashed = false;
    #region Monobehavior Methods

    private void Update()
    {
        if (Time.deltaTime >= timeToWait)
        {
            if (playerInRange && IsEnemyInPlayerView())
            {
                if (!hasFlashed)
                {
                    StartCoroutine(FlashEffect());
                }
            }
        }
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
        flashScreen.SetActive(true);

        //flashScreenImage
        Color initialColor = flashScreenImage.color;

        //fully transparent
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);

        //fade out completely
        float timePassed = 0f;
        while (timePassed < flashDuration)
        {
            timePassed += Time.deltaTime;
            //fade out to transparent
            float alpha = Mathf.Lerp(1f, 0f, timePassed / flashDuration);

            flashScreenImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        //set to zero opacity to make sure
        flashScreenImage.color = targetColor;

        flashScreen.SetActive(false);
    }


    private bool IsEnemyInPlayerView()
    {
        Vector3 directionToPlayer = (transform.position - target.transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
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
