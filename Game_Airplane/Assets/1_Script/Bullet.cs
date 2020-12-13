﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int dmg;
    public float speed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderPlayer" && collision.gameObject.name == "Laser")
            Destroy(gameObject);

        else if (collision.gameObject.tag == "Boom")
            Destroy(gameObject);
    }
}