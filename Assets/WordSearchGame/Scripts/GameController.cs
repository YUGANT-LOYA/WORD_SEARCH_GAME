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
        public LevelDataInfo levelDataInfo;
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
            VERTICAL,
            HORIZONTAL,
            STRAIGHT_DIAGONAL,
            REVERSE_DIAGONAL
        }



        private void Awake()
        {
            CreateSingleton();
            ClearLevelContainer();
            GameStartInfo();
            CreateLevel();
        }

        private void Start()
        {
            
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
            AssignLevelData();
            currLevel.AssignLevelHandler(levelHandler);
            levelHandler.AssignLevel(currLevel);
        }

        private void AssignLevelData()
        {
            currLevel.gridSize = levelDataInfo.levelInfo[DataHandler.Instance.CurrLevelNumber].gridSize;

        }

        public string GetGridDataOfLevel()
        {
            return levelDataInfo.levelInfo[DataHandler.Instance.CurrLevelNumber].gridData;
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
            CreateLevel();
            ResetData();
        }

        void ResetData()
        {
            levelHandler.inputGridsList.Clear();
            levelHandler.totalGridsList.Clear();
        }
    }
}
