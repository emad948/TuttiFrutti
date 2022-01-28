using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Mirror;

public class PlayerSound : NetworkBehaviour
{
    [SerializeField]
    private AudioClip[] clips;


    private AudioSource audioSource;

    private void Awake(){
        audioSource = GetComponent<AudioSource>();
    }

    private void Jump(){
        
        audioSource.PlayOneShot(clips[0]);
    }

    private void RunningOne(){
        
        audioSource.PlayOneShot(clips[1]);
    }
     private void RunningTwo(){
        
        audioSource.PlayOneShot(clips[2]);
    }
}
