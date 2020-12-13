using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public float speed;
    public float maxSpawnTime;
    public float curSpawnTime;
    public static int gameScore;

    public bool isGameOver;

    public GameObject player;
    public GameObject gameOverSet;
    public GameObject gmaeStart;

    public Image[] lifeImages;
    public Text gameScoreText;

    public ObjectManager objectManager;

    void Update()
    {
        gameScoreText.text = string.Format("{0:n0}", gameScore);
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
    
    // 적 생성 시간 계산용
    void ReSpawn()
    {
        curSpawnTime += Time.deltaTime;
    }

    public void PlayerLifeSet(int life)
    {
        Debug.Log("Life 처리");
        for (int i = 0; i< 3; i++)
            lifeImages[i].color = new Color(1, 1, 1, 0);

        for (int i = 0; i < life; i++)
            lifeImages[i].color = new Color(1, 1, 1, 1);
    }

    // 게임 점수 계산
    public static void GameScoreUp(int score)
    {
        gameScore += score;
    }

    public void GameStart()
    {

    }


    public void GameOver()
    {
        isGameOver = true;
        gameOverSet.SetActive(true);
    }



}
