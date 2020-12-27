﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public int laserDmg;
    public float maxLaserScale;
    public float laserScale;
    public bool isLaserShoot;       // 레이저 발사 상태
    public bool isHitEnemy;

    float laserShowTime;

    public GameObject player;
    //public Player playerCode;
    public ObjectManager objectManager;
    public GameManager gameManager;

    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (isLaserShoot)
        {
            LaserLengthenTime();
            LaserLineRenderer();
        }
    }

/*    void HotKey()
    {
        if (Input.GetKeyDown(KeyCode.L)) LaserShoot();
    }*/


    // 레이저 발사
    public void LaserShoot()
    {

        Debug.Log("Player.isPlayerDead :  " + Player.isPlayerDead + ",      isLaserShoot : " + isLaserShoot);

        if (gameManager.curLaserCoolTime <= gameManager.maxLaserCoolTime / 3f) return;


        if (!Player.isPlayerDead && !isLaserShoot)
        {

            if (gameManager.curLaserCoolTime > gameManager.maxLaserCoolTime)            // 레이저 최고 쿨타임 이상이면
            {
                laserShowTime = 9f;     // 9초 사용
            }
            else if (gameManager.curLaserCoolTime > gameManager.maxLaserCoolTime / 1.5f)  // 레이저 최고 쿨타임의 2/3 이상이면
            {
                laserShowTime = 6f;     // 6초 사용
            }
            else if (gameManager.curLaserCoolTime > gameManager.maxLaserCoolTime / 3f)    // 레이저 최고 쿨타임의 1/3  이상이면
            {
                laserShowTime = 3f;     // 3초 사용
            }


            isLaserShoot = true;
            gameManager.curLaserCoolTime = 0;
            lr.enabled = isLaserShoot;
            objectManager.itmeShieldSound.Play();               // 레이저 나타날 때 사운드 효과
            Invoke("LaserHide", laserShowTime);
        }

    }

    public void LaserHide()
    { // 레이저 숨기기

        isLaserShoot = false;
        lr.enabled = isLaserShoot;
    }


    void LaserLineRenderer()
    {
        if (!isLaserShoot)
        {
            laserScale = 0;
            return;
        }

        // LineRenderer(레이저 선, 하얀 선) 의 시작 위치를 Player 위치에서 한 칸  위로 설정
        lr.SetPosition(0, new Vector3(player.transform.position.x, player.transform.position.y + 0.6f));

        // 충돌체크용 Ray를 발사 (player.transform.position에서 transform.up의 방향으로 laserScale.y의 길이만큼)
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(player.transform.position.x, transform.position.y + 0.6f), transform.up, laserScale);

        // 충돌체크용 Ray를 시각적으로 확인하기 위한 코드 (개발자용 코드, 여기에선 Scene뷰의 빨간선)
        Debug.DrawRay(player.transform.position, transform.up * laserScale, Color.red, 0.1f);

        // hit는 충돌한 Object의 정보를 저장, 이 if문에서는 'Ray에 충돌한 Object의 정보'가 있는지 체크
        if (hit.collider)
        {
            if (hit.collider.tag == "Enemy")
            {

                // 적 데미지 적용
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                enemy.isLaserHit = true;
                
                // LineRenderer(레이저 선, 하얀 선) 의 끝 위치를 hit.point으로 설정
                lr.SetPosition(1, hit.point);
            }
            else if(hit.collider.tag == "EnemyBullet")
            {
                // 레이저에 맞은 것이 적 총알이면
                Destroy(hit.collider.gameObject);
            }
            // LineRenderer(레이저 선, 하얀 선) 의 끝 위치를 player.transform.position.y + laserScale.y으로 설정
            else lr.SetPosition(1, new Vector3(player.transform.position.x, player.transform.position.y + laserScale, 0));
        }
        // LineRenderer(레이저 선, 하얀 선) 의 끝 위치를 player.transform.position.y + laserScale.y으로 설정
        else 
            lr.SetPosition(1, new Vector3(player.transform.position.x, player.transform.position.y + laserScale, 0));
    }


    void LaserLengthenTime()
    {
        laserScale += Time.deltaTime * 100f;
    }


}
