using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundSound : MonoBehaviour
{
    // GameManager에 포함됨.

    public Image[] bgSoundImgs;             // 화면 오른쪽 위 스피커 모양 이미지
    public Sprite[] sprite;                 // 소리 재생, 정지 이미지
    public Button BGIconButton;             // 버튼으로 만들어 클릭 되게 만듬

    public bool isBGSound;                  // 배경음악 On / Off 상태 설정
    public ObjectManager objectManager;     // 오브젝트 메니저 가져오기

    public void BGIconONOFF()
    {
        // 스피커 이미지 변경
        if(isBGSound)  // 배경음악이 재생중이면  
            BGIconButton.image.sprite = sprite[0];
        else           // 그렇지 않으면
            BGIconButton.image.sprite = sprite[1];
    }

    // 단축키 F10 으로 배경음악 켜고/끄기
    public void BGSoundOnOff()
    {
        if (isBGSound)                                  // 배경음악이 재생 중이면
        {
            isBGSound = false;                          // 상태를 false 설정
            objectManager.backgroundSound.Stop();       // 배경음악 정지
        }
        else                                            // 아니면
        {
            isBGSound = true;                           // 상태를 ture 설정
            objectManager.backgroundSound.Play();       // 배경음악 재생
        }
    }
}
