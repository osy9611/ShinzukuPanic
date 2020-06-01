using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGunManager : MonoBehaviour {
    public GameObject player;
    Player p;
    public WaponType WT;
    public bool get = false;
    public float Speed;
    public float Power;
    public float Penetration;   //관통력
    public int Magazine;        //총알탄수
    public float Angle;
    public float AngleDivision; //총알을 몇도 단위로 나눌것인가
    public int BulletCount;     //총알을 만들 단위
    public bool Auto;
    public int SoundNum;
	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player");
        p = player.GetComponent<Player>();
        AngleDivision = (int)(Angle / BulletCount);
	}


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            if (p.MainWapon == WaponType.NULL || p.MainWapon2 == WaponType.NULL)
            {
                GetWapon();
                gameObject.SetActive(false);
            }
           
        }
        
    }

  
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.tag == "Player")
    //    {

    //        if (p.MainWapon == WaponType.NULL || p.MainWapon2 == WaponType.NULL)
    //        {
    //            Debug.Log("샷건 충돌");
    //            GetWapon();
    //            gameObject.SetActive(false);
    //        }
    //        else
    //        {
    //            Debug.Log("샷건 충돌 안함");
    //            //Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
    //            return;
    //        }
    //    }

    //    if (collision.collider.tag == "Enemy")
    //    {
    //        return;
    //    }
    //    if (collision.collider.tag == "DistaceEnemy")
    //    {
    //        return;
    //    }

    //}

    void GetWapon()
    {
        p.SetWapon(WT);
        p.SetShotBullet(Speed, Power, Penetration, Magazine, Auto, Angle, AngleDivision, BulletCount, SoundNum);
    }
}
