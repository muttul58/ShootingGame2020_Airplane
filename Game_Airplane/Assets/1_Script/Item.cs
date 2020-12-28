using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // 아이템을 외곽 경계 또는 플레이어에 다으면 소멸
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderPlayer" || collision.gameObject.tag == "Player")
            Destroy(gameObject);
    }
}
