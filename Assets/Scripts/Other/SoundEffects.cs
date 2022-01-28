using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Mirror;

public class SoundEffects: NetworkBehaviour
{
    [SerializeField]
    private AudioClip clip;


    private AudioSource audioSource;

    private void Awake(){
        audioSource = GetComponent<AudioSource>();
    }

    private void PlayEffect(){
        
        audioSource.PlayOneShot(clip);
    }

}
