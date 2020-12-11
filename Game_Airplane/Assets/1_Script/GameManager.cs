using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public float speed;
    public float maxSpawnTime;
    public float curSpawnTime;
    public static int gameScore;


    public ObjectManager objectManager;


    void Start()
    {
        
    }

    void Update()
    {
        EnemySpawn();
        ReSpawn();
    }

    void EnemySpawn()
    {
        if (curSpawnTime < maxSpawnTime)
            return;

        maxSpawnTime = Random.Range(1.5f, 3.0f);
        
        int ran = Random.Range(0, 10);
        int index;

        if (ran < 2) return;
        else if (ran < 4) index = 0;
        else if (ran < 7) index = 1;
        else index = 2;

        Vector3 ops = new Vector3(Random.Range(-5.5f, 5.5f), 5, 0);
        Instantiate(objectManager.enemyObj[index], ops, transform.rotation);

        curSpawnTime = 0;
    }

    void ReSpawn()
    {
        curSpawnTime += Time.deltaTime;
    }
}
