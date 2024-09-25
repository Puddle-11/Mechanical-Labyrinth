using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class BootLoadManager : MonoBehaviour
{
    public static BootLoadManager instance;
    [SerializeField] private string startScene;
    [SerializeField] private Image loadingBar;
    [SerializeField] private Animator SceneChangeAnimation;
    [SerializeField] private TMP_Text currLevel;
    [SerializeField] private TMP_Text nextLevel;

    [SerializeField] private GameObject loadingScreenObj;
    [SerializeField] private GameObject sceneChangeObj;


    public delegate void SceneEvent();
    public SceneEvent startLoadEvent;
    public SceneEvent stopLoadEvent;
    private bool inLoadScreen;
    private CurrentStats currentStats;
    private bool runningEndAnimation = false;
    public bool IsLoading() {return inLoadScreen;}
    public CurrentStats GetSave()
    {
        return currentStats;
    }
    public void SetSave(CurrentStats _save)
    {
        currentStats = _save;
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
        if(GameManager.instance != null)
        {
            nextLevel.text = GameManager.instance.GetCurrentLevel().ToString();

            currLevel.text = Mathf.Clamp(GameManager.instance.GetCurrentLevel()-1, 0, Mathf.Infinity).ToString();

        }
        loadingScreenObj.SetActive(true);
    }
    public void EndSceneAnimation()
    {
        StartCoroutine(EndSceneAnimationDelay());
    }
    public IEnumerator EndSceneAnimationDelay()
    {

        if (runningEndAnimation) yield break;
        runningEndAnimation = true;
        sceneChangeObj.SetActive(true);
        SceneChangeAnimation.SetTrigger("EndScene");

        yield return new WaitUntil(() => SceneChangeAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95);
        stopLoadEvent?.Invoke();
        yield return null;
        sceneChangeObj.SetActive(false);
        runningEndAnimation = false;
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
        runningEndAnimation = false;
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
