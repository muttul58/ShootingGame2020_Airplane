﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed;
    public float bulletSpeed;

    public static int power;            // 총알 업그레이드 1,2,3
    public static int life;
    public static int boomCount;
    public int bulletType;              // 1: 직사형,  2: 방사형

    public float maxBulletShootTime;    // 총알 나가는 간격
    public float curBulletShootTime;
    
    public float maxPowerPoint;
    public float curPowerPoint;// 플레이어 Power를 Up 하기위한 점수 2000이상이면 Power +1


    public float curLaserMoveTime;     // 레이저가 점점 길어지는 효과 시간

    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    public static bool isPlayerDead;
    public bool isBoom;
    public bool isShield;
    public bool isBGSound;

    public bool isPetL;
    public bool isPetR;

    public GameObject shield;
    public GameObject laser;        // 레이저 오브젝트
    public Laser laserCode;         // 레이지 스크립트

    public Image powerGauge;    

    public ObjectManager objectManager;
    public GameManager gameManager;

    public GameObject petLObj;
    public GameObject petRObj;

    private void Awake()
    {

    }

    void Start()
    {
        speed = 3;
        power = 1;
        life = 3;
        boomCount=0;
        bulletType = 1;
        isBGSound = true;
        isPlayerDead = true;
        maxPowerPoint = 2000f;
    }

    void Update()
    {
        PowerCoolTime();
        PlayerMove();       // 플레이어 이동
        BulletShoot();      // 플레이어 총알 발사
        Reload();           // 총알 리노드
        HotKey();
    }

    // 단축키 설정
    void HotKey()
    {
        if      (Input.GetKeyDown(KeyCode.F1)) bulletType = 1;
        else if (Input.GetKeyDown(KeyCode.F2)) bulletType = 2;
        else if (Input.GetKeyDown(KeyCode.F5)) power = 1;
        else if (Input.GetKeyDown(KeyCode.F6)) power = 2;
        else if (Input.GetKeyDown(KeyCode.F7)) power = 3;
        else if (Input.GetKeyDown(KeyCode.F9)) ShieldShow();
        else if (Input.GetKeyDown(KeyCode.F10)) BGSoundOnOff();
        else if (Input.GetKeyDown(KeyCode.F12)) GameManager.GameScoreUp(100000);
        else if (Input.GetKeyDown(KeyCode.L)) laserCode.LaserShoot();
        else if (Input.GetKeyDown(KeyCode.S)) ShieldHotKey();
        else if (Input.GetKeyDown(KeyCode.B)) if (boomCount > 0) BoomShow();
    }


    void PlayerMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1))
            h = 0;

        float v = Input.GetAxisRaw("Vertical");
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1))
            v = 0;

        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;
        transform.position = curPos + nextPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderPlayer")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
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
        // Shield가 켜진 상태로 보스에 다으면
        else if (isShield && (collision.gameObject.tag == "EnemyB"))
        {
            //Destroy(collision.gameObject);
            ScoreUp(100);       // 점수 계산
        }

        // Shield가 켜진 상태로 적과 총알에 맞은 경우
        else if (isShield && (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet"))
        {
            Destroy(collision.gameObject);
            ScoreUp(100);       // 점수 계산
        }
        // 플레이어 죽음 : Shield가 꺼진 상태로 적과 총알에 맞은 경우
        else if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            laserCode.curLaserCoolTime = 0;
            laserCode.isLaserShoot = false;
            curPowerPoint = 0;              // Power 업그레드 점수 0 초기화
            isPlayerDead = true;            // Player 사망

            laser.GetComponent<Laser>().LaserHide();

            if (petLObj.gameObject.activeSelf == true)          // 펫 숨기기
                petLObj.gameObject.SetActive(false);
            if (petRObj.gameObject.activeSelf == true)
                petRObj.gameObject.SetActive(false);

            gameObject.SetActive(false);    // 플레이서 숨기기

            objectManager.deadPlayerSound.Play();
            GameObject eff = Instantiate(objectManager.deadPlayerEffect, transform.position, transform.rotation);
            Destroy(eff, 1.5f);

            // Power 1 감소
            if (power > 1) power--;

            BulletDestroy(); // 생성된 적과 적의 총알 모두 소멸

            life--;
            gameManager.PlayerLifeSet(life);

            if (life <= 0)
                gameManager.GameOver();
            else
                Invoke("ReloadPlayer", 2f);

        }

        // Pet(펫) 아이템을 먹은 경우
        else if (collision.gameObject.tag == "ItemPet")
        {
            if (petLObj.gameObject.activeSelf == false)
                petLObj.gameObject.SetActive(true);
            else if (petRObj.gameObject.activeSelf == false)
                petRObj.gameObject.SetActive(true);
            else GameManager.GameScoreUp(500);

            objectManager.itmePowerSound.Play();
            Destroy(collision.gameObject);
        }

        // Life(생명) 아이템을 먹은 경우
        else if (collision.gameObject.tag == "ItemLife")
        {
            life++;
            objectManager.itmeLifeSound.Play();
            Destroy(collision.gameObject);

            if (life > 3)
            {
                life = 3;
                ScoreUp(500);       // 점수 계산
            }
            else
            {
                gameManager.PlayerLifeSet(life);
            }
        }
        // Shield(쉴드) 아이템을 먹은 경우
        else if (collision.gameObject.tag == "ItemShield")
        {
            objectManager.itmeShieldSound.Play();
            Destroy(collision.gameObject);

            if (isShield)
            {
                ScoreUp(500);       // 점수 계산
            }
            else
            {
                ShieldShow();
            }
        }
        // Boom(폭탄) 아이템을 먹은 경우
        else if (collision.gameObject.tag == "ItemBoom")
        {
            boomCount++;
            objectManager.itmeShieldSound.Play();
            Destroy(collision.gameObject);

            if (boomCount > 3)
            {
                boomCount = 3;
                ScoreUp(500);       // 점수 계산
            }
            else
            {
                gameManager.PlayerBoomSet(boomCount);
            }
        }
        // Coin(코인) 아이템을 먹은 경우
        else if (collision.gameObject.tag == "ItemCoin")
        {
            objectManager.itmePowerSound.Play();
            Destroy(collision.gameObject);

            ScoreUp(500);       // 점수 계산
        }

    }

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
            bulletSpeed = 2 + power;
            maxBulletShootTime = 0.5f - (0.1f * power);
        }
        else
        {
            bulletSpeed = 1.0f + (0.5f * power);
            maxBulletShootTime = 0.6f - (0.1f * power);
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
        if(boomCount > 0)
        {
            boomCount--;
            gameManager.PlayerBoomSet(boomCount);
            objectManager.boomPlayerSound.Play();
            GameObject boomEff = Instantiate(objectManager.boomEffect, transform.position + Vector3.up * 4f, transform.rotation);
            Destroy(boomEff, 1.5f);
            BulletDestroy(); // 생성된 적과 적의 총알 모두 소멸
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
        if(isShield==false) Invoke("ShieldHide", 5f);
    }
    void ShieldHide()
    {
        shield.SetActive(false);
    }


    // Power슬라이드 max 값, value 값 초기화
    void PowerCoolTime()
    {
        if (isPlayerDead == true)
        {
            curPowerPoint = 0;
            powerGauge.fillAmount = 1f;
        }

        powerGauge.fillAmount = curPowerPoint / maxPowerPoint;
    }

    // 플레이어 총알 업그레이드용 
    public void PowerUpPoint(int enemyScore)
    {
        if (power < 3)
        {
            curPowerPoint += (float)enemyScore;

            if (curPowerPoint > 2000 && power == 1)
            {
                power++;
                curPowerPoint = 0;
                maxPowerPoint = 5000;
                powerGauge.fillAmount = 0;
            }
            else if (curPowerPoint > 5000 && power == 2)
            {
                power++;
                curPowerPoint = 0;
                powerGauge.fillAmount = 0;
            }
        }
    }



    // 단축키 F10 으로 배경음악 켜고/끄기
    public void BGSoundOnOff()
    {
        if (isBGSound)
        {
            isBGSound = false;
            objectManager.backgroundSound.Stop();
        }
        else
        {
            isBGSound = true;
            objectManager.backgroundSound.Play();
        }
    }

    // 플레이어 다시 나타나게 하기
    void ReloadPlayer()
    {
        transform.position = new Vector3(0, -4, 0);
        gameObject.SetActive(true);
        isPlayerDead = false;
    }


    void ScoreUp(int score)
    {
        GameManager.gameScore += score;    // 게임 점수 누적
        PowerUpPoint(score);         // 플레이어 총알 업그레이드 용 점수
    }

}
