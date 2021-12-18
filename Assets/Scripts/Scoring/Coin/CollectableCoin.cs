using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableCoin : MonoBehaviour
{

    public GameObject Torus;
    
    public Material simpleMaterial;
    public GameObject Indicator;
    private GameObject sessionPlayer;
    private GameObject playerCollected;
    private float fadeState = 0;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            if(!playerCollected) return;
            print("update");
            Vector3 targetPosition = playerCollected.transform.position + Vector3.up - transform.rotation.eulerAngles * 0.5f;
            Torus.transform.position = Vector3.Lerp(Torus.transform.position, targetPosition, Time.deltaTime * 2);
            fadeState += Time.deltaTime;
            Torus.GetComponent<Animator>().SetFloat("Fade", fadeState);

    }

    private void touched(GameObject other){
        print("touched");
        print(other.layer);
        if (playerCollected || (1<<other.layer & LayerMask.GetMask("Pla_yer")) == 0) return;
        print("touched2");
        Indicator.SetActive(false);
        playerCollected = other;
        Destroy(gameObject, 2f);
    }
    
    
}
