using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DistanceEnemy : MonoBehaviour {

    public WaponType WP;
    public EnemyState ES;        //적의 상태
    public EnemyManager EM;
    DistanceRagdollObject DieDoll;
   // public Animator anim;        //애니메이션 넣을때 사용할것
    public AnimationClip[] Ani;   //애니메이션을 동작시킬 함수
    Animation animation;
    public Transform ShotPoint;
    public GameObject Bullet;
    float Speed;            
    public float MoveSpeed;     //적이 움직이는 속도
    public float AttackSpeed;   //공격속도
    public bool item=false;
    public float Range;     //플레이어와의 거리
    public float Damage;        //데미지
    public bool on;
    float timer;
    public float Attacktimer;
    public GameObject Player;    //플레이어 위치
    Player p;
    private Color col;
    /*현재 이팩트가 없으므로 나중에 작업*/
    public Transform FX_Point;  //이팩트를 발생시킬 위치
    public GameObject Hit_FX;   //공격 이팩트

    //사운드 관련 함수들
    public string SlashZombieSound; //좀비를 배는 사운드
    private AudioManager theAudio;
    bool AttackSoundOn;
    //좀비 피통관련
    public float MaxHp = 100;
    public float Hp = 100;       //적 피통

    public Transform[] Spawn;    //몬스터 재활용을 위하여 스폰지역을 저장한다
    public GameObject Item;
    public GameObject GranadeItem;
    public int WaponNumber;
    public NavMeshAgent nav;
    GameObject NavPlayer;
    public bool NotPooling;

    /*아이템 확률 관련함수*/
    public float WaponPercentage, WaponPercentage2;
    public GameObject[] WaponSet;
    public GameObject[] WaponSet2;
    public float GranadePercentage, GranadePercentage2;
    public GameObject[] GrandeSet;
    public GameObject[] GrandeSet2;
    public bool ExplosionBody;
    void DistanceCheck()        //플레이어와 나와의 거리를 측정
    {
        if (ES != EnemyState.Idle&&ES!=EnemyState.Die)
        {
            if (Vector3.Distance(Player.transform.position, transform.position) >= 0 && ES != EnemyState.Attack)
            {
                //ES = EnemyState.Move;
                StartCoroutine(MoveWait());
                //anim.SetBool("Walk", true);
            }
        }
        if (Vector3.Distance(Player.transform.position, transform.position) <= 20)
        {
            StartCoroutine(SoundOn());
        }
    }
    IEnumerator SoundOn()
    {
        if (AttackSoundOn == false)
        {
            int idx = Random.Range(1, 3);
            theAudio.Play("Female_Zombie" + idx);
            yield return new WaitForSeconds(0.5f);
            AttackSoundOn = true;
        }
    }
    IEnumerator MoveWait()
    {
        if (ES == EnemyState.Move&&ES!=EnemyState.Attack)
        {
            nav.speed = MoveSpeed;
            yield return new WaitForSeconds(0.5f);
            if (ES == EnemyState.Move)
            {
                nav.speed = MoveSpeed;
            }
            else
            {
                nav.speed = 0;
            }
        }
    }
    void Move()     //적이 움직임
    {
        nav.SetDestination((Player.transform.position));
    }


    void AttackRageCheck()   //적이 공격하기 전에 플레이어와의 거리가 가까운지를 채크하여 상태를 변경
    {
        if (Vector3.Distance(Player.transform.position, this.transform.position) <= Range)
        {
            nav.speed = 0;
            if (on == false)
            {
                ES = EnemyState.Attack;
                Attack_On();
                on = true;
            }
        }
        else if (Vector3.Distance(Player.transform.position, this.transform.position) > Range)
        {
            ES = EnemyState.Move;
            
            //StartCoroutine(MoveWait());
        }
    }

    public void Attack_On()
    {
        if (ES != EnemyState.Die)
        {

            transform.rotation = Quaternion.LookRotation((new Vector3(Player.transform.position.x, this.transform.position.y, Player.transform.position.z) - transform.position));
            StartCoroutine(Shot());
            //Player.GetComponent<Player>().Hurt(Damage);
        }
    }

    private IEnumerator Shot()
    {
      
        if (ES != EnemyState.Die)
        {
            yield return new WaitForSeconds(0.5f);
            GameObject bullet = Instantiate(Bullet, ShotPoint.position, transform.rotation);
            //anim.SetBool("Attack", true);
            //ES = EnemyState.Idle;
            yield return new WaitForSeconds(0.3f);
            ES = EnemyState.Idle;
            yield return new WaitForSeconds(Attacktimer);
            on = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if(ES==EnemyState.Die)
                Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            //Destroy(collision.gameObject);
            Bullet bullet = other.GetComponent<Bullet>();
            bullet.PenetrationEnemy();
            //죽은게 아니면 함수를 실행한다
            if (ES != EnemyState.Die)
            {
                Hurt(bullet.Power);
                if(gameObject.activeSelf==true)
                {
                    StartCoroutine(ShowEffect(other));
                }
            }
        }
    }

    IEnumerator ShowEffect(Collider other)
    {
        Hit_FX.transform.LookAt(other.transform.forward);
        //Hit_FX.transform.position = other.transform.position;
        Hit_FX.SetActive(true);

        yield return new WaitForSeconds(0.15f);

    }

    public void FireHurt(float timer, float damage)
    {
        if (Hp > 0)
        {
            StartCoroutine(FireDot(timer, damage));
        }

    }
    IEnumerator FireDot(float timer, float Damage)
    {
        for (int i = 0; i < timer; i++)
        {
            if (Hp > 0)
            {
                Hp -= Damage;
                yield return new WaitForSeconds(1.0f);
            }
            else
            {
                break;
            }
        }

        if (Hp > 0)
        {
            ExplosionBody = false;
        }
        else if (Hp <= 0)
        {
            ES = EnemyState.Die;
            nav.speed = 0f;
            theAudio.Play("Female_Zombie_Die");
            Hit_FX.SetActive(false);
            DieDoll.Blood.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            if (ExplosionBody == true)
            {
                DieDoll.ChageExpBody();
                DieDoll.ExpBody.transform.position = transform.position;
            }
           
        }
    }

    public void Hurt(float damage)  //피통이 0보다 크면 데미지를 받고 피통이 0이 되면 사망
    {
        if (Hp > 0)
        {
            ES = EnemyState.Hurt;
            //적이 맞았을 때 이팩트를 발동한다
            //GameObject FX = Instantiate(Hit_FX, FX_Point.position, Quaternion.LookRotation(FX_Point.forward));
            if (p.PS == PlayerState.MeleeAttack)
            {
                theAudio.Play(SlashZombieSound);
            }
            Hp -= damage;

            
        }
        if (Hp <= 0)
        {
            ES = EnemyState.Die;
            nav.speed = 0f;

            Hit_FX.SetActive(false);
            DieDoll.Blood.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            if (ExplosionBody == true)
            {
                DieDoll.ChageExpBody();
                DieDoll.ExpBody.transform.position = transform.position;
            }
            else
            {
                DieDoll.ChangeRagdoll();
                DieDoll.ragdollObj.transform.LookAt(transform.forward);
                DieDoll.ragdollObj.transform.position = transform.position;
            }

        }
    }

    public void Set()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
       
        if (item == true)
        {
            //yield return new WaitForSeconds(2.0f);
            //아이템을 생성하고 펄스값으로 바꾼다
            Item.SetActive(true);
            Item.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            if (GranadeItem != null)
            {
                GranadeItem.SetActive(true);
                GranadeItem.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            }
            WaponPersentageCheck();
            GranadePersentCheck();
            //item = false;
        }
        if (item == false)
        {
            WaponPersentageCheck();
            GranadePersentCheck();
        }

        int idx = Random.Range(1, Spawn.Length);
        nav.enabled = false;
        transform.position = new Vector3(Spawn[idx].position.x, Spawn[idx].position.y, Spawn[idx].position.z);
        nav.enabled = true;
        Hp = MaxHp;
        ES = EnemyState.Idle;
        ExplosionBody = false;
    }

    private IEnumerator DieSet()
    {
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        yield return new WaitForSeconds(4.0f);
        if (item == true)
        {
            //yield return new WaitForSeconds(2.0f);
            //아이템을 생성하고 펄스값으로 바꾼다
            Item.SetActive(true);
            Item.transform.position = new Vector3(transform.position.x, -0.45f, transform.position.z);

            if (GranadeItem != null)
            {
                GranadeItem.SetActive(true);
                GranadeItem.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            }
            WaponPersentageCheck();
            GranadePersentCheck();
            //item = false;
        }
        if (item == false)
        {
            WaponPersentageCheck();
            GranadePersentCheck();
        }
        gameObject.SetActive(false);
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
        int idx = Random.Range(1, Spawn.Length);
        nav.enabled = false;
        transform.position = new Vector3(Spawn[idx].position.x, Spawn[idx].position.y, Spawn[idx].position.z);
        nav.enabled = true;
        Hp = MaxHp;
        ES = EnemyState.Idle;
        gameObject.SetActive(true);
        //yield return new WaitForSeconds(2.0f);
    }

    void WaponPersentageCheck()
    {
        float Waponrand = Random.Range(0, 100);
        if (Waponrand <= WaponPercentage - 1)
        {
            item = true;
            Item = Instantiate(WaponSet[Random.Range(0, WaponSet.Length)]);
            Item.SetActive(false);
        }
        else if (Waponrand < WaponPercentage2 - 1)
        {
            item = true;
            Item = Instantiate(WaponSet2[Random.Range(0, WaponSet2.Length)]);
            Item.SetActive(false);
        }
        else
        {
            item = false;
        }
    }

    void GranadePersentCheck()
    {
        float Granaderand = Random.Range(0, 100);
        if (Granaderand <= GranadePercentage - 1)
        {
            GranadeItem = Instantiate(GrandeSet[Random.Range(0, GrandeSet.Length)]);
            GranadeItem.SetActive(false);
        }
        else if (Granaderand <= GranadePercentage2 - 1)
        {
            GranadeItem = Instantiate(GrandeSet2[Random.Range(0, GrandeSet2.Length)]);
            GranadeItem.SetActive(false);
        }

    }

    private void Awake()
    {
       
        //anim = GetComponent<Animator>();
        animation = GetComponent<Animation>();
        nav = GetComponent<NavMeshAgent>();
        Bullet.GetComponent<EnemyBullet>().Power = Damage;
        p = GameObject.Find("Player").GetComponent<Player>();
        DieDoll = transform.parent.gameObject.GetComponent<DistanceRagdollObject>();
    }

    // Use this for initialization
    void Start () {
        Player = GameObject.Find("Player");
        NavPlayer = GameObject.FindGameObjectWithTag("Player");
        if (item == true)
        {
            WaponPersentageCheck();
            GranadePersentCheck();
        }
        Spawn = GameObject.Find("Spawn Point").GetComponentsInChildren<Transform>();
        if (NotPooling == false)
        {
            int idx = Random.Range(1, Spawn.Length);
            nav.enabled = false;
            transform.position = new Vector3(Spawn[idx].position.x, Spawn[idx].position.y, Spawn[idx].position.z);
            
            transform.rotation = Spawn[idx].rotation;
            nav.enabled = true;
        }
        theAudio = FindObjectOfType<AudioManager>();
        //ES = EnemyState.Move;
    }
	
    void AnimationUpdate()
    {
        if(ES==EnemyState.Idle)
        {
            animation.CrossFade(Ani[2].name, 0.2f);
        }
        else if(ES==EnemyState.Move)
        {
            animation.CrossFade(Ani[0].name,0.2f);
        }
        else if(ES==EnemyState.Attack)
        {
            animation.CrossFade(Ani[1].name, 0.2f);
        }
     
    }
	// Update is called once per frame
	void Update () {
        if (ES != EnemyState.Die)
        {
            DistanceCheck();
            Move();
            AttackRageCheck();
        }
        AnimationUpdate();

    }
}
