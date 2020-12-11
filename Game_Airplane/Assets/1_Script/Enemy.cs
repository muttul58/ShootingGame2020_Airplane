using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float speed;
    public int hp;
    public int enemyScore;
    public string enemyName;
    public ObjectManager objectManager;
    //public GameManager gameManager;
    public Player player;
    
    void Awake()
    {
        objectManager = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();

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
        // 플레이어 총알에 맞으면
        else if(collision.gameObject.tag == "PlayerBullet")
        {
            if (hp == 0) return;
            Destroy(collision.gameObject);  // 총알 소멸
            Effect("H");


            // 총알코드 가져오기
            Bullet bulletCode = collision.gameObject.GetComponent<Bullet>();

            hp -= bulletCode.dmg;
            // Debug.Log("hp : " + hp);
            if (hp <= 0)
            {
                Destroy(gameObject);
                Effect("D");
                GameManager.gameScore += enemyScore;
                // Debug.Log("점수 : " + GameManager.gameScore);
            }
        }
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
