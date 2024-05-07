using UnityEngine;
using UnityEngine.SceneManagement;

// Contains code for buttons in the pause menu
// Is also responsible for disabling/re-enabling other canvases when the pause menu is brought up/put away 
public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject healthBarUI;
    public GameObject controlsUI;
    private GunFill gunFill;

    private void Awake()
    {
        GameObject existingHealthBarCanvas = GameObject.FindWithTag("HealthBarCanvas");

        if (existingHealthBarCanvas != null)
        {
            healthBarUI = existingHealthBarCanvas;
        }
        else
        {
            GameObject healthBarCanvas = new GameObject("HealthBarCanvas");
            healthBarCanvas.tag = "HealthBarCanvas";
            healthBarUI = healthBarCanvas;
            DontDestroyOnLoad(healthBarUI);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
                DisableAll();
            }
            else
            {
                Pause();
            }
        }

        GameObject existingHealthBarCanvas = GameObject.FindWithTag("HealthBarCanvas");

        if (existingHealthBarCanvas != null)
        {
            healthBarUI = existingHealthBarCanvas;
        }
        else
        {
            Debug.Log("No health bar canvas");
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        if (healthBarUI != null)
        {
            healthBarUI.SetActive(true);
        }
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Restart()
    {
        if (SceneManager.GetActiveScene().name == "1stDay")
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
        else if (SceneManager.GetActiveScene().name == "TestMap")
        {
            Time.timeScale = 1;
            if (PlayerControls.inventory != null)
            {
                PlayerControls.inventory.Clear();
            }
            SceneManager.LoadScene("TestMap");
            PlayerControls.instance = FindObjectOfType<PlayerControls>();
            if (PlayerControls.instance != null)
            {
                PlayerControls.instance.ResetMovement();
            }
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        if (healthBarUI != null)
        {
            healthBarUI.SetActive(false);
        }
        else
        {
            Debug.Log("Health bar UI is null");
        }
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ReloadTutorial()
    {
        Time.timeScale = 1;
        if (PlayerControls.inventory != null)
        {
            PlayerControls.inventory.Clear();
        }
        PlayerControls.isUsingGreen = false;
        SceneManager.LoadScene("Tutorial");
        PlayerControls.instance = FindObjectOfType<PlayerControls>();
        if (PlayerControls.instance != null)
        {
            PlayerControls.instance.ResetMovement();
        }
        foreach (var display in FindObjectsOfType<InventoryDisplay>())
        {
            display.ClearAllSlots();
        }
    }

    void DisableAll()
    {
        pauseMenuUI.SetActive(false);
        controlsUI.SetActive(false);
    }
}
