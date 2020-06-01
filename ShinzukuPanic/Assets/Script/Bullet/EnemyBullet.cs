using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour {

    public float Speed = 10.0f;   //총알 속도     
    public float Power = 12f;   //데미지
    public float Life = 2f;     //총알 지속시간
    public float Penetration;
    public Enemy EC;
    // Use this for initialization
    void Start()
    {
        //GetComponent<Rigidbody>().AddRelativeForce(transform.forward * Speed*Time.deltaTime);

    }

    public void PenetrationEnemy()
    {
        if (Penetration == 0)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Penetration -= 1;
            if (Penetration <= 0)
            {
                Destroy(this.gameObject);
            }
        }

    }
    // Update is called once per frame
    void Update()
    {
        Life -= Time.deltaTime;
        if (Life <= 0f)
        {
            Destroy(this.gameObject);   //총알 지속시간이 끝나면 오브젝트를 파괴
        }
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);
        //transform.position = new Vector3(transform.position.x, 1.004f, transform.position.z);
    }


}
