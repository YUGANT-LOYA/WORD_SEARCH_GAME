using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
        [SerializeField] CanvasGroup fadeCanvasGroup;
        Level currLevel;


        [SerializeField] float timeToSwitchToNextLevel = 1f;

        public enum InputDirection
        {
            NONE,
            TOP,
            BOTTOM,
            LEFT,
            RIGHT,
            TOP_LEFT,
            TOP_RIGHT,
            BOTTOM_LEFT,
            BOTTOM_RIGHT,
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
        }

        private void Start()
        {
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
            AssignLevelData();
        }

        private void AssignLevelData()
        {
            currLevel.AssignLevelHandler(levelHandler);
            levelHandler.AssignLevel(currLevel);
            levelHandler.LevelStartInit();
            currLevel.gridSize = GetLevelDataInfo().gridSize;
            levelHandler.GetGridData();
            currLevel.StartInit();
        }

        public TextAsset GetGridDataOfLevel()
        {
            return levelDataInfo.levelInfo[DataHandler.Instance.CurrLevelNumber].level_CSV;
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
            levelHandler.wordList.Clear();
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

            StartCoroutine(nameof(FadeScreen));
        }

        public void PreviousLevel()
        {
            if (DataHandler.Instance.CurrLevelNumber > 0)
            {
                DataHandler.Instance.CurrLevelNumber--;
            }
            else
            {
                DataHandler.Instance.CurrLevelNumber = levelDataInfo.levelInfo.Count - 1;
            }

            StartCoroutine(nameof(FadeScreen));
        }

        void FadeScreen()
        {
            fadeCanvasGroup.DOFade(1f, timeToSwitchToNextLevel / 2f).OnComplete(() =>
            {
                RestartLevel();
                fadeCanvasGroup.DOFade(0f, timeToSwitchToNextLevel / 2f).OnComplete(() =>
                {

                });
            });
        }
    }
}
