using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletRemove : MonoBehaviour {

    //public GameObject sparkEffect;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            //스파크 효과 함수 호출
            //ShowEffect(other);
            Destroy(other.gameObject);
        }
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.collider.tag=="Bullet")
    //    {
    //        //스파크 효과 함수 호출
    //        ShowEffect(collision);
    //        Destroy(collision.gameObject);
    //    }
    //}

    void ShowEffect(Collision coll)    
    {
        //충돌 지점의 정보를 추출
        ContactPoint contact = coll.contacts[0];
        //회전각도 추출
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contact.normal);

        //스파크 효과를 생성 추후에 사용할것
       // GameObject spark = Instantiate(sparkEffect, contact.point + (-contact.normal) * 0.05f, rot);

        //스파크 효과의 부모를 드럼통 또는 벽으로 설정 추후에 사용할 것
        //spark.transform.SetParent(this.transform);
    }
   
}
