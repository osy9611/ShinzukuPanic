using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaponManager : MonoBehaviour {

    public GameObject player;
    Player p;
    public WaponType WT;
    public bool get=false;
    public float Speed;
    public float Power;
    public float Penetration;   //관통력
    public int Magazine;        //총알탄수
    public bool Auto;
    public int SoundNum;
    public float Delay;
    //public float RotSpeed = 10;
    private void Start()
    {
        player = GameObject.Find("Player");
        p = player.GetComponent<Player>();
    }
    //private void Update()
    //{
    //    transform.Rotate(new Vector3(0, RotSpeed, 0));
    //}
    void GetWapon()
    {
        p.SetWapon(WT);
        p.SetBullet(Speed, Power, Penetration, Magazine,Auto,SoundNum,Delay);
       
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            if(p.MainWapon != WaponType.NULL && p.MainWapon2 != WaponType.NULL)
            {
                Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
                return;
            }
            else 
            {
                GetWapon();
                gameObject.SetActive(false);
            }
        }

        if(collision.collider.tag=="Enemy")
        {
            return;
        }
        if (collision.collider.tag == "DistaceEnemy")
        {
            return;
        }
       
    }
}
