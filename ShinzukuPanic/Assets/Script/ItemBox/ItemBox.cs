using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ItemBox : MonoBehaviour {

    public float Timer;
    public GameObject ShopUI;
    private Player player;
    public int HealCount;
    public float PentrationPoint;
    public float GardPoint;

    public Text[] Status;

    public Transform[] Spawn;
    public bool On;
    bool ShopCheck;

    public bool FirstSet;
    public void HpKit()
    {
        player.HealKitCount += HealCount;
        player.AttackOff = false;
        ShopUI.SetActive(false);
        Time.timeScale = 1;
        StartCoroutine(Off());
    }

    public void Penetration()
    {
        player.PentrationPoint += PentrationPoint;
        player.AttackOff = false;
        ShopUI.SetActive(false);
        player.Bullets.GetComponent<Bullet>().Penetration += PentrationPoint;
        player.MainBullet.GetComponent<Bullet>().Penetration += PentrationPoint;
        player.MainBullet2.GetComponent<Bullet>().Penetration += PentrationPoint;
        Time.timeScale = 1;

        StartCoroutine(Off());
    }

    public void Gard()
    {
        player.GardPoint = GardPoint;
        player.AttackOff = false;
        ShopUI.SetActive(false);
        Time.timeScale = 1;
        StartCoroutine(Off());
    }

    public void Granade()
    {
        player.GranadeCount += 2;
        player.AttackOff = false;
        ShopUI.SetActive(false);
        Time.timeScale = 1;
        StartCoroutine(Off());
    }

    public void Molotov()
    {
        player.FireBombCount += 2;
        player.AttackOff = false;
        ShopUI.SetActive(false);
        Time.timeScale = 1;
        StartCoroutine(Off());
    }
    IEnumerator Off()
    {
        yield return new WaitForSeconds(2.0f);
        int idx = Random.Range(1, Spawn.Length);
        transform.position = new Vector3(Spawn[idx].position.x, Spawn[idx].position.y, Spawn[idx].position.z);
        gameObject.SetActive(false);
        On = false;
        ShopCheck = false;
    }

    public void ShopSpawnOn()
    {
        gameObject.SetActive(true);
        On = true;
    }
    public void ShopOn()
    {
        if (ShopCheck == false)
        {
            ShopUI.SetActive(true);
            ShopCheck = true;
        }
        //On = true;
        //gameObject.SetActive(true);
    }

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        Spawn = GameObject.Find("ShopSpawnPoint").GetComponentsInChildren<Transform>();
    }


    // Use this for initialization
    void Start () {
      
        Status[0].text = "Hp: " + player.Hp.ToString("F2");
        Status[1].text = "Gard: " + player.GardPoint.ToString("F2");
        Status[2].text = "Penetration: " + player.PentrationPoint.ToString("F2");
       
        if (FirstSet == false)
        {
            transform.position = new Vector3(player.transform.position.x + 2.0f, transform.position.y, player.transform.position.z);
            FirstSet = true;
        }
    }
	
	
}
