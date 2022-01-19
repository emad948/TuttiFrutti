using System;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    [HideInInspector] public GameNetworkManager _gameNetworkManager;
    public GameObject landingPagePanel;
    private bool useSteam = false;
    //[SerializeField] public bool testMode = false;
    public Button toggleSteamButton;
    public TMP_Text steamErrorText;

    private void Start()
    {
        _gameNetworkManager = (GameNetworkManager) NetworkManager.singleton;
        _gameNetworkManager.menuStart();
        steamErrorText.enabled = false;
    }

    public void HostLobby()
    {
        if (useSteam)
        {
            if (SteamManager.Initialized)
            {
                //TODO @Emad change lobby to public ?
                landingPagePanel.SetActive(false);
                SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 18);
                return;
            }
            else
            {
                steamErrorText.enabled = true;
            }
        }
        else
        {
            landingPagePanel.SetActive(false);
            _gameNetworkManager.StartHost();
        }
    }

    public bool getUseSteam()
    {
        return useSteam;
    }

    public void quitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void toggleUseSteam()
    {
        useSteam = !useSteam;
        _gameNetworkManager.setUseSteam(useSteam);
        // if (useSteam)
        // {
        //     toggleSteamButton.GetComponentInChildren<TMP_Text>().color = Color.green;
        // }
        // else
        // {
        //     toggleSteamButton.GetComponentInChildren<TMP_Text>().color = Color.red;
        // }
        _gameNetworkManager.menuStart();
    }
}