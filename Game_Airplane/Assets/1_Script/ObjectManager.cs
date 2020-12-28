using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject playerBulletObjA;     // 플레이어 총알 A형
    public GameObject playerBulletObjB;     // 플레이어 총알 B형
    public GameObject playerBulletObjL;     // 플레이어 총알 L형

    public GameObject petBulletObj;         // 펫 총알

    public GameObject enemyBulletObjL;      // EnemyL 총알
    public GameObject enemyBulletObjM;      // EnemyM 총알
    public GameObject enemyBulletObjS;      // EnemyS 총알
    public GameObject enemyBulletObjB;      // Boss 총알

    public GameObject[] enemyObj;           // Enemy 오브젝트 배열

    public GameObject[] deadEnemyEffect;    // 적 파괴 이팩 배열
    public GameObject deadPlayerEffect;     // 플레이어 파괴 이팩트
    public GameObject boomEffect;           // 폭탄 이팩트

    // 0: Life,  1: Shield,  2: Power,  3: Boom
    public GameObject[] itemObjs;           // 아이템 오프젝트 배열

    public AudioSource backgroundSound;     // 배경 음악
    public AudioSource bulletShootSound;    // 총알 발사 소리
    public AudioSource LaserShootSound;     // 레이저 발사 소리
    public AudioSource boomPlayerSound;     // 폭탄 터지는 소리

    public AudioSource deadPlayerSound;     // 플레이어 파괴 소리
    public AudioSource deadEnemySound;      // 적 파괴 소리

    public AudioSource itmeShieldSound;     // 쉴드 아이템 소리
    public AudioSource itmePowerSound;      // 파워 아이템 소리
    public AudioSource itmeLifeSound;       // 생명 아이템 소리
}
