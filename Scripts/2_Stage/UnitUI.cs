using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    //============================================
    public Image image;
    public Text text;
    public Unit unit;
    //============================================
    private Color myBlue =  new Color(100f / 255f, 200f / 240f, 1f);
    private Color myRed =   new Color(          1, 148f / 255f, 148f / 255f);
    private Color myGreen = new Color(101f / 255f, 233f / 255f, 113f / 255f);
    private Color myGray =  new Color(204f / 255f, 204f / 255f, 204f / 255f);
    //============================================
    public void SetUI()
    {
        if (unit.number == 0)
        {
            this.gameObject.SetActive(false);
            unit.empty = true;
        }
        else
        {
            this.gameObject.SetActive(true);            
            unit.empty = false;

            if(unit.side == Side.Gray)
            {
                if(unit.number > 0)
                    text.text = "+" + unit.number.ToString();
                else
                    text.text = unit.number.ToString();
            }
            else
            {
                text.text = unit.number.ToString();
            }            
        }

        if (unit.side == Side.Blue)
        {
            //image.sprite = color[0];
            image.color = myBlue;
        }
        else if (unit.side == Side.Red)
        {
            //image.sprite = color[1];
            image.color = myRed;
        }
        else if (unit.side == Side.Green)
        {
            image.color = myGreen;
        }
        else
        {
            //image.sprite = color[2];
            image.color = myGray;
        }
    }
    //============================================
}
