using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnInfo : MonoBehaviour
{
    //============================================
    public int number;
    public ButtonSide side;
    public Image image;
    public Text text;
    //============================================
    private Color normalColor;
    private Color pressColor;
    //============================================
    void Start()
    {
        normalColor = image.color;
        pressColor = MainLogic.Instance.yellowColor;
    }
    //============================================
    public void SetInfo(int number, ButtonSide side, float x, float y)
    {
        this.number = number;

        image.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
        text.GetComponent<RectTransform>().sizeDelta = new Vector2(x, y);
        text.fontSize = Mathf.FloorToInt(x * 0.41f);
        text.gameObject.SetActive(false);
    }
    //============================================
    public void PressColor()
    {
        image.color = pressColor;
    }

    public void ResetColor()
    {
        image.color = normalColor;
    }
    //============================================
}
