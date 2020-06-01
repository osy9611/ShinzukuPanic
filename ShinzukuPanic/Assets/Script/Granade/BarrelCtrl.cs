using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour {

    private int HitCount = 0;   //총알이 맞은 횟수
    private Rigidbody rb;   //Rigidbody 컴포토넌트를 저장할 변수

    public Mesh[] meshes;   //찌그러진 드럼통의 메쉬를 저장할 배열
    private MeshFilter meshFilter;  //MeshFilter 컴포넌트를 저장할 변수
    public Texture[] textures;  //드럼통의 텍스처를 저장할 배열
    private MeshRenderer _render;   //MeshRenderer 컴포넌트를 저장할 변수

    public float expRadius = 10.0f;
    public float power;
    public float upforce;
    public float damage;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();
        _render = GetComponent<MeshRenderer>();
        //_render.material.mainTexture = textures[Random.Range(0, textures.Length)];  //불규칙적으로 텍스쳐를 사용
       
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet")) //총알 오브젝트 태그비교
        {
            HitCount++;
            
            if (HitCount == 3) //총알의 충돌횟수를 증가,3발 이상 맞았는지 확인
            {
                ExpBarrel();
            }
        }
    }


    void ExpBarrel()// 폭발효과를 처리할 함수
    {
        //GameObject effect = Instantiate(ExpEffect, transform.position, Quaternion.identity);  //추후 사용예정
        //Destroy(effect, 2.0f);

        Vector3 explosionPosition = transform.position; //폭발이 일어나는 지점을 선정
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, expRadius);

        foreach(Collider hit in colliders)
        {
            Rigidbody _rb = hit.GetComponent<Rigidbody>();

            if (_rb != null)
            {
                _rb.AddExplosionForce(power, explosionPosition, expRadius, upforce, ForceMode.Impulse);
            }

            if(hit.gameObject.tag=="Enemy")
            {
                Enemy enemy = hit.gameObject.GetComponent<Enemy>();

                if(enemy.ES!=EnemyState.Die)
                {
                    enemy.Hurt(damage);
                }
            }

            if (hit.gameObject.tag == "Player")
            {
                power = 0;
                
                Physics.IgnoreCollision(hit.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
            }
        }

        gameObject.SetActive(false);
       //int idx = Random.Range(0, meshes.Length);

       //meshFilter.sharedMesh = meshes[idx];    //찌그러진 메쉬를  적용
    }

  
}
