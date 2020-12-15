using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    Rigidbody rigid;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }
    GameObject ddd;

    // Update is called once per frame
    void Update()
    {
        rigid.velocity = Vector3.up*100f*Time.deltaTime;
    }
}
