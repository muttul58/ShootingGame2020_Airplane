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


    // HPbar 표시에 사용

    public Sprite[] sprites;    // 플레이어 총알에 맞는 효과용

    public bool isLaserHit;     // 플레이어 Laser에 맞은 것 확인
    public float laserDelay;    // Laser에 맞으면 Delay 시간 마다 HP 감소

    public GameObject player;
    public ObjectManager objectManager;
    public SpriteRenderer spriteRenderer;

    Animator animator;
    Bullet bulletCode;

    void Awake()
    {
        objectManager = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        player = GameObject.FindWithTag("Player");

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector3.down * speed;

        animator = GetComponent<Animator>();
        
    }

    void Start()
    {
    }

    void Update()
    {
    }

    void OnHit()
    {
        

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hp == 0) return;

        // 플레이어 총알에 맞으면
        animator.SetTrigger("OnHit");
        Effect("H"); // Hit Effect

        switch (collision.gameObject.tag)
        {
            case "Player":
                hitDmg = 50;
                break;
            case "Shield":
                hitDmg = 100;
                break;
            case "PlayerBullet":
                Destroy(collision.gameObject);
                bulletCode = collision.gameObject.GetComponent<Bullet>();
                hitDmg = bulletCode.dmg;
                break;
            case "Laser":
                bulletCode = collision.gameObject.GetComponent<Bullet>();
                hitDmg = bulletCode.dmg;
                break;
        }

        hp -= hitDmg;
                
        // Debug.Log("hp : " + hp);
        if (hp <= 0)
        {
            Effect("D");  // Dead Effect
            Destroy(gameObject);
            GameManager.gameScore += enemyScore;  // 점수 누적
            ItemDrop();   // 아이템 랜덤 생성
            // Debug.Log("점수 : " + GameManager.gameScore);
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
                hp -= bulletCode.dmg;

                //spriteRenderer.sprite = sprites[1];
                //Invoke("EnemySpriteSwap", 0.1f);

                Effect("H"); // Hit Effect
                laserDelay = 0.1f;
            }
            else
            {
                laserDelay -= Time.deltaTime;
            }

            if (hp <= 0)
            {
                Destroy(gameObject);
                GameManager.gameScore += enemyScore;  // 점수 누적
                Effect("D");  // Dead Effect
                ItemDrop();   // 아이템 랜덤 생성
                              // Debug.Log("점수 : " + GameManager.gameScore);
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
        int ran = Random.Range(0, 10);
        int itemIndex = 0;
        if (ran < 2) return;
        else if (ran < 4) itemIndex = 0;
        else if (ran < 6) itemIndex = 1;
        else if (ran < 8) itemIndex = 2;
        else itemIndex = 3;

        Instantiate(objectManager.itemObjs[itemIndex], transform.position, transform.rotation);
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
        
        Destroy(deadEff, desTime);  // 1.5초 후 파괴 이팩트 소멸
    }

}
