using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace AnotherWorld.Core
{
    public interface ISaveLoadManager
    {
        public void SaveGameData(GameData gameData);
        public GameData LoadGameData();
        public void ClearGameData();
    }


    public class SaveLoadManager : Manager<SaveLoadManager>, ISaveLoadManager
    {
        public void SaveGameData(GameData gameData)
        {
            ES3Settings settings = new ES3Settings(ES3.Location.File);
            settings.path = "GameData.es3";

            FieldInfo[] fields = typeof(GameData).GetFields();
            foreach (var field in fields)
            {
                ES3.Save(field.Name, field.GetValue(gameData), settings);
            }

            //ES3.Save("level", gameData.level, settings);
            //ES3.Save("attemptNumber", gameData.attemptNumber, settings);
            //ES3.Save("haptics", gameData.hapticsEnabled, settings);
        }

        public GameData LoadGameData()
        {
            ES3Settings settings = new ES3Settings(ES3.Location.File);
            settings.path = "GameData.es3";
            GameData gameData = new GameData();

            if (!ES3.FileExists("GameData.es3"))
            {
                SaveGameData(new GameData());

                //ES3.Save("level", 0, settings);
                //ES3.Save("attemptNumber", 0, settings);
                //ES3.Save("haptics", true, settings);
            }

            FieldInfo[] fields = typeof(GameData).GetFields();
            foreach (var field in fields)
            {
                field.SetValue(gameData, ES3.Load(field.Name, field.GetValue(gameData), settings));
            }

            //gameData.level = ES3.Load("level", 0, settings);
            //gameData.attemptNumber = ES3.Load("attemptNumber", 0, settings);
            //gameData.hapticsEnabled = ES3.Load("haptics", true, settings);

            return gameData;
        }


        public void ClearGameData()
        {
            foreach (var directory in Directory.GetDirectories(Application.persistentDataPath))
            {
                DirectoryInfo data_dir = new DirectoryInfo(directory);
                data_dir.Delete(true);
            }

            foreach (var file in Directory.GetFiles(Application.persistentDataPath))
            {
                FileInfo file_info = new FileInfo(file);
                file_info.Delete();
            }
        }
    }


    [System.Serializable]
    public class GameData
    {
        public SettingsData settingsData;
        public float playTime = 0;
        public int level = 0;
        public int attemptNumber = 0;
        public int tripleWaveIndex = 0;
        public int coin = 0;
        public bool tutorial;
        public List<int> towerLevels;
        public List<List<BlockType>> towerCurrentBlocks;

        public GameData()
        {
            settingsData = new SettingsData();
            towerLevels = new List<int>();
            towerCurrentBlocks = new List<List<BlockType>>();
        }
    }



}


