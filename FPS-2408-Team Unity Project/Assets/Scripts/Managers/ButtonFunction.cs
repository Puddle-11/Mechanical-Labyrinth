using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunction : MonoBehaviour
{

    public void resume()
    {
        UIManager.instance.StateUnpause();
    }
    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        UIManager.instance.StateUnpause();
    }
    public void quit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void respawn()
    {
        GameManager.instance.respawn();
    }
    // Start is called before the first frame update

}
