﻿using System.Collections;
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
    public Slider HPbar;  // 생성된 HPbar
    public Slider HpBar_Basic;  // 프리팹으로 할당 한 HpBar

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

/*        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector3.down * speed;*/

        animator = GetComponent<Animator>();


    }

    void Start()
    {
        HPbar = Instantiate(HpBar_Basic) as Slider;
        HPbar.transform.SetParent(GameObject.Find("EnemyHpBar_Canvas").transform);
        HPbar.transform.SetAsFirstSibling();
        HPbar.transform.localScale = new Vector3(0.01f, 0.02f, 0);
        HPbar.transform.localRotation = Quaternion.Euler(0, 0, 0);
        HPbar.tag = "EnemyBoosHPbar";
        HPbar.maxValue = hp;


        
    }

    void Update()
    {
        HpBar_Setting();
    }

    // 적 HpBar 초기화
    void HpBar_Setting()
    {
        HPbar.value = hp;
        HPbar.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1.8f, gameObject.transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hp <= 0) return;

        if (collision.gameObject.tag == "PlayerBullet" || collision.gameObject.tag == "Laser")
        {
            // 플레이어 총알에 맞으면
            animator.SetTrigger("OnHit");
            Effect("H"); // Hit Effect

            if (collision.gameObject.tag == "PlayerBullet")
            {
                Destroy(collision.gameObject);
            }
                bulletCode = collision.gameObject.GetComponent<Bullet>();
                hitDmg = bulletCode.dmg;
                hp -= hitDmg;
        }

                
        // Debug.Log("hp : " + hp);
        if (hp <= 0)
        {
            Effect("D");  // Dead Effect
            Destroy(HPbar.gameObject);
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
        deadEff.transform.localScale = new Vector3(3f, 3f, 0);
        Destroy(deadEff, desTime);  // 1.5초 후 파괴 이팩트 소멸
    }

}
