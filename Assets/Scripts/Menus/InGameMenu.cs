using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;


public class InGameMenu : NetworkBehaviour
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
        if (isServer)
        {
            NetworkServer.DisconnectAll();
            ((GameNetworkManager) NetworkManager.singleton).StopHost();
        }
        else
        {
            ((GameNetworkManager) NetworkManager.singleton).StopClient();
        }
        SceneManager.LoadScene(0);
    }

}