using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMSound : MonoBehaviour {

    BGMManager BGM;

    public int PlayMusicTrack;

    private void Awake()
    {
        BGM = FindObjectOfType<BGMManager>();
    }
    // Use this for initialization
    void Start () {
       
        BGM.Play(PlayMusicTrack);
        BGM.FadeInMusic();
    }

   
}
