using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(BtnInfo))]
public class EventTriggerColumn : EventTrigger
{
    //============================================
    private Vector3 downPosition;
    private Vector3 upPosition;
    private BtnInfo btn;
    private UIObjects uiObj;
    private MainLogic ml;
    private float dragDistance;
    private Vector3 resetPosition;    
    //============================================
    void Start()
    {
        btn = this.GetComponent<BtnInfo>();
        uiObj = UIObjects.Instance;
        ml = MainLogic.Instance;
        dragDistance = 60f * (float)(4f / uiObj.TILE_SIZE);
        resetPosition = this.transform.localPosition;

        Debug.LogFormat("number : {0} / position : {1} / event trigger column(down)'s dragDistance : {1}", btn.number, this.transform.localPosition, dragDistance);
    }
    //============================================
    public override void OnPointerDown(PointerEventData data)
    {
        if (MainLogic.Instance.state == State.Moving)
            return;

        StartCoroutine(ScaleUp());

        //> Set unit UI dummy
        for (int i = uiObj.TILE_SIZE; i < ml.tileLength; i++)
        {
            Unit unit = ml.units[btn.number][i];
            ml.unitUIDummies[i - uiObj.TILE_SIZE].gameObject.SetActive(true);
            ml.unitUIDummies[i - uiObj.TILE_SIZE].localPosition = unit.transform.localPosition;
            ml.unitUIDummies[i - uiObj.TILE_SIZE].GetComponent<Unit>().SetInfo(unit.rePosition, unit.number, unit.side, false);
            ml.unitUIDummies[i - uiObj.TILE_SIZE].GetComponent<UnitUI>().SetUI();
        }

        //> Set black panel
        uiObj.blackPanel.gameObject.SetActive(true);

        for (int i = 0; i < uiObj.TILE_SIZE; i++)
        {
            ml.blackTiles[i].sizeDelta = new Vector2(uiObj.TILE_SIZE_DELTA.x, uiObj.TILE_SIZE_DELTA.y * uiObj.TILE_SIZE);
            ml.blackTiles[i].transform.localPosition = new Vector3(uiObj.TILE_SIZE_DELTA.x * (i + 0.5f + (-uiObj.TILE_SIZE * 0.5f)), 0, 0);
            ml.blackTiles[i].GetComponent<Image>().color = ml.blackColorA03;
        }

        if (btn.number == uiObj.TILE_SIZE)
        {
            //> this tile exist left
            ml.blackTiles[0].GetComponent<Image>().color = ml.blackColorA003;
            ml.blackTiles[1].GetComponent<Image>().color = ml.blackColorA003;
        }
        else if (btn.number == ml.tileLength - 1)
        {
            //> this tile exist right
            ml.blackTiles[ml.blackTiles.Length - 1].GetComponent<Image>().color = ml.blackColorA003;
            ml.blackTiles[ml.blackTiles.Length - 2].GetComponent<Image>().color = ml.blackColorA003;
        }
        else
        {
            //> this tile exist center
            int index = btn.number - uiObj.TILE_SIZE;
            ml.blackTiles[index - 1].GetComponent<Image>().color = ml.blackColorA003;
            ml.blackTiles[index].GetComponent<Image>().color = ml.blackColorA003;
            ml.blackTiles[index + 1].GetComponent<Image>().color = ml.blackColorA003;
        }

        //> render order
        for (int i = 0; i < ml.buttons.Length; i++)
        {
            if (ml.buttons[i].GetComponent<BtnInfo>().Equals(btn))
            {
                ml.buttons[i].GetComponent<RectTransform>().SetSiblingIndex(105);
                break;
            }
        }
        
        Debug.Log("down pos : " + data.position);
        downPosition = data.position;
    }

    IEnumerator ScaleUp()
    {
        btn.PressColor();
        RectTransform rt = btn.image.GetComponent<RectTransform>();
        Vector2 needSize = rt.sizeDelta * 1.4f;

        while (rt.sizeDelta.x < needSize.x)
        {
            rt.sizeDelta *= 1.07f;
            yield return new WaitForEndOfFrame();
        }

        rt.sizeDelta = needSize;
    }
    //============================================
    public override void OnDrag(PointerEventData data)
    {
        if (MainLogic.Instance.state == State.Moving)
            return;
        
        float x = data.position.x - uiObj.TILE_SIZE_DELTA.x * 0.5f;
        if (btn.number <= 0 + uiObj.TILE_SIZE)
        {
            //> only move right
            for (int i = uiObj.TILE_SIZE; i < ml.tileLength; i++)
            {
                if (x > ml.units[btn.number + 1][i].transform.localPosition.x)
                {                
                    //> Right
                    ml.unitUIDummies[i - uiObj.TILE_SIZE].localPosition = ml.units[btn.number + 1][i].transform.localPosition;
                    this.transform.localPosition = new Vector3(ml.units[btn.number + 1][i].transform.localPosition.x, resetPosition.y, 0);
                }
                else
                {
                    if (x >= ml.units[btn.number][i].transform.localPosition.x)
                    {
                        //> Move
                        ml.unitUIDummies[i - uiObj.TILE_SIZE].localPosition = new Vector3(x, ml.units[btn.number][i].transform.localPosition.y, 0);
                        this.transform.localPosition = new Vector3(x, this.transform.localPosition.y, 0);
                    }
                    else
                    {
                        //> Stop
                        ml.unitUIDummies[i - uiObj.TILE_SIZE].localPosition = ml.units[btn.number][i].transform.localPosition;
                        this.transform.localPosition = new Vector3(ml.units[btn.number][i].transform.localPosition.x, resetPosition.y, 0);
                    }
                }
            }
        }
        else if (btn.number >= ml.tileLength - 1)
        {
            //> only move left
            for (int i = uiObj.TILE_SIZE; i < ml.tileLength; i++)
            {
                if (x < ml.units[btn.number - 1][i].transform.localPosition.x)
                {
                    //> Left
                    ml.unitUIDummies[i - uiObj.TILE_SIZE].localPosition = ml.units[btn.number - 1][i].transform.localPosition;
                    this.transform.localPosition = new Vector3(ml.units[btn.number - 1][i].transform.localPosition.x, resetPosition.y, 0);
                }
                else
                {
                    if (x <= ml.units[btn.number][i].transform.localPosition.x)
                    {
                        //> move
                        ml.unitUIDummies[i - uiObj.TILE_SIZE].localPosition = new Vector3(x, ml.units[btn.number][i].transform.localPosition.y, 0);
                        this.transform.localPosition = new Vector3(x, this.transform.localPosition.y, 0);
                    }
                    else
                    {
                        //> stop
                        ml.unitUIDummies[i - uiObj.TILE_SIZE].localPosition = ml.units[btn.number][i].transform.localPosition;
                        this.transform.localPosition = new Vector3(ml.units[btn.number][i].transform.localPosition.x, resetPosition.y, 0);
                    }
                }
            }
        }
        else
        {
            //> move both
            for (int i = uiObj.TILE_SIZE; i < ml.tileLength; i++)
            {
                if (x > ml.units[btn.number + 1][i].transform.localPosition.x)
                {                
                    //> Right
                    ml.unitUIDummies[i - uiObj.TILE_SIZE].localPosition = ml.units[btn.number + 1][i].transform.localPosition;
                    this.transform.localPosition = new Vector3(ml.units[btn.number + 1][i].transform.localPosition.x, this.transform.localPosition.y, 0);
                }
                else if (x < ml.units[btn.number - 1][i].transform.localPosition.x)
                {
                    //> Left
                    ml.unitUIDummies[i - uiObj.TILE_SIZE].localPosition = ml.units[btn.number - 1][i].transform.localPosition;
                    this.transform.localPosition = new Vector3(ml.units[btn.number - 1][i].transform.localPosition.x, this.transform.localPosition.y, 0);
                }
                else
                {
                    ml.unitUIDummies[i - uiObj.TILE_SIZE].localPosition = new Vector3(x, ml.units[btn.number][i].transform.localPosition.y, 0);
                    this.transform.localPosition = new Vector3(x, this.transform.localPosition.y, 0);
                }
            }
        }
    }
    //============================================
    public override void OnPointerUp(PointerEventData data)
    {
        if (MainLogic.Instance.state == State.Moving)
            return;
        
        StartCoroutine(ScaleDown());

        //> reset
        this.transform.localPosition = resetPosition;

        //> Set unit UI dummy
        for (int i = 0; i < uiObj.TILE_SIZE; i++)
        {
            ml.unitUIDummies[i].gameObject.SetActive(false);
        }

        //> Set black panel
        uiObj.blackPanel.gameObject.SetActive(false);

        Debug.Log("up pos : " + data.position);
        upPosition = data.position;

        float distance = Vector3.Distance(upPosition, downPosition);
        Debug.Log("distance between upPosition, downPosition : " + distance);

        if (distance < dragDistance)
        {
            Debug.Log("None drag");
            return;
        }

        Vector3 normal = (upPosition - downPosition).normalized;
        Debug.Log(normal);

        //> Drag direction
        if (Mathf.Abs(normal.x) >= 0.5f)
        {
            if (normal.x > 0)
            {
                //Right
                Debug.Log("Right");

                if (btn.number < ml.tileLength - 1)
                {
                    ml.SetUseEventTrigger(false);
                    ml.TileMove(btn.number, Direction.Right);
                }
            }
            else if (normal.x < 0)
            {
                //Left
                Debug.Log("Left");

                if (btn.number > 0 + uiObj.TILE_SIZE)
                {
                    ml.SetUseEventTrigger(false);
                    ml.TileMove(btn.number, Direction.Left);
                }
            }
        }
    }

    IEnumerator ScaleDown()
    {
        btn.ResetColor();
        RectTransform rt = btn.image.GetComponent<RectTransform>();
        Vector2 needSize = uiObj.TILE_SIZE_DELTA_CHILD * 0.5f;

        while (rt.sizeDelta.x > needSize.x)
        {
            rt.sizeDelta *= 0.95f;
            yield return new WaitForEndOfFrame();
        }

        rt.sizeDelta = needSize;
    }
    //============================================
}
