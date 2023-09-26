using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class Level : MonoBehaviour
    {
        [Header("Main Info")]
        //public Vector2 lineOffset;
        private LevelHandler _levelHandler;

        public Vector2Int gridSize;
        private readonly int[] _randomScreenPointArr = { -1, 1 };
        public Ease gridPlacementEase;

        [Tooltip("The Distance outside the screen from where the grid should come and place in the grid")]
        public float distanceFromScreen = 250f;

        [Tooltip("Time to place Grid in its original Position")]
        public float timeToPlaceGrid = 0.4f;

        [SerializeField] Button restartButton, hintButton;
        public Transform rotationContainer, midPanelContainerTrans;
        [SerializeField] Transform gridContainer, lineParentTrans, quesParentTrans, hintContainer;
        [SerializeField] LineRenderer lineRenderer;
        private float _currGridWidth, _currGridHeight, _lineRendererWidth = 0.4f;
        [SerializeField] private float gridSpacing = 10f;
        private List<QuesGrid> _quesList;

        [Header("Input Data Info")] [SerializeField]
        TextMeshProUGUI touchText, levelNumText;

        public string TouchTextData
        {
            get => touchText.text;
            set => touchText.text = value;
        }

        public string LevelNumData
        {
            get => levelNumText.text;
            set => levelNumText.text = value;
        }

        public void StartInit()
        {
            //Debug.Log("Level StartInit Called !");
            SetGridSize();
            CreateGrid();
            SetLineRendererWidth();
            _levelHandler.GenerateNewLine();
            InitQuesList(_levelHandler.wordList);
            LevelNumData = $"Level {DataHandler.instance.CurrLevelNumber + 1}";
        }

        public void FillData(LevelHandler handler)
        {
            AssignLevelHandler(handler);
            restartButton.onClick.AddListener(() => { GameController.instance.RestartLevel(); });
            hintButton.onClick.AddListener(() => { _levelHandler.ShowHint(); });
        }

        public float GetLineRendererWidth()
        {
            return _lineRendererWidth;
        }

        private void SetLineRendererWidth()
        {
            var size = _currGridWidth > _currGridHeight ? _currGridHeight : _currGridWidth;
            _lineRendererWidth = size / 250f;
        }

        public Transform GetLineParentTrans()
        {
            return lineParentTrans;
        }

        public LineRenderer GetLineRenderer()
        {
            return lineRenderer;
        }

        public Transform GetHintContainer()
        {
            return hintContainer;
        }

        public void SetLineRenderer(LineRenderer line, Color color)
        {
            lineRenderer = line;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }

        public void SetLineRendererPoint(int index, Vector2 mousePos)
        {
            RectTransform gridRect = gridContainer.GetComponent<RectTransform>();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRect, mousePos, _levelHandler.cam,
                out var canvasMousePos);

            // Set the position in the LineRenderer
            lineRenderer.SetPosition(index, canvasMousePos);
        }

        private void AssignLevelHandler(LevelHandler handler)
        {
            _levelHandler = handler;
        }

        public Transform GetGridContainerTrans()
        {
            return gridContainer;
        }

        void SetGridSize()
        {
            float width = gridContainer.GetComponent<RectTransform>().sizeDelta.x;
            float height = gridContainer.GetComponent<RectTransform>().sizeDelta.y;
            //Debug.Log($"Width : {width} , Height : {height}");
            float spacingX = (gridSize.y - 1) * gridSpacing;
            float spacingY = (gridSize.x - 1) * gridSpacing;

            float gridWidth = (width - spacingX) / (gridSize.y);
            float gridHeight = (height - spacingY) / (gridSize.x);

            _currGridWidth = gridWidth;
            _currGridHeight = gridHeight;
        }

        Vector2 GetRandomPointOutOfScreen()
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float randomX = 0f, randomY = 0f;
            int side = Random.Range(0, _randomScreenPointArr.Length);

            //Side  = -1 means Horizontal and Side = 1 means Vertical
            if (_randomScreenPointArr[side] == -1)
            {
                int xPoint = Random.Range(0, _randomScreenPointArr.Length);

                if (_randomScreenPointArr[xPoint] == -1)
                {
                    //When the side is selected as Left Side.
                    randomX = Random.Range(-screenWidth / 2 - distanceFromScreen,
                        -screenWidth / 2 - (distanceFromScreen * 2));
                    randomY = Random.Range(-screenHeight / 2, screenHeight);
                }
                else if (_randomScreenPointArr[xPoint] == 1)
                {
                    //When the side is selected as Right Side.
                    randomX = Random.Range(screenWidth / 2 + distanceFromScreen,
                        screenWidth / 2 + (distanceFromScreen * 2));
                    randomY = Random.Range(-screenHeight / 2, screenHeight);
                }
            }
            else
            {
                int yPoint = Random.Range(0, _randomScreenPointArr.Length);

                if (_randomScreenPointArr[yPoint] == -1)
                {
                    //When the side is selected as Bottom Side.
                    randomX = Random.Range(-screenWidth / 2, -screenWidth / 2 - distanceFromScreen);
                    randomY = Random.Range(-screenHeight / 2 - distanceFromScreen,
                        -screenHeight / 2 - (distanceFromScreen * 2));
                }
                else if (_randomScreenPointArr[yPoint] == 1)
                {
                    //When the side is selected as Top Side.
                    randomX = Random.Range(screenWidth, screenWidth + distanceFromScreen);
                    randomY = Random.Range(screenHeight / 2 + distanceFromScreen,
                        screenHeight / 2 + (distanceFromScreen * 2));
                }
            }

            return new Vector2(randomX, randomY);
        }

        private void CreateGrid()
        {
            GameObject gridPrefab = DataHandler.instance.gridPrefab;
            RectTransform gridRect = gridContainer.GetComponent<RectTransform>();
            var gridContainerSize = gridRect.sizeDelta;
            float defaultXPos = (-gridContainerSize.x / 2) + _currGridWidth / 2;
            float defaultYPos = (gridContainerSize.y / 2) - _currGridHeight / 2;
            Vector2 startPos = new Vector2(defaultXPos, defaultYPos);
            //Debug.Log("Start Pos : " + startPos);
            for (int i = 0; i < gridSize.x; i++)
            {
                for (int j = 0; j < gridSize.y; j++)
                {
                    GameObject gmObj = Instantiate(gridPrefab, gridContainer);
                    RectTransform gmRect = gmObj.GetComponent<RectTransform>();
                    gmRect.sizeDelta = new Vector2(_currGridWidth, _currGridHeight);
                    gmRect.anchoredPosition = GetRandomPointOutOfScreen();
                    gmObj.name = $"Grid_{i}_{j}";
                    Grid gridScript = gmObj.GetComponent<Grid>();
                    gridScript.GridID = new Vector2Int(i, j);
                    _levelHandler.totalGridsList.Add(gridScript);
                    AssignGridData(gridScript, i, j);
                    gmRect.transform.DOLocalMove(startPos, timeToPlaceGrid).SetEase(gridPlacementEase);
                    
                    if (DataHandler.instance.CurrLevelNumber < _levelHandler.showGridTillLevel)
                    {
                        Image gridContainerImg = gridContainer.GetComponent<Image>();
                        Color color = gridContainerImg.color;
                        gridContainerImg.color = new Color(color.r, color.g, color.b, 0f);
                        //gridContainerLayoutGroup.spacing = new Vector2(10f, 10f);
                        gridScript.SetGridBg(true);
                    }
                    else
                    {
                        gridScript.SetGridBg(false);
                    }

                    startPos += new Vector2(gridSpacing + _currGridWidth, 0);
                }

                startPos.x = defaultXPos;
                startPos.y = defaultYPos - ((i + 1) * _currGridHeight) - (gridSpacing * (i + 1));
            }
        }

        private void AssignGridData(Grid gridScript, int row, int column)
        {
            string str = _levelHandler.gridData[row][column].ToString().ToUpper();
            gridScript.GridTextData = str;

            if (string.IsNullOrWhiteSpace(str))
            {
                gridScript.isSelectable = false;
            }
        }

        private void ResetQuesData()
        {
            for (int i = quesParentTrans.childCount - 1; i >= 0; i--)
            {
                Destroy(quesParentTrans.GetChild(i).gameObject);
            }

            _quesList = new List<QuesGrid>();
        }

        private void InitQuesList(List<LevelHandler.LevelWords> wordsList)
        {
            ResetQuesData();

            GameObject quesPrefab = DataHandler.instance.quesPrefab;

            foreach (var word in wordsList)
            {
                GameObject ques = Instantiate(quesPrefab, quesParentTrans);
                QuesGrid quesGridScript = ques.GetComponent<QuesGrid>();
                _quesList.Add(quesGridScript);
                quesGridScript.QuesTextData = word.wordInfo.word;
            }
        }

        public void UpdateQuesList(string ans)
        {
            foreach (var ques in _quesList)
            {
                if (ques.QuesTextData == ans && !ques.isMarked)
                {
                    Debug.Log("Ques Matched !");
                    ques.isMarked = true;
                    ques.StrikeQues();
                }
            }
        }

        public QuesGrid GetQuesGrid(string ans, string revStr)
        {
            foreach (QuesGrid quesGrid in _quesList)
            {
                if (quesGrid.QuesTextData == ans || quesGrid.QuesTextData == revStr)
                {
                    return quesGrid;
                }
            }

            return null;
        }

        public bool IsAllQuesMarked()
        {
            foreach (QuesGrid quesGrid in _quesList)
            {
                if (!quesGrid.isMarked)
                {
                    return false;
                }
            }

            return true;
        }
    }
}