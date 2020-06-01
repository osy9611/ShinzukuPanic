using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Idle,
    Move,
    Attack,
    Hurt,
    Die,
}

public enum EnemyType
{
    Tank,
    Dog,
    Female,
    Male,
}
public class Enemy : MonoBehaviour {

    public WaponType WP;
    public EnemyState ES;        //적의 상태
    public EnemyType ET;         //적의 종류
    public EnemyManager EM;
    public Animator anim;        //애니메이션 넣을때 사용할것
    public AnimationClip[] Ani;  //애니메이션을 동작시킬 함수
    RagdollObject DieDoll;
    Animation animation;
    float Speed;            
    public float MoveSpeed;     //적이 움직이는 속도
    public float AttackSpeed;   //공격속도
    public bool item=false;
    public float Range;     //플레이어와의 거리
    public float Damage;        //데미지
    public GameObject Player;    //플레이어 위치
    Player p;
    GameObject NavPlayer;
    private Color col;
    /*현재 이팩트가 없으므로 나중에 작업*/
    public Transform FX_Point;  //이팩트를 발생시킬 위치
    public GameObject Hit_FX;   //공격 이팩트
    public GameObject Blood;
    private float BloodCount;
    // 사운드 관련 함수들
    public string SlashZombieSound; //좀비를 배는 사운드
    private AudioManager theAudio;
    public bool AttackSoundOn;

    //좀비 피통관련
    public float MaxHp = 100;
    public float Hp = 100;       //적 피통

    public Transform[] Spawn;    //몬스터 재활용을 위하여 스폰지역을 저장한다
    public GameObject Item;
    public GameObject GranadeItem;
    public float WaponPercentage, WaponPercentage2;
    public GameObject[] WaponSet;
    public GameObject[] WaponSet2;
    public float GranadePercentage, GranadePercentage2;
    public GameObject[] GrandeSet;
    public GameObject[] GrandeSet2;
    public int WaponNumber;
    public NavMeshAgent nav;
    public bool NotPooling;
    public float FadeSpeed;
    public bool ExplosionBody;
    public string SoundName;
    void DistanceCheck()        //플레이어와 나와의 거리를 측정
    {
        if (Vector3.Distance(Player.transform.position, transform.position) >= 0&&ES!=EnemyState.Attack)
        {
            StartCoroutine(MoveWait());
            if (ES != EnemyState.Hurt)
            {
                ES = EnemyState.Move;
                Hit_FX.SetActive(false);
            }
        }
        if(Vector3.Distance(Player.transform.position, transform.position) <= 20)
        {
            StartCoroutine(SoundOn());
        }
    }

    IEnumerator MoveWait()
    {
        nav.speed = MoveSpeed;
        yield return new WaitForSeconds(1.0f);
        if (ES == EnemyState.Move||ES==EnemyState.Hurt)
        {
            nav.speed = MoveSpeed;
        }
        else
        {
            nav.speed=0;
        }
    }
    void Move()     //적이 움직임
    {
        nav.SetDestination((Player.transform.position));
    }


    void AttackRageCheck()   //적이 공격하기 전에 플레이어와의 거리가 가까운지를 채크하여 상태를 변경
    {
        if (Vector3.Distance(Player.transform.position,this.transform.position)<= Range)
        {
            nav.speed = 0;
            Attack_On();
        }
        else if (Vector3.Distance(Player.transform.position, this.transform.position) > Range)
        {
            if (ES != EnemyState.Hurt&&ES!=EnemyState.Idle)
            {
                ES = EnemyState.Move;
                Hit_FX.SetActive(false);
            }
        }
    }

    IEnumerator SoundOn()
    {
        if (AttackSoundOn == false)
        {
            if (ET == EnemyType.Tank)
            {
                theAudio.Play("Tank_Zombie");
            }
            else if(ET==EnemyType.Male)
            {
                int idx = Random.Range(1, 3);
                theAudio.Play("Male_Zombie"+ idx);
            }
            else if(ET==EnemyType.Dog)
            {
                int idx = Random.Range(1, 3);
                theAudio.Play("Dog_Zombie" + idx);
            }
            else if(ET==EnemyType.Female)
            {
                int idx = Random.Range(1, 3);
                theAudio.Play("Female_Zombie" + idx);
            }
            yield return new WaitForSeconds(0.5f);
            AttackSoundOn = true;
        }
    }
    public void Attack_On()
    {
        transform.rotation = Quaternion.LookRotation((new Vector3(Player.transform.position.x, this.transform.position.y, Player.transform.position.z) - transform.position));
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        AttackSoundOn = true;
        ES = EnemyState.Attack;
        yield return new WaitForSeconds(AttackSpeed);
        if (ES == EnemyState.Attack)
        {
            Player.GetComponent<Player>().Hurt(Damage);
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
                if (gameObject.activeSelf == true)
                {
                    StartCoroutine(ShowEffect(other));
                }
            }
        }
    }

    IEnumerator ShowEffect(Collider other)
    {
        Hit_FX.transform.LookAt(other.transform.forward);
        Hit_FX.transform.position = other.transform.position;
        Hit_FX.SetActive(true);
        ES = EnemyState.Hurt;
        yield return new WaitForSeconds(1.0f);
        ES = EnemyState.Move;
        //Hit_FX.SetActive(false);
    }

    public void FireHurt(float timer,float damage)
    {
        if (Hp > 0)
        {
           
            StartCoroutine(FireDot(timer,damage));
        }
       
    }

    IEnumerator FireDot(float timer,float Damage)
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

        if(Hp>0)
        {
            ExplosionBody = false;
        }
        else if (Hp <= 0)
        {
            ES = EnemyState.Die;
            nav.speed = 0f;

            Hit_FX.SetActive(false);
            DieDoll.Blood.transform.position = new Vector3(transform.position.x, DieDoll.Blood.transform.position.y + transform.position.y, transform.position.z);
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
           
            //적이 맞았을 때 이팩트를 발동한다
            //GameObject FX = Instantiate(Hit_FX, FX_Point.position, Quaternion.LookRotation(FX_Point.forward));
            if(p.PS==PlayerState.MeleeAttack)
            {
                theAudio.Play(SlashZombieSound);
            }
            Hp -= damage;

           
        }
        if (Hp <= 0)
        {
            ES = EnemyState.Die;
            nav.speed = 0f;
            if(ET==EnemyType.Tank)
            {
                theAudio.Play("Tank_Zombie_Die");
            }
            else if(ET==EnemyType.Male)
            { 
                theAudio.Play("Male_Zombie_Die");
            }
            else if(ET==EnemyType.Dog)
            {
                theAudio.Play("Dog_Zombie_Die");
            }
            else if(ET==EnemyType.Female)
            {
                theAudio.Play("Female_Zombie_Die");
            }
            Hit_FX.SetActive(false);
            DieDoll.Blood.transform.position = new Vector3(transform.position.x, transform.position.y , transform.position.z);
            if (ExplosionBody==true)
            {
                DieDoll.ChageExpBody();
                DieDoll.ExpBody.transform.position = transform.position;
            }
            else
            {
                if(ET==EnemyType.Dog)
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
    }

    public void Set()
    {
        if (gameObject.activeSelf == false)
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
        }
    }

    private IEnumerator DieSet()
    {
        gameObject.SetActive(false);
        
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
        yield return new WaitForSeconds(4.0f);
        if(item==true)
        {
            //yield return new WaitForSeconds(2.0f);
            //아이템을 생성하고 펄스값으로 바꾼다
           Item.SetActive(true);
           Item.transform.position = new Vector3(transform.position.x, -0.45f, transform.position.z);
            
            if(GranadeItem!=null)
            {
                GranadeItem.SetActive(true);
                GranadeItem.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            }
            WaponPersentageCheck();
            GranadePersentCheck();
            //item = false;
        }
        if(item==false)
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
        gameObject.SetActive(true);
        DieDoll.ragdollObj.SetActive(false);
        ExplosionBody = false;
        //yield return new WaitForSeconds(2.0f);
    }
    
    void WaponPersentageCheck()
    {
        float Waponrand = Random.Range(0, 100);

        if (Waponrand <= WaponPercentage - 1&&WaponPercentage!=0)
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
        if (GranadePercentage != 0)
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
        else
        {
            return;
        }
    }

    void AnimationUpdate()
    {
        if ((ES == EnemyState.Move || ES == EnemyState.Hurt)&&ES!=EnemyState.Idle)
        {
            animation.CrossFade(Ani[0].name, 0.2f);
        }
        else if (ES == EnemyState.Attack)
        {
            animation.CrossFade(Ani[1].name, 0.2f);
        }
        else if(ES==EnemyState.Idle)
        {
            animation.CrossFade(Ani[2].name, 0.2f);
        }
      
    }

    private void Awake()
    {
        //Transform parent = GetComponentInParent
        DieDoll = transform.parent.gameObject.GetComponent<RagdollObject>();
        theAudio = FindObjectOfType<AudioManager>();
    }
    // Use this for initialization
    void Start () {

        Player = GameObject.Find("Player");

        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        animation = GetComponent<Animation>();

        p = GameObject.Find("Player").GetComponent<Player>();
        if (item==true)
        {
            WaponPersentageCheck();
            GranadePersentCheck();
        }
        NavPlayer = GameObject.FindGameObjectWithTag("Player");
        Spawn = GameObject.Find("Spawn Point").GetComponentsInChildren<Transform>();
        if (NotPooling == false)
        {
            int idx = Random.Range(1, Spawn.Length);
            nav.enabled = false;
            transform.position = new Vector3(Spawn[idx].position.x, Spawn[idx].position.y, Spawn[idx].position.z);
            transform.rotation = Spawn[idx].rotation;
            nav.enabled = true;
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
