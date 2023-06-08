using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GooglePlayGames;

public enum QuestType
{
    Turn
}

public class QuestManager : WeakSingletonMonoBehaviour<QuestManager>
{
    //============================================
    public QuestType type;
    public int unitMakeCount;
    public int unitGoalCount;
    public int turnGoalCount;
    //============================================
    private UIObjects uiObj;
    private MainLogic ml;
    private int goalCount;
    private int turnCount;
    private ResultInfo resultInfo;
    //============================================
    protected override void Awake()
    {
        base.Awake();

        if (GameData.Instance != null)
        {
            unitMakeCount = GameData.Instance.selectTable.unitMakeCount;
            unitGoalCount = GameData.Instance.selectTable.unitGoalCount;
            turnGoalCount = GameData.Instance.selectTable.turnGoalCount;
        }

        uiObj = UIObjects.Instance;
        ml = MainLogic.Instance;
        ml.questInfo.turnCount.text = (turnGoalCount - turnCount).ToString(); //> 목표 턴 수.
        ml.questInfo.unitMakeCount.text = unitMakeCount.ToString(); //> 만들 갯수.
        ml.questInfo.unitGoalCount.text = unitGoalCount.ToString(); //> 만들 숫자.
        
        resultInfo = UIObjects.Instance.resultPanel.GetComponent<ResultInfo>();

        StartCoroutine(CheckUndoButton());
        StartCoroutine(CheckTurnAddButton());
    }

    IEnumerator CheckUndoButton()
    {
        while (turnCount == 0)
        {
            yield return new WaitForEndOfFrame();
        }

        uiObj.btnUndo.SetActive(true);
    }

    IEnumerator CheckTurnAddButton()
    {
        while (turnGoalCount - turnCount > 5)
        {
            yield return new WaitForEndOfFrame();
        }

        uiObj.btnTurnAdd.SetActive(true);
    }
    //============================================
    public void CheckGoal()
    {
        CheckType_Turn();
        ml.questInfo.turnCount.text = (turnGoalCount - turnCount).ToString();
        ml.state = State.InGame;
        ml.SetUseEventTrigger(true);
        Debug.Log("Tile move / state = InGame");
    }
    //============================================
    void CheckType_Turn()
    {
        turnCount++;
        
        int count = 0;        
        for (int i = 0; i < ml.units.Length; i++)
        {
            for (int j = 0; j < ml.units[i].Length; j++)
            {
                if (ml.units[i][j].side == Side.Blue && ml.units[i][j].number >= unitGoalCount)
                {
                    count++;
                }
            }
        }
        
        if (count >= unitMakeCount)
        {
            int score = (turnGoalCount - turnCount + 5) * GameData.Instance.selectTable.stageNumber * 10;

            resultInfo.gameObject.SetActive(true);
            resultInfo.result.text = "SUCCESS";
            resultInfo.restart.gameObject.SetActive(false);
            resultInfo.scoreCount.text = score.ToString();
            ml.state = State.Clear;
            GameData.Instance.selectTable.clear = true;
            GameData.currentStageNumber++;
            GameData.totalScore += (long)score;

            GameData.Save<List<Table>>(ref GameData.table, KEY.TABLE);
            GameData.Save<int>(ref GameData.currentStageNumber, KEY.CURRENT_STAGE_NUMBER);
            GameData.Save<long>(ref GameData.totalScore, KEY.TOTAL_SCORE);

            Social.ReportScore(GameData.totalScore, GPGSids.leaderboard_total_score, success =>
            {
                string debugMsg;
                if (success)
                    debugMsg = "Reported score successfully";
                else
                    debugMsg = "Failed to report score";

                Debug.Log(debugMsg);
                uiObj.debugPanel.GetComponent<DebugInfo>().debug.text += debugMsg;
            });

            AdsManager.Instance.ShowAds(AdsType.StageEnd);

            /*   Action<> 타입 사용 방법.
             *   Action<T>는 리턴 값이 없는 함수에 사용되는 Delegate이다. callback 메서드에 주로 사용된다.
             *   callback 메서드란, 인자로 넣은 위치로 반환값을 줄테니
             *   그 반환값을 인자로 사용하는 메서드를 사용해서 원하는 내용을 작성 할 수 있도록 하는 녀석이다.
             *   중요한 특징은 리턴값이 없다는 것!!             *   
             *   
             *   Action<bool> CallbackScore = CheckScore;
             *   Social.Active.ReportScore(GameData.totalScore, GPGSids.leaderboard_total_score, CallbackScore);
             *   
             *   반환 값이 없고 T를 인자로 사용하는 메서드를 만들어서 Action<T>에 넘겨주면 된다.
             *   *   
             *   void CheckScore(bool success)
             *   {
             *        Debug.Log(success ? "Reported score successfully" : "Failed to report score");
             *   }
             */

            return;
        }

        if(turnCount >= turnGoalCount)
        {   
            resultInfo.gameObject.SetActive(true);
            resultInfo.result.text = "FAIL";
            resultInfo.scoreCount.text = "0";
            ml.state = State.Over;

            AdsManager.Instance.ShowAds(AdsType.StageEnd);
        }
    }
    //============================================
}
