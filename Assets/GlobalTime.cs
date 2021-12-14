using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class GlobalTime : NetworkBehaviour
{
    private char sepFloat = '.';
    private string xAfterDot(float num, int x)
    {
        string b = num.ToString();
        int dot = b.IndexOf(sepFloat);
        string[] seperated = b.Split(sepFloat);
        x = x < seperated[1].Length ? x : seperated[1].Length - 1;
        return seperated[0] + sepFloat + seperated[1].Substring(0, x);
    }

    public Text timeText;
    [SyncVar] public float time = -3f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<NetworkIdentity>().isServer)
        {
            time += Time.deltaTime;
        }
        timeText.text = xAfterDot(time, 2);

    }
}
