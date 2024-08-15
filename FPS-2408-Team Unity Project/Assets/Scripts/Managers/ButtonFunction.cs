using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunction : MonoBehaviour
{
    public void resume()
    {
        UIManager.instance.stateUnpause();
    }
    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        UIManager.instance.stateUnpause();
    }
    public void quit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    // Start is called before the first frame update

}
