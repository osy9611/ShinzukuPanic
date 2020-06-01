using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public struct Ranking
{
    public string time;
    public string name;
    public float Dummytime;
    public int rank;
}
public enum GameState
{
    Play,
    Pause,
    End,
}
public class GameManager : MonoBehaviour {

    public GameState GS;
    public EnemyManager EM;
    public GameObject fade;
    public GameObject GameOverUI;
    public GameObject GameClearUI;
    public Text Timer;
    public float time;
   
  
    public int min = 0;
    public Player player;
    public Text BulletCount;
    public Text BulletCount2;
    public Sprite[] BulletAuto;
    public Image WeaponImage;
    public Image WeaponNameImage;
    public Image BulletAutoImage;
    float WeaponMove;
    float OriginWeaponPos;
    public Image Granade;
    public Image Heal;
    public Text GranadeCount;
    public Text GranadeCount2;
    public Sprite[] Weapons;
    public Sprite[] Granades;
    public Sprite[] WeaponName;
    float i;
    public bool MainMenu;
    private EnemyManager enemyManager;

    //아이템 스폰 관련 함수들 
    public float ItemSpawnTime;
    private float ItemSpawnTimer;
    private ItemBox itembox;
    public GameObject ItmeManger;
    public Transform Box;
    public Text ItemBoxInformation;

    //메인화면
    public GameObject Menu;
    public GameObject StageSelect;
    public GameObject IntroducePage;
    bool Mode;
    AudioManager theAudio;
    //피가 적을때 발동되는 피화면
    public Image BloodFade;
    float color;

    //로딩페이지
    public Image LoadGage;
    public bool LoadingDone;
    public Text PressAnyKey;

    public string StageName;

    //랭킹 시스템 관련
    Ranking DummyRank;
    public InputField Name;
    public Text EndTime;
    public GameObject WriteName;
    [SerializeField]
    public Ranking[] Hardranking = new Ranking[11];

    [SerializeField]
    public Ranking[] Easyranking = new Ranking[11];

    public GameObject UI;   //랭킹 버튼을 누를때 오브젝트들을 끔
    public GameObject Ranking;
    public GameObject[] RankData;
    public GameObject MakeRankData;
    float positiony;
    public bool ShowRank;

    public void GameOver()
    {
        GS = GameState.End;
        if (WriteName.activeSelf == false)
        {
            GameOverUI.SetActive(true);
        }
        Timer =Timer.GetComponent<Text>();
    }

    public void MainTitle()
    {
        //fade.SetActive(true);
        SceneManager.LoadScene("MainMenu");
        //fade.SetActive(false);
    }

    public void Replay()
    {
        GS = GameState.Play;
        theAudio.OncePlay(12);
        SceneManager.LoadScene(StageName);
        Time.timeScale = 1;
    }

    public void GameStart()
    {
        theAudio.OncePlay(12);
        Menu.SetActive(false);
        StageSelect.SetActive(true);
    }

    public void EasyMode()
    {
        theAudio.OncePlay(12);
        if(ShowRank==false)
        {
            StageSelect.SetActive(false);
            IntroducePage.SetActive(true);
            LoadingDone = true;
        }
        else
        {
            UI.SetActive(false);
            Ranking.SetActive(true);
            StageSelect.SetActive(false);
            positiony = transform.position.y + 250;
            GameObject RankingUI = GameObject.Find("Content");
            for (int i = 0; i < 10; i++)
            {
                GameObject dummy = Instantiate(MakeRankData);
                dummy.transform.position = Vector3.zero;
                dummy.transform.parent = RankingUI.transform;
                dummy.transform.position = new Vector3(transform.position.x, positiony, transform.position.z);
                RankData[i] = dummy;
                positiony -= 156;

                GameObject NameText = RankData[i].transform.Find("Name").gameObject;
                GameObject TimeText = RankData[i].transform.Find("Time").gameObject;

                NameText.GetComponent<Text>().text = Easyranking[i].name;
                TimeText.GetComponent<Text>().text = Easyranking[i].time;
            }
        }
    }

    public void HardMode()
    {
        theAudio.OncePlay(12);
        if (ShowRank == false)
        {
            Time.timeScale = 1;
            //SceneManager.LoadScene("SampleScene");
            StageSelect.SetActive(false);
            IntroducePage.SetActive(true);
            LoadingDone = true;
            Mode = true;
            //StartCoroutine(WaitLoding()); 
        }
        else
        {
            UI.SetActive(false);
            Ranking.SetActive(true);
            StageSelect.SetActive(false);
            positiony = transform.position.y + 250;
            GameObject RankingUI = GameObject.Find("Content");
            for (int i = 0; i < 10; i++)
            {
                GameObject dummy = Instantiate(MakeRankData);
                dummy.transform.position = Vector3.zero;
                dummy.transform.parent = RankingUI.transform;
                dummy.transform.position = new Vector3(transform.position.x, positiony, transform.position.z);
                RankData[i] = dummy;
                positiony -= 156;

                GameObject NameText = RankData[i].transform.Find("Name").gameObject;
                NameText.GetComponent<Text>().text = Hardranking[i].name;
                GameObject TimeText = RankData[i].transform.Find("Time").gameObject;
                TimeText.GetComponent<Text>().text = Hardranking[i].time;
            }
        }


    }
    
    public void ChangeEasyMode()
    {
        SceneManager.LoadScene("EasyCity");
    }

    public void ChangeMode()
    {
        
        if (LoadGage.fillAmount == 1)
        {
            if (Mode == false)
            {
                SceneManager.LoadScene("EasyCity");
            }
            else if(Mode==true)
            {
                SceneManager.LoadScene("Station");
                //int RandomStage = Random.Range(0, 2);
                //if (RandomStage == 0)
                //{
                   
                //}
                //else if (RandomStage == 1)
                //{
                //    SceneManager.LoadScene("City");
                //}
            }
        }
    }
   
    public void GameEnd()
    {
        Debug.Log("실행");
        theAudio.OncePlay(12);
        Application.Quit();
        EM.GetComponent<EnemyManager>().GameOver = false;
    }

    public void GameClear()
    {
        if (min > -1)
        {
            time += Time.deltaTime;
        }

        if (time > 60)
        {
            min++;
            time =0 ;
        }

        if (min != -1)
        {
            Timer.text = string.Format("{0:D2} : {1:D2}", min, (int)time);
        }
        else if (min == -1)
        {
            Timer.text = string.Format("{0:D2} : {1:D2}", 0, (int)0);
        }
    }

    public void CreateBox()
    {
        if (ItemSpawnTimer > 0)
        {
            if (itembox.On == false)
            {
                ItemSpawnTimer -= Time.deltaTime;
            }
        }
        else if (ItemSpawnTimer < 0)
        {
            itembox.ShopSpawnOn();
            StartCoroutine(Imformation());
            ItemSpawnTimer = ItemSpawnTime;
        }
    }
    
    IEnumerator Imformation()
    {
        ItemBoxInformation.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        ItemBoxInformation.gameObject.SetActive(false);
    }

    public void WeaponUISet()
    {
        int id = player.BS.Id;
        int id2 = player.BS2.Id;

        switch (player.KeyNum)
        {
            case 1:
                WeaponImage.transform.localScale = new Vector3(4.67735f, 1.238889f, 0);
                WeaponImage.transform.position = new Vector3(OriginWeaponPos, WeaponImage.transform.position.y, WeaponImage.transform.position.z);
                WeaponImage.sprite = Weapons[id];
                WeaponNameImage.sprite = WeaponName[id];
                BulletCount.text = player.BS.BulletCount.ToString("F0");
                BulletCount2.text = player.BS.BulletCount.ToString("F0");
                if (player.BS.Auto == true)
                {
                    BulletAutoImage.sprite = BulletAuto[0];
                }
                else
                {
                    BulletAutoImage.sprite = BulletAuto[1];
                }
                break;
            case 2:
                WeaponImage.transform.localScale = new Vector3(4.67735f, 1.238889f, 0);
                WeaponImage.transform.position = new Vector3(OriginWeaponPos, WeaponImage.transform.position.y, WeaponImage.transform.position.z);
                WeaponImage.sprite = Weapons[id2];
                WeaponNameImage.sprite = WeaponName[id2];
                BulletCount.text = player.BS2.BulletCount.ToString("F0");
                BulletCount2.text = player.BS2.BulletCount.ToString("F0");
                if (player.BS2.Auto == true)
                {
                    BulletAutoImage.sprite = BulletAuto[0];
                }
                else
                {
                    BulletAutoImage.sprite = BulletAuto[1];
                }
                break;
            case 3:
                WeaponImage.transform.localScale = new Vector3(2.63082f, 1.23889f, 0);            
                WeaponImage.transform.position = new Vector3(WeaponMove, WeaponImage.transform.position.y,WeaponImage.transform.position.z);
                WeaponImage.sprite = Weapons[0];
                WeaponNameImage.sprite = WeaponName[0];
                BulletAutoImage.sprite = BulletAuto[1];
                BulletCount.text = "-";
                BulletCount2.text = "-";
                break;
        }

        switch (player.WheelCount)
        {
            case 0:
                Granade.sprite = Granades[0];
                GranadeCount.text = "X" + player.GranadeCount.ToString("F0");
                GranadeCount2.text = "X" + player.GranadeCount.ToString("F0");
                break;
            case 1:
                Granade.sprite = Granades[1];
                GranadeCount.text = "X" + player.CraymoreCount.ToString("F0");
                GranadeCount2.text = "X" + player.CraymoreCount.ToString("F0");
                break;
            case 2:
                Granade.sprite = Granades[2];
                GranadeCount.text = "X" + player.FireBombCount.ToString("F0");
                GranadeCount2.text = "X" + player.FireBombCount.ToString("F0");
                break;
        }
       
        if (player.GranadeDrop == false)
        {
            Granade.fillAmount = 0;
            Granade.fillAmount += player.timer/2.9f;
        }

        if (player.HealOff == false)
        {
            Heal.fillAmount = 0;
            Heal.fillAmount += player.HealCoolTime / 2.9f;
        }
        else if (player.HealKitCount == 0)
        {
            Heal.color = new Color(Heal.color.r, Heal.color.g, Heal.color.b, 0.5f);
        }

        
    }
    
    public void BloodScreen()
    {
        StartCoroutine(Fade());
    }

    public void SetRank()
    {
        GameOverUI.SetActive(false);
        WriteName.SetActive(true);
    }
    
    public void Rank()
    {
        if(SceneManager.GetActiveScene().name == "EasyCity")
        {
            Easyranking[10].name = Name.text;
            Easyranking[10].time = Timer.text;
            Easyranking[10].Dummytime = min * 60 + time;

            for(int i=0;i<Easyranking.Length-1;i++)
            {
                for(int j=0;j<Easyranking.Length-1-i;j++)
                {
                    if (Easyranking[j].Dummytime < Easyranking[j+1].Dummytime)
                    {
                        DummyRank = Easyranking[j];
                        Easyranking[j] = Easyranking[j + 1];
                        Easyranking[j + 1] = DummyRank;
                    }
                }
            }
            for (int i=0;i<Easyranking.Length;i++)
            {
                PlayerPrefs.SetString(i.ToString(), Easyranking[i].name);
                PlayerPrefs.SetString("EasyTime"+i.ToString(), Easyranking[i].time);
                PlayerPrefs.SetFloat("EasyDummyTime" + i.ToString(), Easyranking[i].Dummytime);
            }
        }
        else if(SceneManager.GetActiveScene().name == "Station")
        {
            Hardranking[10].name = Name.text;
            Hardranking[10].time = Timer.text;
            Hardranking[10].Dummytime = min * 60 + time;

            for (int i = 0; i < Hardranking.Length - 1; i++)
            {
                for (int j = 0; j < Hardranking.Length - 1 - i; j++)
                {
                    if (Hardranking[j].Dummytime < Hardranking[j + 1].Dummytime)
                    {
                        DummyRank = Hardranking[j];
                        Hardranking[j] = Hardranking[j + 1];
                        Hardranking[j + 1] = DummyRank;
                    }
                }
            }
            for (int i = 0; i < Hardranking.Length; i++)
            {
                PlayerPrefs.SetString(i.ToString(), Hardranking[i].name);
                PlayerPrefs.SetString("HardTime" + i.ToString(), Hardranking[i].time);
                PlayerPrefs.SetFloat("HardDummyTime" + i.ToString(), Hardranking[i].Dummytime);
            }
        }
    }

    public void RankOn()
    {
        //Menu.SetActive(false);
        StageSelect.SetActive(true);
        ShowRank = true;
    }
   
    public void Back()
    {
        ShowRank = false;
        for(int i=0;i<RankData.Length;i++)
        {
            Destroy(RankData[i]);
        }
        UI.SetActive(true);
        Ranking.SetActive(false);
    }
    IEnumerator Fade()
    {
        if (BloodFade.color.a <= 0)
        {
            color += Time.deltaTime / 2.0f;
            BloodFade.color = new Color(BloodFade.color.r, BloodFade.color.g, BloodFade.color.b, color);
        }
        else if(BloodFade.color.a==1)
        {
            color -= Time.deltaTime / 2.0f;
            BloodFade.color = new Color(BloodFade.color.r, BloodFade.color.g, BloodFade.color.b, color);
        }

        yield return new WaitForSeconds(0.15f);
    }

    private void Update()
    {
        if (LoadingDone == true)
        {
            time += Time.deltaTime;
            LoadGage.fillAmount += time / 100.0f;
            if (time > 5.0)
            {
                time = 0;
                LoadingDone = false;
                LoadGage.gameObject.SetActive(false);
                PressAnyKey.gameObject.SetActive(true);

            }
        }
    }

    private void Start()
    {
        
        for (int i=0;i<Easyranking.Length;i++)
        {
            Easyranking[i].name = PlayerPrefs.GetString(i.ToString());
            Easyranking[i].time = PlayerPrefs.GetString("EasyTime"+ i.ToString());
            Easyranking[i].Dummytime = PlayerPrefs.GetFloat("EasyDummyTime" + i.ToString());

            Hardranking[i].name = PlayerPrefs.GetString(i.ToString());
            Hardranking[i].time = PlayerPrefs.GetString("HardTime" + i.ToString());
            Hardranking[i].Dummytime = PlayerPrefs.GetFloat("HardDummyTime" + i.ToString());
            
        }
        if (MainMenu == false)
        {
            player = GameObject.Find("Player").GetComponent<Player>();
            Granade = Granade.GetComponent<Image>();
            WeaponImage.sprite = Weapons[0];
            BulletAutoImage.sprite = BulletAuto[1];
            WeaponNameImage.sprite = WeaponName[0];
            BulletCount.text = "-";
            BulletCount2.text = "-";
            Box = ItmeManger.transform.Find("shop");
            itembox = Box.GetComponent<ItemBox>();
            ItemSpawnTimer = ItemSpawnTime;
            enemyManager = FindObjectOfType<EnemyManager>();
            WeaponMove = WeaponImage.transform.position.x - 107;
            OriginWeaponPos = WeaponImage.transform.position.x;
            WeaponImage.transform.localScale = new Vector3(2.63082f, 1.23889f, 0);
            WeaponImage.transform.position = new Vector3(WeaponMove, WeaponImage.transform.position.y, WeaponImage.transform.position.z);
        }
        color = 0;
        theAudio = FindObjectOfType<AudioManager>();
        StageName = SceneManager.GetActiveScene().name;

    }

   
}
