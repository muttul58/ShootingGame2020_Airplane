using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{

    public float maxBulletShootTime;    // 총알 나가는 간격
    public float curBulletShootTime;

    public Player playerCode;
    public Laser laserCode;
    public ObjectManager objectManager;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PetLBullet();
        Reload();
    }

    void PetLBullet()
    {
        // 레이저 발사 이면 총알 발사 안됨
        if (laserCode.isLaserShoot)
            return;

        // 스페이스 키 누르면 총알 발사
        if (!Input.GetKey("space"))
            return;

        if (gameObject.activeSelf == false)
            return;

        if (curBulletShootTime < maxBulletShootTime)
            return;


        // 총알타입(bulletType)  1: 직선 발사, 2: 점점 퍼지게 발사
        // 파워(power) 1, 2, 3에 따라 총알 형태와 속도 설정
        if (playerCode.bulletType == 1)
        {
            playerCode.bulletSpeed = 2 + Player.power;
            maxBulletShootTime = 0.5f - (0.1f * Player.power);
        }
        else
        {
            playerCode.bulletSpeed = 1.0f + (0.5f * Player.power);
            maxBulletShootTime = 0.6f - (0.1f * Player.power);
        }

        // 총알 생성                       
        GameObject bullet_01 = Instantiate(objectManager.petBulletObj          // objectManager 에있는 playerBulletObjB 총알 사용
                                            , transform.position + Vector3.up * 0.7f  // 플레이어 위치에서 위쪽으로 0.7 만큼 위에
                                            , transform.rotation);                    // 회전 없이
        Rigidbody2D rigid_01 = bullet_01.GetComponent<Rigidbody2D>();               // 중력 적용
        rigid_01.AddForce(Vector3.up * playerCode.bulletSpeed, ForceMode2D.Impulse);           // 위쪽으로 bulletSpeed 만큼 속도록 이동
        
        curBulletShootTime = 0;

    }
    // 총알 발사 시간 계산
    void Reload()
    {
        curBulletShootTime += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            gameObject.SetActive(false);

            objectManager.deadPlayerSound.Play();
            GameObject eff = Instantiate(objectManager.deadPlayerEffect, transform.position, transform.rotation);
            Destroy(eff, 1.5f);
        }
    }
}
