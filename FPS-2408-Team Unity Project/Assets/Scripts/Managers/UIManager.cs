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
    [Header("Crosshair Settings")]
    [Space]
    [SerializeField] private int C_maxSpread;
    [SerializeField] private int C_spreadFactor;
    [SerializeField] private Color C_crosshairColor;
    [SerializeField] private float C_lineDistance;
    [SerializeField] private float C_lineLength;
    [SerializeField] private float C_lineThickness;
    [SerializeField] private float C_centerDotSize;
    [SerializeField] private Crosshair crosshairRef;

    [Space]
    [Header("Misc")]
    [Space]
    [SerializeField] private TMP_Text currAmmoField;
    [SerializeField] private Image ammoFillup;
    [SerializeField] private TMP_Text enemyCountField;
    [SerializeField] private GameObject enemyCountObj;
    private bool isPause = false;
    [SerializeField] private UIObj[] ConstUI;




  
    [System.Serializable]
    private struct UIObj
    {
        [HideInInspector] public bool CUI_currentState;

        public GameObject CUI_obj;
    }
    [System.Serializable]
    private struct Crosshair
    {
        public GameObject centerDot;
        public GameObject[] horizontalLine;
        public GameObject[] verticalLine;
    }
   
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
    private void Start()
    {
        UpdateCrosshair();

    }
    private void Update()
    {
        if (BootLoadManager.instance == null || (BootLoadManager.instance != null && !BootLoadManager.instance.IsLoading()))
        {
            if (Input.GetButtonDown("Cancel"))
            {
                if (menuActive == null)
                {
                    StatePause();
                }
                else if (menuActive == menuPause)
                {

                    StateUnpause();

                }
            }
        }
    }

    public void StatePause()
    {
        SetPause(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        if (menuActive != null && menuActive.activeInHierarchy) menuActive.SetActive(false);
        menuActive = menuPause;
        menuActive.SetActive(true);
        
    }
    public void StateUnpause()
    {
        SetPause(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (menuActive != null)
        {
            menuActive.SetActive(false);
        }
        menuActive = null;
        for (int i = 0; i < ConstUI.Length; i++)
        {
            if (ConstUI[i].CUI_obj != null)
            {
                ConstUI[i].CUI_obj.SetActive(ConstUI[i].CUI_currentState);
            }
        }
    }
    public void SetPause(bool _val)
    {
        isPause = _val;
        Time.timeScale = _val ? 0 : 1;
        if (_val == true) return;
        for (int i = 0; i < ConstUI.Length; i++)
        {
            if (ConstUI[i].CUI_obj != null)
            {
                ConstUI[i].CUI_currentState = ConstUI[i].CUI_obj.activeInHierarchy;
                ConstUI[i].CUI_obj.SetActive(false);
            }
        }
    }
    public void OpenLoseMenu()
    {
        SetPause(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
    public void ToggleWinMenu(bool _val)
    {
        menuWin.SetActive(_val);
    }

    public void AmmoDisplay(int curr, int max)
    {
        //currAmmo
        ammoFillup.fillAmount = (float)curr / max;
        currAmmoField.text = curr + "/" + max;
    }
    public void UpdateAmmoFill(float curr, float max)
    {
        ammoFillup.fillAmount = curr / max;

    }
    public void UpdateCrosshair()
    {
        crosshairRef.centerDot.GetComponent<Image>().color = C_crosshairColor;
        crosshairRef.centerDot.transform.localScale = Vector2.one * C_centerDotSize;

        for (int i = 0; i < crosshairRef.horizontalLine.Length; i++)
        {
            GameObject curr = crosshairRef.horizontalLine[i];


            curr.GetComponent<Image>().color = C_crosshairColor;
            Vector2 normDir = new Vector2(curr.transform.localPosition.normalized.x,0);
            curr.transform.localPosition = normDir * C_lineDistance + normDir * (curr.transform.localScale.x / 2) + normDir * C_centerDotSize/2;
            curr.transform.localScale = new Vector2(C_lineLength, C_lineThickness);

        }

        for (int i = 0; i < crosshairRef.verticalLine.Length; i++)
        {
            GameObject curr = crosshairRef.verticalLine[i];

            curr.GetComponent<Image>().color = C_crosshairColor;
            Vector2 normDir = new Vector2(0,curr.transform.localPosition.normalized.y);
            curr.transform.localPosition = normDir * C_lineDistance + normDir * (curr.transform.localScale.y / 2) + normDir * C_centerDotSize / 2;
            curr.transform.localScale = new Vector2(C_lineThickness, C_lineLength);
        }
    }
    public void UpdateCrosshairSpread(float _val)
    {
        for (int i = 0; i < crosshairRef.horizontalLine.Length; i++)
        {
            GameObject curr = crosshairRef.horizontalLine[i];
            Vector2 normDir = new Vector2(curr.transform.localPosition.normalized.x, 0);
            curr.transform.localPosition = normDir * C_lineDistance + normDir * (curr.transform.localScale.x / 2) + normDir * C_centerDotSize / 2 + normDir *Mathf.Clamp( _val * C_spreadFactor, 0, C_maxSpread);

        }

        for (int i = 0; i < crosshairRef.verticalLine.Length; i++)
        {
            GameObject curr = crosshairRef.verticalLine[i];
            Vector2 normDir = new Vector2(0, curr.transform.localPosition.normalized.y);
            curr.transform.localPosition = normDir * C_lineDistance + normDir * (curr.transform.localScale.y / 2) + normDir * C_centerDotSize / 2 + normDir * Mathf.Clamp(_val * C_spreadFactor, 0, C_maxSpread);
        }

    }
}
