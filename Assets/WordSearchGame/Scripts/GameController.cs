using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;

        [Header("References")]
        [SerializeField] UI_Manager ui_Manager;
        [SerializeField] LevelDataInfo levelDataInfo;
        [SerializeField] LevelHandler levelHandler;
        [SerializeField] Transform levelContainer;
        [SerializeField] GameObject levelPrefab;
        Level currLevel;

        public enum InputDirection
        {
            NONE,
            TOP_LEFT,
            TOP_RIGHT,
            BOTTOM_LEFT,
            BOTTOM_RIGHT,
            BOTTOM,
            LEFT,
            RIGHT,
            TOP
        }

        public enum Direction
        {
            NONE,
            VERTICAL,
            HORIZONTAL,
            STRAIGHT_DIAGONAL,
            REVERSE_DIAGONAL
        }



        private void Awake()
        {
            CreateSingleton();
            GameStartInfo();
            RestartLevel();
        }



        void CreateSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        void CreateLevel()
        {
            GameObject level = Instantiate(levelPrefab, levelContainer);
            currLevel = level.GetComponent<Level>();
            currLevel.AssignLevelHandler(levelHandler);
            levelHandler.AssignLevel(currLevel);
            levelHandler.Init();
            AssignLevelData();
        }

        private void AssignLevelData()
        {
            currLevel.gridSize = GetLevelDataInfo().gridSize;
            ResetData();
            levelHandler.GetGridData();
            currLevel.StartInit();
        }

        public string GetGridDataOfLevel()
        {
            return levelDataInfo.levelInfo[DataHandler.Instance.CurrLevelNumber].gridData;
        }

        public LevelDataInfo.LevelInfo GetLevelDataInfo()
        {
            Debug.Log("Curr Level Num : " + DataHandler.Instance.CurrLevelNumber);
            return levelDataInfo.levelInfo[DataHandler.Instance.CurrLevelNumber];
        }

        void GameStartInfo()
        {
            DataHandler.Instance.CurrLevelNumber = 0;
        }

        void ClearLevelContainer()
        {
            if (levelContainer.childCount > 0)
            {
                for (int i = levelContainer.childCount - 1; i >= 0; i--)
                {
                    Destroy(levelContainer.GetChild(i).gameObject);
                }
            }
        }

        public UI_Manager GetUIManager()
        {
            return ui_Manager;
        }

        public void RestartLevel()
        {
            ClearLevelContainer();
            ResetData();
            CreateLevel();
        }

        void ResetData()
        {
            levelHandler.inputGridsList.Clear();
            levelHandler.totalGridsList.Clear();
            levelHandler.answerList.Clear();
        }

        public void NextLevel()
        {
            if (DataHandler.Instance.CurrLevelNumber < levelDataInfo.levelInfo.Count - 1)
            {
                DataHandler.Instance.CurrLevelNumber++;
            }
            else
            {
                DataHandler.Instance.CurrLevelNumber = 0;
            }

            RestartLevel();
        }
    }
}
