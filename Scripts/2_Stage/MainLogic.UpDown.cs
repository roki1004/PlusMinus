using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MainLogic
{
    //============================================
    IEnumerator MoveUpDown(int start, Direction dir)
    {
        int end = GetIndex(start, 1, dir);
        Debug.LogFormat("start : {0} / end : {1} / direction : {2}", start, end, dir.ToString());

        //> undo
        UnitCopy(unitsUndo, units);

        //> unit move for battle
        unitMoveDummies[0].transform.localPosition = units[7][start].transform.localPosition;
        unitMoveDummies[1].transform.localPosition = units[7][end].transform.localPosition;
        while (Vector3.Distance(unitMoveDummies[0].transform.localPosition, unitMoveDummies[1].transform.localPosition) > 2f)
        {
            yield return new WaitForEndOfFrame();

            for (int i = uiObj.TILE_SIZE; i < tileLength; i++)
            {
                Vector3 lerp = Vector3.Lerp(units[i][start].transform.localPosition, units[i][end].transform.localPosition, Time.deltaTime * moveSpeed);
                units[i][start].transform.localPosition = lerp;

                lerp = Vector3.Lerp(unitMoveDummies[0].transform.localPosition, unitMoveDummies[1].transform.localPosition, Time.deltaTime * moveDummySpeed);
                unitMoveDummies[0].transform.localPosition = lerp;
            }
        }

        //> battle
        for (int i = uiObj.TILE_SIZE; i < tileLength; i++)
        {
            //> plus (include gray)
            if (units[i][start].side == units[i][end].side)
                units[i][end].number += units[i][start].number;
            //> minus
            else
            {
                //> include gray
                if (units[i][start].side == Side.Gray)
                {
                    units[i][end].number += units[i][start].number;

                    if (units[i][end].number < 0)
                    {
                        units[i][end].side = units[i][start].side;
                    }
                }
                else if (units[i][end].side == Side.Gray)
                {
                    units[i][end].number += units[i][start].number;

                    if (units[i][end].number > 0)
                    {
                        units[i][end].side = units[i][start].side;
                    }
                }
                else
                {
                    //> only blue / red / green
                    if (units[i][start].number > units[i][end].number)
                    {
                        units[i][end].number = units[i][start].number - units[i][end].number;
                        units[i][end].side = units[i][start].side;

                    }
                    else if (units[i][end].number > units[i][start].number)
                    {
                        units[i][end].number = units[i][end].number - units[i][start].number;
                    }
                    else
                    {
                        units[i][end].number = 0;
                    }
                }
            }

            units[i][start].number = 0;
            units[i][start].transform.localPosition = units[i][start].rePosition;

            units[i][start].GetComponent<UnitUI>().SetUI();
            units[i][end].GetComponent<UnitUI>().SetUI();

            GameObject go = pm.Pop();
            go.transform.localPosition = units[i][end].GetComponent<RectTransform>().localPosition;
        }

        //> charge unit in tiles
        StartCoroutine(ChargeUnitUpDown(start, dir, chargeSpeed));
    }
    //============================================
    IEnumerator ChargeUnitUpDown(int start, Direction dir, float chargeSpeed)
    {
        int beforeStart = GetIndex(start, -1, dir);
        int afterStart = GetIndex(start, 1, dir);
        Debug.LogFormat("beforeStart : {0} / start : {1} / afterStart : {2} / direction : {3}", beforeStart, start, afterStart, dir.ToString());

        unitMoveDummies[0].transform.localPosition = units[7][beforeStart].transform.localPosition;
        unitMoveDummies[1].transform.localPosition = units[7][start].transform.localPosition;
        while (Vector3.Distance(unitMoveDummies[0].transform.localPosition, unitMoveDummies[1].transform.localPosition) > 2f)
        {
            yield return new WaitForEndOfFrame();

            for (int i = uiObj.TILE_SIZE; i < tileLength; i++)
            {
                Vector3 lerp = Vector3.zero;

                if (units[i][afterStart].empty)
                    lerp = Vector3.Lerp(units[i][beforeStart].transform.localPosition, units[i][afterStart].transform.localPosition, Time.deltaTime * chargeSpeed);
                else
                    lerp = Vector3.Lerp(units[i][beforeStart].transform.localPosition, units[i][start].transform.localPosition, Time.deltaTime * chargeSpeed);

                units[i][beforeStart].transform.localPosition = lerp;

                lerp = Vector3.Lerp(unitMoveDummies[0].transform.localPosition, unitMoveDummies[1].transform.localPosition, Time.deltaTime * moveDummySpeed);
                unitMoveDummies[0].transform.localPosition = lerp;
            }
        }

        //> move info
        for (int i = uiObj.TILE_SIZE; i < tileLength; i++)
        {
            int index = 0;

            if (units[i][afterStart].empty)
            {
                index = afterStart;
            }
            else
            {
                index = start;
            }

            units[i][index].number = units[i][beforeStart].number;
            units[i][index].side = units[i][beforeStart].side;
            units[i][index].GetComponent<UnitUI>().SetUI();

            units[i][beforeStart].transform.localPosition = units[i][beforeStart].rePosition;
            units[i][beforeStart].number = 0;
            units[i][beforeStart].GetComponent<UnitUI>().SetUI();
        }

        if (dir == Direction.Down)
        {   
            start--;
            if (start > 0)
                StartCoroutine(ChargeUnitUpDown(start, dir, chargeAddSpeed += chargeAddSpeed));
            else
            {
                //> charge unit in reserve
                ChargeUnitInReserveUpDown(0, 2);

                //> check quest                
                QuestManager.Instance.CheckGoal();
            }
        }
        else
        {   
            start++;
            if (start < (uiObj.UNIT_SIZE - 1))
                StartCoroutine(ChargeUnitUpDown(start, dir, chargeAddSpeed += chargeAddSpeed));
            else
            {
                //> charge unit in reserve
                ChargeUnitInReserveUpDown(uiObj.UNIT_SIZE - 2, uiObj.UNIT_SIZE);

                //> check quest                
                QuestManager.Instance.CheckGoal();
            }
        }
    }
    //============================================
    void ChargeUnitInReserveUpDown(int start, int end)
    {
        CalculateAllUnitCount();

        for (int i = uiObj.TILE_SIZE; i < tileLength; i++)
            for (int j = start; j < end; j++)
                if (units[i][j].empty)
                {
                    Side side = (Side)Random.Range(uiObj.START_USE_SIDE, uiObj.MAX_USE_SIDE);

                    if (side == Side.Gray)
                    {
                        int rand = 0;
                        while (rand == 0)
                        {
                            rand = Random.Range(1, 7) - 4;
                        } 

                        units[i][j].number = rand;
                        units[i][j].side = side;
                        units[i][j].GetComponent<UnitUI>().SetUI();
                    }
                    else
                    {
                        units[i][j].number = Random.Range(1, 4);
                        units[i][j].side = side;
                        units[i][j].GetComponent<UnitUI>().SetUI();
                    }
                }                
    }
    //============================================
}
