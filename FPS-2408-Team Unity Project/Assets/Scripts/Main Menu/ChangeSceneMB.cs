using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneMB : BaseMenuButton, IMenuButton
{
    [SerializeField] private int sceneIndex;
    public void Click()
    {
        SceneManager.LoadScene(sceneIndex);

    }
}
