using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class InGameMenu : MonoBehaviour
{
    public GameObject Panel;
    private bool GameIsPaused = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        GameIsPaused = false;
        Panel.SetActive(false);
    }

    void Pause()
    {
        GameIsPaused = true;
        Panel.SetActive(true);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

}