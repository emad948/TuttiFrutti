using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class finish : MonoBehaviourPunCallbacks
{
    public void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        SceneManager.LoadScene(0);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

}
