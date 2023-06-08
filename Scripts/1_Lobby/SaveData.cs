using UnityEngine;
using System.Collections.Generic;
//> Convert.ToBase64String
using System;
//> Memory System
using System.IO;
//> Binary Format
using System.Runtime.Serialization.Formatters.Binary;

public enum KEY
{
    TABLE,
    CURRENT_STAGE_NUMBER,
    TOTAL_SCORE
}

public partial class GameData : MonoBehaviour
{
    //============================================
    public static void Save<T>(ref T component, KEY key)
    {
        //> save current tables
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, component);
        PlayerPrefs.SetString(key.ToString(), Convert.ToBase64String(ms.GetBuffer()));
    }
    //============================================
    public static void Load<T>(ref T component, KEY key)
    {
        string data = PlayerPrefs.GetString(key.ToString());

        if (!string.IsNullOrEmpty(data))
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(Convert.FromBase64String(data));
            component = (T)bf.Deserialize(ms);
        }
        else
        {
            if (key == KEY.TABLE)
            {
                string filePath = "1_Lobby/2_Text/Data";
                TextAsset asset = (TextAsset)Resources.Load(filePath, typeof(TextAsset));

                if(asset == null)
                {
                    Debug.LogError("Data is not exist");
                    return;
                }

                TextReader reader = new StringReader(asset.text);
                string text = reader.ReadLine();
                int length = int.Parse(text);

                for (int i = 0; i < length; i++)
                {
                    text = reader.ReadLine();
                    string[] stageData = text.Split(',');

                    Side startUseSide = Side.Blue;
                    switch (stageData[8])
                    {
                        case "GRAY": startUseSide = Side.Gray; break;
                        case "BLUE": startUseSide = Side.Blue; break;
                        case "RED": startUseSide = Side.Red; break;
                        case "GREEN": startUseSide = Side.Green; break;
                    }

                    Side maxUseSide = Side.Blue;
                    switch (stageData[9])
                    {
                        case "GRAY": maxUseSide = Side.Gray; break;
                        case "BLUE": maxUseSide = Side.Blue; break;
                        case "RED": maxUseSide = Side.Red; break;
                        case "GREEN": maxUseSide = Side.Green; break;
                    }
                    
                    Table tb = new Table
                        (
                        int.Parse(stageData[0]),
                        int.Parse(stageData[1]),
                        int.Parse(stageData[2]),
                        int.Parse(stageData[3]),
                        int.Parse(stageData[4]),
                        bool.Parse(stageData[5]),
                        bool.Parse(stageData[6]),
                        bool.Parse(stageData[7]),
                        startUseSide,
                        maxUseSide
                        );

                    table.Add(tb);
                }
            }

            if (key == KEY.CURRENT_STAGE_NUMBER)
            {
                currentStageNumber = 1;
            }

            if(key == KEY.TOTAL_SCORE)
            {
                totalScore = 0;
            }
        }
    }
    //============================================
}
