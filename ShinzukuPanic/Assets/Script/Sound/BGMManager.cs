using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour {

    static public BGMManager instance;
    public bool InstaceCheck;
    public AudioClip[] clips;     //배경음악들

    public AudioSource source;

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);
    private void Awake()
    {
        if (InstaceCheck == true)
        {
            if (instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
        }
    }
    // Use this for initialization
    void Start () {
        source =GetComponent<AudioSource>();
	}
	
    public void SetSound(int playMusicTrack)
    {
        source.clip = clips[playMusicTrack];
    }
	public void Play(int playMusicTrack)
    {
        source.clip = clips[playMusicTrack];
        source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }

    public void FadeOutMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutMusicCoroutine());
    }

    IEnumerator FadeOutMusicCoroutine()
    {
        for (float i = 1.0f; i <= 0; i--)
        {
            source.volume = i;
            yield return waitTime;
        }
    }

    
    public void FadeInMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInMusicCoroutine());
    }

    IEnumerator FadeInMusicCoroutine()
    {
        for (float i = 0.0f; i <= 1; i++)
        {
            source.volume = i;
            yield return waitTime;
        }
    }
}
