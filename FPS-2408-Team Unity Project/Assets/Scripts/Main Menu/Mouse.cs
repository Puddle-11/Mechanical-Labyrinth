using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{

    [SerializeField] private Camera mainCamera;
    [SerializeField] private float zOffset;
    [SerializeField] private bool DisplayDefaultCursor;
    [SerializeField] private MainSceneManager MSMRef;
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
        IMenuButton MBref;
        if (collision.TryGetComponent<IMenuButton>(out MBref))
        {
            MSMRef.currentButton = MBref;
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        IMenuButton MBref;
        if (collision.TryGetComponent<IMenuButton>(out MBref))
        {
            if (MSMRef.currentButton == MBref)
            {
                MSMRef.currentButton = null;
            }
        }

    }
  
}
