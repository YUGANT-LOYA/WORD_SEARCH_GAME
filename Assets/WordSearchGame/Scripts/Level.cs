using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static YugantLoyaLibrary.WordSearchGame.LevelHandler;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class Level : MonoBehaviour
    {
        [Header("Main Info")]
        //public Vector2 lineOffset;
        private LevelHandler _levelHandler;
        public Vector2Int gridSize;
        [SerializeField] Button restartButton, hintButton;
        public Transform rotationContainer, midPanelContainerTrans;
        [SerializeField] Transform gridContainer, lineParentTrans, quesParentTrans, hintContainer;
        [SerializeField] GridLayoutGroup gridContainerLayoutGroup;
        [SerializeField] LineRenderer lineRenderer;
        private float _currGridWidth, _currGridHeight, _lineRendererWidth = 0.4f;
        private List<QuesGrid> _quesList;
        [Header("Input Data Info")]
        [SerializeField] TextMeshProUGUI touchText, levelNumText;

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
            SetGridLayout();
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

            RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRect, mousePos, _levelHandler.cam, out var canvasMousePos);

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

        void SetGridLayout()
        {
            //To confirm that grid is square and should have all element filled.
            int gridLength = gridSize.x >= gridSize.y ? gridSize.x : gridSize.y;

            //gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridContainerLayoutGroup.constraintCount = gridLength;
        }

        void SetGridSize()
        {
            float width = gridContainer.GetComponent<RectTransform>().sizeDelta.x;
            float height = gridContainer.GetComponent<RectTransform>().sizeDelta.y;

            //Debug.Log($"Width : {width} , Height : {height}");
            float spacingX = (gridSize.y - 1) * gridContainerLayoutGroup.spacing.y;
            float spacingY = (gridSize.x - 1) * gridContainerLayoutGroup.spacing.x;

            float gridWidth = (width - spacingX) / (gridSize.y);
            float gridHeight = (height - spacingY) / (gridSize.x);

            _currGridWidth = gridWidth;
            _currGridHeight = gridHeight;

            //Debug.Log($"Cell Width : {currGridWidth} , Cell Height : {currGridHeight}");

            gridContainerLayoutGroup.cellSize = new Vector2(_currGridWidth, _currGridHeight);
        }

        void CreateGrid()
        {
            GameObject gridPrefab = DataHandler.instance.gridPrefab; 
            for (int i = 0; i < gridSize.x; i++)
            {
                for (int j = 0; j < gridSize.y; j++)
                {
                    GameObject gmObj = Instantiate(gridPrefab, gridContainer);
                    gmObj.name = $"Grid_{i}_{j}";
                    Grid gridScript = gmObj.GetComponent<Grid>();
                    gridScript.GridID = new Vector2Int(i, j);
                    _levelHandler.totalGridsList.Add(gridScript);
                    AssignGridData(gridScript, i, j);

                   
                    
                    if (DataHandler.instance.CurrLevelNumber < _levelHandler.showGridTillLevel)
                    {
                        Image gridContainerImg = gridContainer.GetComponent<Image>();
                        Color color = gridContainerImg.color;
                        gridContainerImg.color = new Color(color.r, color.g, color.b,0f);
                        gridContainerLayoutGroup.spacing = new Vector2(10f, 10f);
                        gridScript.SetGridBg(true);
                    }
                    else
                    {
                        gridScript.SetGridBg(false);
                    }
                }
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

        private void InitQuesList(List<LevelWords> wordsList)
        {
            ResetQuesData();

            GameObject quesPrefab = DataHandler.instance.quesPrefab;
            
            foreach (var word in wordsList)
            {
                GameObject ques = Instantiate(quesPrefab, quesParentTrans);
                QuesGrid quesGridScript = ques.GetComponent<QuesGrid>();
                _quesList.Add(quesGridScript);
                quesGridScript.quesTextData = word.wordInfo.word;
            }
        }

        public void UpdateQuesList(string ans)
        {
            foreach (var ques in _quesList)
            {
                if (ques.quesTextData == ans && !ques.isMarked)
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
                if (quesGrid.quesTextData == ans || quesGrid.quesTextData == revStr)
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
