using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BootLoadManager : MonoBehaviour
{
    [SerializeField] private string startScene;
    public static BootLoadManager instance;
     public delegate void OnSceneChange();
    public OnSceneChange sceneChangeEvent;
    public OnSceneChange gameSceneChangeEvent;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
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
    public void LoadScene(string _sceneName)
    {
        Debug.Log("Attemped Load Scene: " + _sceneName);
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name != "BootLoader" && SceneManager.GetSceneAt(i).name != "GameLoader")
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i).name);
            }

        }
        if (sceneChangeEvent != null)
        {
            sceneChangeEvent.Invoke();
        }
        SceneManager.LoadScene(_sceneName, LoadSceneMode.Additive);
    }
    public void LoadGameScene(string _sceneName)
    {
        
        if (GameManager.instance == null)
        {
            EnterGameMode();
        }

        LoadScene(_sceneName);
        StartCoroutine(InvokeGameSceneChange());
    }
    private IEnumerator InvokeGameSceneChange()
    {
        yield return null;
        if (gameSceneChangeEvent != null) gameSceneChangeEvent.Invoke();
    }
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
}
