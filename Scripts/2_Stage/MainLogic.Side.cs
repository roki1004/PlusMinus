using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MainLogic
{
    //============================================
    IEnumerator MoveSide(int start, Direction dir)
    {
        int end = GetIndex(start, 1, dir);
        Debug.LogFormat("start : {0} / end : {1} / direction : {2}", start, end, dir.ToString());

        //> undo
        UnitCopy(unitsUndo, units);

        //> unit move for battle
        unitMoveDummies[0].transform.localPosition = units[start][7].transform.localPosition;
        unitMoveDummies[1].transform.localPosition = units[end][7].transform.localPosition;
        while (Vector3.Distance(unitMoveDummies[0].transform.localPosition, unitMoveDummies[1].transform.localPosition) > 2f)
        {
            yield return new WaitForEndOfFrame();

            for (int i = uiObj.TILE_SIZE; i < tileLength; i++)
            {
                Vector3 lerp = Vector3.Lerp(units[start][i].transform.localPosition, units[end][i].transform.localPosition, Time.deltaTime * moveSpeed);
                units[start][i].transform.localPosition = lerp;

                lerp = Vector3.Lerp(unitMoveDummies[0].transform.localPosition, unitMoveDummies[1].transform.localPosition, Time.deltaTime * moveDummySpeed);
                unitMoveDummies[0].transform.localPosition = lerp;
            }
        }

        //> battle
        for (int i = uiObj.TILE_SIZE; i < tileLength; i++)
        {
            //> plus (include gray)
            if (units[start][i].side == units[end][i].side)
                units[end][i].number += units[start][i].number;
            //> minus
            else
            {
                //> include gray
                if (units[start][i].side == Side.Gray)
                {
                    units[end][i].number += units[start][i].number;

                    if (units[end][i].number < 0)
                    {
                        units[end][i].side = units[start][i].side;
                    }
                }
                else if (units[end][i].side == Side.Gray)
                {
                    units[end][i].number += units[start][i].number;

                    if (units[end][i].number > 0)
                    {
                        units[end][i].side = units[start][i].side;
                    }
                }
                else
                {
                    //> blue / red / green
                    if (units[start][i].number > units[end][i].number)
                    {
                        units[end][i].number = units[start][i].number - units[end][i].number;
                        units[end][i].side = units[start][i].side;

                    }
                    else if (units[end][i].number > units[start][i].number)
                    {
                        units[end][i].number = units[end][i].number - units[start][i].number;
                    }
                    else
                    {
                        units[end][i].number = 0;
                    }
                }
            }

            units[start][i].number = 0;
            units[start][i].transform.localPosition = units[start][i].rePosition;

            units[start][i].GetComponent<UnitUI>().SetUI();
            units[end][i].GetComponent<UnitUI>().SetUI();
            
            GameObject go = pm.Pop();
            go.transform.localPosition = units[end][i].GetComponent<RectTransform>().localPosition;
        }

        //> charge unit in tiles
        StartCoroutine(ChargeUnitSide(start, dir, chargeSpeed));
    }
    //============================================
    IEnumerator ChargeUnitSide(int start, Direction dir, float chargeSpeed)
    {
        int beforeStart = GetIndex(start, -1, dir);
        int afterStart = GetIndex(start, 1, dir);
        Debug.LogFormat("beforeStart : {0} / start : {1} / afterStart : {2} / direction : {3}", beforeStart, start, afterStart, dir.ToString());

        unitMoveDummies[0].transform.localPosition = units[beforeStart][7].transform.localPosition;
        unitMoveDummies[1].transform.localPosition = units[start][7].transform.localPosition;
        while (Vector3.Distance(unitMoveDummies[0].transform.localPosition, unitMoveDummies[1].transform.localPosition) > 2f)
        {
            yield return new WaitForEndOfFrame();

            for (int i = uiObj.TILE_SIZE; i < tileLength; i++)
            {
                Vector3 lerp = Vector3.zero;

                if (units[afterStart][i].empty)
                    lerp = Vector3.Lerp(units[beforeStart][i].transform.localPosition, units[afterStart][i].transform.localPosition, Time.deltaTime * chargeSpeed);
                else
                    lerp = Vector3.Lerp(units[beforeStart][i].transform.localPosition, units[start][i].transform.localPosition, Time.deltaTime * chargeSpeed);

                units[beforeStart][i].transform.localPosition = lerp;

                lerp = Vector3.Lerp(unitMoveDummies[0].transform.localPosition, unitMoveDummies[1].transform.localPosition, Time.deltaTime * moveDummySpeed);
                unitMoveDummies[0].transform.localPosition = lerp;
            }
        }

        //> move info
        for (int i = uiObj.TILE_SIZE; i < tileLength; i++)
        {
            int index = 0;

            if (units[afterStart][i].empty)
            {
                index = afterStart;
            }
            else
            {
                index = start;
            }

            units[index][i].number = units[beforeStart][i].number;
            units[index][i].side = units[beforeStart][i].side;
            units[index][i].GetComponent<UnitUI>().SetUI();

            units[beforeStart][i].transform.localPosition = units[beforeStart][i].rePosition;
            units[beforeStart][i].number = 0;
            units[beforeStart][i].GetComponent<UnitUI>().SetUI();
        }

        if (dir == Direction.Right)
        {
            start--;
            if (start > 0)
                StartCoroutine(ChargeUnitSide(start, dir, chargeAddSpeed+= chargeAddSpeed));
            else
            {
                //> charge unit in reserve
                ChargeUnitInReserveSide(0, 2);

                //> check quest                
                QuestManager.Instance.CheckGoal();
            }
        }
        else
        {
            start++;
            if (start < (uiObj.UNIT_SIZE - 1))
                StartCoroutine(ChargeUnitSide(start, dir, chargeAddSpeed += chargeAddSpeed));
            else
            {
                //> charge unit in reserve
                ChargeUnitInReserveSide(uiObj.UNIT_SIZE - 2, uiObj.UNIT_SIZE);

                //> check quest                
                QuestManager.Instance.CheckGoal();
            }
        }
    }
    //============================================
    void ChargeUnitInReserveSide(int start, int end)
    {
        CalculateAllUnitCount();

        for (int i = uiObj.TILE_SIZE; i < tileLength; i++)
            for (int j = start; j < end; j++)
                if (units[j][i].empty)
                {
                    Side side = (Side)Random.Range(uiObj.START_USE_SIDE, uiObj.MAX_USE_SIDE);

                    if (side == Side.Gray)
                    {
                        int rand = 0;
                        while (rand == 0)
                        {
                            rand = Random.Range(1, 7) - 4;
                        }

                        units[j][i].number = rand;
                        units[j][i].side = side;
                        units[j][i].GetComponent<UnitUI>().SetUI();
                    }
                    else
                    {
                        //> 어떻게 컬러 배율을 맞춘다?.
                        units[j][i].number = Random.Range(1, 4);
                        units[j][i].side = side;
                        units[j][i].GetComponent<UnitUI>().SetUI();
                    }
                }
    }
    //============================================
}
