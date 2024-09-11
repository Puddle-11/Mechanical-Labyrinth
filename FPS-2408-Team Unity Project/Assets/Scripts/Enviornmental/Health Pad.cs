using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPad : MonoBehaviour
{
    //[SerializeField] int HealAmount;
    [SerializeField] GameObject pad;
    bool OnPad = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.instance.playerRef)
        {
            OnPad = true;
            HealPlayer();
            FlashGreen();
        }
    }
    void HealPlayer() {
        GameManager.instance.playerControllerRef.ResetHealth();

    }
    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator FlashGreen() {
        pad.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        pad.SetActive(false);
    }
}
