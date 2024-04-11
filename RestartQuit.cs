using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class RestartQuit : MonoBehaviour
{
    public void Restart()
    {
        Time.timeScale = 1;
        if (PlayerControls.inventory != null)
        {
            PlayerControls.inventory.Clear();
        }
        SceneManager.LoadScene("1stDay");
        PlayerControls.instance = FindObjectOfType<PlayerControls>();
        if (PlayerControls.instance != null)
        {
            PlayerControls.instance.ResetMovement();
        }
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
        if (PlayerControls.inventory != null)
        {
            PlayerControls.inventory.Clear();
        }
    }
}
