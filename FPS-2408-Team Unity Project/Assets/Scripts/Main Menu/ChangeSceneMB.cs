using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneMB : BaseMenuButton, IMenuButton
{
    [SerializeField] private string sceneName;
    [SerializeField] private CurrentStats save;
    public override void Click()
    {
        BootLoadManager.instance.LoadGameScene(sceneName);
        BootLoadManager.instance.SetSave(save);

    }

}
