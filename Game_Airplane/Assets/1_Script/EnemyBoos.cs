using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBoos : MonoBehaviour
{

    public float speed;
    public int hp;
    public int hitDmg;
    public int enemyScore;
    public string enemyName;

    public float maxShootTime;
    public float curShootTime;


    float bulletSpeed;
    public int patternIndex;
    public int curPatternCount;
    public int[] maxPatternCount;



    // HPbar 표시에 사용
    public Slider HPbar;  // 생성된 HPbar
    public Slider HpBar_Basic;  // 프리팹으로 할당 한 HpBar

    public bool isLaserHit;     // 플레이어 Laser에 맞은 것 확인
    public float laserDelay;    // Laser에 맞으면 Delay 시간 마다 HP 감소

    public GameObject gameManager;
    public GameObject player;
    public Player playerCode;
    public ObjectManager objectManager;
    public SpriteRenderer spriteRenderer;

    Animator animator;
    Bullet bulletCode;

    void Awake()
    {
        objectManager = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        gameManager = GameObject.FindWithTag("GameController");

        player = GameObject.FindWithTag("Player");
        playerCode = GameObject.Find("Player").GetComponent<Player>();

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector3.down * speed;

        animator = GetComponent<Animator>();
    }

    void Start()
    {
        HPbar = Instantiate(HpBar_Basic) as Slider;
        HPbar.transform.SetParent(GameObject.Find("EnemyHpBar_Canvas").transform);
        HPbar.transform.SetAsFirstSibling();
        HPbar.transform.localScale = new Vector3(0.01f, 0.02f, 0);
        HPbar.transform.localRotation = Quaternion.Euler(0, 0, 0);
        
        HPbar.maxValue = hp;

        Think();
    }

    void Think()
    {
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;
        curPatternCount = 0;

        switch (patternIndex)
        {
            case 0:
                FireFoward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
        }
    }

    // 앞으로 4발 발사
    void FireFoward()
    {

        bulletSpeed = 6.0f ;
        float posX;

        if (curPatternCount % 2 == 0)
            posX = 0.8f;
        else
            posX = 0.7f;

        for(int i=0; i<5; i++)
        {
            posX -= 0.2f;
            GameObject bullet_RR = Instantiate(objectManager.enemyBulletObjL, transform.position, transform.rotation);
            Rigidbody2D rigid_RR = bullet_RR.GetComponent<Rigidbody2D>();
            rigid_RR.AddForce((Vector2.down + new Vector2(posX,  0))  * bulletSpeed, ForceMode2D.Impulse);
        }

        curPatternCount++;

        // 패턴 카운터
        if (curPatternCount <= maxPatternCount[patternIndex])
            Invoke("FireFoward", 1.5f);
        else 
            Invoke("Think", 2.0f);
    }

    // 플레이어 방향으로 샷건
    void FireShot()
    {
        bulletSpeed = 6.0f;
        float posX = 0.6f;

        for(int i=0; i < 5; i++)
        {
            posX += -0.2f;
            GameObject bullet = Instantiate(objectManager.enemyBulletObjL, transform.position, transform.rotation);
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 playerPos = player.transform.position - transform.position;
            rigid.AddForce((playerPos.normalized + new Vector2(posX, 0)) * bulletSpeed, ForceMode2D.Impulse);
        }

        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireShot", 3.5f);
        else
            Invoke("Think", 3f);
    }

    // 부채모양으로 발사
    void FireArc()
    {

        bulletSpeed = 6.0f;

        GameObject bullet = Instantiate(objectManager.enemyBulletObjS, transform.position, transform.rotation);
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        //  좌.우로 회전 하며 발사 하기위해 Sin, Con 사용
        Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 10 * curPatternCount / maxPatternCount[patternIndex]), -1);
        rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);


        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireArc", 0.15f);
        else
            Invoke("Think", 6);
    }


    // 원 형태로 전체 공격
    void FireAround()
    {
        // 총알 항상 동일한 위치 발사하는 것을 방지하기 위해 총의 수를 변경
        int roundNum;
        if (curPatternCount % 2 == 0)
            roundNum = 30;
        else
            roundNum = 40;

        bulletSpeed = 2f;  // 총알 속도

        for (int index=0; index<roundNum; index++)
        {
            GameObject bullet = Instantiate(objectManager.enemyBulletObjB, transform.position, transform.rotation);
            bullet.transform.position = transform.position;
            // 총알 방향 초기화
            bullet.transform.rotation = Quaternion.identity;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            //  좌.우로 회전 하며 발사 하기위해 Sin, Con 사용
            Vector2 dirVec = new Vector2( Mathf.Cos(Mathf.PI * 2 * index / roundNum)
                                        , Mathf.Sin(Mathf.PI * 2 * index / roundNum) );
            rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);

            // 총알 진행방향으로 회전하기
            Vector3 rotVec = Vector3.forward * 360 * index / roundNum + Vector3.forward * 90;
            bullet.transform.Rotate(rotVec);
        }

        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireAround", 1f);
        else
            Invoke("Think", 3f);
    }

    // 보스가 활성화 되면 2초 후에 정지
    void OnEnable()
    {
        Invoke("EnemyBossStop", 1.3f);    // 보스 이동 정지
    }


    // 내려오던 보스 정지
    void EnemyBossStop()
    {
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;
    }

    void Update()
    {
        HpBar_Setting();
    }

    // 적 HpBar 초기화
    void HpBar_Setting()
    {
        HPbar.value = hp;
        HPbar.transform.position = new Vector3( gameObject.transform.position.x, 
                                                gameObject.transform.position.y + 1.8f, 
                                                gameObject.transform.position.z );
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hp <= 0) return;

        if (collision.gameObject.tag == "PlayerBullet" || collision.gameObject.tag == "Laser")
        {
            // 플레이어 총알에 맞으면
            animator.SetTrigger("OnHit");
            Effect("H"); // Hit Effect
            ScoreUp(10);  // 점수 누적

            if (collision.gameObject.tag == "PlayerBullet")
            {
                Destroy(collision.gameObject);
                bulletCode = collision.gameObject.GetComponent<Bullet>();
                hitDmg = bulletCode.dmg;
                hp -= hitDmg;
            }

            if (hp <= 0)
            {
                Effect("D");                // Dead Effect
                Destroy(HPbar.gameObject);
                Destroy(gameObject);

                ScoreUp(enemyScore);        // 점수 누적
                ItemDrop();                 // 아이템 랜덤 생성
                GameManager.isGameClear = true;
                //Invoke("GameManger.GameClear", 5f);   
            }
        }
    }

    // 레이저가 적에게 충돌 상태이면
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Laser")
        {
            isLaserHit = true;

            if (laserDelay <= 0)
            {
                bulletCode = collision.gameObject.GetComponent<Bullet>();
                hp -= bulletCode.dmg;

                Effect("H"); // Hit Effect
                laserDelay = 0.1f;
            }
            else
            {
                laserDelay -= Time.deltaTime;
            }

            if (hp <= 0)
            {
                Destroy(HPbar.gameObject);
                Destroy(gameObject);
                GameManager.gameScore += enemyScore;  // 점수 누적
                Effect("D");  // Dead Effect
                ItemDrop();   // 아이템 랜덤 생성
                GameManager.isGameClear = true;
                //Invoke("GameManger.GameClear", 5f);
                              // Debug.Log("점수 : " + GameManager.gameScore);
                //Debug.Log("Boss isGameClear : " + GameManager.isGameClear);

            }
        }
    }

    // 레이저가 적에게 떯어진 상태이면
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Laser")
        {
            isLaserHit = false;
            laserDelay = 0.1f;
        }
    }

    // 아이템 랜덤 생성
    void ItemDrop()
    {
        for(int i = 0; i<10; i++)
        {
            int ran = Random.Range(0, 10);
            int itemIndex = 0;
            if (ran < 2) itemIndex = 0;
            else if (ran < 4) itemIndex = 1;
            else if (ran < 6) itemIndex = 2;
            else if (ran < 8) itemIndex = 3;
            else itemIndex = 4;

            float posX = Random.Range(-2.0f, 2.0f);
            float posY = Random.Range(-2.0f, 2.0f);

            Instantiate( objectManager.itemObjs[itemIndex], 
                         transform.position + Vector3.up * posX + Vector3.left * posY, 
                         transform.rotation );

        }
        // Coin 아이템 100개
        for (int j = 0; j < 100; j++)
        {
            float posX = Random.Range(-3.0f, 3.0f);
            float posY = Random.Range(-3.0f, 2.0f); // 0.00 0.00

            Instantiate(objectManager.itemObjs[4],
                            transform.position + Vector3.up * posX + Vector3.left * posY,
                            transform.rotation);
        }

    }

    // 적 파괴 이팩트 
    void Effect(string type)
    {
        int index;
        float desTime;

        if (type == "D")
        {
            index = 0;
            desTime = 1.5f;
        }
        else
        {
            index = 3;
            desTime = 1.0f;
        }

        objectManager.deadEnemySound.Play();  // 파괴 소리 재생
        // 파괴 이팩트 
        GameObject deadEff = Instantiate(objectManager.deadEnemyEffect[index], transform.position, transform.rotation);
        deadEff.transform.localScale = new Vector3(3f, 3f, 0);
        Destroy(deadEff, desTime);  // 1.5초 후 파괴 이팩트 소멸
    }

    void ScoreUp(int score)
    {
        GameManager.gameScore += score;    // 게임 점수 누적
        playerCode.PowerUpPoint(score);    // 플레이어 총알 업그레이드 용 점수
    }

}
