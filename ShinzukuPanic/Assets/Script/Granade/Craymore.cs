using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Craymore : MonoBehaviour {

    public float radius;
    public float range;
    public float AttackRange;
    public float damage;
    GameObject LeftLine;
    GameObject RightLine;
    GameObject Player;
    public GameObject Effect;
    LineRenderer Line,Line2;
    bool ExpOn;
    int count;
    public Collider[] target;
    public bool PlayerSet;
    public bool Drop;
    Follow CameraShake;
    private AudioManager theAudio;
    public string GranadeSound;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Enemy")
        {
            CheckRange();
            Debug.Log("일반 적 걸림"+ collision.gameObject.transform.position);
            count++;
        }

        if (collision.tag == "DistanceEnemy")
        {
            CheckRange();
            Debug.Log("원거리 적 걸림" + collision.gameObject.transform.position);
            count++;
        }
    }

    void CheckRange()
    {
        if (ExpOn == false)
        {
            target = Physics.OverlapSphere(transform.position, AttackRange);
         
            for (int i = 0; i < target.Length; i++)
            {
                Vector3 TargetDir = (target[i].transform.position - transform.position).normalized;
                float Angle = Vector3.Angle(transform.forward,TargetDir);
                if (Vector3.Dot(transform.forward, TargetDir) > Mathf.Cos((radius / 2) * Mathf.Deg2Rad))
                {
                    if (target[i].CompareTag("Enemy"))
                    {
                        if (PlayerSet == true)
                        {
                            target[i].GetComponent<Enemy>().ExplosionBody = true;
                            target[i].GetComponent<Enemy>().Hurt(damage);
                           
                            ExpOn = true;
                            StartCoroutine(ExpOff());
                        }
                    }
                    if (target[i].CompareTag("DistanceEnemy"))
                    {
                        if (PlayerSet == true)
                        {
                            target[i].GetComponent<DistanceEnemy>().ExplosionBody = true;
                            target[i].GetComponent<DistanceEnemy>().Hurt(damage);
                            ExpOn = true;
                            StartCoroutine(ExpOff());
                        }
                    }
                }
            }
        }
    }

    IEnumerator ExpOff()
    {
        if (ExpOn == true)
        {
            GameObject Effect2 = Instantiate(Effect, transform.position, transform.rotation);
            theAudio.Play(GranadeSound);
            CameraShake.CameraShake(0.5f, 0.5f);
            yield return new WaitForSeconds(1.0f);
            Destroy(gameObject);
            Destroy(Effect2.gameObject);
            //gameObject.SetActive(false);
            ExpOn = false;
            //gameObject.SetActive(true);
        }
    }
    public Vector3 DirFromAngle(float angleInDegrees)
    {
        //탱크의 좌우 회전값 갱신
        angleInDegrees += transform.eulerAngles.y;
        //경계 벡터값 반환
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    void Draw()
    {
        Vector3 leftBoundary = DirFromAngle(-radius / 2);
        Vector3 rightBoundary = DirFromAngle(radius / 2);
      
        Line.SetPosition(0, transform.position);
        Line.SetPosition(1, transform.position + leftBoundary * range);
        
        Line2.SetPosition(0, transform.position);
        Line2.SetPosition(1, transform.position + rightBoundary * range);
    }

   
    private void Start()
    {
        LeftLine = GameObject.Find("LeftLine");
        Line = LeftLine.GetComponent<LineRenderer>();
        RightLine = GameObject.Find("RightLine");
        Line2 = RightLine.GetComponent<LineRenderer>();
        Player = GameObject.Find("Player");
        if (PlayerSet == false)
        {
            LeftLine.gameObject.SetActive(false);
            RightLine.gameObject.SetActive(false);
        }
        else
        {
            LeftLine.gameObject.SetActive(true);
            RightLine.gameObject.SetActive(true);
        }
        CameraShake = FindObjectOfType<Follow>();
        theAudio = FindObjectOfType<AudioManager>();
    }
    private void Update()
    {
        if (PlayerSet == true)
        {
            Draw();
        }
    }

}
