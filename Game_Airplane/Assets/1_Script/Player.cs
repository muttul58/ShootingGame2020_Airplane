using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed;
    public float bulletSpeed;

    public static int power;
    public static int life;
    public static int boomCount;
    public int bulletType;
    public int laserValue;

    public float maxBulletShootTime;
    public float curBulletShootTime;
    
    public float maxLaserCoolTime;
    public float curLaserCoolTime;
    public float maxLaserTime;
    public float curLaserTime;

    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    public static bool isPlayerDead;
    public bool isBoom;
    public bool isShield;
    //public bool isLaserCoolTimeAdd;
    public bool isLaserShoot;
    public bool isBGSound;

    public Slider hpSlider;  // 레이저 모으는 게이지 표시

    public GameObject shield;
    public GameObject laser;

    public ObjectManager objectManager;
    public GameManager gameManager;

    private void Awake()
    {

    }

    void Start()
    {
        speed = 5;
        power = 1;
        life = 3;
        boomCount=0;
        bulletType = 1;
        isBGSound = true;
        isPlayerDead = true;
        maxLaserCoolTime = 15f;    // 1분, 2분, 3분 이 지나면 레이저 사용가능 
                                    // 3초, 6초, 9초 사용 가능
    }

    void Update()
    {


        //LaserSet();         // 레이저 초기화 및 value 값 적용 누적
        LaserCoolTime();
        PlayerMove();       // 플레이어 이동
        BulletShoot();      // 플레이어 총알 발사
        Reload();           // 총알 리노드
        HotKey();
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
            GameManager.GameScoreUp(100);
        }

        // Shield가 켜진 상태로 적과 총알에 맞은 경우
        else if (isShield && (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet"))
        {
            Destroy(collision.gameObject);
            GameManager.GameScoreUp(100);
        }
        // Shield가 꺼진 상태로 적과 총알에 맞은 경우
        else if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            curLaserCoolTime = 0;
            hpSlider.value = 0;
            isPlayerDead = true;      // Player 사망
            laser.SetActive(false);   // Player 가 죽으면 Laser도 안보이게 설정

            gameObject.SetActive(false);
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
        // Power(파워) 아이템을 먹은 경우
        else if (collision.gameObject.tag == "ItemPower")
        {
            power++;
            objectManager.itmePowerSound.Play();
            Destroy(collision.gameObject);

            if (power > 3)
            {
                power = 3;
                GameManager.GameScoreUp(500);
            }
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
                GameManager.GameScoreUp(500);
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
                GameManager.GameScoreUp(500);
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
                GameManager.GameScoreUp(500);
            }
            else
            {
                gameManager.PlayerBoomSet(boomCount);
            }
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



    void BulletShoot()
    {
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
            GameObject bullet_01 = Instantiate(objectManager.playerBulletObjB, transform.position + Vector3.up * 0.7f, transform.rotation);
            Rigidbody2D rigid_01 = bullet_01.GetComponent<Rigidbody2D>();
            rigid_01.AddForce(Vector3.up * bulletSpeed, ForceMode2D.Impulse);

        }
        else if (bulletType == 2 && power == 1)  // 좌, 중, 우 각 1발 퍼지면서 이동
        {
            GameObject bullet_01R = Instantiate(objectManager.playerBulletObjA, transform.position + Vector3.up * 0.7f, transform.rotation);
            GameObject bullet_01C = Instantiate(objectManager.playerBulletObjA, transform.position + Vector3.up * 0.7f, transform.rotation);
            GameObject bullet_01L = Instantiate(objectManager.playerBulletObjA, transform.position + Vector3.up * 0.7f, transform.rotation);
            Rigidbody2D rigid_01R = bullet_01R.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_01C = bullet_01C.GetComponent<Rigidbody2D>();
            Rigidbody2D rigid_01L = bullet_01L.GetComponent<Rigidbody2D>();

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

    // 단축키 설정
    void HotKey()
    {
        if (Input.GetKeyDown(KeyCode.F1)) bulletType = 1;
        if (Input.GetKeyDown(KeyCode.F2)) bulletType = 2;
        if (Input.GetKeyDown(KeyCode.F5)) power = 1;
        if (Input.GetKeyDown(KeyCode.F6)) power = 2;
        if (Input.GetKeyDown(KeyCode.F7)) power = 3;
        if (Input.GetKeyDown(KeyCode.B)) if(boomCount>0) BoomShow();
        if (Input.GetKeyDown(KeyCode.S)) ShieldHotKey();
        if (Input.GetKeyDown(KeyCode.F9)) ShieldShow();
        if (Input.GetKeyDown(KeyCode.F10)) BGSoundOnOff();
        if (Input.GetKeyDown(KeyCode.L)) LaserShoot();
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

    // 레이저 슬라이드 max 값, value 값 초기화
    void LaserCoolTime()
    {
        if (isPlayerDead == true || isLaserShoot == true)
        {
            curLaserCoolTime = 0;
            hpSlider.value = 0;
        }
        hpSlider.value = curLaserCoolTime / maxLaserCoolTime;
        curLaserCoolTime += Time.deltaTime;

    }

    // 레이저 발사
    void LaserShoot()
    {
        if (!isPlayerDead && !isLaserShoot)
        {
            // 레이저 최고 쿨타임 이상이면
            if (curLaserCoolTime > maxLaserCoolTime)
            {
                LaserShow(3);
            }
            // 레이저 최고 쿨타임의 2/3 이상이면
            else if (curLaserCoolTime > maxLaserCoolTime/1.5f)
            {
                LaserShow(2);
            }
            // 레이저 최고 쿨타임의 1/3  이상이면
            else if (curLaserCoolTime > maxLaserCoolTime/3f)
            {
                LaserShow(1);
            }
        }
       
    }

    // 레이저 발사
    void LaserShow(int laserLevel)
    {
        curLaserCoolTime = 0;         // 레이저 쿨타임 0으로 초기화
        isLaserShoot = true;                    // 레이저 발사중임을 설정

        float laserShowTime = 0;      // 레이저 사용 시간 설정 변수

        if (laserLevel == 1)
        {
            laserShowTime = 3f;     // 3초 사용
            //hpSlider.image.color = new Color(1, 0, 0, 1);
        }
        else if (laserLevel == 2)
        {
            laserShowTime = 6f;     // 6초 사용
            //hpSlider.image.color = new Color(0, 1, 0, 1);
        }
        else if (laserLevel == 3)
        {
            laserShowTime = 9f;     // 9초 사용
            //hpSlider.image.color = new Color(0, 0, 1, 1);
        }
        laser.SetActive(true);                  // 레이저 보이기
        objectManager.itmeShieldSound.Play();   // 레이저 나타날 때 사운드 효과
        Invoke("LaserHide", laserShowTime);                // 레이저 숨기기
    }

    // 레이저 숨기기
    void LaserHide()
    {
        isLaserShoot = false;
        laser.SetActive(false);
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
        laser.SetActive(false);
        isPlayerDead = false;
    }

}
