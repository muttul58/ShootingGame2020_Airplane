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
    public bool isBoosPlay;

    public GameObject player;
    public GameObject gameOverSet;
    public GameObject gameStart;

    public Image[] lifeImages;
    public Image[] boomImages;
    public Text gameScoreText;

    public ObjectManager objectManager;


    private void Start()
    {

    }

    void Update()
    {
        gameScoreText.text = string.Format("{0:n0}", gameScore);
        if (!isBoosPlay)
        {
            EnemySpawn();
            ReSpawn();
        }
    }

    void EnemySpawn()
    {
        if (isGameOver)
            return;

        if (!player.activeSelf) // 플레이어가 꺼져있으면, 죽어있으면
            return;

        if (curSpawnTime < maxSpawnTime)
            return;

        if(gameScore <= 10000)
        {
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
        else
        {
            isBoosPlay = true;
            EnemyDestroy();                 // 적과 총알 소멸
            Invoke("EnemyBossSpawn", 2f);   // 보스 생성

        }

    }

    // 보스 생성전 모든 적과 총알 소멸
    void EnemyDestroy()
    {
        // 생성된 적 모두 소멸
        GameObject[] enemyDes = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemyDes.Length; i++)
            Destroy(enemyDes[i]);

        // 생성된 적의 총알 모두 소멸
        GameObject[] enemyBulletDes = GameObject.FindGameObjectsWithTag("EnemyBullet");
        for (int i = 0; i < enemyBulletDes.Length; i++)
            Destroy(enemyBulletDes[i]);
    }
    
    // 보스 생성
    void EnemyBossSpawn()
    {
        Vector3 ops = new Vector3(0, 5.5f, 0);
        Instantiate(objectManager.enemyObj[3], ops, transform.rotation);
    }

    
    // 적 생성 시간 계산용
    void ReSpawn()
    {
        curSpawnTime += Time.deltaTime;
    }

    public void PlayerLifeSet(int life)
    {
        for (int i = 0; i< 3; i++)
            lifeImages[i].color = new Color(1, 1, 1, 0);

        for (int i = 0; i < life; i++)
            lifeImages[i].color = new Color(1, 1, 1, 1);
    }

    public void PlayerBoomSet(int boom)
    {
        for (int i = 0; i < 3; i++)
            boomImages[i].color = new Color(1, 1, 1, 0);

        for (int i = 0; i < boom; i++)
            boomImages[i].color = new Color(1, 1, 1, 1);
    }

    // 게임 점수 계산
    public static void GameScoreUp(int score)
    {
        gameScore += score;
    }

    // 게임 시작 또는 다시 시작
    public void GameStart()
    {
        gameScore = 0;
        GameSetting();
    }

    // 게임 이어하기
    public void GameContinue()
    {
        GameSetting();
    }

    // 게임 시작 초기 설정
    void GameSetting()
    {
        Player.life = 3;
        Player.power = 1;
        isGameOver = false;
        gameStart.SetActive(false);
        PlayerLifeSet(3);

        player.transform.position = new Vector3(0, -4, 0);
        player.SetActive(true);

        if (gameOverSet.activeSelf == true)
            gameOverSet.SetActive(false);
    }

    // 게임 종료 화면 표시
    public void GameOver()
    {
        isGameOver = true;
        gameOverSet.SetActive(true);
    }



}
