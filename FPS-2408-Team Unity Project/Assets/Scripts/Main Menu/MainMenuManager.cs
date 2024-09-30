using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class MainMenuManager : MonoBehaviour
{


    public IMenuButton currentButton;
    public float buttonHoverScale;
    public AudioSource clickSFXSource;
    public AudioSource selectSFXSource;

    public AudioClip clickSound;
    public float clickVolume;
    public float selectVolume;

    [SerializeField] private GameObject currMenu;
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void ChangeMenu(GameObject _newMenu)
    {
        if (_newMenu == null) return;
        if (currMenu != null) currMenu.SetActive(false);
        currMenu = _newMenu;
        currMenu.SetActive(true);
    }

    public void ChangeButton(IMenuButton _nextButton)
    {
        if (AudioManager.instance != null && selectSFXSource != null)
        {
            AudioManager.instance.PlaySound(selectSFXSource, clickSound, SettingsController.soundType.SFX);
        }

        if (currentButton != null) currentButton.Deselect();
        currentButton = _nextButton;
        if (currentButton != null) currentButton.Select();
    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0) && currentButton != null)
        {
            if (AudioManager.instance != null && clickSFXSource != null)
            {

                AudioManager.instance.PlaySound(clickSFXSource, clickSound, SettingsController.soundType.SFX);

            }
            currentButton.Click();
        }
    }
}
