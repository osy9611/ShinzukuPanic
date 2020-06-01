using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUi : MonoBehaviour {

    GameObject HP_gage;
    public Player player;
    public Text[] BulletCount;
    public Image[] WeaponImage;
    public Text[] WeaponText;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player").GetComponent<Player>();

    }
	public void DecreaseHP()
    {
        HP_gage.GetComponent<Image>().fillAmount -= 0.1f;
    }
	// Update is called once per frame
	void Update () {
		
	}
}
