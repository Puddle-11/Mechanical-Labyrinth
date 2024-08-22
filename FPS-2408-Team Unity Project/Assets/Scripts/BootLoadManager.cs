using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BootLoadManager : MonoBehaviour
{
    [SerializeField] private string startScene;
    public static BootLoadManager instance;
     public delegate void SceneEvent();
    public SceneEvent startLoadEvent;
    public SceneEvent stopLoadEvent;
    public SceneEvent sceneChangeEvent;
    private void Awake()
    {
        if(instance == null)instance = this;
        else
        {
            Debug.LogWarning("ERROR: Two boot loaders initialized, make sure the bootloader scene is only being run once on start");
            Destroy(this);
        }
    }
    private void Start()
    {

        LoadScene(startScene);
    }

    #region LoadScene
    public void LoadScene(string _sceneName)
    {
        startLoadEvent?.Invoke();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name != "BootLoader" && SceneManager.GetSceneAt(i).name != "GameLoader")
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i).name);
            }

        }

         SceneManager.LoadScene(_sceneName, LoadSceneMode.Additive);
        StartCoroutine(SetScene(_sceneName));
    }
    public IEnumerator SetScene(string _sceneName)
    {
        yield return null;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sceneName));
        stopLoadEvent?.Invoke();
        sceneChangeEvent?.Invoke();
    }
    public void LoadGameScene(string _sceneName)
    {
        
        if (GameManager.instance == null)
        {
            EnterGameMode();
        }

        LoadScene(_sceneName);
    }
    #endregion

    #region EnterExit Gamemode
    public void EnterGameMode()
    {
        UnLoadScene("GameLoader");
        SceneManager.LoadScene("GameLoader", LoadSceneMode.Additive);
    }
    public void ExitGameMode()
    {
        UnLoadScene("GameLoader");
        LoadScene("Main Menu");
    }
    public void EnterGameMode(string _sceneName)
    {
        UnLoadScene("GameLoader");
        SceneManager.LoadScene("GameLoader", LoadSceneMode.Additive);
        LoadScene(_sceneName);
    }
    #endregion

 
    #region Unload Scene
    public void UnLoadScene(string _sceneName)
    {
        UnLoadScene(new string[] {_sceneName});
    }
    public void UnLoadScene(string[] _scenes)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            for (int j = 0; j < _scenes.Length; j++)
            {

                if (SceneManager.GetSceneAt(i).name == _scenes[j])
                {
                    SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i).name);
                }
            }

        }
    }
    #endregion
}
