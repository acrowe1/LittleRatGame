using UnityEngine;
using UnityEngine.SceneManagement;

// Contains code for the buttons on the main menu 
// The commented out functions are functions that were previously used, but aren't used anymore, but could be useful maybe 
public class MainMenu : MonoBehaviour
{
    public PlayerControls playerControls;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void PlayTutorial()
    {
        if (playerControls == null)
        {
            Debug.LogError("PlayerControls is not assigned in the Inspector!");
            return;
        }

        Time.timeScale = 1;
        SceneManager.LoadScene("Tutorial");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (playerControls == null)
        {
            Debug.LogError("PlayerControls is not assigned in the Inspector!");
            return;
        }

        if (scene.name == "Tutorial")
        {
            PlayerControls foundPlayerControls = FindObjectOfType<PlayerControls>();
            if (foundPlayerControls != null)
            {
                foundPlayerControls.ResetMovement();
            }
            else
            {
                Debug.LogError("Player controls is null!");
            }
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
    }

    public void QuitToMenu()
    {
        Time.timeScale = 0;
        SceneManager.LoadScene("MainMenu");
    }
}
