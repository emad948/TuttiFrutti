using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PerfectMatchMenu : NetworkBehaviour
{
    public GameObject Panel;
    private bool GameIsPaused = false;

   
   

  

    public void LeaveGame()
    {
        if (isServer)
        {
            NetworkServer.Shutdown();
            ((GameNetworkManager) NetworkManager.singleton).StopHost();
        }
        else
        {
            ((GameNetworkManager) NetworkManager.singleton).StopClient();
        }

        SceneManager.LoadScene(0);
    }
}