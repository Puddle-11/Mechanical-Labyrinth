using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunction : MonoBehaviour
{

    public void resume()
    {
        UIManager.instance?.StateUnpause();
    }
    public void restart()
    {
        BootLoadManager.instance?.LoadGameScene(SceneManager.GetActiveScene().name);
        GameManager.instance?.respawn();
        UIManager.instance?.StateUnpause();
    }
    public void quit()
    {
        Time.timeScale = 1;
        BootLoadManager.instance?.ExitGameMode();
    }

    public void respawn()
    {
        GameManager.instance?.respawn();
        GameManager.instance?.MoveToRespawn();
    }
    // Start is called before the first frame update

}
