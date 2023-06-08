using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public partial class GameData : MonoBehaviour
{
    //====================================================
    public bool login = false;
    //====================================================
    private void GPG_Init()
    {
        /*
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            //> enables saving game progress.
            //> .EnableSavedGames()

            //> registers a callback to handle game invitations received while the game is not running.
            //> .WithInvitationDelegate(< callback method >)

            //> registers a callback for turn based match notifications received while the game is not running.
            //> .WithMatchDelegate(< callback method >)

            //> requests the email address of the player be available.
            //> Will bring up a prompt for consent.
            //> .RequestEmail()

            //> requests a server auth code be generated so it can be passed to an
            //> associated back end server application and exchanged for an OAuth token.
            .RequestServerAuthCode(false)

            //> requests an ID token be generated. This OAuth token can be used to
            //> identify the player to other services such as Firebase.
            .RequestIdToken()

            .Build();
        */
                
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

        /*
         * After activated, you can access the Play Games platform through Social.Active.
         * You should only call PlayGamesPlatform.Activate once in your application.
         * Making this call will not display anything on the screen and will not interact with the user in any way.
         */
    }

    private void OnApplicationQuit()
    {   
        PlayGamesPlatform.Instance.SignOut();
    }
    //====================================================
    public void Login()
    {
        if (login)
            return;

        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Debug.Log("success to login");
                login = true;
                UIObjects_Lobby.Instance.btnGPG.GetComponent<UnityEngine.UI.Image>().sprite = UIObjects_Lobby.Instance.gpgOn;
            }
            else
            {
                Debug.Log("fail to login");
            }
        });
    }

    public void ShowLeaderboard()
    {
        if(!login)
            Login();

        //> 리더보드 페이지로 연결한다.
        Social.ShowLeaderboardUI();

        //> 특정 리더보드를 직접 보여줄 때 사용한다.
        //> 지금은 total score 보드 페이지로 연결 된다.
        //> PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSids.leaderboard_total_score);
    }
    //====================================================
}
