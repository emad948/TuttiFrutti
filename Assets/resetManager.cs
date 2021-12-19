using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resetManager : MonoBehaviour
{
    public GameObject prefab;
    //public GameObject gml;
    
    // Start is called before the first frame update
    void Awake()
    {
        Instantiate(prefab);
    }

}
