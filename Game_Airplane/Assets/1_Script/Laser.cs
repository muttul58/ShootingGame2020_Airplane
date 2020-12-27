using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Laser : MonoBehaviour
{
    public int laserDmg;
    public float maxLaserScale;
    public float laserScale;
    public bool isLaserShoot;       // 레이저 발사 상태
    public bool isHitEnemy;
    public float maxLaserCoolTime;  // 레이저 최고 쿨타임
    public float curLaserCoolTime;  // 레이저 현재 쿨타임

    float laserShowTime;
    public Image laserGauge;        // 레이저 게이지 이미지

    public GameObject player;

    //public Player playerCode;
    public ObjectManager objectManager;
    public GameManager gameManager;

    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();

        maxLaserCoolTime = 3f;    // 1분, 2분, 3분 이 지나면 레이저 사용가능 
                                  // 3초, 6초, 9초 사용 가능
    }

    void Update()
    {
        if (isLaserShoot)
        {
            LaserLengthenTime();
            LaserLineRenderer();
        }

        // 레이저 게이지 표시
        LaserCoolTime();
    }

/*    void HotKey()
    {
        if (Input.GetKeyDown(KeyCode.L)) LaserShoot();
    }*/


    // 레이저 발사
    public void LaserShoot()
    {

        //if (curLaserCoolTime <= maxLaserCoolTime / 3f) return;

        if (!Player.isPlayerDead && !isLaserShoot)
        {

            if (curLaserCoolTime > maxLaserCoolTime)            // 레이저 최고 쿨타임 이상이면
            {
                laserShowTime = 9f;     // 9초 사용
            }
            else if (curLaserCoolTime > maxLaserCoolTime / 1.5f)  // 레이저 최고 쿨타임의 2/3 이상이면
            {
                laserShowTime = 6f;     // 6초 사용
            }
            else if (curLaserCoolTime > maxLaserCoolTime / 3f)    // 레이저 최고 쿨타임의 1/3  이상이면
            {
                laserShowTime = 3f;     // 3초 사용
            }

            isLaserShoot = true;                    // 레이저 발사 함
            curLaserCoolTime = 0;                   // 레이저 쿨타임 초기화 0
            lr.enabled = isLaserShoot;              // LineRenderer 활성화
            objectManager.itmeShieldSound.Play();   // 레이저 나타날 때 사운드 효과
            Invoke("LaserHide", laserShowTime);     // LineRenderer 비활성화
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


    // 레이저 슬라이드 max 값, value 값 초기화
    void LaserCoolTime()
    {
        if (Player.isPlayerDead == true || isLaserShoot == true)
        {
            curLaserCoolTime = 0f;
            laserGauge.fillAmount = 0f;
        }

        curLaserCoolTime += Time.deltaTime;
        laserGauge.fillAmount = curLaserCoolTime / maxLaserCoolTime;
    }

}
