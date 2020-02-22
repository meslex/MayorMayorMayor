using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public GameObject pauseMenu;
    public bool IsPaused { get { return paused; } }
    private bool paused;

    void Start()
    {
        if(pauseMenu != null)
            pauseMenu.SetActive(false);
        paused = false;
    }

    void Update()
    {
        if (Input.GetKeyUp("escape"))
        {
            if (!paused)
            {
                pauseMenu.SetActive(true);
                paused = true;
                Time.timeScale = 0f;
            }
            else
            {
                Resume();
            }
           
        }

    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        paused = false;
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        Resume();
        SceneManager.LoadScene("Main");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
