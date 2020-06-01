using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    public WaponType WP;
    public GameObject[] wapon;
    public GameObject[] enemy;
    public GameObject[] tankenemy;
    public GameObject Manger;
    public Transform[] Spawn;
    public List<GameObject> EnemyPool;
    public float CreateTime;
    public float MaxEnemy;    //몇마리로 설정할것인가
    public bool GameOver;
    public int itemSet;
    public int itemSet2;
    public int WaponSet;
    public int WaponSet2;
    public int TankEnemyCount;
    public int Count = 0;
    bool PoolOff;
   
   
    void Start()
    {
        int[] array = new int[TankEnemyCount];
        for (int i = 0; i < array.Length; i++)
        {
            int EnemyNumber = Random.Range(0, (int)MaxEnemy);
            array[i] = EnemyNumber;
        }
        
        if (Spawn.Length > 0)
        {
            EnemyPool = new List<GameObject>();
            if (PoolOff==false)
            {
                for (int i = 0; i < MaxEnemy; i++)
                {
                    int NormalEnemyType = Random.Range(0, enemy.Length);
                    GameObject Enemy = Instantiate(enemy[NormalEnemyType]);
                    Enemy.transform.parent = Manger.transform;
                    Enemy.SetActive(false);
                    EnemyPool.Add(Enemy);
                }
                for (int a = 0; a < TankEnemyCount; a++)
                {
                    int EnemyType = Random.Range(0, tankenemy.Length);
                    GameObject TankEnemy = Instantiate(tankenemy[EnemyType]);
                    TankEnemy.transform.parent = Manger.transform;
                    TankEnemy.SetActive(false);
                    EnemyPool[array[a]] = TankEnemy;
                   
                }
                StartCoroutine(CreateMonster());
                //PoolOff = true;
            }
        }

    }

    public void SetOff(int TankZombieNum)
    {
        TankEnemyCount = TankZombieNum;
        gameObject.SetActive(false);
    }
   public void CreateItem()
    {
        while(true)
        {
            WaponSet = Random.Range(0, wapon.Length);
            WaponSet2 = Random.Range(0, wapon.Length);
            itemSet = Random.Range(1, (int)MaxEnemy);
            itemSet2 = Random.Range(1, (int)MaxEnemy);
            if(itemSet!=itemSet2&&WaponSet!=WaponSet2)
            {
                break;
            }
        }

    }

    public IEnumerator CreateMonster()
    {
        while (!GameOver)
        {
            yield return new WaitForSeconds(CreateTime);
            int idx = Random.Range(0, Spawn.Length);

            if (EnemyPool.Count > 0)
            {
                EnemyPool[0].SetActive(true);
                EnemyPool.RemoveAt(0);
            }
        }
    }
}
