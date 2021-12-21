using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionController : NetworkBehaviour
{
    [SyncVar] private bool hostStopped = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isServer && hostStopped)
        {
            ((GameNetworkManager) NetworkManager.singleton).StopClient();
        }
    }
    // stops the host after all clients have (been) stopped
    private void disconnecting()
    {
        ((GameNetworkManager) NetworkManager.singleton).StopHost();
        SceneManager.LoadScene(0);
    }

    public void backToMenu()
    {
        if (isServer)
        {
            //NetworkServer.DisconnectAll();
            Invoke("disconnecting",0.5f);
        }
        else
        {
            ((GameNetworkManager) NetworkManager.singleton).StopClient();
            SceneManager.LoadScene(0);
        }
    }
}
