using System.Collections;
using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class InGameMenu : NetworkBehaviour
{
    public GameObject Panel;
    private bool GameIsPaused = false;
    private GameNetworkManager _gameNetworkManager;


    void Start()
    {
        _gameNetworkManager = ((GameNetworkManager) NetworkManager.singleton);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

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
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Panel.SetActive(false);
    }

    void Pause()
    {
        GameIsPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Panel.SetActive(true);
    }

    public void LeaveGame()
    {
        if (isServer)
        {
            _gameNetworkManager.StopHost();
            NetworkServer.Shutdown();
        }
        
        if (_gameNetworkManager.usingSteam)
        {
            SteamMatchmaking.LeaveLobby(SteamUser.GetSteamID());
        }

        _gameNetworkManager.StopClient();

        SceneManager.LoadScene(0);
    }
}