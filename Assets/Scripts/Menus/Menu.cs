using System;
using System.Collections.Generic;
using Mirror;
using Mirror.FizzySteam;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    private GameNetworkManager _gameNatMan;
    public GameObject landingPagePanel;
    private bool useSteam = false;
    public TMP_Text lobbyNameText;
    public TMP_Text steamErrorText;
    public GameObject afterSteam;
    public GameObject locallyOrSteam;

    // For SteamLobbies
    // Source: https://github.com/FatRodzianko/steamworks-tutorial/blob/main/LICENSE
    [Header("Lobby List UI")] [SerializeField]
    private GameObject LobbyListPanel;

    [SerializeField] private GameObject LobbyListItemPrefab;
    [SerializeField] private GameObject ContentPanel;
    [SerializeField] private TMP_InputField searchBox;
    public bool didPlayerSearchForLobbies = false;

    [Header("Friends Lobby List UI")] [SerializeField]
    private GameObject FriendsLobbyContentPanel;

    [Header("Create Lobby UI")] [SerializeField]
    private GameObject CreateLobbyPanel;

    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private TMP_InputField localLobbyName_InputField;
    [SerializeField] private Toggle friendsOnlyToggle;
    public bool didPlayerNameTheLobby = false;
    public string lobbyName;
    public List<GameObject> listOfLobbyListItems = new List<GameObject>();

    private void Start()
    {
        _gameNatMan = (GameNetworkManager) NetworkManager.singleton;
        _gameNatMan.menuStart();
        steamErrorText.enabled = false;
        _gameNatMan.setUseSteam(false);
        _gameNatMan.updateMenuReference();
    }

    public void HostLobby()
    {
        if (useSteam)
        {
            if (SteamManager.Initialized) // TODO: && has internet connection
            {
                landingPagePanel.SetActive(false);

                ELobbyType newLobbyType;
                if (friendsOnlyToggle.isOn)
                {
                    newLobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;
                }
                else
                {
                    newLobbyType = ELobbyType.k_ELobbyTypePublic;
                }

                if (!string.IsNullOrEmpty(lobbyNameInputField.text))
                {
                    didPlayerNameTheLobby = true;
                    lobbyName = lobbyNameInputField.text;
                }

                SteamMatchmaking.CreateLobby(newLobbyType, _gameNatMan.maxConnections);
                return;
            }
            else
            {
                Debug.Log("Steam not initialized!");
            }
        }
        else
        {
            lobbyName = localLobbyName_InputField.text;
            landingPagePanel.SetActive(false);
            _gameNatMan.StartHost();
        }
    }

    public void SteamCheckStartGame()
    {
        if (!SteamManager.Initialized)
        {
            steamErrorText.enabled = true;
        }
        else
        {
            StartGameWithSteam();
            steamErrorText.enabled = false;
            afterSteam.SetActive(true);
            locallyOrSteam.SetActive(false);
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

    public void StartGameWithSteam()
    {
        useSteam = !useSteam;
        _gameNatMan.setUseSteam(useSteam);
        _gameNatMan.menuStart();
    }

    public void setUseSteam(bool steam)
    {
        useSteam = steam;
        _gameNatMan.setUseSteam(steam);
    }

    public void GetListOfLobbies(bool _public)
    {
        _gameNatMan.GetListOfLobbies(_public);
    }

    public void JoinLobby(CSteamID lobbyId)
    {
        SteamMatchmaking.JoinLobby(lobbyId);
    }

    public void DisplayLobbies(List<CSteamID> lobbyIDS, LobbyDataUpdate_t result)
    {
        for (int i = 0; i < lobbyIDS.Count; i++)
        {
            if (lobbyIDS[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                if (didPlayerSearchForLobbies)
                {
                    if (SteamMatchmaking.GetLobbyData((CSteamID) lobbyIDS[i].m_SteamID, "name").ToLower()
                        .Contains(searchBox.text.ToLower()))
                    {
                        GameObject newLobbyListItem = Instantiate(LobbyListItemPrefab) as GameObject;
                        LobbyListItem newLobbyListItemScript = newLobbyListItem.GetComponent<LobbyListItem>();
                        newLobbyListItemScript.lobbySteamId = (CSteamID) lobbyIDS[i].m_SteamID;
                        newLobbyListItemScript.lobbyName =
                            SteamMatchmaking.GetLobbyData((CSteamID) lobbyIDS[i].m_SteamID, "name");
                        newLobbyListItemScript.numberOfPlayers =
                            SteamMatchmaking.GetNumLobbyMembers((CSteamID) lobbyIDS[i].m_SteamID);
                        newLobbyListItemScript.maxNumberOfPlayers =
                            SteamMatchmaking.GetLobbyMemberLimit((CSteamID) lobbyIDS[i].m_SteamID);
                        newLobbyListItemScript.SetLobbyItemValues();
                        newLobbyListItem.transform.SetParent(ContentPanel.transform);
                        newLobbyListItem.transform.localScale = Vector3.one;

                        listOfLobbyListItems.Add(newLobbyListItem);
                    }
                }
                else
                {
                    GameObject newLobbyListItem = Instantiate(LobbyListItemPrefab) as GameObject;
                    LobbyListItem newLobbyListItemScript = newLobbyListItem.GetComponent<LobbyListItem>();
                    newLobbyListItemScript.lobbySteamId = (CSteamID) lobbyIDS[i].m_SteamID;
                    newLobbyListItemScript.lobbyName =
                        SteamMatchmaking.GetLobbyData((CSteamID) lobbyIDS[i].m_SteamID, "name");
                    newLobbyListItemScript.numberOfPlayers =
                        SteamMatchmaking.GetNumLobbyMembers((CSteamID) lobbyIDS[i].m_SteamID);
                    newLobbyListItemScript.maxNumberOfPlayers =
                        SteamMatchmaking.GetLobbyMemberLimit((CSteamID) lobbyIDS[i].m_SteamID);
                    newLobbyListItemScript.SetLobbyItemValues();
                    newLobbyListItem.transform.SetParent(ContentPanel.transform);
                    newLobbyListItem.transform.localScale = Vector3.one;

                    listOfLobbyListItems.Add(newLobbyListItem);
                }

                return;
            }
        }

        if (didPlayerSearchForLobbies)
            didPlayerSearchForLobbies = false;
    }

    public void DisplayFriendsLobbies(List<CSteamID> lobbyIDS)
    {
        for (int i = 0; i < lobbyIDS.Count; i++)
        {
            GameObject newLobbyListItem = Instantiate(LobbyListItemPrefab) as GameObject;
            LobbyListItem newLobbyListItemScript = newLobbyListItem.GetComponent<LobbyListItem>();

            newLobbyListItemScript.lobbySteamId = (CSteamID) lobbyIDS[i].m_SteamID;
            newLobbyListItemScript.lobbyName =
                SteamMatchmaking.GetLobbyData((CSteamID) lobbyIDS[i].m_SteamID, "name");
            newLobbyListItemScript.numberOfPlayers =
                SteamMatchmaking.GetNumLobbyMembers((CSteamID) lobbyIDS[i].m_SteamID);
            newLobbyListItemScript.maxNumberOfPlayers =
                SteamMatchmaking.GetLobbyMemberLimit((CSteamID) lobbyIDS[i].m_SteamID);
            newLobbyListItemScript.SetLobbyItemValues();
            newLobbyListItem.transform.SetParent(FriendsLobbyContentPanel.transform);
            newLobbyListItem.transform.localScale = Vector3.one;
            listOfLobbyListItems.Add(newLobbyListItem);
        }
    }

    public void DestroyOldLobbyListItems()
    {
        foreach (GameObject lobbyListItem in listOfLobbyListItems)
        {
            GameObject lobbyListItemToDestroy = lobbyListItem;
            Destroy(lobbyListItemToDestroy);
            lobbyListItemToDestroy = null;
        }

        listOfLobbyListItems.Clear();
    }

    public void SearchForLobby()
    {
        if (!string.IsNullOrEmpty(searchBox.text))
        {
            didPlayerSearchForLobbies = true;
        }
        else
            didPlayerSearchForLobbies = false;

        GetListOfLobbies(true); // TODO change depending on if friends/public
    }
}