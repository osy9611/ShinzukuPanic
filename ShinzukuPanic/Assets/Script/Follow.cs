using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

    public GameObject Target;
    public float Distance;      //카메라와 오브젝트의 거리
    public float Height;        //카메라의 높이
    public float Speed;         //카메라 속도
    Vector3 Pos;    //위치를 임시저장을 위한
    float originHeight;
   
    public float heightAboveObstacle = 12.0f;
    public float castOffset = 1.0f;

    private Transform tr;
    Renderer hitcolor;
    Color color;
    bool Check;
    public float FadeSpeed;
    float fade;


    //카메라 쉐이크를 하기위한 함수들
    Vector3 OriginPos;

    private void Start()
    {
        Target = GameObject.Find("Player");
        tr = GetComponent<Transform>();
        fade = 1;
        originHeight = Height;
        OriginPos = transform.localPosition;
     
    }

    public void CameraShake(float _amount,float _duration)
    {
        StartCoroutine(Shake(_amount, _duration));
    }
    public IEnumerator Shake(float _amount, float _duration)
    {
        float timer = 0;
        while (timer <= _duration)
        {
            transform.localPosition = (Vector3)Random.insideUnitCircle * _amount + Pos;

            timer += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = Pos;
    }

    void Update () {
        Pos = new Vector3(Target.transform.position.x, Target.transform.position.y+Height, Target.transform.position.z - Distance);

        this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, Pos, Speed * Time.deltaTime);

        Vector3 castTarget = Target.transform.position + (Target.transform.up * castOffset);
        Vector3 castDir = (castTarget - tr.position).normalized;
        RaycastHit hit;
        
        if(Physics.Raycast(tr.position,castDir,out hit,Mathf.Infinity))
        {
            if(hit.collider.CompareTag("Wall"))
            {
                Height = Mathf.Lerp(Height, heightAboveObstacle, Time.deltaTime * Speed);
                hitcolor = hit.collider.gameObject.GetComponent<Renderer>();
                color = hitcolor.material.color;
                if (fade > 0.3)
                {
                    fade -= Time.deltaTime / FadeSpeed;
                    hitcolor.material.color = new Color(hitcolor.material.color.r, hitcolor.material.color.g, hitcolor.material.color.b, fade);
                    
                }
                Check = true;
            }
            else if(!hit.collider.CompareTag("Wall"))
            {
                Height = Mathf.Lerp(Height, originHeight, Time.deltaTime * Speed);
                if (Check == true)
                {
                    if (fade < 1)
                    {
                        fade += Time.deltaTime / FadeSpeed;
                        hitcolor.material.color = new Color(hitcolor.material.color.r, hitcolor.material.color.g, hitcolor.material.color.b, fade);
                    }
                }
            }
        }
       
    }
}
