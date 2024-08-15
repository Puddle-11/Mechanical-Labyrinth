using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [Space]
    [Header("Menus")]
    [Space]
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuActive;
    [Space]
    [Header("Damage Indicator")]
    [Space]
    [SerializeField] private float flashDamageTime;
    [SerializeField] private GameObject flashDamageRef;
    [Space]
    [Header("Health")]
    [Space]

    [SerializeField] private Image playerHealth;
    [SerializeField] private Gradient playerHealthColor;
    [Space]
    [Header("Misc")]
    [Space]
    [SerializeField] private TMP_Text currAmmoField;
    [SerializeField] private Image ammoFillup;
    [SerializeField] private TMP_Text enemyCountField;
    [SerializeField] private GameObject enemyCountObj;
    private bool isPause = false;

    public void ToggleEnemyCount(bool _val)
    {
        enemyCountObj.SetActive(_val);
    }
    public bool GetStatePaused()
    {
        return isPause;
    }
    public void SetEnemyCount(int _val)
    {
        enemyCountField.text = _val.ToString();
    }
    public void UpdateHealthBar(float _val) //Takes a NORMALIZED value
    {
        playerHealth.color = playerHealthColor.Evaluate(_val);
        playerHealth.fillAmount = _val;

    }
    public IEnumerator flashDamage()
    {
        flashDamageRef.SetActive(true);
        yield return new WaitForSeconds(flashDamageTime);
        flashDamageRef.SetActive(false);

    }
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("found Two UI managers in scene\nDestroyed UIManager at " + gameObject.name);
            Destroy(this);
        }
    }
    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(isPause);

            }
            else if (menuActive == menuPause)
            {

                stateUnpause();

            }
        }
    }

    public void statePause()
    {
        isPause = !isPause;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void stateUnpause()
    {
        isPause = !isPause;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(isPause);
        menuActive = null;
    }
    public void OpenLoseMenu()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
    public void ToggleWinMenu(bool _val)
    {
        menuWin.SetActive(_val);
    }

    public void ammoDisplay(int curr, int max)
    {
        //currAmmo
        ammoFillup.fillAmount = (float)curr / max;
        currAmmoField.text = curr + " / " + max;
    }
    public void updateAmmoFill(float curr, float max)
    {
        ammoFillup.fillAmount = curr / max;

    }
}
