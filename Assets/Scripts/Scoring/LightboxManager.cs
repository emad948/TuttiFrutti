using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightboxManager : MonoBehaviour
{
    public GameObject box1;
    public GameObject box2;
    public GameObject box3;

    private GameObject[] boxes;
    public void setActiveBox(int j){
        for (int i = 0; i < boxes.Length; i++){
            boxes[i].SetActive(i == j-1);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        boxes = new GameObject[] {box1, box2, box3};
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}