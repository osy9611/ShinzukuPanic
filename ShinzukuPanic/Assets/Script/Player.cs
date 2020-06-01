using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerState
{
    Idle,
    Walk,
    Run,
    Attack,
    GranadeAttack,
    MeleeAttack,
    Roll,
    Die,
    WaponChange
}

public enum Wapon
{
    MainWapon,
    MainWapon2,
    Gun,
    Granade,
}

public enum WaponType
{
    NULL,
    P220,
    UMP45,
    Type89,
    MP7,
    M1014,
    Type64
}

public enum GranadeType
{
    Granade,
    Craymore,
    Firebomb
}

public struct BulletState
{
    public int Id;
    public float Speed;
    public float Power;
    public float Penetration;
    public float BulletCount;
    public bool Auto;
    public int SoundNum;
    public float Delay;
}

public class Player : MonoBehaviour {

    static public Player instance;

    public PlayerState PS;
    public Wapon WP;
    Wapon Wp2;
    public WaponType MainWapon;
    public WaponType MainWapon2;
    public GranadeType GS;
    public GameManager GM;
    public float WalkSpeed;
    public float RunSpeed;
    public float WheelSpeed;
    public float BackSpeed;
    private float Speed;

    Animation animation;
    public AnimationClip[] Ani; // 애니메이션을 동작시킬 함수
    public GameObject[] Wapons;
    public GameObject Katana;
    public Camera camera;

    /*총알 관련 함수들*/
    public BulletState BS;
    public BulletState BS2;
    public GameObject MainBullet;
    public GameObject MainBullet2;
    public GameObject Bullets;       //총알
    public GameObject[] ShotPoint;     //총알을 발사할 지점
    public GameObject GranadeShotPoint;
    public GameObject[] ShotPointRotation;
    public GameObject SetPoint;
    GameObject ShotGunPoint;

    //사운드 관련 함수들
    public GameObject[] ShotFX;       //총을 발사할때 발동시킬 이팩트
    public string SlashSound;       //그냥 배는 사운드
    public string GranadePinset;    //폭탄 던질때 사운드
    
    private AudioManager theAudio;
    //수류탄 관련 함수들
    public GameObject[] Granade;      //수류탄
    public int GranadeCount;
    public int CraymoreCount;         //크레이모어 카운트
    public int FireBombCount;         //화염병 카운트
    public float WaitGranadeTime;
    public bool GranadeDrop=true;           //폭탄을 던질지 판단하는 여부

    //캐릭터 상태관련 함수들
    public float Hp;    //플레이어의 피통
    public float PentrationPoint;
    public float GardPoint;
    public GameObject Hp_gage;  //Hp 게이지
    public GameObject Curser; //마우스 커서 디자인
    public bool on = false;
    public bool on2 = false;
    public bool die;
    bool Chage;
    bool WaitTime;
    bool Stop;
    public float timer = 0;
    float ItemID;
    float x;
    float z;
    /*근접 공격 및 구르기 체크관련 함수*/
    public bool RollCheck;
    public bool MeleeChck;
    public int KeyNum;
    public int WheelCount;
    public GameObject SlashFX;
    public float KatanaDamage;
    //회복키트 및 상점 관련함수
    public float ItemBoxRange;  //상점과의 거리
    public int HealKitCount;
    public int HPS; //회복량을 설정하는 함수
    bool HealOn;    //회복할때 UI게이지를 바꾸기 위함
    ItemBox itembox;
    Transform Box;
    public GameObject ItemManager;
    public bool AttackOff;  //키트 아이템을 누를 때 공격을 멈춰야하기 떄문에
    public bool HealOff;
    public float HealCoolTime;
    public float HealTime;

    //카메라 쉐이크 관련
    Follow CameraShake;
    float ShakeCount;
    void Move() //기본적인 플레이어의 움직임
    {
        x = Input.GetAxisRaw("Vertical");
        z = Input.GetAxisRaw("Horizontal");
        if (PS != PlayerState.Attack)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
               
                if (PS != PlayerState.Roll)
                {
                    if (PS != PlayerState.MeleeAttack)
                    {
                        if (x == -1)
                        {
                            Speed = BackSpeed;
                        }
                        else
                        {
                            Speed = WalkSpeed;
                        }
                        PS = PlayerState.Walk;
                        //StartCoroutine(WalkSound());
                    }
                    
                }
                if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    Speed = WalkSpeed;
                }
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    if (x == 1)
                    {
                        if (RollCheck == false)
                        {
                            RollCheck = true;
                            StartCoroutine(Roll());
                        }
                    }
                }

               
                if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    if (x != -1)
                    {
                        if (MeleeChck == false)
                        {
                            SlashFX.SetActive(true);
                            MeleeChck = true;
                            theAudio.Play(SlashSound);
                            StartCoroutine(MeleeAttack());
                        }
                    }
                }
            }

            
            if (MainWapon != WaponType.NULL)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    WP = Wapon.MainWapon;
                    theAudio.OncePlay(10);
                    if (MainWapon==MainWapon2)
                    {
                        Wapons[0].SetActive(false);
                        Wapons[BS.Id].SetActive(true);
                        Wapons[BS2.Id].SetActive(true);
                    }
                    else
                    {
                        Wapons[0].SetActive(false);
                        Wapons[BS.Id].SetActive(true);
                        Wapons[BS2.Id].SetActive(false);
                    }
                    StartCoroutine(WaponChange());
                    KeyNum = 1;
                }

                
            }
            if (MainWapon2 != WaponType.NULL)
            {
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    theAudio.OncePlay(10);
                    WP = Wapon.MainWapon2;
                    if (MainWapon == MainWapon2)
                    {
                        Wapons[0].SetActive(false);
                        Wapons[BS.Id].SetActive(true);
                        Wapons[BS2.Id].SetActive(true);
                    }
                   else
                    {
                        Wapons[0].SetActive(false);
                        Wapons[BS.Id].SetActive(false);
                        Wapons[BS2.Id].SetActive(true);
                    }
                    StartCoroutine(WaponChange());
                    KeyNum = 2;
                }
            }
            if (WP != Wapon.Gun)
            {
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    WP = Wapon.Gun;
                    theAudio.OncePlay(10);
                    if (MainWapon2 == WaponType.NULL)
                    {
                        Wapons[BS.Id].SetActive(false);
                        Wapons[0].SetActive(true);
                    }
                    else if (MainWapon2==WaponType.NULL&& MainWapon == WaponType.NULL)
                    {
                        Wapons[0].SetActive(true);
                    }
                    else
                    {
                        Wapons[0].SetActive(true);
                        Wapons[BS.Id].SetActive(false);
                        Wapons[BS2.Id].SetActive(false);
                    }
                    StartCoroutine(WaponChange());
                    KeyNum = 3;
                }
            }
            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            {
                StartCoroutine(WaitIdle());
            }


            if (Input.GetKeyDown(KeyCode.G))
            {
                BulletCountCheck();
                if (WP == Wapon.MainWapon)
                {
                    if (MainWapon2 == WaponType.NULL)
                    {
                        WP = Wapon.Gun;
                        StartCoroutine(WaponChange());
                        Wapons[0].SetActive(true);
                        Wapons[BS.Id].SetActive(false);
                        MainWapon = WaponType.NULL;
                        BS.BulletCount = 0;
                        KeyNum = 3;

                    }
                    else
                    {
                        BS.BulletCount = 0;
                        StartCoroutine(WaponChange());
                        Wapons[BS.Id].SetActive(false);
                        Wapons[BS2.Id].SetActive(true);
                        //MainWapon = WaponType.NULL;
                        KeyNum = 2;
                    }
                }
                if (WP == Wapon.MainWapon2)
                {
                    
                    if (MainWapon == WaponType.NULL)
                    {
                        WP = Wapon.Gun;
                        StartCoroutine(WaponChange());
                        Wapons[0].SetActive(true);
                        Wapons[BS2.Id].SetActive(false);
                        MainWapon2 = WaponType.NULL;
                        BS2.BulletCount = 0;
                        KeyNum = 3;

                    }
                    else
                    {
                        WP = Wapon.MainWapon;
                        StartCoroutine(WaponChange());
                        Wapons[BS.Id].SetActive(true);
                        Wapons[BS2.Id].SetActive(false);
                        MainWapon2 = WaponType.NULL;
                        BS2.BulletCount = 0;
                        KeyNum = 1;
                    }
                }

            }
        }



        if (Input.GetKeyDown(KeyCode.S))
        {
            Speed = BackSpeed;
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            Speed = WalkSpeed;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Stop = true;
            on = false;
            on2 = false;
           
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (AttackOff == false)
            {
                CheckShot();
            }
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            if (PS != PlayerState.Die)
            {
                StartCoroutine("Fire");
            }
        }

        if (Input.GetMouseButtonDown(0) && Input.GetMouseButtonDown(1))
        {
            if (PS != PlayerState.Die)
            {
                StartCoroutine("Fire");
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            WheelCount++;
            switch (WheelCount)
            {
                case 0:
                    GS = GranadeType.Granade;
                    break;
                case 1:
                    GS = GranadeType.Craymore;
                    break;
                case 2:
                    GS = GranadeType.Firebomb;
                    break;
                case 3:
                    WheelCount = 0;
                    GS = GranadeType.Granade;
                    break;
            }
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            if (HealOff == true)
            {
                HealOff = false;
                if (HealKitCount > 0 && Hp <= 70)
                {
                    Hp += HPS;
                    Hp_gage.GetComponent<Image>().fillAmount += 0.3f;
                    HealKitCount--;

                }
                else if (HealKitCount > 0 && Hp >= 70)
                {
                    Hp = 100;
                    Hp_gage.GetComponent<Image>().fillAmount = 100;
                    HealKitCount--;
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position,transform.forward,out hit, ItemBoxRange))
            {
                if(hit.collider.gameObject.tag=="Shop")
                {
                    Time.timeScale = 0.3f;
                    itembox.ShopOn();
                    AttackOff = true;
                }
            }
        }

        if (GranadeDrop==false)
        {
            timer += Time.deltaTime;
            if (timer > WaitGranadeTime)
            {
                GranadeDrop = true;
                timer = 0;
            }
        }

        if (HealOff==false)
        {
            HealCoolTime += Time.deltaTime;
            if(HealCoolTime>HealTime)
            {
                HealOff = true;
                HealCoolTime = 0;
            }
        }
       
        transform.Translate(z * Speed * Time.deltaTime, 0f, x * Speed * Time.deltaTime);
        BulletCountCheck();
        //transform.position = new Vector3(transform.position.x,0.0f, transform.position.z);
    }

    void LookUpdate()   //마우스의 위치에 따라 방향이 바뀜
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Plane Ground = new Plane(Vector3.up, Vector3.zero);
        float Distance;
        if (Ground.Raycast(ray, out Distance))
        {
            Vector3 point = ray.GetPoint(Distance);
            if (Vector3.Distance(transform.position, point) > 2.0)
            {
                transform.LookAt(new Vector3(point.x,transform.position.y,point.z));
            }
        }

       
    }

   
    void AnimationUpdate() //애니메이션을 실행할 함수
    {
        if (PS == PlayerState.Idle)
        {
            if (WP == Wapon.Gun)
            {
                animation.CrossFade(Ani[7].name, 0.2f);
            }
            else
            {
                animation.CrossFade(Ani[0].name, 0.2f);
            }
        }
        else if (PS == PlayerState.Walk)
        {
            if (WP == Wapon.Gun)
            {
                if (PS != PlayerState.Attack && !animation.IsPlaying(Ani[3].name))
                {
                    if (z == 1 && x == 0)
                    {
                        animation.CrossFade(Ani[11].name, 0.2f);
                    }
                    else if (z == -1 && x == 0)
                    {
                        animation.CrossFade(Ani[12].name, 0.2f);
                    }
                    else if (x == -1)
                    {
                        animation.CrossFade(Ani[13].name, 0.2f);
                    }
                    else
                    {
                        animation.CrossFade(Ani[8].name, 0.2f);
                    }
                }
                else
                {
                    animation.CrossFade(Ani[8].name, 0.2f);
                }
            }
            else
            {
                if (PS != PlayerState.Attack && !animation.IsPlaying(Ani[3].name))
                {
                    if (z == 1 && x == 0)
                    {
                        animation.CrossFade(Ani[18].name, 0.2f);
                    }
                    else if (z == -1 && x == 0)
                    {
                        animation.CrossFade(Ani[19].name, 0.2f);
                    }
                    else if (x == -1)
                    {
                        animation.CrossFade(Ani[20].name, 0.2f);
                    }
                    else
                    {
                        animation.CrossFade(Ani[1].name, 0.2f);
                    }
                }
                else
                {
                    animation.CrossFade(Ani[1].name, 0.2f);
                }
            }

        }
        else if (PS == PlayerState.Run)
        {
            if (WP == Wapon.Gun)
            {
                if (PS != PlayerState.Attack && !animation.IsPlaying(Ani[3].name))
                {
                    if (z == 1 && x == 0)
                    {
                        animation.CrossFade(Ani[11].name, 0.2f);
                    }
                    else if (z == -1 && x == 0)
                    {
                        animation.CrossFade(Ani[12].name, 0.2f);
                    }
                    else if (x == -1)
                    {
                        animation.CrossFade(Ani[13].name, 0.2f);
                    }
                    else
                    {
                        animation.CrossFade(Ani[8].name, 0.2f);
                    }


                }
                else
                {
                    animation.CrossFade(Ani[8].name, 0.2f);
                }
            }
            else
            {
                if (PS != PlayerState.Attack && !animation.IsPlaying(Ani[3].name))
                {
                    if (z == 1 && x == 0)
                    {
                        animation.CrossFade(Ani[18].name, 0.2f);
                    }
                    else if (z == -1 && x == 0)
                    {
                        animation.CrossFade(Ani[19].name, 0.2f);
                    }
                    else if (x == -1)
                    {
                        animation.CrossFade(Ani[20].name, 0.2f);
                    }
                    else
                    {
                        animation.CrossFade(Ani[1].name, 0.2f);
                    }
                    
                }
                else
                {
                    animation.CrossFade(Ani[1].name, 0.2f);
                }
            }
        }
        else if (PS == PlayerState.Attack)
        {
            if (WP == Wapon.Granade)
            {
                if(GS==GranadeType.Craymore)
                {
                    animation.CrossFade(Ani[27].name, 0.2f);
                }
                else
                {
                    if (x == 0 && z == 0)
                    {

                        animation.CrossFade(Ani[26].name, 0.2f);
                    }
                    else
                    {
                        animation.CrossFade(Ani[10].name, 0.2f);
                    }
                }
            }
            else if (WP == Wapon.Gun)
            {
                if (z == 1 && x == 0)
                {
                    animation.CrossFade(Ani[15].name, 0.2f);
                }
                else if (z == -1 && x == 0)
                {
                    animation.CrossFade(Ani[16].name, 0.2f);
                }
                else if (x == -1)
                {
                    animation.CrossFade(Ani[17].name, 0.2f);
                }
                else if(x==0&&z==0)
                {
                    animation.CrossFade(Ani[7].name, 0.2f);
                }
                else
                {
                    animation.CrossFade(Ani[14].name, 0.2f);
                }
            }
            else if (WP == Wapon.MainWapon || WP == Wapon.MainWapon2)
            {
                if (z == 1 && x == 0)
                {
                  
                    animation.CrossFade(Ani[22].name, 0.2f);
                }
                else if (z == -1 && x == 0)
                {
                    animation.CrossFade(Ani[23].name, 0.2f);
                }
                else if (x == -1)
                {
                    animation.CrossFade(Ani[24].name, 0.2f);
                }
                else if (x == 0 && z == 0)
                {
                    animation.CrossFade(Ani[3].name, 0.2f);
                }
                else
                {
                    animation.CrossFade(Ani[21].name, 0.2f);
                }
            }
        }
        else if (PS == PlayerState.Die)
        {
            if (die == false)
            {
                animation.CrossFade(Ani[4].name, 0.2f);
                die = true;
            }

        }
        else if (PS == PlayerState.Roll)
        {
            animation.CrossFade(Ani[5].name, 0.2f);
        }
        else if (PS == PlayerState.WaponChange)
        {
            animation.CrossFade(Ani[6].name, 0.2f);
        }
        else if(PS==PlayerState.MeleeAttack)
        {
            animation.CrossFade(Ani[25].name, 0.2f);
        }
    }

    public void Hurt(float damage)  //피통이 0보다 크면 데미지를 받고 피통이 0이 되면 사망
    {
        if (Hp > 0)
        {
            if (HealOn == false)
            {
                if(GardPoint>0)
                {
                    damage -= (GardPoint * 0.01f);
                    Hp -= damage;
                }
                else
                {
                    Hp -= damage;
                }
                Hp_gage.GetComponent<Image>().fillAmount -= damage / 100;
               
                if (ShakeCount==10)
                {
                    //CameraShake.CameraShake(0.5f, 0.5f);
                    ShakeCount = 0;
                }
                else
                {
                    ShakeCount++;
                }
               
            }
          
        }
        if(Hp<=0)       
        {
            Speed = 0f;
            PS = PlayerState.Die;
            GM.GameOver();
           
        }
    }
    public void SetBullet(float speed,float power,float pentration,int bulletcount,bool auto,int Sound,float Delay)
    {
        if (BS.BulletCount==0)
        {
            BS.Speed=speed;
            BS.Power = power;
            BS.Penetration = pentration+PentrationPoint;
            BS.BulletCount = bulletcount;
            BS.Auto = auto;
            BS.SoundNum = Sound;
            BS.Delay = Delay;
            Bullet temp = MainBullet.GetComponent<Bullet>();
            temp.Speed = speed;
            temp.Power = power;
            temp.Penetration = pentration + PentrationPoint;
            Debug.Log(temp.Penetration);

            if (WP==Wapon.MainWapon)
            {
                KeyNum = 1;
            }
            if (WP == Wapon.MainWapon2)
            {
                KeyNum = 2;
            }
            if (WP == Wapon.Gun)
            {
                KeyNum = 3;
            }

        }
        else if(BS.BulletCount != 0)
        {
            BS2.Speed = speed;
            BS2.Power = power;
            BS2.Penetration = pentration + PentrationPoint;
            Debug.Log(BS2.Penetration);
            BS2.BulletCount = bulletcount;
            BS2.Auto = auto;
            BS2.SoundNum = Sound;
            BS2.Delay = Delay;
            Bullet temp = MainBullet2.GetComponent<Bullet>();
            temp.Speed = speed;
            temp.Power = power;
            temp.Penetration = pentration + PentrationPoint;

            if (WP == Wapon.MainWapon)
            {
                KeyNum = 1;
            }
            if (WP == Wapon.MainWapon2)
            {
                KeyNum = 2;
            }
            if (WP == Wapon.Gun)
            {
                KeyNum = 3;
            }
        }
    }
    public void SetShotBullet(float speed,float power,float pentration, int bulletcount,bool auto,float Angle,float AngleDivision,float BulletCount,int Sound)
    {
        theAudio.OncePlay(10);
        if (BS.BulletCount == 0)
        {
            BS.Speed = speed;
            BS.Power = power;
            BS.Penetration = pentration+PentrationPoint;
            BS.BulletCount = bulletcount;
            BS.Auto = auto;
            BS.SoundNum = Sound;
            Bullet temp = MainBullet.GetComponent<Bullet>();
            temp.Speed = speed;
            temp.Power = power;
            temp.Penetration = pentration + PentrationPoint;

            if (WP == Wapon.MainWapon)
            {
                KeyNum = 1;
            }
            if (WP == Wapon.MainWapon2)
            {
                KeyNum = 2;
            }
            if (WP == Wapon.Gun)
            {
                KeyNum = 3;
            }
        }
        else if (BS.BulletCount != 0)
        {
            BS2.Speed = speed;
            BS2.Power = power;
            BS2.Penetration = pentration + PentrationPoint;
            BS2.BulletCount = bulletcount;
            BS2.Auto = auto;
            BS2.SoundNum = Sound;
            Bullet temp = MainBullet2.GetComponent<Bullet>();
            temp.Speed = speed;
            temp.Power = power;
            temp.Penetration = pentration + PentrationPoint;

            if (WP == Wapon.MainWapon)
            {
                KeyNum = 1;
            }
            if (WP == Wapon.MainWapon2)
            {
                KeyNum = 2;
            }
            if (WP == Wapon.Gun)
            {
                KeyNum = 3;
            }
        }

        for (int i = 0; i < BulletCount; i++)
        {
            ShotPointRotation[i] = Instantiate(ShotPoint[4]);
            ShotPointRotation[i].transform.parent = SetPoint.transform;
            Quaternion rot = Quaternion.identity;
            rot.eulerAngles = new Vector3(0, 130+ (-Angle + (AngleDivision * i)), 0);
            ShotPointRotation[i].transform.localRotation = rot;
        }
    }
    public void SetWapon(WaponType WT)
    {
       
        theAudio.OncePlay(10);
        if (MainWapon==WaponType.NULL)
        {
            MainWapon = WT;
            BS.Id = (int)WT-1;
            
        }
        else if(MainWapon2 ==WaponType.NULL)
        {
            MainWapon2 = WT;
            BS2.Id = (int)WT-1;
        }
    }
    private void CheckShot()
    {
        if (WP == Wapon.MainWapon)
        {
            on = true;
            StartCoroutine("MainFire");
        }

        if (WP == Wapon.MainWapon2)
        {
            on2 = true;
            StartCoroutine("Main2Fire");
        }

        if (WP == Wapon.Gun)
        {
            if (PS != PlayerState.Die)
            {
                
                StartCoroutine("GunFire");
            }
        }
    }
    private IEnumerator WaitIdle()
    {
        yield return new WaitForSeconds(0.05f);
        PS = PlayerState.Idle;
    }
    private IEnumerator Roll()
    {
        if (RollCheck == true)
        {
           
            Speed = WheelSpeed;
            PS = PlayerState.Roll;
            yield return new WaitForSeconds(1.0f);
            PS = PlayerState.Idle;
            RollCheck = false;
        }
    }
    private IEnumerator MeleeAttack()
    {
        if (MeleeChck == true)
        {
            Speed = WheelSpeed;
            Wapons[0].SetActive(false);
            Wapons[BS.Id].SetActive(false);
            Wapons[BS2.Id].SetActive(false);
            Katana.SetActive(true);
            PS = PlayerState.MeleeAttack;
            yield return new WaitForSeconds(0.5f);
            PS = PlayerState.Idle;
            Katana.SetActive(false);
            //SlashFX.transform.LookAt(-transform.forward);
            SlashFX.SetActive(false);
            if (WP == Wapon.Gun)
            {
                yield return new WaitForSeconds(0.5f);
                Wapons[0].SetActive(true);
            }
            else if (WP == Wapon.MainWapon)
            {
                yield return new WaitForSeconds(0.5f);
                Wapons[BS.Id].SetActive(true);
            }
            else if (WP == Wapon.MainWapon2)
            {
                yield return new WaitForSeconds(0.5f);
                Wapons[BS2.Id].SetActive(true);
            }
            MeleeChck = false;
        }
    }
    private IEnumerator MainFire()
    {
        if (BS.Auto == true && WP==Wapon.MainWapon)
        {
            while (on)
            {
                if (BS.BulletCount > 0)
                {
                    GameObject bullet = Instantiate(MainBullet, ShotPoint[BS.Id].transform.position, transform.rotation);
                    theAudio.OncePlay(BS.SoundNum);
                    BS.BulletCount--;
                    ShotFX[BS.Id].SetActive(true);
                    PS = PlayerState.Attack;
                    yield return new WaitForSeconds(BS.Delay);
                    ShotFX[BS.Id].SetActive(false);
                   
                    PS = PlayerState.Idle;
                }
                else
                {
                    break;
                }

            }

        }
        else if (BS.Auto == false)
        {
            if (PS == PlayerState.Attack && BS.BulletCount>0)
            {
                if (MainWapon == WaponType.M1014)
                {
                    StartCoroutine(ShotGun());
                    BS.BulletCount--;
                
                }
                else if (MainWapon != WaponType.M1014)
                {
                    GameObject bullet = Instantiate(MainBullet, ShotPoint[BS.Id].transform.position, transform.rotation);
                    BS.BulletCount--;
                    theAudio.OncePlay(BS.SoundNum);
                    ShotFX[BS.Id].SetActive(true);
                    yield return new WaitForSeconds(BS.Delay);
                    ShotFX[BS.Id].SetActive(false);
                }
                
               
            }
            if (PS == PlayerState.Walk || PS == PlayerState.Run || PS == PlayerState.Idle)
            {
                PS = PlayerState.Attack;
                yield return new WaitForSeconds(1.0f);
                PS = PlayerState.Idle;
            }
        }

    }
    private IEnumerator Main2Fire()
    {
        if (BS2.Auto == true)
        {
            while (on2)
            {
                if (BS2.BulletCount > 0)
                {
                    GameObject bullet = Instantiate(MainBullet2, ShotPoint[BS2.Id].transform.position, transform.rotation);
                    theAudio.OncePlay(BS2.SoundNum);
                    BS2.BulletCount--;
                    ShotFX[BS2.Id].SetActive(true);
                    PS = PlayerState.Attack;
                    yield return new WaitForSeconds(BS2.Delay);
                    ShotFX[BS2.Id].SetActive(false);

                    PS = PlayerState.Idle;
                }
                else
                {
                    break;
                }
            }
        }
        else if (BS2.Auto == false)
        {
            if (PS == PlayerState.Attack && BS2.BulletCount>0)
            {
                if (MainWapon2 == WaponType.M1014)
                {

                    StartCoroutine(ShotGun());
                    BS2.BulletCount--;

                }
                else if (MainWapon2 != WaponType.M1014)
                {
                    GameObject bullet = Instantiate(MainBullet2, ShotPoint[BS2.Id].transform.position, transform.rotation);
                    BS2.BulletCount--;
                    theAudio.OncePlay(BS2.SoundNum);
                    ShotFX[BS2.Id].SetActive(true);
                    yield return new WaitForSeconds(BS2.Delay);
                    ShotFX[BS2.Id].SetActive(false);
                }
            }
            if (PS == PlayerState.Walk || PS == PlayerState.Run || PS == PlayerState.Idle)
            {
                PS = PlayerState.Attack;
                yield return new WaitForSeconds(1.0f);
                PS = PlayerState.Idle;

            }

        }
    }
    private IEnumerator GunFire()
    {
      
        if (PS == PlayerState.Attack)
        {
            theAudio.OncePlay(7);
            GameObject bullet = Instantiate(Bullets, ShotPoint[0].transform.position, transform.rotation);
            ShotFX[0].SetActive(true);
            yield return new WaitForSeconds(0.25f);
            ShotFX[0].SetActive(false);
        }
        if (PS == PlayerState.Walk || PS == PlayerState.Run || PS == PlayerState.Idle)
        {
            PS = PlayerState.Attack;
            yield return new WaitForSeconds(1.0f);
            PS = PlayerState.Idle;
        }
    }
    public IEnumerator Fire()
    {
        if (GranadeDrop == true)
        {
            if (GS == GranadeType.Granade)
            {
                if (GranadeCount != 0)
                {
                    
                    Wp2 = WP;
                    WP = Wapon.Granade;
                    GameObject granade = Instantiate(Granade[0], GranadeShotPoint.transform.position, GranadeShotPoint.transform.rotation);
                    Physics.IgnoreCollision(granade.GetComponent<Collider>(), GetComponent<Collider>());
                    PS = PlayerState.Attack;
                    GranadeCount--;
                    theAudio.Play(GranadePinset);
                    yield return new WaitForSeconds(0.5f);
                    PS = PlayerState.Idle;
                    GranadeDrop = false;
                    WP = Wp2;
                    //on = true;
                    //on2 = true;
                   
                }
            }
            else if(GS==GranadeType.Craymore)
            {
                if (CraymoreCount != 0)
                {
                    Wp2 = WP;
                    WP = Wapon.Granade;
                    GameObject granade = Instantiate(Granade[1], transform.position, transform.rotation);
                    Physics.IgnoreCollision(granade.GetComponent<Collider>(), GetComponent<Collider>());
                    PS = PlayerState.Attack;
                    CraymoreCount--;
                    yield return new WaitForSeconds(0.5f);
                    PS = PlayerState.Idle;
                    GranadeDrop = false;
                    WP = Wp2;
                    on = true;
                    on2 = true;
                }
            }else if(GS==GranadeType.Firebomb)
            {
                if (FireBombCount != 0)
                {
                    Wp2 = WP;
                    GameObject granade = Instantiate(Granade[2], GranadeShotPoint.transform.position, GranadeShotPoint.transform.rotation);
                    Physics.IgnoreCollision(granade.GetComponent<Collider>(), GetComponent<Collider>());
                    PS = PlayerState.Attack;
                    WP = Wapon.Granade;
                    FireBombCount--;
                    yield return new WaitForSeconds(0.5f);
                    PS = PlayerState.Idle;
                    GranadeDrop = false;
                    WP = Wp2;
                    on = true;
                    on2 = true;
                }
            }
           
         
        }
        //catrige.Play();       //추후 작업을 할때 사용하겠음
    }

    private IEnumerator ShotGun()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject Shotbullet = Instantiate(Bullets, ShotPoint[4].transform.position, ShotPointRotation[i].transform.rotation);
        }
        ShotFX[4].SetActive(true);
        yield return new WaitForSeconds(0.15f);
        theAudio.OncePlay(6);
        ShotFX[4].SetActive(false);
    }

    private IEnumerator WalkSound()
    {
        yield return new WaitForSeconds(0.15f);
        theAudio.OncePlay(5);
    }

    void BulletCountCheck()
    {
        if(MainWapon!=WaponType.NULL&&BS.BulletCount<=0)
        {
            if (MainWapon2 != WaponType.NULL)
            {
                Wapons[BS.Id].SetActive(false);
                BS.Id = BS2.Id;
                BS.Speed = BS2.Speed;
                BS.Power = BS2.Power;
                BS.Penetration = BS2.Penetration;
                BS.BulletCount = BS2.BulletCount;
                BS2.BulletCount = 0;
                BS.Auto = BS2.Auto;
                BS.Delay = BS2.Delay;
                BS.SoundNum = BS2.SoundNum;
                MainWapon = MainWapon2;
                MainWapon2 = WaponType.NULL;
                on = false;
                on2 = false;
                Wapons[BS.Id].SetActive(true);
                KeyNum = 1;

            }
            else if (MainWapon2 == WaponType.NULL)
            {
                WP = Wapon.Gun;
                KeyNum = 3;
                on = false;
                on2 = false;
                MainWapon = WaponType.NULL;
                Wapons[0].SetActive(true);
                Wapons[BS.Id].SetActive(false);
                Wapons[BS2.Id].SetActive(false);
                BS.BulletCount = 0;
            }
        }
        else if (MainWapon2 != WaponType.NULL && BS2.BulletCount <= 0)
        {
            MainWapon2 = WaponType.NULL;
            on = false;
            on2 = false;
        }
    }

    private IEnumerator WaponChange()
    {
        if (PS == PlayerState.Walk || PS == PlayerState.Run || PS == PlayerState.Idle)
        {
            PS = PlayerState.WaponChange;
            yield return new WaitForSeconds(0.25f);
            PS = PlayerState.Idle;
        }
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag=="Enemy")
        {
            if(PS==PlayerState.MeleeAttack)
            {
                collision.gameObject.GetComponent<Enemy>().Hurt(KatanaDamage);
               
            }
            else
            {
                return;
            }
        }

        if (collision.gameObject.tag == "DistanceEnemy")
        {
            if (PS == PlayerState.MeleeAttack)
            {
                collision.gameObject.GetComponent<DistanceEnemy>().Hurt(KatanaDamage);
            }
            else
            {
                return;
            }
        }

        if(collision.gameObject.tag=="Wall")
        {
            Speed = 0;
        }

       
    }

    private void OnTriggerEnter(Collider other)
    {
       if(other.tag == "EnemyBullet")
        {
            EnemyBullet bullet = other.GetComponent<EnemyBullet>();
            if(PS!=PlayerState.Die)
            {
                Hurt(bullet.Power);
                Destroy(other.gameObject);
            }
        }

        if (other.tag == "Granade")
        {
            GranadeCount += 1;
            Destroy(other.gameObject);
        }

        if (other.tag == "Craymore")
        {
            CraymoreCount += 1;
            Destroy(other.gameObject);
        }

        if(other.tag == "FireBottle")
        {
            FireBombCount += 1;
            Destroy(other.gameObject);
        }

        if(other.tag=="Kit")
        {
            HealKitCount += 1;
            Destroy(other.gameObject);
        }
     
    }

    private void Awake()
    {
        //ItemManager = GameObject.Find("Shops");
        Box = ItemManager.transform.Find("shop");
        itembox = Box.GetComponent<ItemBox>();
        CameraShake = FindObjectOfType<Follow>();
    }
    // Use this for initialization
    void Start () {
        animation = GetComponent<Animation>();
        GM.GetComponent<GameManager>();
        theAudio = FindObjectOfType<AudioManager>();
        //itembox = FindObjectOfType<ItemBox>();
        WP = Wapon.Gun;
        //GS = GranadeType.Granade;
    }
	
    void HaveWeaponCheck()
    {
        if(WP==Wapon.MainWapon)
        {
            if(!Wapons[BS.Id].activeSelf&&MainWapon!=WaponType.NULL)
            {
                Wapons[BS.Id].SetActive(true);
            }
        }
        else if (WP == Wapon.MainWapon2)
        {
            if (!Wapons[BS2.Id].activeSelf && MainWapon2 != WaponType.NULL)
            {
                Wapons[BS2.Id].SetActive(true);
            }

        }
        else if(WP == Wapon.Gun)
        {
            Wapons[0].SetActive(true);
        }
    }
    // Update is called once per frame
    void Update () {
        if (PS != PlayerState.Die)
        {
            Move();
            LookUpdate();
            HaveWeaponCheck();
            GM.GameClear();
            GM.WeaponUISet();
            GM.CreateBox();
            
            //GM.BloodScreen();
        }
        AnimationUpdate();    //애니메이션을 실행시키겠지만 추후에 하겠음
       
    }
}
