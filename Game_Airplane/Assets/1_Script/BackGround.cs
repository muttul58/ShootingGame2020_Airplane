using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public string bgName;
    public float speed;
    public int pos;
    
    public int startIndex;
    public int endIndex;
    public Transform[] bgImgs;


    private void Update()
    {
        if(bgName == "Land") pos = 19;
        else pos = Random.Range(40, 60);
        
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;
        transform.position = curPos + nextPos;

        if(bgImgs[startIndex].position.y < -pos )  // 14 : bockgroundEnd의 y 좌표값
        {
            Vector3 endPos = bgImgs[endIndex].localPosition;
            bgImgs[startIndex].localPosition = endPos + Vector3.up * pos;

            int temp = startIndex;
            startIndex = endIndex;
            endIndex = temp;
        }
    }
}
