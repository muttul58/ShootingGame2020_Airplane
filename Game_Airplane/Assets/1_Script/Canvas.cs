using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Canvas : MonoBehaviour
{
    public Image[] bgSoundImgs;
    public Sprite[] sprite;
    public Button BGIconButton;
    public Player playerCode;


    public bool isBGSoundOn;

    public void BGIconONOFF()
    {
        if(playerCode.isBGSound)
            BGIconButton.image.sprite = sprite[0];
        else
            BGIconButton.image.sprite = sprite[1];

    }
}
