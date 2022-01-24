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

    //[SerializeField] public bool testMode = false;
    public TMP_Text steamErrorText;

    private void Start()
    {
        _gameNatMan = (GameNetworkManager) NetworkManager.singleton;
        _gameNatMan.menuStart();
        steamErrorText.enabled = false;
        _gameNatMan.setUseSteam(false);
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
                    //Debug.Log("CreateNewLobby: friendsOnlyToggle is on. Making lobby friends only.");
                    newLobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;
                }
                else
                {
                    //Debug.Log("CreateNewLobby: friendsOnlyToggle is OFF. Making lobby public.");
                    newLobbyType = ELobbyType.k_ELobbyTypePublic;
                }

                if (!string.IsNullOrEmpty(lobbyNameInputField.text))
                {
                    //Debug.Log("CreateNewLobby: player created a lobby name of: " + lobbyNameInputField.text);
                    didPlayerNameTheLobby = true;
                    //lobbyName = lobbyNameInputField.text;
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
            landingPagePanel.SetActive(false);
            _gameNatMan.StartHost();
        }
    }

    public GameObject afterSteam;
    public GameObject locallyOrSteam;
    
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
    [SerializeField] private Toggle friendsOnlyToggle;

    public bool didPlayerNameTheLobby = false;
    //public string lobbyName;
    public List<GameObject> listOfLobbyListItems = new List<GameObject>();

    public void GetListOfLobbies()
    {
        //Debug.Log("Trying to get list of available lobbies ...");
        //LobbyListPanel.SetActive(true);

        _gameNatMan.GetListOfLobbies();
    }

    public void JoinLobby(CSteamID lobbyId)
    {
        Debug.Log("JoinLobby: Will try to join lobby with steam id: " + lobbyId.ToString());
        SteamMatchmaking.JoinLobby(lobbyId);
    }

    public void DisplayLobbies(List<CSteamID> lobbyIDS, LobbyDataUpdate_t result)
    {
        for (int i = 0; i < lobbyIDS.Count; i++)
        {
            if (lobbyIDS[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                //Debug.Log("Lobby " + i + " :: " + SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDS[i].m_SteamID, "name") + " number of players: " + SteamMatchmaking.GetNumLobbyMembers((CSteamID)lobbyIDS[i].m_SteamID).ToString() + " max players: " + SteamMatchmaking.GetLobbyMemberLimit((CSteamID)lobbyIDS[i].m_SteamID).ToString());

                if (didPlayerSearchForLobbies)
                {
                    //Debug.Log("OnGetLobbyInfo: Player searched for lobbies");
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

                        if (_gameNatMan.publicLobbies)
                        {
                            newLobbyListItem.transform.SetParent(ContentPanel.transform);
                            newLobbyListItem.transform.localScale = Vector3.one;
                        }
                        else
                        {
                            newLobbyListItem.transform.SetParent(FriendsLobbyContentPanel.transform);
                            newLobbyListItem.transform.localScale = Vector3.one;
                        }

                        listOfLobbyListItems.Add(newLobbyListItem);
                    }
                }
                else
                {
                    //Debug.Log("OnGetLobbyInfo: Player DID NOT search for lobbies");
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
                    if (_gameNatMan.publicLobbies)
                    {
                        newLobbyListItem.transform.SetParent(ContentPanel.transform);
                        newLobbyListItem.transform.localScale = Vector3.one;
                    }
                    else
                    {
                        newLobbyListItem.transform.SetParent(FriendsLobbyContentPanel.transform);
                        newLobbyListItem.transform.localScale = Vector3.one;
                    }

                    listOfLobbyListItems.Add(newLobbyListItem);
                }

                return;
            }
        }

        if (didPlayerSearchForLobbies)
            didPlayerSearchForLobbies = false;
    }

    public void getAndDisplayFriendsLobbies()
    {
        _gameNatMan.FriendsLobbies();
    }
    
    public void DestroyOldLobbyListItems()
    {
        //Debug.Log("DestroyOldLobbyListItems");
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

        GetListOfLobbies();
    }
}