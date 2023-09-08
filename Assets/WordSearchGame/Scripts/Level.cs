using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class Level : MonoBehaviour
    {
        [Header("Main Info")]
        LevelHandler levelHandler;
        public Vector2Int gridSize;
        [SerializeField] GameObject gridPrefab;
        [SerializeField] Transform gridContainer, lineParentTrans;
        [SerializeField] GridLayoutGroup gridContainerLayoutGroup;
        [SerializeField] LineRenderer lineRenderer;

        [Header("Grid Data")]
        [TextArea(5, 5)]
        public string gridData;

        float currGridWidth, currGridHeight;

        [Header("Input Data Info")]
        public TextMeshProUGUI touchText;
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


        private void Awake()
        {
            SetGridLayout();
            SetGridSize();
        }

        private void Start()
        {
            StartInit();
            CreateGrid();
        }

        void StartInit()
        {

        }

        public Transform GetLineParentTrans()
        {
            return lineParentTrans;
        }

        public LineRenderer GetLineRenderer()
        {
            return lineRenderer;
        }

        public void SetLineRendererPoint(int index, Vector2 pos)
        {
            Vector2 canvasMousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(levelHandler.canvasRectTrans, pos, levelHandler.cam, out canvasMousePos);

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

            Debug.Log($"Width : {width} , Height : {height}");
            float spacingX = (gridSize.x - 1) * gridContainerLayoutGroup.spacing.x;
            float spacingY = (gridSize.y - 1) * gridContainerLayoutGroup.spacing.y;

            float gridWidth = (float)(width - spacingX) / (float)(gridSize.x);
            float gridHeight = (float)(height - spacingY) / (float)(gridSize.y);

            currGridWidth = gridWidth;
            currGridHeight = gridHeight;

            Debug.Log($"Cell Width : {currGridWidth} , Cell Height : {currGridHeight}");

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
                    AssignGridData(i,j);
                    GenerateRandom_ASCII_Code(gridScript);
                    gridScript.SetBoxColliderSize(currGridWidth / 2, currGridHeight / 2);
                    gridScript.SetLineColorTransform(currGridWidth, currGridHeight);
                }
            }

        }

        private void AssignGridData(int row , int column)
        {
            string gridData = GameController.Instance.GetGridDataOfLevel();

           

            for(int i = 0;i < gridData.Length;i++)
            {

            }
           
        }

        void GenerateRandom_ASCII_Code(Grid grid)
        {
            int randomASCII_Val = Random.Range(065, 091);
            char letter = (char)randomASCII_Val;
            grid.gridTextData = letter.ToString();
        }

    }

}
