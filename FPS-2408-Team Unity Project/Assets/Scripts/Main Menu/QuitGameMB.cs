using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGameMB : BaseMenuButton, IMenuButton
{
    public override void Click()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#else
        Application.Quit();

#endif
    }
  
}
