using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    [SerializeField] private MainMenuManager mainMenuManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float zOffset;
    [SerializeField] private bool DisplayDefaultCursor;
    void Update()
    {
        if (Cursor.visible && DisplayDefaultCursor == false)
        {
            Cursor.visible = false;

        }

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = zOffset;
        transform.position = mouseWorldPosition;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IMenuButton MBref))
        {
            mainMenuManager.ChangeButton(MBref);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IMenuButton MBref))
        {
            mainMenuManager.ChangeButton(null);
        }

    }
  
}
