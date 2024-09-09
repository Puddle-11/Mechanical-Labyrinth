using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneMB : BaseMenuButton, IMenuButton
{
    [SerializeField] private string sceneName;
    [SerializeField] private CurrentStats save;
    [SerializeField] private TMP_Text levelField;
    [SerializeField] private TMP_Text deathField;
     
    public void Update()
    {
       if(levelField != null) levelField.text = save.S_Level.ToString();
       if(deathField != null)  deathField.text = save.S_TotalDeaths.ToString();
    }
    public override void Click()
    {
        BootLoadManager.instance.LoadGameScene(sceneName);
        BootLoadManager.instance.SetSave(save);

    }

}
