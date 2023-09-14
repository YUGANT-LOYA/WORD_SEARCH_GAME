using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static YugantLoyaLibrary.WordSearchGame.LevelHandler;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class Level : MonoBehaviour
    {
        [Header("Main Info")]
        //public Vector2 lineOffset;
        LevelHandler levelHandler;
        public Vector2Int gridSize;
        [SerializeField] Button restartButton, hintButton;
        public Transform rotationContainer, midPanelContainerTrans;
        [SerializeField] GameObject gridPrefab, quesPrefab;
        [SerializeField] Transform gridContainer, lineParentTrans, quesParentTrans, hintContainer;
        [SerializeField] GridLayoutGroup gridContainerLayoutGroup;
        [SerializeField] LineRenderer lineRenderer;
        float currGridWidth, currGridHeight, lineRendererWidth = 0.4f;
        List<QuesGrid> quesList;
        [Header("Input Data Info")]
        [SerializeField] TextMeshProUGUI touchText, levelNumText;

        public string touchTextData
        {
            get
            {
                return touchText.text;
            }
            set
            {
                touchText.text = value;
            }
        }

        public string levelNumData
        {
            get
            {
                return levelNumText.text;
            }
            set
            {
                levelNumText.text = value;
            }
        }

        public void StartInit()
        {
            //Debug.Log("Level StartInit Called !");
            SetGridLayout();
            SetGridSize();
            CreateGrid();
            SetLineRendererWidth();
            levelHandler.GenerateNewLine();
            InitQuesList(levelHandler.wordList);
            levelNumData = $"Level {DataHandler.Instance.CurrLevelNumber + 1}";
        }

        public void FillData(LevelHandler handler)
        {
            AssignLevelHandler(handler);
            restartButton.onClick.AddListener(() => { GameController.Instance.RestartLevel(); });
            hintButton.onClick.AddListener(() => { levelHandler.ShowHint(); });
        }

        public float GetLineRendererWidth()
        {
            return lineRendererWidth;
        }

        private void SetLineRendererWidth()
        {
            float size;
            if (currGridWidth > currGridHeight)
            {
                size = currGridHeight;
            }
            else
            {
                size = currGridWidth;
            }

            lineRendererWidth = size / 250f;

            //lineOffset = midPanelContainerTrans.GetComponent<RectTransform>().anchoredPosition;
            //Debug.Log("Line Offset : " + lineOffset);
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
            Vector2 canvasMousePos;

            RectTransform gridRect = gridContainer.GetComponent<RectTransform>();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRect, mousePos, levelHandler.cam, out canvasMousePos);

            // Set the position in the LineRenderer
            lineRenderer.SetPosition(index, canvasMousePos);

        }

        public void AssignLevelHandler(LevelHandler handler)
        {
            levelHandler = handler;
        }

        public Transform GetGridContainerTrans()
        {
            return gridContainer;
        }

        void SetGridLayout()
        {

            int gridLength;

            //To confirm that grid is square and should have all element filled.
            if (gridSize.x >= gridSize.y)
            {
                gridLength = gridSize.x;
            }
            else
            {
                gridLength = gridSize.y;
            }

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

            float gridWidth = (float)(width - spacingX) / (float)(gridSize.y);
            float gridHeight = (float)(height - spacingY) / (float)(gridSize.x);

            currGridWidth = gridWidth;
            currGridHeight = gridHeight;

            //Debug.Log($"Cell Width : {currGridWidth} , Cell Height : {currGridHeight}");

            gridContainerLayoutGroup.cellSize = new Vector2(currGridWidth, currGridHeight);
        }

        void CreateGrid()
        {
            for (int i = 0; i < gridSize.x; i++)
            {
                for (int j = 0; j < gridSize.y; j++)
                {
                    GameObject gmObj = Instantiate(gridPrefab, gridContainer);
                    gmObj.name = $"Grid_{i}_{j}";
                    Grid gridScript = gmObj.GetComponent<Grid>();
                    gridScript.gridID = new Vector2Int(i, j);
                    levelHandler.totalGridsList.Add(gridScript);
                    AssignGridData(gridScript, i, j);

                   
                    
                    if (DataHandler.Instance.CurrLevelNumber < levelHandler.showGridTillLevel)
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
            gridScript.gridTextData = levelHandler.gridData[row][column].ToString().ToUpper();
        }

        public void ResetQuesData()
        {
            for (int i = quesParentTrans.childCount - 1; i >= 0; i--)
            {
                Destroy(quesParentTrans.GetChild(i).gameObject);
            }

            quesList = new List<QuesGrid>();
        }

        public void InitQuesList(List<LevelWords> wordsList)
        {
            ResetQuesData();

            for (int i = 0; i < wordsList.Count; i++)
            {
                GameObject ques = Instantiate(quesPrefab, quesParentTrans);
                QuesGrid quesGridScript = ques.GetComponent<QuesGrid>();
                quesList.Add(quesGridScript);
                quesGridScript.quesTextData = wordsList[i].wordInfo.word;
            }
        }

        public void UpdateQuesList(string ans)
        {
            for (int i = 0; i < quesList.Count; i++)
            {
                if (quesList[i].quesTextData == ans && !quesList[i].isMarked)
                {
                    Debug.Log("Ques Matched !");
                    quesList[i].isMarked = true;
                    quesList[i].StrikeQues();
                }
            }
        }

        public QuesGrid GetQuesGrid(string ans, string revStr)
        {
            foreach (QuesGrid quesGrid in quesList)
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
            bool isComplete = true;

            for (int i = 0; i < quesList.Count; i++)
            {
                QuesGrid quesGrid = quesList[i];

                if (!quesGrid.isMarked)
                {
                    isComplete = false;
                    return isComplete;
                }
            }

            return isComplete;
        }

    }

}
