using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float speed;
    public int hp;
    public int enemyScore;
    public string enemyName;

    public Sprite[] sprites; 

    public ObjectManager objectManager;
  //public GameManager gameManager;
    public Player player;
    public SpriteRenderer spriteRenderer;

    void Awake()
    {
        objectManager = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        //gameManager = GetComponent<GameManager>();
        player = GetComponent<Player>();

        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector3.down * speed;

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    
    void OnTriggerEnter2D(Collider2D collision)
    {
        // 화면 밖으로 나가거나 플레이어에 다으면 소멸
        if(collision.gameObject.tag == "BorderEnemy" || collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }

        // 쉴드에 다이면
        else if(collision.gameObject.tag == "Shield")
        {
            Destroy(gameObject);
            GameManager.gameScore += enemyScore;  // 점수 누적
            Effect("D");  // Dead Effect
            ItemDrop();   // 아이템 랜덤 생성
        }

        // 플레이어 총알에 맞으면
        else if(collision.gameObject.tag == "PlayerBullet" || collision.gameObject.tag == "Laser")
        {
            if (hp == 0) return;
            if(collision.gameObject.tag != "Laser")
                Destroy(collision.gameObject);  // 총알 소멸
            Effect("H"); // Hit Effect


            // 총알코드 가져오기
            Bullet bulletCode = collision.gameObject.GetComponent<Bullet>();

            hp -= bulletCode.dmg;

            // Debug.Log("hp : " + hp);
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
        GameObject deadEff = Instantiate(objectManager.deadEnemyEffect[index], transform.position, transform.rotation);
        
        Destroy(deadEff, desTime);  // 1.5초 후 파괴 이팩트 소멸
    }

}
