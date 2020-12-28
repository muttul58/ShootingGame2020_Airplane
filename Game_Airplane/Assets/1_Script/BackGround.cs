using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public string bgName;       // 백그라운드 이름
    public float speed;         // 이동 속도
    public float pos;           // 이미지 위치
    
    public int startIndex;      // 아래쪽 이미지
    public int endIndex;        // 위쪽   이미지

    public Transform[] bgImgs;  // 이미지 위치 저장

    private void Update()
    {
        if(bgName == "Land") pos = 18.8f;   // 이미지가 땅이면 이동 위치
        else pos = Random.Range(40, 60);    // 이미지가 구름의 이동 위치 랜덤하게
        
        Vector3 curPos = transform.position;    // 현재 이미지의 위치
        // 이동할 이미지의 위치 = 아래 방향으로 speed의 속도 이동한 위치
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;
        // 현재 위치 =  현재 위치 + 이동할 위치
        transform.position = curPos + nextPos;

        // 스타트인덱스 이미지의 y 위치가 -pos 보다 작으면 이동
        if(bgImgs[startIndex].position.y < -pos )  // pos : bockgroundEnd의 y 좌표값
        {
            // 끝 위치 구하기
            Vector3 endPos = bgImgs[endIndex].localPosition;
            // 아래쪽 이미지를 위쪽이지 끝으로 이동
            bgImgs[startIndex].localPosition = endPos + Vector3.up * pos;

            // 이미지의 인덱스를 교환
            int temp = startIndex;
            startIndex = endIndex; // 아래쪽의 이미지를 위쪽으로 설정하고
            endIndex = temp;       // 위쪽의  이미지를 아래쪽으로 설정
        }
    }
}
