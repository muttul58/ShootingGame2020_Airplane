using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public float speed;
    public float maxSpawnTime;
    public float curSpawnTime;
    public static int gameScore;

    public bool isGameOver;
    public static bool isGameClear;
    public bool isBoosPlay;

    public GameObject player;
    public GameObject gameOverSet;
    public GameObject gameStart;
    public GameObject gameClear;

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
        if (isGameClear && gameClear.activeSelf == false) Invoke("GameClear", 20f);
    }

    void EnemySpawn()
    {
        if (isGameOver)
            return;

        if (!player.activeSelf) // 플레이어가 꺼져있으면, 죽어있으면
            return;

        if (curSpawnTime < maxSpawnTime)
            return;

        if(gameScore <= 1000)
        {
            maxSpawnTime = Random.Range(1.2f, 2.0f);
        
            int ran = Random.Range(0, 10);
            int index;

            if (ran < 7) index = 2;
            else if (ran < 9) index = 1;
            else index = 0;

            Vector3 ops = new Vector3(Random.Range(-4f, 4f), 5, 0);
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
        Vector3 ops = new Vector3(0, 4.5f, 0);
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
        gameStart.SetActive(false);
        GameSetting("start");
    }

    public void GameReStart()
    {
        if (isGameClear)
        {
            isGameClear = false;
            gameClear.SetActive(false);
            Time.timeScale = 1; 
        }

        SceneManager.LoadScene(0);
    }


    public void GameClear()
    {
        gameClear.SetActive(true);
        Time.timeScale = 0;
    }

    // 게임 이어하기
    public void GameContinue()
    {
        GameSetting("continue");
    }

    // 게임 시작 초기 설정
    void GameSetting(string type)
    {

        Player.life = 3;
        Player.power = 1;
        Player.isPlayerDead = false;
        isGameOver = false;
        if(type == "start") gameScore = 0;
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

    // 게임 끄기
    public void GameQuit()
    {
        Application.Quit();
    }

}
