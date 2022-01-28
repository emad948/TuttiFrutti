using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    public AudioSource playSound;
   void OnTriggerEnter(Collider collision){
       
       playSound.Play();
   }
}
