using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GranadeState
{
    Move,
    End
}

public class Granade : MonoBehaviour {

    public GranadeState GS;
    private int HitCount = 0;   //총알이 맞은 횟수
    private Rigidbody rb;   //Rigidbody 컴포토넌트를 저장할 변수
    public Camera camera;
    private MeshFilter meshFilter;  //MeshFilter 컴포넌트를 저장할 변수
    public Texture[] textures;  //드럼통의 텍스처를 저장할 배열
    private MeshRenderer _render;   //MeshRenderer 컴포넌트를 저장할 변수
    Follow CameraShake;
    public float expRadius = 10.0f;
    public float power;
    public float upforce;
    public float damage;

    /*포물선 이동(연습1)*/
    private float Timer;
    GameObject Player;
    public Transform ShotPoint;
    Vector3 GranadePos;
    public float Speed;

    private float tx;
    private float ty;
    private float tz;
    private float v;
    public float g = 9.8f;
    private float elapsed_time;
    public float max_height;
    private float t;
    private Vector3 start_pos;
    private Vector3 end_pos;
    private float dat;
    float tx1;
    float ty1;
    float tz1;
    Vector3 tpos;
    bool Stop;
    bool ExpOn;
    //파티클
    public GameObject Explosion;
    GameObject Effect;
    //폭발 사운드
    public string GranadeSound;
    private AudioManager theAudio;

    private void Awake()
    {
        Player = GameObject.Find("Player");
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();
        _render = GetComponent<MeshRenderer>();
        //_render.material.mainTexture = textures[Random.Range(0, textures.Length)];  //불규칙적으로 텍스쳐를 사용
        //Player = GameObject.Find("Player");
        theAudio = FindObjectOfType<AudioManager>();
        theAudio.SetVolume(GranadeSound, 0.5f);
        Timer = Time.deltaTime;
        CameraShake = FindObjectOfType<Follow>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy")) //총알 오브젝트 태그비교
        {
            ExpBarrel();
            
        }

        if (collision.collider.CompareTag("DistanceEnemy")) //총알 오브젝트 태그비교
        {
            ExpBarrel();
        }

        if(collision.collider.CompareTag("Floor"))
        {
            ExpBarrel();
        }

        if(collision.collider.CompareTag("Wall"))
        {
            Stop = true;
        }
    }
    IEnumerator ExpTimer()
    {
        yield return new WaitForSeconds(3.0f);
     
        theAudio.Play(GranadeSound);
        Destroy(gameObject);
        ExpBarrel();
    }

    void ExpBarrel()// 폭발효과를 처리할 함수
    {
        if (ExpOn == false)
        {
            ExpOn = true;
            theAudio.Play(GranadeSound);
            CameraShake.CameraShake(0.5f, 0.5f);
            Effect = Instantiate(Explosion, transform.position, transform.rotation);
            Effect.transform.parent = transform;
            Vector3 explosionPosition = transform.position; //폭발이 일어나는 지점을 선정
            Collider[] colliders = Physics.OverlapSphere(explosionPosition, expRadius);
            
            foreach (Collider hit in colliders)
            {
                Rigidbody _rb = hit.GetComponent<Rigidbody>();

                if (_rb != null)
                {
                    _rb.AddExplosionForce(power, explosionPosition, expRadius, upforce, ForceMode.Impulse);
                }

                if (hit.gameObject.tag == "Enemy")
                {
                    Enemy enemy = hit.gameObject.GetComponent<Enemy>();

                    if (enemy.ES != EnemyState.Die)
                    {
                        enemy.ExplosionBody = true;
                        enemy.Hurt(damage);
                       
                    }
                    StartCoroutine(ExpWait());
                }

                if (hit.gameObject.tag == "DistanceEnemy")
                {
                    DistanceEnemy enemy = hit.gameObject.GetComponent<DistanceEnemy>();

                    if (enemy.ES != EnemyState.Die)
                    {
                        enemy.ExplosionBody = true;
                        enemy.Hurt(damage);
                    }
                    StartCoroutine(ExpWait());
                }
                if (hit.gameObject.tag == "Floor")
                {
                    StartCoroutine(ExpWait());
                }
                if (hit.gameObject.tag == "Player")
                {
                    power = 0;
                }
            }
        }
        //Explosion.SetActive(false);
    }
    IEnumerator ExpWait()
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(gameObject);
        Destroy(Effect.gameObject);
    }
    public void MoveTo()
    {
        if (GS == GranadeState.Move)
        {
            Transform pos = Player.GetComponent<Player>().GranadeShotPoint.transform;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10000f))
            {
               
                //hit.point = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                GranadePos = hit.point;
               
            }
            
            
            GS = GranadeState.End;
        }
        StartCoroutine("Shot");
    }
    

    IEnumerator Shot()
    {
        start_pos = transform.position;
        end_pos = GranadePos;

        var dh = GranadePos.y - transform.position.y;
        var mh = (GranadePos.y+max_height) - transform.position.y;
       
        if(mh<0)
        {
            Mathf.Abs(mh);
        }
        ty = Mathf.Sqrt(2 * g * mh);
        
        float a = g;
        float b = -2 * ty;
        float c = 2 * dh;

        dat = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);

        tx = -(start_pos.x - end_pos.x) / dat;
        tz = -(start_pos.z - end_pos.z) / dat;
        
        elapsed_time = 0;

        while(true)
        {
            this.elapsed_time += Time.deltaTime;

            tx1 = start_pos.x + this.tx * elapsed_time;
            ty1 = start_pos.y + (this.ty * elapsed_time) - (0.5f * g * elapsed_time * elapsed_time);
            tz1 = start_pos.z + this.tz * elapsed_time;
            tpos = new Vector3(tx1, ty1, tz1);
            
            transform.position = tpos;
            if (Stop == false)
            {
                if (this.elapsed_time >= this.dat)
                {
                    transform.position = new Vector3(transform.position.x, Player.GetComponent<Player>().transform.position.y, transform.position.z);
                    break;
                }
            }
            yield return null;
        }
       
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GS != GranadeState.End)
        {
            if (Player.GetComponent<Player>().GranadeDrop == true)
            {
                MoveTo();

                Player.GetComponent<Player>().GranadeDrop = false;
            }
        }
    }
}
