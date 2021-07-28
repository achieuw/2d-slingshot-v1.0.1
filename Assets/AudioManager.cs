using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : GenericSingleton<AudioManager>
{
    [SerializeField] AudioSource audio;
    public AudioClip slingSound;

    public void StopAndPlay(AudioClip clip)
    { 
        audio.PlayOneShot(clip);
    }
}
