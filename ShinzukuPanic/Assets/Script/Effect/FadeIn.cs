using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Fade
{
    FadeIn,
    FadeOut,
    Fade
}
public class FadeIn : MonoBehaviour {
    public Fade FD;
    public float animTime = 2f;     //Fade 애니메이션 재생 시간(단위:초)

    private Image fadeImage;        //UGUI의 Image컴포넌트 참조 변수

    private float FadeInstart = 0f;       //Mathf.Lerp 메소드의 첫번째 값
    private float FadeInend = 1f;         //Mathf.Lerp 메소드의 두번째 값
    private float time = 0f;        //Mathf.Lerp 메소드의 시간 값

    private float FadeOutstart = 1f;       //Mathf.Lerp 메소드의 첫번째 값
    private float FadeOutend = 0f;         //Mathf.Lerp 메소드의 두번째 값

    private bool isPlaying = false; //Fade 애니메이션의 중복 재생을 방지하기위함
    void Awake () {
        fadeImage = GetComponent<Image>();
        StertFadeAnim();
    }

    public void StertFadeAnim()     //애니메이션이 재생중이면 중복 재생되지 않도록 리턴
    {
        if(isPlaying == true)
        {
            return;
        }
        
        StartCoroutine("PlayFade");//Fade 애니메이션 재생
       
    }

    IEnumerator PlayFade()
    {
        isPlaying = true; //애니메이션을 재생한다

        if (FD == Fade.FadeIn)
        {

            Color color = fadeImage.color;
            time = 0f;
            color.a = Mathf.Lerp(FadeInstart, FadeInend, time);

            while (color.a < 1f)
            {
                time += Time.deltaTime / animTime;  //2초 동안 재생될 수 있도록 animTime으로 나누기
                color.a = Mathf.Lerp(FadeInstart, FadeInend, time); //알파 값 계산
                fadeImage.color = color;
                yield return null;
            }
        }
        else if(FD==Fade.FadeOut)
        {
            Color color = fadeImage.color;
            time = 0f;
            color.a = Mathf.Lerp(FadeOutstart, FadeOutend, time);
            while (color.a > 0f)
            {
                time += Time.deltaTime / animTime;  //2초 동안 재생될 수 있도록 animTime으로 나누기
                color.a = Mathf.Lerp(FadeOutstart, FadeOutend, time); //알파 값 계산
                fadeImage.color = color;
                yield return null;
            }
        }

        isPlaying = false;
        Destroy(this.gameObject);
    }

   
}
