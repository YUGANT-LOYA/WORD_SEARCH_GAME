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
        [SerializeField] GameObject gridPrefab;
        [SerializeField] Transform gridContainer;
        [SerializeField] GridLayoutGroup gridContainerLayoutGroup;
        public Vector2Int gridSize;
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
            Init();
            SetGridLayout();
            SetGridSize();
            CreateGrid();
        }

        void Init()
        {

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
            float spacingX =  (gridSize.x - 1) * gridContainerLayoutGroup.spacing.x;
            float spacingY = (gridSize.y - 1) * gridContainerLayoutGroup.spacing.y;

            float gridWidth = (float)(width - spacingX) / (float)(gridSize.x);
            float gridHeight = (float)(height - spacingY)/ (float)(gridSize.y);

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
                    //gmObj.GetComponent<Grid>().UpdateColliderSize(currGridWidth, currGridHeight);
                }
            }
        }

       
    }

}
