using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnInfoAds : MonoBehaviour
{
    //============================================
    public Image image;
    public Text text;
    //============================================
    private Color normalColor;
    private Color pressColor;
    private Color normalTextColor;
    private Color pressTextColor;
    //============================================
    void Start()
    {   
        normalColor = image.color;
        pressColor = MainLogic.Instance.undoColor;

        normalTextColor = text.color;
        pressTextColor = Color.white;
    }
    //============================================
    public void SetInfo(float x, float y)
    {
        image.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
        text.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
        text.fontSize = Mathf.FloorToInt(x * 0.122f);        
    }
    //============================================
    public void PressColor()
    {
        image.color = pressColor;
        text.color = pressTextColor;
    }

    public void ResetColor()
    {
        image.color = normalColor;
        text.color = normalTextColor;
    }
    //============================================
}
