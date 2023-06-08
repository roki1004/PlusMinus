using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UnitUI))]
public class Unit : MonoBehaviour
{
    //============================================
    public Vector3 rePosition;
    public int number;
    public Side side;
    public bool empty;
    //============================================
    public void SetInfo(Vector3 rePosition, int number, Side side, bool empty)
    {
        this.rePosition = rePosition;
        this.number = number;
        this.side = side;
        this.empty = empty;
    }
    //============================================
}