﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int dmg;
    public float speed;

    public bool isRotate;

    private void Update()
    {
        if (isRotate)
            transform.Rotate(Vector3.forward * 10);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderPlayer")
        {
            if (gameObject.tag != "Laser")
            Destroy(gameObject);
        }

        else if (collision.gameObject.tag == "Boom")
            Destroy(gameObject);
    }


}
