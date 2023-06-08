using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UGUIAnimation : MonoBehaviour
{
    //============================================
    public Sprite[] sprite;
    public float frame = 60f;
    //============================================
    private Image canvasImage;
    private int index;
    private float frameTime;
    private float checkTime;
    //============================================
    void Awake()
    {
        frameTime = 1f / sprite.Length;
    }

    void Update()
    {
        checkTime += Time.deltaTime;

        if (checkTime >= frameTime)
        {
            checkTime = 0;
            index++;

            if (index >= sprite.Length)
            {
                index = 0;
            }

            canvasImage.sprite = sprite[index];
        }
    }
    //============================================
    public void SetCanvasSprite(ref Image image, string spritePath)
    {
        sprite = Resources.LoadAll<Sprite>(spritePath) as Sprite[];
        canvasImage = image;
        canvasImage.sprite  = this.sprite[0];
        index = 0;
    }
    //============================================
}
