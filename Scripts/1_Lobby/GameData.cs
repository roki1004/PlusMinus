using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Table
{
    public int stageNumber;
    public int tileSize;
    public int turnGoalCount;
    public int unitMakeCount;
    public int unitGoalCount;    
    public bool clear;
    public bool open;
    public bool ads;
    public Side startUseSide;
    public Side maxUseSide;   

    public Table(int stageNumber, int tileSize, int turnGoalCount, int unitMakeCount, int unitGoalCount, bool clear, bool open, bool ads, Side startUseSide, Side maxUseSide)
    {
        this.stageNumber = stageNumber;
        this.tileSize = tileSize;
        this.turnGoalCount = turnGoalCount;
        this.unitMakeCount = unitMakeCount;
        this.unitGoalCount = unitGoalCount;
        this.clear = clear;
        this.open = open;
        this.ads = ads;
        this.startUseSide = startUseSide;
        this.maxUseSide = maxUseSide;
    }
}

public partial class GameData : MonoBehaviour
{
    //====================================================
    public static GameData    Instance = null;
    public static List<Table> table =  new List<Table>();
    public static int         currentStageNumber;
    public static long        totalScore;
    //====================================================
    public bool  deleteSaveData;
    public Table selectTable;
    //====================================================
    private void Awake()
    {
        if (Instance != null)
            return;
        
        Instance = this;
        GPG_Init();        

        if (deleteSaveData)
        {
            PlayerPrefs.DeleteAll();
        }

        DontDestroyOnLoad(this);
        Load<List<Table>>(ref table, KEY.TABLE);
        Load<int>(ref currentStageNumber, KEY.CURRENT_STAGE_NUMBER);
        Load<long>(ref totalScore, KEY.TOTAL_SCORE);
    }
    //====================================================
}
