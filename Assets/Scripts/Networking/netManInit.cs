using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

// Instantiates the NetworkManager Object (including it´s children) once at the start of the game. 
// This way multiple creations and deletions of NetworkManagers can be avoided.
public class netManInit : MonoBehaviour
{ 
    public static netManInit _instance;
    public GameObject prefab;
    
    void Awake()
    {
        if (_instance != null)
        {
            ((GameNetworkManager) NetworkManager.singleton)._menu = GameObject.FindGameObjectWithTag("MainMenuDisplayTag").GetComponent<Menu>();
            Destroy(this);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            // --- 
            Instantiate(prefab);
        }
    }

}
