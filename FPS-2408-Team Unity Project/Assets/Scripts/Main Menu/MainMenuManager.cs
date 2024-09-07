using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{


    public IMenuButton currentButton;
    public float buttonHoverScale;
    [SerializeField] private GameObject currMenu;
    public void ChangeMenu(GameObject _newMenu)
    {
        if (_newMenu == null) return;
        if (currMenu != null) currMenu.SetActive(false);
        currMenu = _newMenu;
        currMenu.SetActive(true);
    }
    public void ChangeButton(IMenuButton _nextButton)
    {
        if(currentButton != null) currentButton.Deselect();
        currentButton = _nextButton;
        if (currentButton != null) currentButton.Select();
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && currentButton != null)
        {
            currentButton.Click();
        }
    }
}
