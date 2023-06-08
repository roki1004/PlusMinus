using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestInfo : MonoBehaviour
{
    //============================================
    public RectTransform bg;
    public Text turnCount;
    public Text turnText;

    public RectTransform option;

    public RectTransform unitImage;
    public Text unitGoalCount;
    public Text unitX;
    public Text unitMakeCount;
    //============================================
    public void SetInfo()
    {
        UIObjects uiObj = UIObjects.Instance;
        float baseSize = uiObj.BASE_SIZE;
        
        bg.sizeDelta = new Vector2(baseSize * 1018f, baseSize * 380f);

        turnText.fontSize = Mathf.FloorToInt(baseSize * 60f);
        RectTransform rt = turnText.GetComponent<RectTransform>();
        rt.sizeDelta = baseSize * rt.sizeDelta;
        rt.anchoredPosition = baseSize * rt.anchoredPosition;
        
        turnCount.fontSize = Mathf.FloorToInt(baseSize * 60f);
        rt = turnCount.GetComponent<RectTransform>();
        rt.sizeDelta = baseSize * rt.sizeDelta;
        rt.anchoredPosition = baseSize * rt.anchoredPosition;

        option.sizeDelta = baseSize * option.sizeDelta;
        option.anchoredPosition = baseSize * option.anchoredPosition;

        unitImage.sizeDelta = baseSize * unitImage.sizeDelta;
        unitImage.anchoredPosition = baseSize * unitImage.anchoredPosition;

        unitGoalCount.fontSize = Mathf.FloorToInt(baseSize * 60f);
        rt = unitGoalCount.GetComponent<RectTransform>();
        rt.sizeDelta = baseSize * rt.sizeDelta;
        rt.anchoredPosition = baseSize * rt.anchoredPosition;

        unitX.fontSize = Mathf.FloorToInt(baseSize * 90f);
        rt = unitX.GetComponent<RectTransform>();
        rt.sizeDelta = baseSize * rt.sizeDelta;
        rt.anchoredPosition = baseSize * rt.anchoredPosition;

        unitMakeCount.fontSize = Mathf.FloorToInt(baseSize * 90f);
        rt = unitMakeCount.GetComponent<RectTransform>();
        rt.sizeDelta = baseSize * rt.sizeDelta;
        rt.anchoredPosition = baseSize * rt.anchoredPosition;
    }
    //============================================
}
