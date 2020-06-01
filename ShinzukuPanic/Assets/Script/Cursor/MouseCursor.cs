using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{

    //마우스 포인터를 사용할 텍스쳐
    public Texture2D CursorTexture;

    //텍스쳐의 중심을 마우스 좌표로 할 것인지 체크
    public bool hotSpotIsCenter = false;

    //텍스처의 어느부분을 마우스의 좌표로 할 것인지 텍스처의 좌표
    public Vector2 AdjustHotSpot = Vector2.zero;

    //내부에서 사용할 필드
    private Vector2 hotSpot;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(MoveCursor());
    }

    IEnumerator MoveCursor()
    {
        yield return new WaitForEndOfFrame();

        if (hotSpotIsCenter)
        {
            hotSpot.x = CursorTexture.width / 2;
            hotSpot.y = CursorTexture.height / 2;
        }
        else
        {
            //중심을 사용하지 않을 경우 Adjust Hot Spot으로 입력 받은
            //것을 사용합니다.
            hotSpot = AdjustHotSpot;
        }
        //이제 새로운 마우스 커서를 화면에 표시합니다.
        Cursor.SetCursor(CursorTexture, hotSpot, CursorMode.Auto);
    }
}
