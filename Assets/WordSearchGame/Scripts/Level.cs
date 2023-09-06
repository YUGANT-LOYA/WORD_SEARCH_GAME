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
        [SerializeField] GameObject gridPrefab;
        [SerializeField] Transform gridContainer, lineParentTrans;
        [SerializeField] GridLayoutGroup gridContainerLayoutGroup;
        [SerializeField] Image lineImg;
        public Vector2Int gridSize;
        float currGridWidth, currGridHeight;

        [Header("Input Data Info")]
        public TextMeshProUGUI touchText;
        [SerializeField] float defaultLineWidth = 100f;
        [SerializeField] float defaultLineHeight = 125f;

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



        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        private void Awake()
        {
            Init();
            SetGridLayout();
            SetGridSize();
        }

        private void Start()
        {
            CreateGrid();
        }

        void Init()
        {

        }

        public Vector2 GetDefaultLineSize()
        {
            return new Vector2(defaultLineWidth, defaultLineHeight);
        }

        public RectTransform GetLineImgRect()
        {
            return lineImg.GetComponent<RectTransform>();
        }

        public Image GetLineImg()
        {
            return lineImg;
        }

        public CapsuleCollider2D GetLineImgCollider()
        {
            return lineImg.gameObject.GetComponent<CapsuleCollider2D>();
        }

        public Transform GetLineParentTrans()
        {
            return lineParentTrans;
        }

        public void AssignLevelHandler(LevelHandler handler)
        {
            levelHandler = handler;
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
                    GenerateRandom_ASCII_Code(gridScript);
                    gridScript.SetBoxColliderSize(currGridWidth / 2, currGridHeight / 2);
                    gridScript.SetLineColorTransform(currGridWidth, currGridHeight);
                }
            }

            GetLineImgRect().sizeDelta = new Vector2(defaultLineWidth, defaultLineHeight);

        }

        void GenerateRandom_ASCII_Code(Grid grid)
        {
            int randomASCII_Val = Random.Range(065, 091);
            char letter = (char)randomASCII_Val;
            grid.gridTextData = letter.ToString();
        }

    }

}
