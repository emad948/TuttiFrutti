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
    private GameNetworkManager _gameNatMan;


    void Start()
    {
        _gameNatMan = ((GameNetworkManager) NetworkManager.singleton);
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
        if (_gameNatMan.usingSteam)
        {
            SteamMatchmaking.LeaveLobby(_gameNatMan.currentLobbyID);
        }

        if (isServer)
        {
            _gameNatMan.StopHost();
            NetworkServer.Shutdown();
        }

        _gameNatMan.StopClient();

        SceneManager.LoadScene(0);
    }
}