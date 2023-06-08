using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum State
{
    InGame = 0,
    Pause,
    Clear,
    Over,
    Moving
}

public enum Side
{
    Gray = 0,
    Blue,
    Red,
    Green,
}

public enum ButtonSide
{
    Column = 0,
    Row
}

public enum Direction
{
    Right = 0,
    Left,
    Up,
    Down
}

public class UnitInfo
{
    public int blueCount;
    public int redCount;
    public int greenCount;
}

public partial class MainLogic : WeakSingletonMonoBehaviour<MainLogic>
{
    //============================================
    public State state = State.InGame;
    public Unit[][] units; //> 타일 유닛이 모두 들어있다.
    public Unit[][] unitsUndo; //> 언두를 위해 타일 유닛을 임시로 담아놓는 곳.
    public GameObject[] unitMoveDummies;
    public RectTransform[] unitUIDummies;
    public RectTransform[] blackTiles;
    public EventTrigger[] eventTriggers; //> 잠시 작동 멈추게 하는 용도.
    public Button[] buttons;
    public QuestInfo questInfo;
    public UnitInfo unitInfo = new UnitInfo(); //> 현재는 DrawCanvas에 유닛 생성시에만 사용중.
    //============================================
    private UIObjects uiObj;
    private ParticleManager pm;
    private float moveSpeed;
    private float moveDummySpeed;
    private float chargeSpeed;
    private float chargeAddSpeed;
    //============================================
    public int tileLength;
    public Color yellowColor = new Color(1f, 206f / 255f, 0);
    public Color undoColor = new Color(41f / 255f, 41f / 255f, 41f / 255f);
    public Color blackColorA03 = new Color(0, 0, 0, 0.3f);
    public Color blackColorA003 = new Color(0, 0, 0, 0.4f);    
    //============================================
    private bool useUndo = false;
    private bool useTurnAdd = false;
    //============================================
    void Awake()
    {
        base.Awake();
        uiObj = UIObjects.Instance;
        pm = ParticleManager.Instance;
        float tileCoefficient = (float)(uiObj.TILE_SIZE / 4f);
        moveSpeed = 7.0f;
        moveDummySpeed = moveSpeed - 0.2f;
        chargeSpeed = 11f * tileCoefficient;
        chargeAddSpeed = 30f * tileCoefficient;
        tileLength = uiObj.TILE_SIZE * 2;

        units = new Unit[uiObj.UNIT_SIZE][];
        unitsUndo = new Unit[uiObj.UNIT_SIZE][];
        unitMoveDummies = new GameObject[2];
        unitUIDummies = new RectTransform[uiObj.TILE_SIZE];
        blackTiles = new RectTransform[uiObj.TILE_SIZE];
        eventTriggers = new EventTrigger[tileLength];
        buttons = new Button[tileLength];

        for (int i = 0; i < units.Length; i++) units[i] = new Unit[uiObj.UNIT_SIZE];
        for (int i = 0; i < unitsUndo.Length; i++) unitsUndo[i] = new Unit[uiObj.UNIT_SIZE];

        for (int i = 0; i < unitsUndo.Length; i++)
            for (int j = 0; j < unitsUndo[i].Length; j++)
                unitsUndo[i][j] = new Unit();

        StartCoroutine(CheckUndoButton());
        StartCoroutine(CheckTurnAddButton());
    }

    IEnumerator CheckUndoButton()
    {
        while (!useUndo)
        {
            yield return new WaitForEndOfFrame();
        }

        uiObj.btnUndo.SetActive(false);
    }

    IEnumerator CheckTurnAddButton()
    {
        while (!useTurnAdd)
        {
            yield return new WaitForEndOfFrame();
        }

        uiObj.btnTurnAdd.SetActive(false);
    }
    //============================================
    //> 버튼이 사용중.
    public void TileMove(int index, Direction dir)
    {
        Debug.LogFormat("Tile move / state = Moving / index = {0}", index);
        state = State.Moving;

        switch (dir)
        {
            case Direction.Right:
            case Direction.Left:
                {
                    StartCoroutine(MoveSide(index, dir));
                }
                break;
            case Direction.Up:
            case Direction.Down:
                {
                    StartCoroutine(MoveUpDown(index, dir));
                }
                break;
        }
    }
    //============================================
    //> use GameData.Side and GameData.UpDown
    int GetIndex(int index, int level, Direction dir)
    {
        switch (dir)
        {
            case Direction.Right:
                return (index + level);
            case Direction.Left:
                return (index - level);
            case Direction.Up:
                return (index - level);
            case Direction.Down:
                return (index + level);
        }

        return 0;
    }

    public void SetUseEventTrigger(bool enabled)
    {
        foreach (EventTrigger et in eventTriggers)
        {
            et.enabled = enabled;
        }

        foreach (Button btn in buttons)
        {
            btn.enabled = enabled;
        }
    }
    //============================================
    public void UnitCopy(Unit[][] receiver, Unit[][] giver)
    {
        for (int i = 0; i < giver.Length; i++)
        {
            for (int j = 0; j < giver[i].Length; j++)
            {
                receiver[i][j].rePosition = giver[i][j].rePosition;
                receiver[i][j].number = giver[i][j].number;
                receiver[i][j].side = giver[i][j].side;
                receiver[i][j].empty = giver[i][j].empty;
            }
        }
    }
    //============================================
    void CalculateAllUnitCount()
    {
        unitInfo.blueCount = 0;
        unitInfo.redCount = 0;
        unitInfo.greenCount = 0;

        for (int i = 0; i < uiObj.UNIT_SIZE; i++)
        {
            for(int j = 0; j < uiObj.UNIT_SIZE; j++)
            {
                if (units[i][j].empty)
                    continue;

                if (units[i][j].side == Side.Blue)
                    unitInfo.blueCount++;
                else if (units[i][j].side == Side.Red)
                    unitInfo.redCount++;
                else if (units[i][j].side == Side.Green)
                    unitInfo.greenCount++;
            }
        }
    }
    //============================================
    public void ShowAds_Undo()
    {
        //> 광고 패널 닫기.
        OffAdsPanel_Undo_And_Use_Undo();

        //> 광고 보여주기.
        Debug.Log("Show Ads for Undo");
        {
            for (int i = 0; i < unitsUndo.Length; i++)
            {
                for (int j = 0; j < unitsUndo[i].Length; j++)
                {
                    Debug.LogFormat("unitsUndo [{0}][{1}] / number : {2} / type : {3}", i, j, unitsUndo[i][j].number, unitsUndo[i][j].side);
                }
            }

            Debug.Log("//=========================================================");

            UnitCopy(units, unitsUndo);

            for (int i = 0; i < units.Length; i++)
            {
                for(int j = 0; j < units[i].Length; j++)
                {
                    units[i][j].GetComponent<UnitUI>().SetUI();
                    Debug.LogFormat("units[{0}][{1}] / number : {2} / type : {3}", i, j, units[i][j].number, unitsUndo[i][j].side);
                }
            }

            Debug.Log("Effect");
            for (int i = uiObj.TILE_SIZE; i < tileLength; i++)
            {
                for (int j = uiObj.TILE_SIZE; j < tileLength; j++)
                {
                    GameObject go = ParticleManager.Instance.Pop();
                    go.transform.localPosition = units[i][j].GetComponent<RectTransform>().localPosition;
                }
            }
        }        
    }

    public void OffAdsPanel_Undo_And_Use_Undo()
    {
        Debug.Log("Off Ads Panel for Undo / and Use undo button");
        useUndo = true;
        uiObj.adsUndoPanel.gameObject.SetActive(false);
    }

    public void OffAdsPanel_Undo()
    {
        Debug.Log("Off Ads Panel for Undo");
        uiObj.adsUndoPanel.gameObject.SetActive(false);
    }

    public void ShowAds_Turn()
    {
        //> 광고 패널 닫기.
        uiObj.adsTurnPanel.gameObject.SetActive(false);

        //> 광고 보여주기.
        Debug.Log("Show Ads for Turn");
        {
            QuestManager.Instance.turnGoalCount += 4;
            QuestManager.Instance.CheckGoal();

            GameObject go = ParticleManager.Instance.Pop();
            go.transform.position = questInfo.turnCount.transform.position;
        }
        useTurnAdd = true;
    }

    public void OffAdsPanel_Turn()
    {
        Debug.Log("Off Ads Panel for Turn");
        uiObj.adsTurnPanel.gameObject.SetActive(false);
    }
    //============================================
    public void ShowOption()
    {   
        uiObj.optionPanel.gameObject.SetActive(true);
    }
    //============================================
}

