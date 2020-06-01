using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour {
    public Text Information;

    public void Enter()
    {
        Information.gameObject.SetActive(true);
    }

    public void Exit()
    {
        Information.gameObject.SetActive(false);
    }
}
