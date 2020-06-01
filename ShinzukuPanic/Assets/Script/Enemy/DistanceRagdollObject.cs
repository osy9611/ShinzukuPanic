using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceRagdollObject : MonoBehaviour {
    public GameObject charObj;
    public GameObject ragdollObj;
    public GameObject Blood;
    public GameObject ExpBody;
    DistanceEnemy enemy;
    //public Rigidbody spine;

    private void Awake()
    {
        enemy = charObj.GetComponent<DistanceEnemy>();
    }

    public void ChangeRagdoll()
    {
        CopyAnimCharacterTransformToRagdoll(charObj.transform, ragdollObj.transform);
        StartCoroutine(ActiveSet());
        ragdollObj.gameObject.SetActive(true);
    }

    public void ChageExpBody()
    {
        StartCoroutine(ExpActiveSet());
    }

    IEnumerator ActiveSet()
    {
        charObj.gameObject.SetActive(false);
        StartCoroutine(ShowBlood());
        yield return new WaitForSeconds(4.0f);
        enemy.Set();
        ragdollObj.gameObject.SetActive(false);
        charObj.gameObject.SetActive(true);

    }

    IEnumerator ExpActiveSet()
    {
        charObj.gameObject.SetActive(false);
        StartCoroutine(ShowBlood());
        yield return new WaitForSeconds(4.0f);
        enemy.Set();
        ExpBody.gameObject.SetActive(false);
        charObj.gameObject.SetActive(true);
    }

    IEnumerator WaitExp()
    {
        ExpBody.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        ExpBody.SetActive(false);
    }
    IEnumerator ShowBlood()
    {
        Blood.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        Blood.SetActive(false);
    }
    private void CopyAnimCharacterTransformToRagdoll(Transform origin, Transform rag)
    {
        for (int i = 0; i < origin.transform.childCount; i++)
        {
            if (origin.transform.childCount != 0)
            {
                CopyAnimCharacterTransformToRagdoll(origin.transform.GetChild(i), rag.transform.GetChild(i));
            }
            rag.transform.GetChild(i).localPosition = origin.transform.GetChild(i).localPosition;
            rag.transform.GetChild(i).localRotation = origin.transform.GetChild(i).localRotation;
        }
    }

}
