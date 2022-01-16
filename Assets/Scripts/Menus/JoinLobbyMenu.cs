using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel;
    [SerializeField] private TMP_InputField addressInput;
    [SerializeField] private Button joinButton;

    private void OnEnable()
    {
        GameNetworkManager.ClientOnConnected += HandleClientConnected;
        GameNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        GameNetworkManager.ClientOnConnected -= HandleClientConnected;
        GameNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
    }

    public void Join()
    {
        //read the address from the input text
        string address = addressInput.text;
        //tell Mirror to connect to this address
        ((GameNetworkManager) NetworkManager.singleton).networkAddress = address;

        ((GameNetworkManager) NetworkManager.singleton).StartClient();

        joinButton.interactable = false;
    }

    //Client successfully connects
    private void HandleClientConnected()
    {
        joinButton.interactable = true;
        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }

    //Failed to joined 
    private void HandleClientDisconnected()
    {
        //joinButton.interactable = true;
    }
}