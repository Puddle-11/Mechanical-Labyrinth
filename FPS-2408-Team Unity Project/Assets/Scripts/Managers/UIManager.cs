using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
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
    [SerializeField] GameObject menuShop;
    [SerializeField] GameObject gunMenuShop;
    [SerializeField] GameObject primaryGunMenuShop;
    [SerializeField] GameObject secondaryGunMenuShop;
    [SerializeField] GameObject ammoMenuShop;
    [SerializeField] GameObject itemMenuShop;
    [SerializeField] GameObject menuControlsLegend;
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
    [SerializeField] private GameObject crosshairObjRef;
    [SerializeField] private GameObject ADSCrosshairObjRef;
    [SerializeField] private int C_maxSpread;
    [SerializeField] private int C_spreadFactor;
    [SerializeField] private Color C_crosshairColor;
    [SerializeField] private float C_lineDistance;
    [SerializeField] private float C_lineLength;
    [SerializeField] private float C_lineThickness;
    [SerializeField] private float C_centerDotSize;
    [SerializeField] private Crosshair crosshairRef;
    [Space]
    [Header("Loss Screen Stats")]

    [Space]
    [SerializeField] private TMP_Text enemiesKilled;
    [SerializeField] private TMP_Text damageDealt;
    [SerializeField] private TMP_Text attemptNumber;

    [SerializeField] private GameObject runStatsObj;

    [Space]
    [Header("Misc")]
    [Space]
    [SerializeField] private TMP_Text currAmmoField;
    [SerializeField] private Image ammoFillup;
    [SerializeField] private TMP_Text enemyCountField;
    [SerializeField] private GameObject enemyCountObj;
    [SerializeField] private Animator UIFadeAnim;
    [SerializeField] private GameObject scrapCountObj;
    [SerializeField] private TMP_Text scrapCount;
    [SerializeField] private TMP_Text pauseScrapCount;
    [SerializeField] private TMP_Text shopScrapCount;

    [Space]
    [Header("Ammo")]
    [Space]
    [SerializeField] private TMP_Text currAmmoInvAmount;
    [SerializeField] private GameObject currAmmoInvParent;
    [SerializeField] private Image currAmmoInvIcon;
    [SerializeField] private Image[] ammoInvIcons;
    [SerializeField] private TMP_Text[] ammoInvAmount;
   
    public UIObj[] ConstUI;
    private bool showingControls = true;
    private int currExternalAmmoInv;
    #region Custom Structs and Enums
    [System.Serializable]
    public struct UIObj
    { public bool CUI_currentState;

        public GameObject CUI_obj;
    }
    [System.Serializable]
    private struct Crosshair
    {
        public GameObject centerDot;
        public GameObject[] horizontalLine;
        public GameObject[] verticalLine;
    }
    #endregion

    [Space]
    [Header("Inventory")]
    [Space]
    [SerializeField] private Image[] currItem;



    public void InitializeInventory()
    {
        int size = GeneralInventory.instance.GetInventorySize();
        for (int i = 0; i < size; ++i)
        {
            Instantiate(Slot, UI);


        }

    }

    public void UpdateExternalAmmoInv(bool _active = true, int _type = 0)
    {
        if(currAmmoInvParent != null) currAmmoInvParent.SetActive(_active);
        currExternalAmmoInv = _active ? _type : -1;
        if(currAmmoInvAmount != null)
            currAmmoInvAmount.text = AmmoInventory.instance.GetAmmoAmount(_type).ToString();
        if (currAmmoInvIcon != null)
            currAmmoInvIcon.sprite = AmmoInventory.instance.GetAmmoIcon(_type);
    }
    public void UpdateInternalAmmoInv(AmmoInventory.bulletType _type)
    {

        if ((int)_type == currExternalAmmoInv)
        {
            UpdateExternalAmmoInv(true, (int)_type);
        }

        if ((int)_type >= AmmoInventory.instance.GetAmmoTypeCount()) return;
        ammoInvIcons[(int)_type].sprite = AmmoInventory.instance.GetAmmoIcon((int)_type);
        if ((int)_type < ammoInvAmount.Length) ammoInvAmount[(int)_type].text = AmmoInventory.instance.GetAmmoAmount((int)_type).ToString();
    }

    public void UpdateInternalAmmoInv()
    {
        for (int i = 0; i < ammoInvIcons.Length; i++)
        {
            
                if(i == (int)currExternalAmmoInv)
                {
                    UpdateExternalAmmoInv(true,i);
                }

            
            if (i >= AmmoInventory.instance.GetAmmoTypeCount()) break; //Exit if reached end of list

            ammoInvIcons[i].sprite = AmmoInventory.instance.GetAmmoIcon(i);

            if (i >= ammoInvAmount.Length) continue; //skip over amount if undefined

            ammoInvAmount[i].text = AmmoInventory.instance.GetAmmoAmount(i).ToString();
        }
    }
    public void SetAttemptNumber(int _val){attemptNumber.text = _val.ToString();}
    public void SetEnemiesKilled(int _val) {enemiesKilled.text = "Enemies Killed: " + _val.ToString();}
    public void SetDamageDealt(ulong _val){ damageDealt.text = "Damage Dealt: " + _val.ToString();}

    public void ToggleEnemyCount(bool _val){ enemyCountObj.SetActive(_val); }
    public void ResetTempUI() { flashDamageRef.SetActive(false); }
    public void SetEnemyCount(int _val) { enemyCountField.text = _val.ToString();}

    public void UpdateHealthBar(float _val) //Takes a NORMALIZED value
    {
        if (playerHealth != null)
        {
            playerHealth.color = playerHealthColor.Evaluate(_val);
            playerHealth.fillAmount = _val;
        }
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
            if (Input.GetButtonDown("tab"))
            {
                ToggleControlsLegend();
            }
        }
    }

    public void StatePause()
    {
        UpdateInternalAmmoInv();
        runStatsObj.SetActive(true);
        FadeUI(true);
        GameManager.instance.SetPause(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (menuActive != null && menuActive.activeInHierarchy) menuActive.SetActive(false);
        menuActive = menuPause;
        menuActive.SetActive(true);
    }
    public void FadeUI(bool _val)
    {
        UIFadeAnim.SetBool("InUI", _val);
    }
    public void StateUnpause()
    {
        runStatsObj.SetActive(false);

        FadeUI(false);

        GameManager.instance.SetPause(false);
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
    
    public void OpenLoseMenu()
    {
        runStatsObj.SetActive(true);
        UIFadeAnim.SetBool("InUI", true);
        GameManager.instance.SetPause(true);
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
    public void UpdateAmmoFill(float val)
    {
        ammoFillup.fillAmount = val;
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
    public void ToggleADS(bool _val) {ADSCrosshairObjRef.SetActive(_val);}
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
    public void ToggleControlsLegend()
    {
        if (showingControls == true){
                
            menuControlsLegend.SetActive(false);
            showingControls = false;

        }
        else if (showingControls == false)
        {
            menuControlsLegend.SetActive(true);
            showingControls = true;
        }
    }

    public void OpenShop()
    {
        FadeUI(true);
        GameManager.instance.SetPause(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (menuActive != null && menuActive.activeInHierarchy)
        {
            menuActive.SetActive(false);
        }
        menuActive = menuShop;
        menuActive.SetActive(true);
    }

    public void CloseShop() 
    {
        FadeUI(false);
        GameManager.instance.SetPause(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (menuActive != null)
        {
            menuActive.SetActive(false);
        }
        menuActive = null;
    }

    public void GunShop()
    {
        if (menuActive != null && menuActive.activeInHierarchy)
        {
            menuActive.SetActive(false);
        }
        menuActive = gunMenuShop;
        menuActive.SetActive(true);
    }

    public void AmmoShop()
    {
        if (menuActive != null && menuActive.activeInHierarchy)
        {
            menuActive.SetActive(false);
        }
        menuActive = ammoMenuShop;
        menuActive.SetActive(true);
    }

    public void ItemShop()
    {
        if (menuActive != null && menuActive.activeInHierarchy)
        {
            menuActive.SetActive(false);
        }
        menuActive = itemMenuShop;
        menuActive.SetActive(true);
    }

    public void Back(GameObject menu)
    {
        if (menuActive != null && menuActive.activeInHierarchy)
        {
            menuActive.SetActive(false);
        }
        menuActive = menu;
        menuActive.SetActive(true);
    }

    public void PrimaryShop()
    {
        if (menuActive != null && menuActive.activeInHierarchy)
        {
            menuActive.SetActive(false);
        }
        menuActive = primaryGunMenuShop;
        menuActive.SetActive(true);
    }

    public void SecondaryShop() 
    {
        if (menuActive != null && menuActive.activeInHierarchy)
        {
            menuActive.SetActive(false);
        }
        menuActive = secondaryGunMenuShop;
        menuActive.SetActive(true);
    }

    public void ToggleScrapCount(bool _val)
    {
        scrapCountObj.SetActive(_val);
    }

    public void UpdateScrapCount(int _val)
    {
        scrapCount.text = _val.ToString();
    }

    public void UpdatePauseMenuScrapCount(int _val)
    {
        pauseScrapCount.text = _val.ToString();
    }

    public void UpdateShopMenuScrapCount(int _val)
    {
        shopScrapCount.text = _val.ToString();
    }
}
