using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{
    public float maxBulletShootTime;    // 총알 나가는 간격
    public float curBulletShootTime;

    public Player playerCode;           // Player 코드 가져오기용
    public Laser laserCode;             // Laser 코드 가져오기용
    public ObjectManager objectManager; // ObjectManager 코드 가져오기용

    void Update()
    {
        PetLBullet();
        Reload();
    }

    void PetLBullet()
    {
        
        if (laserCode.isLaserShoot)  // 레이저 발사 이면 총알 발사 안됨
            return;
        
        if (!Input.GetKey("space"))  // 스페이스 키 누르면 총알 발사
            return;
        
        if (gameObject.activeSelf == false)  // 펫이 숨기기 상태이면
            return;

        if (curBulletShootTime < maxBulletShootTime)  // 총알 발사 시간이 안됨면
            return;

        // 총알타입(bulletType)  1: 직선 발사, 2: 점점 퍼지게 발사
        // 파워(power) 1, 2, 3에 따라 총알 형태와 속도 설정
        if (playerCode.bulletType == 1) // 타입 1인 경우
        {
            playerCode.bulletSpeed = 2 + Player.power;          // 총알 스피드 설정
            maxBulletShootTime = 0.5f - (0.1f * Player.power);  // 총알 발사 시간 설정
        }
        else  // 타입 2인 경우
        {
            playerCode.bulletSpeed = 1.0f + (0.5f * Player.power);  // 총알 스피드 설정
            maxBulletShootTime = 0.6f - (0.1f * Player.power);      // 총알 발사 시간 설정
        }

        // 총알 생성                       
        GameObject bullet_01 = Instantiate(objectManager.petBulletObj                 // objectManager 에있는 playerBulletObjB 총알 사용
                                            , transform.position + Vector3.up * 0.7f  // 플레이어 위치에서 위쪽으로 0.7 만큼 위에
                                            , transform.rotation);                    // 회전 없이
        Rigidbody2D rigid_01 = bullet_01.GetComponent<Rigidbody2D>();                 // 중력 적용
        rigid_01.AddForce(Vector3.up * playerCode.bulletSpeed, ForceMode2D.Impulse);  // 위쪽으로 bulletSpeed 만큼 속도록 이동
        
        curBulletShootTime = 0;  // 총알 발사 시간 누적 초기화

    }
    // 총알 발사 시간 누적
    void Reload()
    {
        curBulletShootTime += Time.deltaTime;
    }


    // 적과 적 총알에 맞으면
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            gameObject.SetActive(false);  // 펫 숨기기

            objectManager.deadPlayerSound.Play();   // 펫 죽는 효과음
            // 펫 파괴 효과 
            GameObject eff = Instantiate(objectManager.deadPlayerEffect, transform.position, transform.rotation);
            Destroy(eff, 1.5f);  // 펫 파괴 효과 소멸
        }
    }
}
