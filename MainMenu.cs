using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("1stDay");
        PlayerControls playerControls = FindObjectOfType<PlayerControls>();
        if (playerControls != null)
        {
            playerControls.ResetMovement();
        }
        else
        {
            Debug.LogError("Player controls is null!");
        }
    }
}
