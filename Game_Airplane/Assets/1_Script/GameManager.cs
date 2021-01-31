using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float maxSpawnTime;      // 적 생성 시간 설정
    public float curSpawnTime;      // 현재 적 생성 시간
    public float bossStagePoint;    // 보스가 나타나는 게임 점수

    public static int gameScore;    // 게임 점수

    public bool isGameOver;         // 게임 종료 확인(플레이가 모두 죽음)
    public static bool isGameClear; // 게임 클리어 확인(보스 죽임)
    public bool isBossPlay;         // 현제 
    
    public GameObject player;       
    public GameObject gameOverSet;  // 게임 종료시 나타나는 UI
    public GameObject gameStart;    // 게임 시작시 나타나는 UI
    public GameObject gameClear;    // 게임 클리어시 타나타는 UI
    public GameObject gameInfo;     // 게임 정보 타나타는 UI
    public Slider gameProgressBor;  // 보스가 나타나기까지 게임 진행상황 게이지

    public Image[] lifeImages;      // 화면 위 왼쪽 플레이어 생명 이미지
    public Image[] boomImages;      // 화면 위 오른쪽  폭탄 갯수 이미지
    public Text gameScoreText;      // 화면 위 중간의 게임 점수 텍스트

    public ObjectManager objectManager; // 오브젝트 메니저 가져오기
    public Laser laserCode;             // 레이저코드 가저오게


    private void Start()
    {
        bossStagePoint = 10000f;             // 소스가 나타나는 게임 점수
        
        gameProgressBor.value = gameScore;   // 게임 진행 게이지 초기화
    }

    void Update()
    {
        // 게임 점수 표시
        GameScoreShow();
        
        // 일반 적(보스 빼고)
        if (!isBossPlay)
        {
            EnemySpawn();  // 적 생성
            ReSpawn();     // 적 생성 시간 누적
        }

        // 게임 클리어시 UI 표시
        if (isGameClear && gameClear.activeSelf == false) Invoke("GameClear", 10f);

        // 게임 정보 On / Off
        ShowGameInfo();

        // 게임 진행 게이지 업데이트
        gameProgressBor.value = gameScore / bossStagePoint;
    }

    // 적 생성
    void EnemySpawn()
    {
        if (isGameOver)                     // 게임 종료이면
            return;

        if (!player.activeSelf)             // 플레이어가 꺼져있으면, 죽어있으면
            return;

        if (curSpawnTime < maxSpawnTime)    // 적 생성 시간인 안됐으면
            return;

        if(gameScore <= bossStagePoint)     // bossStagePoint 이하이면 적 L, M , S 생성
        {
            maxSpawnTime = Random.Range(1.2f, 2.0f);    // 적 생성시간 랜덤하게 설정
        
            int ran = Random.Range(0, 10);  
            int index;

            if (ran < 7) index = 2;         // EnemyS 생성
            else if (ran < 9) index = 1;    // EnemyM 생성
            else index = 0;                 // EnemyL 생성

            Vector3 ops = new Vector3(Random.Range(-4f, 4f), 5, 0);  // 적 생성 위치 설정

            // 오브젝트 메니저에 있는 적 선택 후 생성
            Instantiate(objectManager.enemyObj[index], ops, transform.rotation);    
            
            curSpawnTime = 0;   // 생성 누적 시간 초기화
        }
        else  // bossStagePoint 이상이면 보스 생성
        {
            isBossPlay = true;              // 보스 생성을 참으로 설정
            EnemyDestroy();                 // 적과 총알 소멸
            Invoke("EnemyBossSpawn", 2f);   // 보스 생성
        }
    }

    // 보스 생성전 모든 적과 총알 소멸
    void EnemyDestroy()
    {
        // 생성된 적 모두 소멸(Tag 가 "Enemy" 찾아서 저장)
        GameObject[] enemyDes = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemyDes.Length; i++)   // 찾은 적 소멸
            Destroy(enemyDes[i]);

        // 생성된 적의 총알 모두 소멸(Tag 가 "EnemyBullet" 찾아서 저장)
        GameObject[] enemyBulletDes = GameObject.FindGameObjectsWithTag("EnemyBullet");
        for (int i = 0; i < enemyBulletDes.Length; i++)  // 찾은 적총알 소멸
            Destroy(enemyBulletDes[i]);
    }
    
    // 보스 생성
    void EnemyBossSpawn()
    {
        Vector3 ops = new Vector3(0, 4.5f, 0);  // 보스 생성위치
        // 오브젝트 메니저에 있는 보스 선택 후 생성
        Instantiate(objectManager.enemyObj[3], ops, transform.rotation);
    }

    // 적 생성 시간 누적 
    void ReSpawn()
    {
        curSpawnTime += Time.deltaTime;
    }

    // 왼쪽 위 플레이어 생명 표시(플레이어가 Life 아이템 획득 또는 죽으면 업데이트)
    public void PlayerLifeSet(int life)
    {
        // 모든 이미지의 알파값을 0으로 만들어 안보이게 함
        for (int i = 0; i< 3; i++)
            lifeImages[i].color = new Color(1, 1, 1, 0);

        // life 만큼 이미지의 알파값을 1으로 만들어 보이게 함
        for (int i = 0; i < life; i++) 
            lifeImages[i].color = new Color(1, 1, 1, 1);
    }

    // 오른쪽 위 폭탄 표시(플레이어가 Boom 아이템을 획득 또는 사용하면 업데이트)
    public void PlayerBoomSet(int boom)
    {
        // 모든 이미지의 알파값을 0으로 만들어 안보이게 함
        for (int i = 0; i < 3; i++)
            boomImages[i].color = new Color(1, 1, 1, 0);

        // boom 만큼 이미지의 알파값을 1으로 만들어 안보이게 함
        for (int i = 0; i < boom; i++)
            boomImages[i].color = new Color(1, 1, 1, 1);
    }

    // 게임 점수 계산
    public static void GameScoreUp(int score)
    {
        // 게임 점수 계산
        gameScore += score;
    }

    // 게임 시작 또는 다시 시작 UI 표시
    public void GameStart()
    {
        gameStart.SetActive(false);
        GameSetting("start");
    }

    // 게임 종료 또는 클리어 후 다시 시작하기
    public void GameReStart()
    {
        // 씬 0 실행
        SceneManager.LoadScene(0);

        if (isGameClear)  // 게임 클리어 이면
        {
            isGameClear = false;
            gameClear.SetActive(false);
            Time.timeScale = 1; 
        }
    }

    // 게임 클리어 UI 표시
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
    // 전달 받은 type : start-처음부터 다시시작, continue - 이어하기
    void GameSetting(string type)
    {

        Player.life = 3;                        // 플레이어 Life 3 으로 설정
        Player.power = 1;                       // 총알 power 1 로 설정
        Player.isPlayerDead = false;            // 플레이어 죽은 상태 아님 설정
        isGameOver = false;                     // 게임 오버 아님 설정
        isGameClear = false;                    // 게임 클리어 아님 설정
        if (type == "start") gameScore = 0;     // 시작 타입이 "start"이면 씬 0 으로 시작
        gameStart.SetActive(false);             // gameStart UI 숨기기
        PlayerLifeSet(3);                       // 플레이어 Life UI 3으로 설정

        // 플레이어 시작위치에서 나타나기
        player.transform.position = new Vector3(0, -4, 0);
        player.SetActive(true);

        // gameOverSet UI On / Off
        if (gameOverSet.activeSelf == true)
            gameOverSet.SetActive(false);
    }

    // 게임 종료 화면 표시
    public void GameOver()
    {
        isGameOver = true;
        gameOverSet.SetActive(true);
    }

    // 게임 끄기(게임 창 닫기)
    public void GameQuit()
    {
        Application.Quit();
    }

    // 게임 정보 On / Off (단축키, 기능 설명) 
    public void ShowGameInfo()
    {
        // 게임 정보 On / Off
        if (Input.GetButtonDown("Cancel"))
        {
            if (gameInfo.activeSelf)
            {
                Time.timeScale = 1;  // 게임 진행 
                gameInfo.SetActive(false);
            }
            else
            {
                Time.timeScale = 0;  // 게임 일시 중지
                gameInfo.SetActive(true);
            }
        }
    }

    // 게임 점수 표시
    public void GameScoreShow()
    {
        if (!isGameClear)  // 게임 클리어가 아니면 위쪽 중앙에 표시
        {
            gameScoreText.transform.position = new Vector2(400f, 1000f);    // 텍스트 위치 설정 
            gameScoreText.transform.localScale = new Vector2(1f, 1f);       // 텍스트 크기 설정
            gameScoreText.text = string.Format("{0:n0}", gameScore);        // 출력 형식 설정
        }
        else
        {
            gameScoreText.transform.position = new Vector2(400f, 650f);
            gameScoreText.transform.localScale = new Vector2(2f, 2f);
            gameScoreText.text = string.Format("{0:n0}", gameScore);
        }
    }
}
