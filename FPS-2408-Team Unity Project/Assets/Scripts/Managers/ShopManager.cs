using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] GameObject shop;
    [SerializeField] GameObject scrapObj;
    [SerializeField] TMP_Text scrapText;

    // Start is called before the first frame update
    void Start()
    {
        shop.SetActive(true);
    }

    // Update is called once per frame

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OpenShop();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CloseShop();
        }
    }



    public void OpenShop()
    {
        UIManager.instance.FadeUI(true);
        shop.SetActive(true);
    }

    public void CloseShop() 
    {
        UIManager.instance.FadeUI(false);
        shop.SetActive(false);
    }
}
