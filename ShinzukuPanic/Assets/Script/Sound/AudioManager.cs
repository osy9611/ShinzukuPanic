using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;     //사운드의 이름
    public AudioClip clip;  //사운드 파일
    private AudioSource source; //사운드 플레이어

    public float Volum;
    public bool loop;

    public void Setsource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.loop = loop;
    }

    public void SetVolume()
    {
        source.volume = Volum;
    }
    public void Play()
    {
        source.Play();
    }

    public void OnePlay()
    {
        if(!source.isPlaying)
        {
            source.Play();
        }
    }

    public void Stop()
    {
        source.Stop();
    }

    public void SetLoop()
    {
        source.loop = true;   
    }

    public void SetLoopCancel()
    {
        source.loop = false;
    }
}

public class AudioManager : MonoBehaviour {

    static public AudioManager instance;
    [SerializeField]
    public Sound[] sounds;
   // Use this for initialization
	void Start () {
		for(int i=0;i<sounds.Length;i++)
        {
            GameObject soundObject = new GameObject("사운드 파일 이름 :" + i + "=" + sounds[i].name);
            sounds[i].Setsource(soundObject.AddComponent<AudioSource>());
            soundObject.transform.SetParent(this.transform);

            //sounds[i].Volum = _Volume;
            sounds[i].SetVolume();
               
        }
	}

    public void Play(string _name)
    {
        for(int i=0;i<sounds.Length;i++)
        {
            if(_name==sounds[i].name)
            {
                sounds[i].Play();
                return;
            }
        }
    }

    public void OncePlay(int num)
    {
        sounds[num].Play();
    }

    public void OnePlay(int num)
    {
        sounds[num].OnePlay();
    }

    public void OnePlay(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (_name == sounds[i].name)
            {
                sounds[i].OnePlay();
                return;
            }
        }
    }
    public void Stop(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (_name == sounds[i].name)
            {
                sounds[i].Stop();
                return;
            }
        }
    }

    public void SetLoop(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (_name == sounds[i].name)
            {
                sounds[i].SetLoop();
                return;
            }
        }
    }

    public void SetLoopCancel(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (_name == sounds[i].name)
            {
                sounds[i].SetLoopCancel();
                return;
            }
        }
    }

    public void SetVolume(string _name,float _Volume)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (_name == sounds[i].name)
            {
                sounds[i].Volum=_Volume;
                sounds[i].SetVolume();
                return;
            }
        }
    }

    private void Awake()
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
