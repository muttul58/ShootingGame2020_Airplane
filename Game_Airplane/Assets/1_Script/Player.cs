using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed;                 // 플레이어 이동 속도
    public float bulletSpeed;           // 총알 이동 속도

    public static int power;            // 총알 업그레이드 1,2,3
    public static int life;             // 플레이어 생명 갯수
    public static int boomCount;        // 폭탄 갯 수
    public int bulletType;              // 1: 직사형,  2: 방사형

    public float maxBulletShootTime;    // 총알 발사 시간 설정
    public float curBulletShootTime;    // 현재 총알 발사 시간 누적
    
    public float maxPowerPoint;         // 총알 업그래이드 되는 값 설정
    public float curPowerPoint;         // 플레이어 Power를 Up 하기위한 
                                        // 점수 2000이상이면 Power +1

    public bool isTouchTop;             // 화면 외곽 콜라이더 설정
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    public static bool isPlayerDead;    // 플레이어 죽음 설정
    public bool isBoom;                 // 폭탄 사용 설정
    public bool isShield;               // 쉴드 사용 설정

    public bool isPetL;                 // 왼쪽   펫 사용 확인
    public bool isPetR;                 // 오른쪽 펫 사용 확인

    public GameObject shield;           // 쉴드 오프젝트
    public GameObject laser;            // 레이저 오브젝트 가져오기
    public Laser laserCode;             // 레이지 스크립트 가져오기

    public Image powerGauge;            // Power 게이지 

    public GameManager gameManager;     // 게임 메니저 가져오기
    public ObjectManager objectManager; // 오브젝트 메니저 가져오기
    public BackGroundSound backGroundSound; // 백그라운드 싸운드 가져오기

    public GameObject petLObj;          // 왼쪽   펫 오브젝트
    public GameObject petRObj;          // 오른쪽 펫 오브젝트

    void Start()
    {
        speed = 3;                      // 플레이어 스피드 3 설정
        power = 1;                      // 파워 1 설정
        life = 3;                       // 생명 3 설정
        boomCount=0;                    // 폭탄 0 설정
        bulletType = 1;                 // 총알 1:직선형, 2:방사형 설정
        isPlayerDead = true;            // 플레이어 죽지 않음 설정
        maxPowerPoint = 2000f;          // 플레이어 총알 Up 점수 2000 설정
    }

    void Update()
    {
        PowerCoolTime();    // 파워 게이지 업데이트
        PlayerMove();       // 플레이어 이동
        BulletShoot();      // 플레이어 총알 발사
        Reload();           // 총알 리노드
        HotKey();           // 단축키 설정
    }

    // 단축키 설정
    void HotKey()
    {
        if      (Input.GetKeyDown(KeyCode.F1)) bulletType = 1;  // 총알 타입 방사형
        else if (Input.GetKeyDown(KeyCode.F2)) bulletType = 2;  // 총알 타입 직선형
        else if (Input.GetKeyDown(KeyCode.F3)) LaserFull();     // 레이저 풀 충전
        else if (Input.GetKeyDown(KeyCode.F5)) power = 1;       // 총알 power 1
        else if (Input.GetKeyDown(KeyCode.F6)) power = 2;       // 총알 power 2
        else if (Input.GetKeyDown(KeyCode.F7)) power = 3;       // 총알 power 3
        else if (Input.GetKeyDown(KeyCode.F9)) ShieldShow();    // 쉴드 사용
        else if (Input.GetKeyDown(KeyCode.F10)) backGroundSound.BGSoundOnOff(); // 배경음악 On / Off
        else if (Input.GetKeyDown(KeyCode.F12)) GameManager.GameScoreUp(1000000); // 보스 생성을 위해 점수 설정
        else if (Input.GetKeyDown(KeyCode.L)) laserCode.LaserShoot();   // 레이저 발사
        else if (Input.GetKeyDown(KeyCode.S)) ShieldHotKey();   // 쉴드 사용        
        else if (Input.GetKeyDown(KeyCode.P)) PetCreate();      // 펫 사용
        else if (Input.GetKeyDown(KeyCode.B)) if (boomCount > 0) BoomShow();    // 폭탄 사용
        else if (Input.GetKeyDown(KeyCode.Escape)) gameManager.ShowGameInfo();  // 게임 정보 표시
    }

    // 플레이어 이동
    void PlayerMove()
    {
        // 좌우 방향키 사용, 화면 외곽에 다으면 이동 못함.
        float h = Input.GetAxisRaw("Horizontal");
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1))
            h = 0;

        // 상하 방향키 사용
        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1))
            v = 0;

        // 플레이어 현재 위치
        Vector3 curPos = transform.position;
        // 이동할 위치 현재 위치에서 방향 만큼 speed 속도로 이동, 1초 동안 이동 거리 동일하게 유지
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime; 
        // 현재 위치 = 현재 위치 + 이동할 위치
        transform.position = curPos + nextPos;
    }

    // 플레이어가 외곽 경계에 다으면
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 외곽 경계의 tag 가 "BorderPlayer" 이면
        if (collision.gameObject.tag == "BorderPlayer")
        {
            switch (collision.gameObject.name)  // 경계의 이름이 
            {                                   // 상, 하, 좌, 우
                case "Top":                     // 다았으면 true 설정
                    isTouchTop = true;
                    break;

                case "Bottom":
                    isTouchBottom = true;
                    break;

                case "Right":
                    isTouchRight = true;
                    break;

                case "Left":
                    isTouchLeft = true;
                    break;
            }
        }

        // Shield가 켜진 상태로 적과 총알에 맞은 경우
        else if (isShield &&  collision.gameObject.tag == "EnemyBullet")
        {
            Destroy(collision.gameObject);
            //ScoreUp(100);       // 점수 계산
        }

        // 플레이어 죽음 : Shield가 꺼진 상태로 적과 총알에 맞은 경우
        else if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            laserCode.curLaserCoolTime = 0;  // 레이저 게이지 0 초기화
            laserCode.isLaserShoot = false;  // 레이저 발사의 경우 취소
            curPowerPoint = 0;               // Power 업그레드 점수 0 초기화
            isPlayerDead = true;             // Player 사망

            laser.GetComponent<Laser>().LaserHide();

            if (petLObj.gameObject.activeSelf == true)      // 펫 숨기기
                petLObj.gameObject.SetActive(false);
            if (petRObj.gameObject.activeSelf == true)      // 펫 보이기
                petRObj.gameObject.SetActive(false);

            gameObject.SetActive(false);                // 플레이서 숨기기

            objectManager.deadPlayerSound.Play();       // 플레이어 죽음 효과음
            // 폭파 이팩트 생성
            GameObject eff = Instantiate(objectManager.deadPlayerEffect, transform.position, transform.rotation);
            Destroy(eff, 1.5f);         // 폭파 이팩트 소멸

            if (power > 1) power--;     // Power 1 감소

            BulletDestroy();            // 생성된 적과 적의 총알 모두 소멸

            life--;                     // 생명 -1
            gameManager.PlayerLifeSet(life);    // 화면 왼쪽 위 생명 UI 설정

            if (life <= 0)              // 생명이 0 이하이면 게임 종료
                gameManager.GameOver();
            else
                Invoke("ReloadPlayer", 2f);     // 그렇지 않으면 2초 후 플레이어 다시 타나남

        }

        // Life(생명) 아이템을 먹은 경우
        else if (collision.gameObject.tag == "ItemLife")
        {
            life++;                             // 생명 +1
            objectManager.itmeLifeSound.Play(); // 효과음 재생
            Destroy(collision.gameObject);      // 생명 아이템 소멸

            if (life > 3)                       // 생명 3보다 크면
            {
                life = 3;                       // 생명 3으로 설정
                ScoreUp(500);                   // 점수 +500
            }
            else                                // 생명 3보다 작으면
            {
                gameManager.PlayerLifeSet(life);   // 화면 왼쪽 위 생명 UI이 수정 
            }
        }

        // Boom(폭탄) 아이템을 먹은 경우
        else if (collision.gameObject.tag == "ItemBoom")
        {
            boomCount++;            // 폭탄 +1
            objectManager.itmeShieldSound.Play();   // 효과음 재생
            Destroy(collision.gameObject);          // 폭탄 아이템 소멸

            if (boomCount > 3)      // 폭탄이 3 이상이면
            {
                boomCount = 3;      // 폭탄 3 설정
                ScoreUp(500);       // 점수 +500
            }
            else                    // 아닌면
            {   // 화면 왼쪽 위 폭탄 UI 수정
                gameManager.PlayerBoomSet(boomCount);
            }
        }

        // Pet(펫) 아이템을 먹은 경우
        else if (collision.gameObject.tag == "ItemPet")
        {
            // 왼쪽 펫이 없으면 나타남
            if (petLObj.gameObject.activeSelf == false)
                petLObj.gameObject.SetActive(true);
            // 왼쪽 펫이 있고, 오른쪽 펫이 없으면 나타남
            else if (petRObj.gameObject.activeSelf == false)
                petRObj.gameObject.SetActive(true);
            // 둘다 있으면 점수 +500
            else GameManager.GameScoreUp(500);

            // 펫 효과음 실행
            objectManager.itmePowerSound.Play();
            // 펫 아이템 소멸
            Destroy(collision.gameObject);
        }

        // Shield(쉴드) 아이템을 먹은 경우
        else if (collision.gameObject.tag == "ItemShield")
        {
            objectManager.itmeShieldSound.Play(); // 효과음 재생
            Destroy(collision.gameObject);

            if (isShield)           // 이미 쉴드 사용중이면
                ScoreUp(500);       // 점수 +500
            else                    // 쉴드 사용하지 않으면
                ShieldShow();       // 쉴드 나타남
        }


        // Coin(코인) 아이템을 먹은 경우
        else if (collision.gameObject.tag == "ItemCoin")
        {
            objectManager.itmePowerSound.Play();    // 효과음 재생
            Destroy(collision.gameObject);          // 코인 아이템 소멸

            ScoreUp(500);                           // 점수 +500
        }

    }

    // 플레이어가 화면 외곽에서 떯어지면
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderPlayer")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;

                case "Bottom":
                    isTouchBottom = false;
                    break;

                case "Right":
                    isTouchRight = false;
                    break;

                case "Left":
                    isTouchLeft = false;
                    break;
            }
        }
    }


    // 총알 발사
    void BulletShoot()
    {
        // 레이저 발사 이면 총알 발사 안됨
        if (laserCode.isLaserShoot)
            return;

        // 스페이스 키 누르면 총알 발사
        if (!Input.GetKey("space"))
            return;
        
        // 총알 발사 시간 설정보다 작으면 발사 안됨.
        if (curBulletShootTime < maxBulletShootTime)
            return;

        // 총알타입(bulletType)  1: 직선 발사, 2: 점점 퍼지게 발사
        // 파워(power) 1, 2, 3에 따라 총알 형태와 속도 설정
        if (bulletType == 1)
        {

            bulletSpeed = 1.0f + (0.5f * power);
            maxBulletShootTime = 0.6f - (0.1f * power);
        }
        else
        {
            bulletSpeed = 2 + power;
            maxBulletShootTime = 0.5f - (0.1f * power);
        }

        // 플레이어의 총알 발사 소리 재생
        objectManager.bulletShootSound.Play();
        
        if (bulletType == 1 && power == 1)  // 직선 1발
        {
            // 총알 생성                       
            GameObject bullet_01 = Instantiate( objectManager.playerBulletObjB          // objectManager 에있는 playerBulletObjB 총알 사용
                                              , transform.position + Vector3.up * 0.7f  // 플레이어 위치에서 위쪽으로 0.7 만큼 위에
                                              , transform.rotation);                    // 회전 없이
            Rigidbody2D rigid_01 = bullet_01.GetComponent<Rigidbody2D>();               // 중력 적용
            rigid_01.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);           // 위쪽으로 bulletSpeed 만큼 속도록 이동

        }
        else if (bulletType == 2 && power == 1)  // 좌, 중, 우 각 1발 퍼지면서 이동
        {
            GameObject bullet_01R = Instantiate(objectManager.playerBulletObjA, transform.position + Vector3.up * 0.7f, transform.rotation);
            GameObject bullet_01C = Instantiate(objectManager.playerBulletObjA, transform.position + Vector3.up * 0.7f, transform.rotation);
            GameObject bullet_01L = Instantiate(objectManager.playerBulletObjA, transform.position + Vector3.up * 0.7f, transform.rotation);
            Rigidbody2D rigid_01R = bullet_01R.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_01C = bullet_01C.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_01L = bullet_01L.GetComponent<Rigidbody2D>();

                                         //  new Vector3(-0.2f, 0, 0)) 총알이 방사형으로 퍼지기 위해 위쪽 위치 X좌표로 -0.2 만큼변경
            rigid_01R.AddForce((Vector3.up + new Vector3(-0.2f, 0, 0)) * bulletSpeed, ForceMode2D.Impulse);
            rigid_01C.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);
            rigid_01L.AddForce((Vector3.up + new Vector3(0.2f, 0, 0)) * bulletSpeed, ForceMode2D.Impulse);

            // 오른쪽 왼쪽 사선으로 나가는 총알 회전
            Vector3 rotVec_01R = Vector3.forward * 12f ;
            Vector3 rotVec_01L = Vector3.forward * -12f ;
            bullet_01R.transform.Rotate(rotVec_01R);
            bullet_01L.transform.Rotate(rotVec_01L);

        }
        else if (bulletType == 1 && power == 2)
        {
            GameObject bullet_02R = Instantiate(objectManager.playerBulletObjB, transform.position + Vector3.up * 0.7f + Vector3.right * -0.2f, transform.rotation);
            GameObject bullet_02L = Instantiate(objectManager.playerBulletObjB, transform.position + Vector3.up * 0.7f + Vector3.right * 0.2f, transform.rotation);
            Rigidbody2D rigid_02R = bullet_02R.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_02L = bullet_02L.GetComponent<Rigidbody2D>();
            rigid_02R.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);
            rigid_02L.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);
        }
        else if (bulletType == 2 && power == 2)
        {
            GameObject bullet_02R = Instantiate(objectManager.playerBulletObjA, transform.position + Vector3.up * 0.7f, transform.rotation);
            GameObject bullet_02C = Instantiate(objectManager.playerBulletObjB, transform.position + Vector3.up * 0.7f, transform.rotation);
            GameObject bullet_02L = Instantiate(objectManager.playerBulletObjA, transform.position + Vector3.up * 0.7f, transform.rotation);
            Rigidbody2D rigid_02R = bullet_02R.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_02C = bullet_02C.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_02L = bullet_02L.GetComponent<Rigidbody2D>();
            rigid_02R.AddForce((Vector3.up + new Vector3(-0.2f, 0, 0)) * bulletSpeed, ForceMode2D.Impulse);
            rigid_02C.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);
            rigid_02L.AddForce((Vector3.up + new Vector3(0.2f, 0, 0)) * bulletSpeed, ForceMode2D.Impulse);
            
            // 오른쪽 왼쪽 사선으로 나가는 총알 회전
            Vector3 rotVec_02R = Vector3.forward * 12f;
            Vector3 rotVec_02L = Vector3.forward * -12f;
            bullet_02R.transform.Rotate(rotVec_02R);
            bullet_02L.transform.Rotate(rotVec_02L);
        }
        else if (bulletType == 1 && power == 3)
        {
            GameObject bullet_03RR = Instantiate(objectManager.playerBulletObjA, transform.position + Vector3.up * 0.7f + Vector3.right * -0.5f, transform.rotation);
            GameObject bullet_03R  = Instantiate(objectManager.playerBulletObjB, transform.position + Vector3.up * 0.7f + Vector3.right * -0.2f, transform.rotation);
            GameObject bullet_03L  = Instantiate(objectManager.playerBulletObjB, transform.position + Vector3.up * 0.7f + Vector3.right *  0.2f, transform.rotation);
            GameObject bullet_03LL = Instantiate(objectManager.playerBulletObjA, transform.position + Vector3.up * 0.7f + Vector3.right *  0.5f, transform.rotation);
            Rigidbody2D rigid_03RR = bullet_03RR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_03R  = bullet_03R.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_03L  = bullet_03L.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_03LL = bullet_03LL.GetComponent<Rigidbody2D>();
            rigid_03RR.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);
            rigid_03R.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);
            rigid_03L.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);
            rigid_03LL.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);
        }
        else if (bulletType == 2 && power == 3)
        {
            GameObject bullet_03R  = Instantiate(objectManager.playerBulletObjA, transform.position + Vector3.up * 0.7f, transform.rotation);
            GameObject bullet_03CC = Instantiate(objectManager.playerBulletObjB, transform.position + Vector3.up * 0.7f + Vector3.right * -0.2f, transform.rotation);
            GameObject bullet_03C  = Instantiate(objectManager.playerBulletObjB, transform.position + Vector3.up * 0.7f + Vector3.right *  0.2f, transform.rotation);
            GameObject bullet_03L  = Instantiate(objectManager.playerBulletObjA, transform.position + Vector3.up * 0.7f, transform.rotation);
            Rigidbody2D rigid_03R  = bullet_03R.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_03CC = bullet_03CC.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_03C  = bullet_03C.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_03L  = bullet_03L.GetComponent<Rigidbody2D>();
            rigid_03R .AddForce((Vector3.up + new Vector3(-0.2f, 0, 0)) * bulletSpeed, ForceMode2D.Impulse);
            rigid_03CC.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);
            rigid_03C .AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);
            rigid_03L .AddForce((Vector3.up + new Vector3(0.2f, 0, 0)) * bulletSpeed, ForceMode2D.Impulse);

            // 오른쪽 왼쪽 사선으로 나가는 총알 회전
            Vector3 rotVec_03R = Vector3.forward * 12f;
            Vector3 rotVec_03L = Vector3.forward * -12f;
            bullet_03R.transform.Rotate(rotVec_03R);
            bullet_03L.transform.Rotate(rotVec_03L);
        }

        // 총알 발사 후 장전으로 현재 시간 0으로 초기화
        curBulletShootTime = 0;
    }

    // 총알 발사 시간 계산
    void Reload()
    {
        curBulletShootTime += Time.deltaTime;
    }


    // 단축키 'B'로 폭탄 켜고/끄기
    void BoomShow()
    {
        if(boomCount > 0)       // 폭탄이 1개 이상이면
        {
            boomCount--;        // 폭탄 -1
            gameManager.PlayerBoomSet(boomCount);   // 화면 왼쪽 위 폭탄 UI 수정
            objectManager.boomPlayerSound.Play();   // 효과음 재생
            // 게임 오프젝트에 있는 폭탄 이팩트 생성
            GameObject boomEff = Instantiate(objectManager.boomEffect, transform.position + Vector3.up * 4f, transform.rotation);
            Destroy(boomEff, 1.5f);                 // 1.5초 후 소멸
            BulletDestroy();    // 생성된 적과 적의 총알 모두 소멸
        }

    }

    void BulletDestroy()
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

    // 단축키 'S'로 쉴드 켜고/끄기
    void ShieldHotKey()
    {
        if (isShield == false)
        {
            isShield = true;
            shield.SetActive(true);
        }
        else
        {
            isShield = false;
            shield.SetActive(false);
        }
    }

    // 쉴드 아이템을 먹은 경우
    void ShieldShow()
    {
        shield.SetActive(true);
        isShield = true;
        if (isShield==true) Invoke("ShieldHide", 5f);
    }
    void ShieldHide()
    {
        isShield = false;
        shield.SetActive(false);
    }

    // Power 게이지 max 값, value 값 초기화
    void PowerCoolTime()
    {
        if (isPlayerDead == true)
        {
            curPowerPoint = 0;
            powerGauge.fillAmount = 1f;
        }
        // 파워 게이지 = 현재 PowerPoint 점수 / 최고 PowerPoint 점수
        powerGauge.fillAmount = curPowerPoint / maxPowerPoint;
    }

    //-0 플레이어 총알 업그레이드용 
    public void PowerUpPoint(int enemyScore)
    {
        if (power < 3)  // power이 3보다 작은 경우
        {
            // 적의 점수 누적
            curPowerPoint += (float)enemyScore;

            // PowerPoint가 2000 이상이면 Power 2
            if (curPowerPoint > 2000 && power == 1)
            {
                power++;                    // 파워 +1
                curPowerPoint = 0;          // 현재 파워포인트 0 초기화
                maxPowerPoint = 5000;       // 다음 파워 업 포인트 5000 설정
                powerGauge.fillAmount = 0;  // 파워 게이지 0 초기화
            }
            // PowerPoint가 5000 이상이면 Power 3
            else if (curPowerPoint > 5000 && power == 2)
            {
                power++;                    // 파워 +1
                curPowerPoint = 0;          // 현재 파워포인트 0 초기화
                powerGauge.fillAmount = 0;  // 파워 게이지 0 초기화
            }                               
        }
    }



    // 플레이어 다시 나타나게 하기
    void ReloadPlayer()
    {
        transform.position = new Vector3(0, -4, 0);     // 나타날 위치 설정
        gameObject.SetActive(true);                     // 나타나기
        isPlayerDead = false;                           // 플레이어 죽지 않은 상태로 설정
    }

    // 게임 점수, Power 점수 계산
    void ScoreUp(int score)
    {
        GameManager.gameScore += score;     // 게임 점수 누적
        PowerUpPoint(score);                // 플레이어 총알 업그레이드 용 점수
    }

    // 레이저 풀 충전
    void LaserFull()
    {
        // 레이저 최고 시간을 0.1초로 설정
        Laser LaserCode = GameObject.Find("Laser").GetComponent<Laser>();
        LaserCode.maxLaserCoolTime = 0.1f; 
    }

    // HotKey "P" 펫 생성 
    void PetCreate()
    {
        // HotKey로 Pet(펫) 생성

        if (petLObj.gameObject.activeSelf == false)
            petLObj.gameObject.SetActive(true);
        else if (petRObj.gameObject.activeSelf == false)
            petRObj.gameObject.SetActive(true);

        objectManager.itmePowerSound.Play();
    }


}
