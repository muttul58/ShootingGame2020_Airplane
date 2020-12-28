using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int dmg;         // 총알 데이미
    public float speed;     // 총알 속도

    public bool isRotate;   // 총알 회전 설정

    private void Update()
    {
        // isRotate 참이면 총알 회전
        if (isRotate)
            transform.Rotate(Vector3.forward * 5);
    }

    // 총알이 화면 외곽 경계에 다으면 소멸
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderPlayer")
            Destroy(gameObject);
    }
}
