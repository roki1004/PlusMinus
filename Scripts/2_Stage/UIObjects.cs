using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIObjects : WeakSingletonMonoBehaviour<UIObjects>
{   
    //============================================
    public CanvasScaler scaler;
    public GameObject tilePrefab;
    public GameObject unitPrefab;
    public GameObject unitMoveDummyPrefab;
    public GameObject unitUIDummyPrefab;
    public GameObject questPrefab;
    public GameObject btnColumnPrefab;
    public GameObject btnRowPrefab;
    public GameObject btnUndoPrefab;
    public GameObject btnTurnPrefab;

    public RectTransform tilePanel;
    public RectTransform unitPanel;
    public RectTransform blackPanel;
    public RectTransform unitUIpanel;
    public RectTransform backgroundPanel;
    public RectTransform backgroundColPanel;
    public RectTransform backgroundRowPanel;
    public RectTransform questPanel;
    public RectTransform btnColumnPanel;
    public RectTransform btnRowPanel;
    public RectTransform particlePanel;
    public RectTransform resultPanel;
    public RectTransform adsUndoPanel;
    public RectTransform adsTurnPanel;
    public RectTransform optionPanel;
    public RectTransform debugPanel;

    //> scaler.referenceResolution에 맞춰 사이즈도 재설정 되어야 한다.
    //============================================
    public float SCALE;
    public float BASE_SIZE;
    public float UI_HEIGHT = 3f;

    public int TILE_SIZE;
    public int UNIT_SIZE;
    public Vector2 TILE_SIZE_DELTA;
    public Vector3 TILE_SIZE_DELTA_CHILD;
    public int START_USE_SIDE;
    public int MAX_USE_SIDE;

    public GameObject btnUndo;
    public GameObject btnTurnAdd;
    //============================================
    private void Awake()
    {   
        base.Awake();        
        scaler.referenceResolution = new Vector2(Screen.width, Screen.height);
        SCALE = scaler.GetComponent<RectTransform>().localScale.x;
        BASE_SIZE = (Screen.width / 1080f);

        if (GameData.Instance != null)
        {
            TILE_SIZE = GameData.Instance.selectTable.tileSize;
            START_USE_SIDE = (int)GameData.Instance.selectTable.startUseSide;
            MAX_USE_SIDE = (int)GameData.Instance.selectTable.maxUseSide + 1;
        }

        UNIT_SIZE = TILE_SIZE * 3;
        float size = Screen.width / (TILE_SIZE + 2);
        TILE_SIZE_DELTA = new Vector2(size, size);
        TILE_SIZE_DELTA_CHILD = new Vector2(size - (BASE_SIZE * 30), size - (BASE_SIZE * 30));

        //> render order
        tilePanel.SetSiblingIndex(0);
        unitPanel.SetSiblingIndex(1);
        blackPanel.SetSiblingIndex(2);
        unitUIpanel.SetSiblingIndex(3);
        backgroundPanel.SetSiblingIndex(4);
        backgroundColPanel.SetSiblingIndex(5);
        backgroundRowPanel.SetSiblingIndex(6);
        questPanel.SetSiblingIndex(7);
        btnColumnPanel.SetSiblingIndex(9);
        btnRowPanel.SetSiblingIndex(10);
        particlePanel.SetSiblingIndex(11);
        resultPanel.SetSiblingIndex(12);
        adsUndoPanel.SetSiblingIndex(13);
        adsTurnPanel.SetSiblingIndex(14);
        optionPanel.SetSiblingIndex(15);
        debugPanel.SetSiblingIndex(16);
    }
    //============================================
}
