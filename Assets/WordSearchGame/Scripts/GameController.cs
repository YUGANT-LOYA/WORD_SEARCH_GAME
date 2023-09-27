using UnityEngine;
using DG.Tweening;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class GameController : MonoBehaviour
    {
        public static GameController instance;

        [Header("References")] public UIManager uiManager;
        [SerializeField] private LevelDataInfo levelDataInfo;
        [SerializeField] private LevelHandler levelHandler;
        [SerializeField] private Transform levelContainer;
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        public Transform coinContainerTran;
        public int coinPoolSize = 20;
        private Level _currLevel;
        private bool _isRestarting;

        [SerializeField] private float timeToSwitchToNextLevel = 1f;

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
            //Application.targetFrameRate = 60;
            CreateSingleton();
        }

        private void Start()
        {
            GameStartInfo();
            StartGame();
        }

        private void CreateSingleton()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        void CreateLevel()
        {
            GameObject level = Instantiate(DataHandler.instance.levelPrefab, levelContainer);
            _currLevel = level.GetComponent<Level>();
            AssignLevelData();
        }

        private void AssignLevelData()
        {
            _currLevel.FillData(levelHandler);
            levelHandler.AssignLevel(_currLevel);
            levelHandler.LevelStartInit();
            _currLevel.gridSize = GetLevelDataInfo().gridSize;
            levelHandler.GetGridData();
            _currLevel.StartInit();
        }

        public TextAsset GetGridDataOfLevel()
        {
            return levelDataInfo.levelInfo[DataHandler.instance.CurrLevelNumber].levelCsv;
        }

        public LevelDataInfo.LevelInfo GetLevelDataInfo()
        {
            Debug.Log("Curr Level Num : " + DataHandler.instance.CurrLevelNumber);
            return levelDataInfo.levelInfo[DataHandler.instance.CurrLevelNumber];
        }

        void GameStartInfo()
        {
            DataHandler.instance.CurrLevelNumber = 0;
            uiManager.coinText.text = DataHandler.instance.initialCoins.ToString();
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

        public UIManager GetUIManager()
        {
            return uiManager;
        }

        public Level GetCurrentLevel()
        {
            return _currLevel;
        }

        public LevelHandler GetLevelHandler()
        {
            return levelHandler;
        }

        private void StartGame()
        {
            ClearLevelContainer();
            ResetData();
            CreateLevel();
        }

        public void RestartLevel()
        {
            if (_isRestarting)
                return;

            //StartGame();
            _isRestarting = true;
            RotateGridContainer();
        }

        private void RotateGridContainer()
        {
            Transform trans = _currLevel.rotationContainer;
            levelHandler.SetLevelRunningBool(false);
            Transform gridContainer = _currLevel.GetGridContainerTrans();


            if (_currLevel.gridSize.y == _currLevel.gridSize.x)
            {
                Debug.Log("Rotate If !");
                trans.DOScale(0.8f, levelHandler.timeToRotateGrid / 2).OnComplete(() =>
                {
                    trans.DOScale(1f, levelHandler.timeToRotateGrid / 2);
                });

                Quaternion localRotation = trans.localRotation;
                trans.DORotate(
                    new Vector3(localRotation.eulerAngles.x, localRotation.eulerAngles.y,
                        localRotation.eulerAngles.z + 90f), levelHandler.timeToRotateGrid).OnComplete(() =>
                {
                    _isRestarting = false;
                    levelHandler.SetLevelRunningBool(true);
                });

                for (int i = 0; i < gridContainer.childCount; i++)
                {
                    GameObject gm = gridContainer.GetChild(i).gameObject;
                    Quaternion rotation = gm.transform.localRotation;
                    gm.transform.DOLocalRotate(
                        new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z - 90f),
                        levelHandler.timeToRotateGrid);
                }
            }
            else
            {
                Debug.Log("Rotate Else !");

                trans.DOScale(0.8f, levelHandler.timeToRotateGrid).OnComplete(() =>
                {
                    trans.DOScale(1f, levelHandler.timeToRotateGrid);
                });

                Quaternion localRotation = trans.localRotation;
                trans.DORotate(
                    new Vector3(localRotation.eulerAngles.x, localRotation.eulerAngles.y,
                        localRotation.eulerAngles.z + 180f), levelHandler.timeToRotateGrid * 2).OnComplete(() =>
                {
                    _isRestarting = false;
                    levelHandler.SetLevelRunningBool(true);
                });

                for (int i = 0; i < gridContainer.childCount; i++)
                {
                    GameObject gm = gridContainer.GetChild(i).gameObject;
                    Quaternion rotation = gm.transform.localRotation;
                    gm.transform.DOLocalRotate(
                        new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z - 180f),
                        levelHandler.timeToRotateGrid * 2);
                }
            }
        }

        void ResetData()
        {
            levelHandler.inputGridsList.Clear();
            levelHandler.totalGridsList.Clear();
            levelHandler.wordList.Clear();
        }

        public void NextLevel()
        {
            if (DataHandler.instance.CurrLevelNumber < levelDataInfo.levelInfo.Count - 1)
            {
                DataHandler.instance.CurrLevelNumber++;
            }
            else
            {
                DataHandler.instance.CurrLevelNumber = 0;
            }

            StartCoroutine(nameof(FadeScreen));
            uiManager.winPanel.SetActive(false);
        }

        public void PreviousLevel()
        {
            if (DataHandler.instance.CurrLevelNumber > 0)
            {
                DataHandler.instance.CurrLevelNumber--;
            }
            else
            {
                DataHandler.instance.CurrLevelNumber = levelDataInfo.levelInfo.Count - 1;
            }

            StartCoroutine(nameof(FadeScreen));
        }

        void FadeScreen()
        {
            fadeCanvasGroup.DOFade(1f, timeToSwitchToNextLevel / 2f).OnComplete(() =>
            {
                StartGame();
                fadeCanvasGroup.DOFade(0f, timeToSwitchToNextLevel / 2f).OnComplete(() => { });
            });
        }
    }
}