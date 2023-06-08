using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuleInfo : MonoBehaviour
{
    //============================================    
    public RectTransform left;
    public Text sign1;
    public Text sign2;
    public RectTransform right;
    //============================================
    private UIObjects uiObj;
    private Color myBlue = new Color(100f / 255f, 200f / 240f, 1f);
    private Color myRed = new Color(1, 148f / 255f, 148f / 255f);
    private Color myGray = new Color(204f / 255f, 204f / 255f, 204f / 255f);    
    //============================================
    public void SetInfo(Side rightSide, string sign1Text, string sign2Text = null)
    {
        //> 현재 사용하지 않는 클래스.
        //> 튜토리얼 및 설명 때문에 놔둠.
        uiObj = UIObjects.Instance;

        left.sizeDelta = uiObj.TILE_SIZE_DELTA * 0.5f;
        left.localPosition = new Vector3(-left.sizeDelta.x, 0, 0);

        sign1.GetComponent<RectTransform>().sizeDelta = uiObj.TILE_SIZE_DELTA;
        sign1.text = sign1Text;
        sign1.fontSize = Mathf.FloorToInt(uiObj.BASE_SIZE * 100f);

        sign2.GetComponent<RectTransform>().sizeDelta = uiObj.TILE_SIZE_DELTA;
        sign2.text = sign2Text;
        sign2.fontSize = Mathf.FloorToInt(uiObj.BASE_SIZE * 100f);

        right.sizeDelta = uiObj.TILE_SIZE_DELTA * 0.5f;
        right.localPosition = new Vector3(right.sizeDelta.x, 0, 0);

        switch (rightSide)
        {
            case Side.Blue:
                right.GetComponent<Image>().color = myBlue;
                break;
            case Side.Red:
                {
                    RectTransform rt = sign1.GetComponent<RectTransform>();
                    right.GetComponent<Image>().color = myRed;
                    rt.localPosition = new Vector3(0, rt.sizeDelta.y * 0.05f, 0);
                }                
                break;
            case Side.Gray:
                {
                    right.GetComponent<Image>().color = myGray;
                    sign1.GetComponent<RectTransform>().localPosition = new Vector3(0, sign1.GetComponent<RectTransform>().sizeDelta.y * 0.05f, 0);
                    sign2.gameObject.SetActive(true);
                    sign2.GetComponent<RectTransform>().localPosition = new Vector3(0, sign2.GetComponent<RectTransform>().sizeDelta.y * -0.1f, 0);
                }
                break;
        }
    }
    //============================================
}
