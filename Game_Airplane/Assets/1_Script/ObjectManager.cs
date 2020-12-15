using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject playerBulletObjA;
    public GameObject playerBulletObjB;
    public GameObject playerBulletObjL;
    public GameObject playerBoomObj;

    public GameObject enemyBulletObjL;
    public GameObject enemyBulletObjM;
    public GameObject enemyBulletObjS;
    public GameObject enemyBulletObjB;

    public GameObject[] enemyObj;

    public GameObject[] deadEnemyEffect;
    public GameObject deadPlayerEffect;
    public GameObject boomEffect;
    
    // 0: Life,  1: Shield,  2: Power,  3: Boom
    public GameObject[] itemObjs;

    public AudioSource backgroundSound;
    public AudioSource bulletShootSound;
    public AudioSource LaserShootSound;
    public AudioSource boomPlayerSound;

    public AudioSource deadPlayerSound;
    public AudioSource deadEnemySound;

    public AudioSource itmeShieldSound;
    public AudioSource itmePowerSound;
    public AudioSource itmeLifeSound;
    
    
    




    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
