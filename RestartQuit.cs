using UnityEngine;
using UnityEngine.SceneManagement;

// Logic for the restart and quit buttons on the death screen/no time screen 
public class RestartQuit : MonoBehaviour
{
    private PlayerControls playerControls;
    public void Restart()
    {
        Time.timeScale = 1;
        if (PlayerControls.inventory != null)
        {
            PlayerControls.inventory.Clear();
        }

        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            SceneManager.LoadScene("Tutorial");
            PlayerControls.isUsingGreen = false;
            PlayerControls.instance = FindObjectOfType<PlayerControls>();
            if (PlayerControls.instance != null)
            {
                PlayerControls.instance.ResetMovement();
            }
            if (PlayerControls.animator.GetBool("isDead"))
            {
                PlayerControls.animator.SetBool("isDead", false);
            }
        }
        else if (SceneManager.GetActiveScene().name == "1stDay")
        {
            SceneManager.LoadScene("1stDay");
            PlayerControls.isUsingGreen = false;
            PlayerControls.instance = FindObjectOfType<PlayerControls>();
            if (PlayerControls.instance != null)
            {
                PlayerControls.instance.ResetMovement();
            }
            if (PlayerControls.animator.GetBool("isDead"))
            {
                PlayerControls.animator.SetBool("isDead", false);
            }
        }
        else if (SceneManager.GetActiveScene().name == "TestMap")
        {
            SceneManager.LoadScene("TestMap");
            PlayerControls.isUsingGreen = false;
            PlayerControls.instance = FindObjectOfType<PlayerControls>();
            if (PlayerControls.instance != null)
            {
                PlayerControls.instance.ResetMovement();
            }
            if (PlayerControls.animator.GetBool("isDead"))
            {
                PlayerControls.animator.SetBool("isDead", false);
            }
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
