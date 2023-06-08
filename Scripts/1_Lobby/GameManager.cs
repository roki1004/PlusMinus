using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : WeakSingletonMonoBehaviour<GameManager>
{
    //====================================================
    public static float BASE_SIZE = (Screen.width / 1080f);
    //====================================================
    private UIObjects_Lobby uiObj;
    private Image[] btnPage;
    private LineInfo[] lineInfo;
    //====================================================
    private Color selectColor = new Color(108f / 255f, 206f / 255f, 245f / 255f);
    private int stageCount = 30;
    private int stageLength = 5;
    //====================================================
    private void Awake()
    {
        base.Awake();
        uiObj = UIObjects_Lobby.Instance;
        lineInfo = new LineInfo[stageCount / stageLength];

        uiObj.scaler.referenceResolution = new Vector2(Screen.width, Screen.height);
        uiObj.pagePanel.sizeDelta = new Vector2(Screen.width, Screen.height);
        uiObj.pagePanel.localPosition = Vector3.zero;
        
        float baseWidth = 150f * BASE_SIZE;
        float baseHeight = 180f * BASE_SIZE;
        Vector2 sizeDelta = new Vector2(baseWidth, baseHeight);

        //> Page BG
        uiObj.pageBG.sizeDelta = uiObj.pageBG.sizeDelta * BASE_SIZE;        
        float bgY = (Screen.height * 0.5f) - Mathf.Floor(uiObj.pageBG.sizeDelta.y * 0.685f);
        uiObj.pageBG.localPosition = new Vector3(0, bgY, 0);        

        //> btnGPG
        uiObj.btnGPG.sizeDelta = uiObj.btnGPG.sizeDelta * BASE_SIZE;
        uiObj.btnGPG.anchoredPosition = uiObj.btnGPG.anchoredPosition * BASE_SIZE;
        uiObj.btnGPG.GetComponent<Button>().onClick.AddListener(delegate { GameData.Instance.Login(); });
        if (GameData.Instance.login)
            Login_UI();

        //> btnRanking
        uiObj.btnRanking.sizeDelta = uiObj.btnRanking.sizeDelta * BASE_SIZE;
        uiObj.btnRanking.anchoredPosition = uiObj.btnRanking.anchoredPosition * BASE_SIZE;
        uiObj.btnRanking.GetComponent<Button>().onClick.AddListener(delegate { GameData.Instance.ShowLeaderboard(); });

        //> btnPage        
        int count = (GameData.table.Count / stageCount) + 1;
        btnPage = new Image[count];
        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(uiObj.btnPagePrefab) as GameObject;
            go.transform.SetParent(uiObj.pagePanel.transform);            

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(BASE_SIZE * 94f, BASE_SIZE * 94f);
            rt.localScale = Vector3.one;

            float x = 0;
            if(count % 2 == 1) //> 홀수.
            {
                x = baseWidth * (i - 1);
            }
            else //> 짝수.
            {
                x = baseWidth * (i - (count / 2 - 1) - 0.5f);
            }
            int index = i;
            float y = (Screen.height * 0.5f) - (rt.sizeDelta.y * 1.38f);
            go.transform.localPosition = new Vector3(x, y, 0);
            go.GetComponent<Button>().onClick.AddListener(delegate { MakePage(index); });
            btnPage[i] = go.GetComponent<Image>();
        }

        //> init stage
        float spaceX = (Screen.width - (baseWidth * 5f)) / 6f;
        for (int i = 0; i < lineInfo.Length; i++)
        {
            float x = (Screen.width * -0.5f) + spaceX;
            float y = (Screen.height * 0.3f) - (baseHeight * i * 1.36f);

            GameObject go = Instantiate(uiObj.linePrefab) as GameObject;
            go.transform.SetParent(uiObj.pagePanel.transform);
            go.transform.localPosition = new Vector3(x, y, 0);
            go.GetComponent<HorizontalLayoutGroup>().spacing = spaceX;
            go.GetComponent<RectTransform>().localScale = Vector3.one;

            LineInfo info = go.GetComponent<LineInfo>();
            for (int j = 0; j < info.btnStageInfo.Length; j++)
            {
                RectTransform rt = info.btnStageInfo[j].GetComponent<RectTransform>();
                rt.sizeDelta = sizeDelta;
                rt.localScale = Vector3.one;                
            }

            lineInfo[i] = info;
        }

        currentIndex = -1;
        MakePage(GameData.currentStageNumber / stageCount);
    }
    //====================================================
    private int currentIndex;

    public void MakePage(int index)
    {
        Debug.LogFormat("currentIndex : {0} / index : {1} / data.table.Count : {2}", currentIndex, index, GameData.table.Count);

        if (currentIndex == index)
            return;

        for(int i = 0; i < lineInfo.Length; i++)
        {
            int lineIndex = i;

            for (int j = 0; j < lineInfo[j].btnStageInfo.Length; j++)
            {
                int stageIndex = j;
                int currentStageNumber = j + (i * stageLength) + (index * stageCount);

                if (currentStageNumber >= GameData.table.Count)
                {
                    lineInfo[i].btnStageInfo[j].gameObject.SetActive(false);                    
                }
                else
                {
                    lineInfo[i].btnStageInfo[j].gameObject.SetActive(true);
                    lineInfo[i].btnStageInfo[j].SetInfo(GameData.table[currentStageNumber]);

                    if(GameData.table[currentStageNumber].ads)
                        lineInfo[i].btnStageInfo[j].GetComponent<Button>().onClick.AddListener
                        ( delegate { ShowAds_Stage(lineIndex, stageIndex, GameData.table[currentStageNumber].stageNumber); } );
                }
            }
        }

        for(int i = 0; i < btnPage.Length; i++)
        {
            if(index == i)
            {
                btnPage[i].color = selectColor;
            }
            else
            {
                btnPage[i].color = Color.white;
            }
        }

        currentIndex = index;
    }
    //====================================================
    public void ShowAds_Stage(int lineIndex, int stageIndex, int currentStageNumber)
    {        
        Debug.LogFormat("data.currentStageNumber : {0} / currentStageNumber : {1}", GameData.currentStageNumber, currentStageNumber);

        if (GameData.currentStageNumber < currentStageNumber)
            return;

        //> 광고 보여주기.
        Debug.Log("Show Ads for Stage");
        {
            GameData.currentStageNumber = currentStageNumber;
            GameData.table[currentStageNumber].ads = false;
            lineInfo[lineIndex].btnStageInfo[stageIndex].OpenNotAds();
        }
    }

    public void Login_UI()
    {
        uiObj.btnGPG.GetComponent<Image>().sprite = uiObj.gpgOn;
    }
    //====================================================
}
