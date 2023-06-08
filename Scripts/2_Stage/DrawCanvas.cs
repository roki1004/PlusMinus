using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawCanvas : MonoBehaviour
{
    //============================================
    private UIObjects uiObj;
    private MainLogic ml;    
    //============================================
    private void Awake()
    {
        uiObj = UIObjects.Instance;
        ml = MainLogic.Instance;

        //> 1. tile panel position(big tile)
        float tilePanelX = -Screen.width * 0.5f + uiObj.TILE_SIZE_DELTA.x * (uiObj.TILE_SIZE * 0.5f + 1f);
        float tilePanelY = -Screen.height * 0.5f + uiObj.TILE_SIZE_DELTA.y * (uiObj.TILE_SIZE * 0.5f + uiObj.UI_HEIGHT);
        Vector3 tilePanelPosition = new Vector3(tilePanelX, tilePanelY, 0);
        uiObj.tilePanel.localPosition = tilePanelPosition;

        //> Tile
        GameObject goTile = Instantiate(uiObj.tilePrefab) as GameObject;
        goTile.transform.SetParent(uiObj.tilePanel.transform);

        RectTransform rtTile = goTile.GetComponent<RectTransform>();
        rtTile.sizeDelta = uiObj.TILE_SIZE_DELTA * (uiObj.TILE_SIZE + 1);
        rtTile.localPosition = Vector3.zero;
        rtTile.localScale = Vector3.one;


        //> base panel position
        float panelX = -Screen.width * 0.5f + uiObj.TILE_SIZE_DELTA.x * 0.5f;
        float panelY = -Screen.height * 0.5f + uiObj.TILE_SIZE_DELTA.y * uiObj.UI_HEIGHT;
        Vector3 basePanelPosition = new Vector3(panelX, panelY, 0);

        //> 2. Unit
        //> 11 == (TILE_SIZE * 3 - 1);
        //> [0, 0] ~ [11, 0]
        //> [0,11] ~ [11,11]

        //> TILE_SIZE가 4일 때,
        //> [4,4][5,4][6,4][7,4]
        //> [4,5][5,5][6,5][7,5]
        //> [4,6][5,6][6,6][7,6]
        //> [4,7][5,7][6,7][7,7]        
        uiObj.unitPanel.localPosition = basePanelPosition;
        int length = uiObj.TILE_SIZE * 2;
        for(int i = 0; i < uiObj.UNIT_SIZE; i++)
        {
            for(int j = 0; j < uiObj.UNIT_SIZE; j++)
            {
                GameObject go = Instantiate(uiObj.unitPrefab) as GameObject;
                go.transform.SetParent(uiObj.unitPanel.transform);
                go.name = "Unit_" + i + "_" + j;

                RectTransform rt = go.GetComponent<RectTransform>();
                rt.sizeDelta = uiObj.TILE_SIZE_DELTA;
                float x = (i - uiObj.TILE_SIZE + 1f) * rt.sizeDelta.x;
                float y = (uiObj.UNIT_SIZE - j - uiObj.TILE_SIZE - 0.5f) * rt.sizeDelta.y;
                rt.localPosition = new Vector3(x, y, 0);
                rt.localScale = Vector3.one;

                Side side = (Side)Random.Range(1, uiObj.MAX_USE_SIDE);
                int rand = Random.Range(1, 4);
                ml.units[i][j] = go.GetComponent<Unit>();
                ml.units[i][j].SetInfo(rt.localPosition, rand, side, false);
                ml.units[i][j].GetComponent<UnitUI>().SetUI();

                //> child
                ml.units[i][j].GetComponent<UnitUI>().image.GetComponent<RectTransform>().sizeDelta = uiObj.TILE_SIZE_DELTA_CHILD;
                ml.units[i][j].GetComponent<UnitUI>().text.GetComponent<RectTransform>().sizeDelta = uiObj.TILE_SIZE_DELTA_CHILD;
                ml.units[i][j].GetComponent<UnitUI>().text.fontSize = Mathf.FloorToInt(uiObj.TILE_SIZE_DELTA.x * 0.41f);

                //> Get count
                if (i >= uiObj.TILE_SIZE && i < length)
                {
                    if (j >= uiObj.TILE_SIZE && j < length)
                    {
                        if (side == Side.Blue)
                            ml.unitInfo.blueCount++;
                        else if (side == Side.Red)
                            ml.unitInfo.redCount++;
                        else if (side == Side.Green)
                            ml.unitInfo.greenCount++;
                    }
                }
            }
        }

        Debug.LogFormat("Blue count : {0} / Red count : {1}", ml.unitInfo.blueCount, ml.unitInfo.redCount);

        //> blue minimum count is 5
        while(ml.unitInfo.blueCount < 5)
        {
            int i = Random.Range(uiObj.TILE_SIZE, length);
            int j = Random.Range(uiObj.TILE_SIZE, length);

            if(ml.units[i][j].side != Side.Blue)
            {
                ml.units[i][j].side = Side.Blue;
                ml.unitInfo.blueCount++;
            }
        }

        //> Units in stage
        //> 여기서 숫자 비슷하게 맞추기.
        if (ml.unitInfo.blueCount > ml.unitInfo.redCount)
        {
            int multiple = (ml.unitInfo.redCount == 0) ? 0 : ml.unitInfo.blueCount / ml.unitInfo.redCount;
            for (int i = uiObj.TILE_SIZE; i < (uiObj.TILE_SIZE * 2); i++)
                for (int j = uiObj.TILE_SIZE; j < (uiObj.TILE_SIZE * 2); j++)
                {
                    if (ml.units[i][j].side == Side.Blue)
                        ml.units[i][j].number = Random.Range(1, 4);
                    else if (ml.units[i][j].side == Side.Red)
                        ml.units[i][j].number = Random.Range(1 * multiple, 4 * multiple);

                    ml.units[i][j].GetComponent<UnitUI>().SetUI();
                }
        }
        else if (ml.unitInfo.redCount > ml.unitInfo.blueCount)
        {
            int multiple = ml.unitInfo.redCount / ml.unitInfo.blueCount;
            for (int i = uiObj.TILE_SIZE; i < (uiObj.TILE_SIZE * 2); i++)
                for (int j = uiObj.TILE_SIZE; j < (uiObj.TILE_SIZE * 2); j++)
                {
                    if (ml.units[i][j].side == Side.Red)
                        ml.units[i][j].number = Random.Range(1, 4);
                    else if (ml.units[i][j].side == Side.Blue)
                        ml.units[i][j].number = Random.Range(1 * multiple, 4 * multiple);

                    ml.units[i][j].GetComponent<UnitUI>().SetUI();
                }
        }
        else
        {
            for (int i = uiObj.TILE_SIZE; i < (uiObj.TILE_SIZE * 2); i++)
                for (int j = uiObj.TILE_SIZE; j < (uiObj.TILE_SIZE * 2); j++)
                {
                    ml.units[i][j].number = Random.Range(1, 4);
                    ml.units[i][j].GetComponent<UnitUI>().SetUI();
                }
        }

        //> Unit move dummy
        for(int i = 0; i < 2; i++)
        {
            GameObject go = Instantiate(uiObj.unitMoveDummyPrefab) as GameObject;
            go.transform.SetParent(uiObj.unitPanel.transform);
            ml.unitMoveDummies[i] = go;
        }

        //> 3. Black panel
        float blackPanelX = tilePanelX;
        float blackPanelY = tilePanelY;
        Vector3 blackPanelPosition = new Vector3(tilePanelX, tilePanelY, 0);
        uiObj.blackPanel.localPosition = blackPanelPosition;

        //> Black Tile
        for(int i = 0; i < uiObj.TILE_SIZE; i++)
        {
            GameObject go = Instantiate(uiObj.tilePrefab) as GameObject;
            go.transform.SetParent(uiObj.blackPanel.transform);
            go.GetComponent<Image>().color = ml.blackColorA03;

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = uiObj.TILE_SIZE_DELTA;
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;

            ml.blackTiles[i] = rt;
        }

        uiObj.blackPanel.gameObject.SetActive(false);

        //> 4. Unit UI dummy
        uiObj.unitUIpanel.localPosition = basePanelPosition;
        for(int i = 0; i < ml.unitUIDummies.Length; i++)
        {
            GameObject go = Instantiate(uiObj.unitUIDummyPrefab) as GameObject;
            go.transform.SetParent(uiObj.unitUIpanel.transform);
            go.GetComponent<Image>().color = ml.yellowColor;

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = uiObj.TILE_SIZE_DELTA;
            rt.localScale = Vector3.one;

            //> child
            Unit unit = go.GetComponent<Unit>();
            unit.GetComponent<UnitUI>().image.GetComponent<RectTransform>().sizeDelta = uiObj.TILE_SIZE_DELTA_CHILD;
            unit.GetComponent<UnitUI>().text.GetComponent<RectTransform>().sizeDelta = uiObj.TILE_SIZE_DELTA_CHILD;
            unit.GetComponent<UnitUI>().text.fontSize = Mathf.FloorToInt(uiObj.TILE_SIZE_DELTA.x * 0.41f);

            ml.unitUIDummies[i] = rt;
            ml.unitUIDummies[i].gameObject.SetActive(false);
        }

        //> Quest
        int fontSize = Mathf.FloorToInt((uiObj.TILE_SIZE_DELTA.x / 216f) * 48f);
        for (int i = 0; i < 1; i++)
        {
            GameObject go = Instantiate(uiObj.questPrefab) as GameObject;
            go.transform.SetParent(uiObj.questPanel.transform);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;
            rt.localPosition = new Vector3(0, uiObj.BASE_SIZE * -222f, 0);
            ml.questInfo = go.GetComponent<QuestInfo>();
            ml.questInfo.SetInfo();
            ml.questInfo.option.GetComponent<Button>().onClick.AddListener(delegate { ml.ShowOption(); });
        }

        //> 5. BG
        uiObj.backgroundPanel.sizeDelta = new Vector2(Screen.width, Screen.height - (uiObj.TILE_SIZE_DELTA.y * (uiObj.TILE_SIZE + uiObj.UI_HEIGHT)));
        uiObj.backgroundPanel.localPosition = new Vector3(0, Screen.height * 0.5f - uiObj.backgroundPanel.sizeDelta.y * 0.5f, 0);

        //> 6. BG tile (Column)
        uiObj.backgroundColPanel.localPosition = basePanelPosition;
        GameObject tileBG = Instantiate(uiObj.tilePrefab) as GameObject;
        tileBG.transform.SetParent(uiObj.backgroundColPanel.transform);        
        RectTransform rtBG = tileBG.GetComponent<RectTransform>();
        rtBG.localScale = Vector3.one;
        rtBG.sizeDelta = new Vector2(Screen.width, uiObj.TILE_SIZE_DELTA.y * uiObj.UI_HEIGHT);
        rtBG.localPosition = new Vector3((Screen.width * 0.5f) - (uiObj.TILE_SIZE_DELTA.x * 0.5f), uiObj.TILE_SIZE_DELTA.y * (-uiObj.UI_HEIGHT * 0.5f), 0);

        //> 7. BG tile (row)
        uiObj.backgroundRowPanel.localPosition = new Vector3(-basePanelPosition.x, basePanelPosition.y, 0);
        for (int i = 0; i < 2; i++)
        {
            tileBG = Instantiate(uiObj.tilePrefab) as GameObject;
            tileBG.transform.SetParent(uiObj.backgroundRowPanel.transform);            
            rtBG = tileBG.GetComponent<RectTransform>();
            rtBG.sizeDelta = new Vector2(uiObj.TILE_SIZE_DELTA.x, uiObj.TILE_SIZE_DELTA.y * (uiObj.TILE_SIZE));

            if (i == 0)
            {
                rtBG.localPosition = new Vector3(-Screen.width + uiObj.TILE_SIZE_DELTA.x, uiObj.TILE_SIZE_DELTA.y * (uiObj.TILE_SIZE * 0.5f), 0);
            }
            else
            {
                rtBG.localPosition = new Vector3(0, uiObj.TILE_SIZE_DELTA.y * (uiObj.TILE_SIZE * 0.5f), 0);
            }
            rtBG.localScale = Vector3.one;
        }
        
        //> 9. Under(btn Column)
        uiObj.btnColumnPanel.localPosition = basePanelPosition;
        for (int i = 0; i < uiObj.TILE_SIZE; i++)
        {
            GameObject go = Instantiate(uiObj.btnColumnPrefab) as GameObject;            
            go.transform.SetParent(uiObj.btnColumnPanel.transform);
            go.GetComponent<BtnInfo>().SetInfo(i + uiObj.TILE_SIZE, ButtonSide.Column, uiObj.TILE_SIZE_DELTA_CHILD.x * 0.5f, uiObj.TILE_SIZE_DELTA_CHILD.y * 0.5f);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = uiObj.TILE_SIZE_DELTA;
            rt.localPosition = new Vector3((i + 1f) * uiObj.TILE_SIZE_DELTA.x, uiObj.TILE_SIZE_DELTA.y * -0.5f, 0);
            rt.localScale = Vector3.one;
            rt.SetSiblingIndex(100);

            ml.eventTriggers[i] = go.GetComponent<EventTriggerColumn>();
            ml.buttons[i] = go.GetComponent<Button>();
        }

        //> 10. Side(btn Row)
        uiObj.btnRowPanel.localPosition = new Vector3(-basePanelPosition.x, basePanelPosition.y, 0);
        for(int i = 0; i < 2; i++)
        {
            for (int j = 0; j < uiObj.TILE_SIZE; j++)
            {
                GameObject go = Instantiate(uiObj.btnRowPrefab) as GameObject;
                go.transform.SetParent(uiObj.btnRowPanel.transform);
                go.GetComponent<BtnInfo>().SetInfo(j + uiObj.TILE_SIZE, ButtonSide.Row, uiObj.TILE_SIZE_DELTA_CHILD.x * 0.5f, uiObj.TILE_SIZE_DELTA_CHILD.y * 0.5f);
                RectTransform rt = go.GetComponent<RectTransform>();
                rt.sizeDelta = uiObj.TILE_SIZE_DELTA;
                rt.localPosition = new Vector3(-i * ((uiObj.TILE_SIZE + 1) * uiObj.TILE_SIZE_DELTA.x) , (uiObj.TILE_SIZE - j - 0.5f) * uiObj.TILE_SIZE_DELTA.y * 1f, 0);
                rt.localScale = Vector3.one;

                ml.eventTriggers[j + uiObj.TILE_SIZE] = go.GetComponent<EventTriggerRow>();
                ml.buttons[j + uiObj.TILE_SIZE] = go.GetComponent<Button>();
            }
        }

        //> btn undo
        float sizeX = ((uiObj.TILE_SIZE * 0.5f) * uiObj.TILE_SIZE_DELTA.x) * 0.9f;
        float sizeY = uiObj.TILE_SIZE_DELTA.y * 0.41f;
        float posSubX = 1.5f + ((uiObj.TILE_SIZE - 4) * 0.25f);
        float posY = uiObj.TILE_SIZE_DELTA.y * (uiObj.TILE_SIZE + 0.5f);
        for (int i = 0; i < 1; i++)
        {
            GameObject go = Instantiate(uiObj.btnUndoPrefab) as GameObject;
            go.transform.SetParent(uiObj.btnColumnPanel.transform);

            RectTransform rt = go.GetComponent<RectTransform>();            
            rt.sizeDelta = new Vector2(sizeX, sizeY);
            rt.localPosition = new Vector3(uiObj.TILE_SIZE_DELTA.x * posSubX, posY, 0);
            rt.localScale = Vector3.one;

            go.GetComponent<BtnInfoAds>().SetInfo(rt.sizeDelta.x, rt.sizeDelta.y);
            go.SetActive(false);
            uiObj.btnUndo = go;
        }

        //> btn turn
        for (int i = 1; i < 2; i++)
        {
            GameObject go = Instantiate(uiObj.btnTurnPrefab) as GameObject;
            go.transform.SetParent(uiObj.btnColumnPanel.transform);            

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(sizeX, sizeY);
            rt.localPosition = new Vector3(uiObj.TILE_SIZE_DELTA.x * (posSubX + (uiObj.TILE_SIZE * 0.5f)), posY, 0);
            rt.localScale = Vector3.one;

            go.GetComponent<BtnInfoAds>().SetInfo(rt.sizeDelta.x, rt.sizeDelta.y);
            go.SetActive(false);
            uiObj.btnTurnAdd = go;
        }

        //> 11. particle panel
        uiObj.particlePanel.localPosition = basePanelPosition;
        ParticleManager.Instance.Init(uiObj.particlePanel.transform);

        //> ads panel undo
        for (int i = 0; i < 1; i++)
        {
            AdsInfo info = uiObj.adsUndoPanel.GetComponent<AdsInfo>();
            info.bg.sizeDelta = new Vector2(Screen.width, Screen.height);
            info.popup.sizeDelta = new Vector2(Screen.width * 0.8f, Screen.width * 0.5f);

            info.questionText.fontSize = Mathf.FloorToInt(uiObj.BASE_SIZE * 50f);
            float adsX = info.popup.sizeDelta.x;
            float adsY = info.popup.sizeDelta.y;
            info.questionText.rectTransform.sizeDelta = new Vector2(adsX, adsY * 0.6f);

            float queY = adsY * 0.5f - info.questionText.rectTransform.sizeDelta.y * 0.65f;
            info.questionText.rectTransform.localPosition = new Vector3(0, queY, 0);

            info.yes.sizeDelta = new Vector2(uiObj.BASE_SIZE * 338f, uiObj.BASE_SIZE * 80f);
            info.yes.localPosition = new Vector3(adsX * 0.5f - info.yes.sizeDelta.x * 0.7f, -adsY * 0.5f + info.yes.sizeDelta.y * 1.5f, 0);            
            info.yes.GetComponent<Button>().onClick.AddListener(delegate { AdsManager.Instance.ShowAds(AdsType.Undo); });
            info.yesText.fontSize = Mathf.FloorToInt(uiObj.BASE_SIZE * 60f);
            info.yesText.rectTransform.sizeDelta = info.yes.sizeDelta;

            info.no.sizeDelta = info.yes.sizeDelta;
            info.no.localPosition = new Vector3(-adsX * 0.5f + info.no.sizeDelta.x * 0.7f, -adsY * 0.5f + info.no.sizeDelta.y * 1.5f, 0);
            info.no.GetComponent<Button>().onClick.AddListener(delegate { ml.OffAdsPanel_Undo(); });
            info.noText.fontSize = info.yesText.fontSize;
            info.noText.rectTransform.sizeDelta = info.no.sizeDelta;

            uiObj.adsUndoPanel.gameObject.SetActive(false);
        }

        //> ads panel turn
        for (int i = 0; i < 1; i++)
        {
            AdsInfo info = uiObj.adsTurnPanel.GetComponent<AdsInfo>();
            info.bg.sizeDelta = new Vector2(Screen.width, Screen.height);
            info.popup.sizeDelta = new Vector2(Screen.width * 0.8f, Screen.width * 0.5f);

            info.questionText.fontSize = Mathf.FloorToInt(uiObj.BASE_SIZE * 50f);
            float adsX = info.popup.sizeDelta.x;
            float adsY = info.popup.sizeDelta.y;
            info.questionText.rectTransform.sizeDelta = new Vector2(adsX, adsY * 0.6f);

            float queY = adsY * 0.5f - info.questionText.rectTransform.sizeDelta.y * 0.65f;
            info.questionText.rectTransform.localPosition = new Vector3(0, queY, 0);

            info.yes.sizeDelta = new Vector2(uiObj.BASE_SIZE * 338f, uiObj.BASE_SIZE * 80f);
            info.yes.localPosition = new Vector3(adsX * 0.5f - info.yes.sizeDelta.x * 0.7f, -adsY * 0.5f + info.yes.sizeDelta.y * 1.5f, 0);
            info.yes.GetComponent<Button>().onClick.AddListener(delegate { AdsManager.Instance.ShowAds(AdsType.Turn); });
            info.yesText.fontSize = Mathf.FloorToInt(uiObj.BASE_SIZE * 60f);
            info.yesText.rectTransform.sizeDelta = info.yes.sizeDelta;

            info.no.sizeDelta = info.yes.sizeDelta;
            info.no.localPosition = new Vector3(-adsX * 0.5f + info.no.sizeDelta.x * 0.7f, -adsY * 0.5f + info.no.sizeDelta.y * 1.5f, 0);
            info.no.GetComponent<Button>().onClick.AddListener(delegate { ml.OffAdsPanel_Turn(); });
            info.noText.fontSize = info.yesText.fontSize;
            info.noText.rectTransform.sizeDelta = info.no.sizeDelta;

            uiObj.adsTurnPanel.gameObject.SetActive(false);
        }

        //> Result
        ResultInfo resultInfo = uiObj.resultPanel.GetComponent<ResultInfo>();
        resultInfo.lobby.sizeDelta = uiObj.BASE_SIZE * resultInfo.lobby.sizeDelta;
        resultInfo.lobby.localPosition = new Vector3(0, -resultInfo.lobby.sizeDelta.y * 1.2f, 0);
        resultInfo.restart.sizeDelta = uiObj.BASE_SIZE * resultInfo.restart.sizeDelta;
        resultInfo.restart.localPosition = new Vector3(0, -resultInfo.lobby.sizeDelta.y * 2.4f, 0);
        resultInfo.result.fontSize = Mathf.FloorToInt(uiObj.BASE_SIZE * resultInfo.result.fontSize);
        resultInfo.result.rectTransform.sizeDelta = uiObj.BASE_SIZE * resultInfo.result.rectTransform.sizeDelta;
        resultInfo.score.fontSize = resultInfo.result.fontSize;
        resultInfo.score.rectTransform.sizeDelta = uiObj.BASE_SIZE * resultInfo.score.rectTransform.sizeDelta;
        resultInfo.score.rectTransform.localPosition = new Vector3(0, resultInfo.score.rectTransform.sizeDelta.y * 3f, 0);
        resultInfo.scoreCount.fontSize = Mathf.FloorToInt(resultInfo.result.fontSize / 1.5f);
        resultInfo.scoreCount.rectTransform.sizeDelta = uiObj.BASE_SIZE * resultInfo.scoreCount.rectTransform.sizeDelta;
        resultInfo.scoreCount.rectTransform.localPosition = new Vector3(0, resultInfo.scoreCount.rectTransform.sizeDelta.y * 2f, 0);

        //> Option
        {
            OptionInfo info = uiObj.optionPanel.GetComponent<OptionInfo>();
            info.bg.sizeDelta = new Vector2(Screen.width, Screen.height);

            Vector2 size = uiObj.BASE_SIZE * new Vector2(600f, 181f);
            info.btnPlay.sizeDelta = size;
            info.btnSound.sizeDelta = size;
            info.btnLobby.sizeDelta = size;
            info.btnRestart.sizeDelta = size;
            info.btnQuit.sizeDelta = size;

            info.btnPlay.anchoredPosition = uiObj.BASE_SIZE * info.btnPlay.anchoredPosition;
            info.btnSound.anchoredPosition = uiObj.BASE_SIZE * info.btnSound.anchoredPosition;
            info.btnLobby.anchoredPosition = uiObj.BASE_SIZE * info.btnLobby.anchoredPosition;
            info.btnRestart.anchoredPosition = uiObj.BASE_SIZE * info.btnRestart.anchoredPosition;
            info.btnQuit.anchoredPosition = uiObj.BASE_SIZE * info.btnQuit.anchoredPosition;
        }

        //> Debug
        {
            uiObj.debugPanel.gameObject.SetActive(false);
            uiObj.debugPanel.localPosition *= uiObj.BASE_SIZE;
            DebugInfo info = uiObj.debugPanel.GetComponent<DebugInfo>();
            info.bg.sizeDelta *= uiObj.BASE_SIZE;            
            info.debug.rectTransform.sizeDelta *= uiObj.BASE_SIZE;
            info.debug.text = string.Empty;
            info.debug.fontSize = Mathf.FloorToInt(info.debug.fontSize * uiObj.BASE_SIZE);
        }
    }
    //============================================
}