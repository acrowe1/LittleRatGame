using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class KeyboardInputEvent : MonoBehaviour
{
    [System.Serializable]
    public class KeyboardEvent : UnityEvent<KeyCode> { }

    public KeyboardEvent onKeyDown;

    void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    if (onKeyDown != null)
                    {
                        onKeyDown.Invoke(keyCode);
                    }
                }
            }
        }
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GoTo1stDay()
    {
        SceneManager.LoadScene("1stDay");
    }

    public void GoToTestMap()
    {
        SceneManager.LoadScene("TestMap");
    }
}
