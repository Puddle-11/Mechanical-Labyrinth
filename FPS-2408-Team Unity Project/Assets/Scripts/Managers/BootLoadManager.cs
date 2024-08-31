using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class BootLoadManager : MonoBehaviour
{
    [SerializeField] private string startScene;
    [SerializeField] private Image loadingBar;
    [SerializeField] private GameObject loadingScreenObj;
    public static BootLoadManager instance;
     public delegate void SceneEvent();
    public SceneEvent startLoadEvent;
    public SceneEvent stopLoadEvent;
    private bool inLoadScreen;

    public bool IsLoading()
    {
        return inLoadScreen;
    }
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
        startLoadEvent += OpenLoadMenu;
        stopLoadEvent += CloseLoadMenu;
        LoadScene(startScene);
    }

    public void OpenLoadMenu()
    {
        inLoadScreen = true;
        loadingBar.fillAmount = 0;
        loadingScreenObj.SetActive(true);
    }
    public void CloseLoadMenu()
    {
        inLoadScreen = false;

        loadingScreenObj.SetActive(false);
    }
    public void UpdateLoadingBar(float _val)
    {
        loadingBar.fillAmount = _val;
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
        if (GameManager.instance == null) stopLoadEvent?.Invoke();
    }
    public void LoadGameScene(string _sceneName)
    {
        
        if (GameManager.instance == null)
        {
            EnterGameMode();
        }
        StartCoroutine(DelayLoadScene(_sceneName));
    }
    public IEnumerator DelayLoadScene(string _sceneName)
    {
        yield return null;
        LoadScene(_sceneName);
    }
    public void ReloadScene()
    {
        string name = SceneManager.GetActiveScene().name;
        UnLoadScene(name);
        LoadScene(name);
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
