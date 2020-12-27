using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Enemy : MonoBehaviour
{
    public float speed;
    public int hp;
    public int enemyScore;
    public string enemyName;

    public float maxShootTime;
    public float curShootTime;


    // HPbar 표시에 사용
    public Slider HPbar;  // 생성된 HPbar
    public Slider HpBar_Basic;  // 프리팹으로 할당 한 HpBar

    public Sprite[] sprites;    // 플레이어 총알에 맞는 효과용

    public bool isHit;          // 플레이어 총알에 맞은 경우
    public bool isLaserHit;     // 플레이어 Laser에 맞은 것 확인
    public float laserDelay;    // Laser에 맞으면 Delay 시간 마다 HP 감소

    public GameObject player;
    public Player playerCode;
    public static bool playerDead;

    public ObjectManager objectManager;
    public SpriteRenderer spriteRenderer;



    Bullet bulletCode;
    Laser laserCode;

    void Awake()
    {
        objectManager = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        player = GameObject.FindWithTag("Player");
        playerCode = GameObject.Find("Player").GetComponent<Player>();

        

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector3.down * speed;
        
    }

    void Start()
    {
        // enemy Hpbar 생성
        HPbar = Instantiate(HpBar_Basic) as Slider;
        HPbar.transform.SetParent(GameObject.Find("EnemyHpBar_Canvas").transform);
        HPbar.transform.SetAsFirstSibling();
        HPbar.transform.localScale = new Vector3(0.005f, 0.016f, 0);
        HPbar.transform.localRotation = Quaternion.Euler(0, 0, 0);

        HPbar.maxValue = hp;

    }

    void Update()
    {
        BulletShoot();
        ReloadShoot();

        HpBar_Setting();
        HPbar_SetActive();

        EnemyIsHitLaser();
    }


    void BulletShoot()
    {
        if (!player.activeSelf) // 플레이어가 꺼져있으면, 죽어있으면
            return; 
        
        if (curShootTime < maxShootTime)
            return;

        if(enemyName == "S" || enemyName == "M")
        {
            //string bulletName =  enemyBulletObjS;
            GameObject bulletS = Instantiate(objectManager.enemyBulletObjS, transform.position, transform.rotation);
            Rigidbody2D rigidS = bulletS.gameObject.GetComponent<Rigidbody2D>();
            Vector3 playerPos = player.transform.position - transform.position;
            rigidS.AddForce(playerPos.normalized * 2, ForceMode2D.Impulse);
        }
        else if (enemyName == "L")
        {
            //string bulletName =  enemyBulletObjS;
            GameObject bulletL = Instantiate(objectManager.enemyBulletObjL, transform.position, transform.rotation);
            Rigidbody2D rigidL = bulletL.gameObject.GetComponent<Rigidbody2D>();
            Vector3 playerPos = player.transform.position - transform.position;
            rigidL.AddForce(playerPos.normalized * 2, ForceMode2D.Impulse);
        }

        maxShootTime = Random.Range(2.0f, 3.0f);
        curShootTime = 0;
    }

    void ReloadShoot()
    {
        curShootTime += Time.deltaTime;
    }

    // 적 HpBar 초기화
    void HpBar_Setting()
    {
        float pos = 0;

        if (enemyName == "L") pos = 1.0f;
        else if (enemyName == "M") pos = 0.8f;
        else if (enemyName == "S") pos = 0.6f;

        HPbar.value = hp;
        HPbar.transform.position = new Vector3( gameObject.transform.position.x, 
                                                gameObject.transform.position.y + pos, 
                                                gameObject.transform.position.z );
    }

    // 멀리있는 적의 HPbar 안보기게 하기
    void HPbar_SetActive()
    {
        HPbar.gameObject.SetActive(false);
        // 플레이어 총알 또는 레이저에 맞으면 HPbar 나타남
        // isHit 가 true 이면

        if (isHit) HPbar.gameObject.SetActive(true);

        /*
                // 플레이어가 가까이(6f) 오면 HPbar 나타나는 코드
                Collider2D[] hitCol = Physics2D.OverlapCircleAll(gameObject.transform.position, 6f);
                HPbar.gameObject.SetActive(false);

                for(int i = 0; i< hitCol.Length; i++)
                {
                    if (hitCol[i].gameObject.name == "Player")
                        HPbar.gameObject.SetActive(true);
                }
        */
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 화면 밖으로 나가거나 플레이어에 다으면 소멸
        if(collision.gameObject.tag == "BorderEnemy" || collision.gameObject.tag == "Player" || collision.gameObject.tag == "Pet")
        {
            Destroy(gameObject);
            Destroy(HPbar.gameObject);
        }

        // 쉴드에 다이면
        else if(collision.gameObject.tag == "Shield")
        {
            Destroy(gameObject);
            Destroy(HPbar.gameObject);
            ScoreUp(enemyScore);  // 점수 누적
            Effect("D");  // Dead Effect
            ItemDrop();   // 아이템 랜덤 생성
        }

        // 플레이어 총알에 맞으면
        else if(collision.gameObject.tag == "PlayerBullet" || collision.gameObject.tag == "Laser")
        {
            if (hp == 0) return;        // 죽은 경우

            if(!isHit) isHit = true;    // 플레이어 총알이나 레이저에 맞은 경우

            ScoreUp(10);  // 점수 누적
            Destroy(collision.gameObject);  // 총알 소멸
            Effect("H"); // Hit Effect


            // 총알코드 가져오기
            bulletCode = collision.gameObject.GetComponent<Bullet>();

            hp -= bulletCode.dmg;
            spriteRenderer.sprite = sprites[1];
            Invoke("EnemySpriteSwap", 0.1f);

            // Debug.Log("hp : " + hp);
            if (hp <= 0)
            {
                Destroy(gameObject);
                Destroy(HPbar.gameObject);
                ScoreUp(enemyScore);  // 점수 누적
                Effect("D");  // Dead Effect
                ItemDrop();   // 아이템 랜덤 생성
                // Debug.Log("점수 : " + GameManager.gameScore);
            }
        }
    }

    // 적이 레이저에 맞은 상태이면
    void EnemyIsHitLaser()
    {
        // 적이 레이저에 맞은 상태이면
        if (isLaserHit)
        {
            // 적에게 데미지 적용 시간이 되면
            if (laserDelay > 0.1f)
            {
                // 레이저 로직에 있는 laserDmg 가져와서 적 hp 감소
                laserCode = GameObject.Find("Laser").GetComponent<Laser>();
                hp -= laserCode.laserDmg;

                spriteRenderer.sprite = sprites[1];     // 레이저에 맞으면 번쩍이는 효과
                Invoke("EnemySpriteSwap", 0.1f);

                Effect("H"); // Hit Effect
                laserDelay = 0f;
            }
            else
            {
                laserDelay += Time.deltaTime;   // 적 데이지 적용 시간 누적
            }

            // 적의 hp가 0이면
            if (hp <= 0)
            {
                Destroy(gameObject);        // 적 소멸
                Destroy(HPbar.gameObject);  // 적 HPbar 소멸
                ScoreUp(enemyScore);        // 점수 누적  
                Effect("D");                // Dead Effect
                ItemDrop();                 // 아이템 랜덤 생성
            }

            isLaserHit = false;             // 레이저의 데미지를 적용시기키 위해 Hit를 true, false 한다
        }
    }

    // 총알이나 레이저에 맞으면 번쩍이는 효과를 주기위해 스프라이트 변경
    void EnemySpriteSwap()
    {
        spriteRenderer.sprite = sprites[0];
    }


    // 아이템 랜덤 생성
    void ItemDrop()
    {
        // 아래 확률로 아이템 생성
        int ran = Random.Range(0, 10);
        int itemIndex = 0;
        if (ran < 2) return;
        else if (ran < 4) itemIndex = 0;
        else if (ran < 6) itemIndex = 1;
        else if (ran < 8) itemIndex = 2;
        else itemIndex = 3;

        // 오브젝트 메니저에 있는 아이템 생성
        Instantiate(objectManager.itemObjs[itemIndex], transform.position, transform.rotation);
    }

    // 적 파괴 이팩트 
    void Effect(string type)
    {
        // 적 이름에 따라 파괴 이팩트 다르게 적용
        float desTime = 1.5f;
        int index=0;
        if (enemyName == "L") index = 0;
        if (enemyName == "M") index = 1;
        if (enemyName == "S") index = 2;
        if (type == "H")
        {
            index = 3;
            desTime = 1.0f;
        }

        objectManager.deadEnemySound.Play();  // 파괴 소리 재생
        // 파괴 이팩트 
        GameObject deadEff = Instantiate(objectManager.deadEnemyEffect[index], transform.position+Vector3.down *0.5f, transform.rotation);
        
        Destroy(deadEff, desTime);  // 1.5초 후 파괴 이팩트 소멸
    }

    // 게임 점수 누적
    void ScoreUp(int score)
    {
        GameManager.gameScore += score;     // 게임 점수 누적
        playerCode.PowerUpPoint(score);     // 플레이어 총알 업그레이드 용 점수
    }

}
