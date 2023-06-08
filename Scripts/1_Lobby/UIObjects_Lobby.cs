using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIObjects_Lobby : WeakSingletonMonoBehaviour<UIObjects_Lobby>
{
    //====================================================
    public CanvasScaler scaler;
    public GameObject linePrefab;
    public RectTransform pagePanel;
    public RectTransform pageBG;
    public RectTransform btnGPG;
    public RectTransform btnRanking;
    public GameObject btnPagePrefab;
    public Sprite gpgOn;
    //====================================================
    private void Awake()
    {
        base.Awake();
    }
    //====================================================
}
