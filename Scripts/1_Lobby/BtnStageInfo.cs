using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BtnStageInfo : MonoBehaviour
{
    //====================================================
    public RectTransform clearMark;    
    public RectTransform lockImage;
    public RectTransform ads;
    public Text adsText;
    public Text stageNumber;
    public GameObject particle;
    //====================================================
    private Table table;

    private Color fontColor = new Color(86f / 255f, 86f / 255f, 86f / 255f);
    private Color lockColor = new Color(147f / 255f, 147f / 255f, 147f / 255f);
    private Color openColor = new Color(201f / 255f, 199f / 255f, 199f / 255f);
    private Color clearColor = new Color(108f / 255f, 207f / 255f, 246f / 255f);
    //====================================================
    public void SetInfo(Table table)
    {
        this.table = table;

        //> size
        float baseSize = GameManager.BASE_SIZE;
        lockImage.sizeDelta = new Vector2(baseSize * 75f, baseSize * 90f);
        clearMark.sizeDelta = new Vector2(baseSize * 92f, baseSize * 71f);
        ads.sizeDelta = new Vector2(baseSize * 100f, baseSize * 120f);
        adsText.GetComponent<RectTransform>().sizeDelta = ads.sizeDelta;
        adsText.fontSize = Mathf.FloorToInt(baseSize * 44f);

        //> stage number
        if (table.stageNumber < 10)
            stageNumber.text = "0" + table.stageNumber;
        else
            stageNumber.text = table.stageNumber.ToString();
        
        stageNumber.fontSize = Mathf.FloorToInt(GameManager.BASE_SIZE * 80f);
        stageNumber.color = fontColor;
        stageNumber.enabled = true;

        //> open
        if (this.table.stageNumber == GameData.currentStageNumber)
        {
            if (this.table.ads)
            {
                lockImage.gameObject.SetActive(false);
                ads.gameObject.SetActive(true);                
                clearMark.gameObject.SetActive(false);
                stageNumber.enabled = false;
                this.GetComponent<Image>().color = lockColor;
                ads.GetComponent<Image>().color = Color.white; ;
            }
            else
            {
                lockImage.gameObject.SetActive(false);
                this.GetComponent<Image>().color = openColor;
                if (!this.table.open)
                {
                    particle.SetActive(true);
                    this.table.open = true;
                }
            }
        }
        //> lock
        else
        {
            if(this.table.ads)
            {
                lockImage.gameObject.SetActive(false);
                ads.gameObject.SetActive(true);
                ads.GetComponent<Image>().color = lockColor;
            }
            else
            {
                lockImage.gameObject.SetActive(true);
                ads.gameObject.SetActive(false);
            }

            clearMark.gameObject.SetActive(false);
            stageNumber.enabled = false;
            this.GetComponent<Image>().color = lockColor;
        }

        //> clear
        if (this.table.clear)
        {
            clearMark.gameObject.SetActive(true);
            lockImage.gameObject.SetActive(false);
            ads.gameObject.SetActive(false);
            stageNumber.enabled = false;
            this.GetComponent<Image>().color = clearColor;            
        }
    }

    public void OpenNotAds()
    {
        lockImage.gameObject.SetActive(false);
        ads.gameObject.SetActive(false);
        this.GetComponent<Image>().color = openColor;
        ads.GetComponent<Image>().color = Color.white; ;
        stageNumber.enabled = true;
        particle.SetActive(true);
    }
    //====================================================
    public void Click()
    {
        if (table.stageNumber > GameData.currentStageNumber)
            return;

        if (table.clear)
            return;

        GameData.Instance.selectTable = table;
        SceneManager.LoadScene("Stage");
    }
    //====================================================
}
