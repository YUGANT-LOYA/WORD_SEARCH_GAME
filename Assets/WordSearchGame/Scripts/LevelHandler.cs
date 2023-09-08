using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.EventSystems;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class LevelHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [HideInInspector] public Camera cam;
        [SerializeField] Level currLevel;
        [SerializeField] Grid currGrid, startingGrid, targetGrid;
        [SerializeField] LayerMask gridLayerMask;
        public char[][] gridData;
        public List<Grid> totalGridsList, inputGridsList;

        public delegate void NewLetterDelegate(Grid grid);
        NewLetterDelegate OnNewLetterAddEvent, OnLetterRemoveEvent, OnDragInputEvent;

        public delegate void GameCompleteDelegate();
        GameCompleteDelegate OnGameCompleteEvent;

        public GameController.InputDirection draggingDirection;
        public GameController.Direction mainDir;

        Vector2 lastPos, startPos, currPos, inputMousePos;
        public RectTransform canvasRectTrans;

        private void OnEnable()
        {
            OnNewLetterAddEvent += AddNewLetter;
            OnDragInputEvent += DraggingInput;
        }

        private void OnDisable()
        {
            OnNewLetterAddEvent -= AddNewLetter;
            OnDragInputEvent -= DraggingInput;
        }

        private void Awake()
        {
            Init();
        }

        void Init()
        {
            cam = Camera.main;
            totalGridsList = new List<Grid>();
            inputGridsList = new List<Grid>();
        }

        public void AssignLevel(Level levelScript)
        {
            currLevel = levelScript;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            GameObject currObj = eventData.pointerCurrentRaycast.gameObject;

            if (currObj != null)
            {
                Grid gridScript = currObj.GetComponent<Grid>();

                if (IsLayerSame(currObj) && !IsGridExistInList(gridScript))
                {
                    //Debug.Log("GameObj : " + eventData.pointerCurrentRaycast.gameObject);
                    InputStartData(gridScript, eventData);
                }
            }


        }

        public void OnDrag(PointerEventData eventData)
        {
            GameObject currObj = eventData.pointerCurrentRaycast.gameObject;
            currPos = Input.mousePosition;
            Debug.Log("curr Pos : " + currPos);
            inputMousePos = Input.mousePosition;

            if (currObj != null && startingGrid != null && IsLayerSame(currObj))
            {
                Grid gridScript = currObj.GetComponent<Grid>();
                currGrid = gridScript;

                if (!IsGridExistInList(gridScript))
                {
                    OnDragInputEvent?.Invoke(gridScript);
                }

                GetGridLineLastPoint();
                SetTargetGrid();

                //Vector2 pos = ConvertLinePointToMousePos(1);
                //Vector3 linePoint = ConvertMouseToLineRendererPoint(pos);
                //Grid g = GetGrid(Input.mousePosition);
                //Vector2 wPos = ConvertTransformToMousePos(g.transform.position);
                //Debug.Log("w Pos : " + wPos);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            startingGrid = null;

            InputDataReset();
        }

        void InputStartData(Grid gridScript, PointerEventData pointerEventData)
        {
            inputMousePos = Input.mousePosition;
            startingGrid = gridScript;
            startPos = inputMousePos;
            currGrid = startingGrid;
            currLevel.GetLineRenderer().gameObject.SetActive(true);
            SetLinePoints(1, 0, startingGrid);
            OnNewLetterAddEvent?.Invoke(gridScript);
        }

        private void DraggingInput(Grid gridScript)
        {

        }


        Grid GetGridLineLastPoint()
        {
            if (currLevel.GetLineRenderer().positionCount == 2)
            {
                Vector2 pos = ConvertLinePointToMousePos(1);
                Grid grid = GetGridByMousePos(pos);

                if(grid != currGrid)
                {
                    Debug.Log("Not Same !");
                }
                else
                {
                    Debug.Log("Same !");
                }
            }
            return null;
        }

        public void SetLinePoints(int totalPoints, int index, Grid grid)
        {
            currLevel.GetLineRenderer().positionCount = totalPoints;
            Vector2 pos = ConvertTransformToMousePos(grid.transform.position);

            currLevel.SetLineRendererPoint(index, pos);
        }

        private Vector3 ConvertMouseToLineRendererPoint(Vector3 mousePosition)
        {
            Vector3 worldMousePosition = cam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, cam.nearClipPlane));
            Vector3 pos = transform.InverseTransformPoint(worldMousePosition);

            Debug.Log("linePoint : " + pos);

            return pos;
        }

        private Vector2 ConvertLinePointToMousePos(int index)
        {
            Vector3 worldPosition = currLevel.GetLineRenderer().transform.TransformPoint(currLevel.GetLineRenderer().GetPosition(index));

            // Convert world position to screen space
            Vector3 screenPosition = cam.WorldToScreenPoint(worldPosition);
            Debug.Log("World Pos : " + screenPosition);

            return screenPosition;
        }

        Vector2 ConvertTransformToMousePos(Vector2 transPos)
        {
            Vector2 pos = cam.WorldToScreenPoint(transPos);
            return pos;
        }

        Grid GetGridByMousePos(Vector3 mousePos)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = mousePos;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
            {
                GameObject hitObject = result.gameObject;



                if (IsLayerSame(hitObject))
                {
                    Grid grid = hitObject.GetComponent<Grid>();

                    if (grid != null)
                    {
                        Debug.Log("Grid Found : " + grid.gameObject.name);
                        return grid;
                    }
                }
            }

            return null;
        }

        bool IsGridExistInList(Grid gridScript)
        {
            if (inputGridsList.Contains(gridScript))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool IsLayerSame(GameObject gm)
        {
            return (gridLayerMask.value & (1 << gm.gameObject.layer)) > 0;
        }

        void AddNewLetter(Grid gridObj)
        {
            gridObj.isMarked = true;
            currLevel.touchTextData += gridObj.gridTextData;
            inputGridsList.Add(gridObj);
        }

        void CheckDirection()
        {
            // gridId x is behaving as column(j).
            //gridId y is behaving as row(i).
            //It is reverse in terms of x and y.
            int firstX = startingGrid.gridID.x;
            int firstY = startingGrid.gridID.y;
            int secondX = targetGrid.gridID.x;
            int secondY = targetGrid.gridID.y;

            if (secondX - firstX == 1)
            {
                if (secondY - firstY == 1)
                {
                    draggingDirection = GameController.InputDirection.BOTTOM_RIGHT;
                }
                else if (secondY - firstY == 0)
                {
                    draggingDirection = GameController.InputDirection.BOTTOM;
                }
                else if (secondY - firstY == -1)
                {
                    draggingDirection = GameController.InputDirection.BOTTOM_LEFT;
                }
            }
            else if (secondX - firstX == 0)
            {
                if (secondY - firstY == 1)
                {
                    draggingDirection = GameController.InputDirection.RIGHT;
                }
                else if (secondY - firstY == 0)
                {
                    draggingDirection = GameController.InputDirection.NONE;
                }
                else if (secondY - firstY == -1)
                {
                    draggingDirection = GameController.InputDirection.LEFT;
                }
            }
            else if (secondX - firstX == -1)
            {
                if (secondY - firstY == 1)
                {
                    draggingDirection = GameController.InputDirection.TOP_RIGHT;
                }
                else if (secondY - firstY == 0)
                {
                    draggingDirection = GameController.InputDirection.TOP;
                }
                else if (secondY - firstY == -1)
                {
                    draggingDirection = GameController.InputDirection.TOP_LEFT;
                }
            }

            //switch (draggingDirection)
            //{
            //    case GameController.InputDirection.NONE:

            //        Debug.Log("None !");

            //        break;

            //    case GameController.InputDirection.TOP:

            //        Debug.Log("TOP !");


            //        break;

            //    case GameController.InputDirection.TOP_LEFT:

            //        Debug.Log("TOP LEFT !");

            //        break;

            //    case GameController.InputDirection.TOP_RIGHT:

            //        Debug.Log("TOP RIGHT !");

            //        break;

            //    case GameController.InputDirection.BOTTOM:

            //        Debug.Log("BOTTOM !");


            //        break;

            //    case GameController.InputDirection.BOTTOM_LEFT:

            //        Debug.Log("BOTTOM LEFT !");

            //        break;

            //    case GameController.InputDirection.BOTTOM_RIGHT:

            //        Debug.Log("BOTTOM RIGHT !");

            //        break;

            //    case GameController.InputDirection.LEFT:

            //        Debug.Log("LEFT !");

            //        break;

            //    case GameController.InputDirection.RIGHT:

            //        Debug.Log("RIGHT !");

            //        break;
            //}

        }

        void InputDataReset()
        {
            for (int i = 0; i < inputGridsList.Count; i++)
            {
                if (!inputGridsList[i].isCorrect)
                {

                }
            }

            inputGridsList.Clear();
            currLevel.touchTextData = "";
            currGrid = null;
            startingGrid = null;
            draggingDirection = GameController.InputDirection.NONE;
            currLevel.GetLineRenderer().positionCount = 0;
            currLevel.GetLineRenderer().gameObject.SetActive(false);
        }

        void ResetListData()
        {
            inputGridsList.Clear();
            totalGridsList.Clear();
        }

        void SetTargetGrid()
        {
            //x behaves as i and y behaves as j.
            if (currGrid.gridID.x - startingGrid.gridID.x == 0)
            {
                //It means that constant X Axis.
                mainDir = GameController.Direction.HORIZONTAL;
                SetLinePoints(2, 1, currGrid);
                Debug.Log("X Direction Pattern Moving !");
            }
            else if (currGrid.gridID.y - startingGrid.gridID.y == 0)
            {
                //It means that constant Y Axis.
                mainDir = GameController.Direction.VERTICAL;
                SetLinePoints(2, 1, currGrid);
                Debug.Log("Y Direction Pattern Moving !");
            }
            else if (currGrid.gridID.y - startingGrid.gridID.y == currGrid.gridID.x - startingGrid.gridID.x)
            {
                //Straight Diagonal Direction Entered.
                mainDir = GameController.Direction.STRAIGHT_DIAGONAL;
                SetLinePoints(2, 1, currGrid);
                Debug.Log("Straight Diagonal Pattern Moving !");
            }
            else if (currGrid.gridID.y - startingGrid.gridID.y == -(currGrid.gridID.x - startingGrid.gridID.x))
            {
                //Reverse Diagonal Direction Entered.
                mainDir = GameController.Direction.REVERSE_DIAGONAL;
                SetLinePoints(2, 1, currGrid);
                Debug.Log("Reverse Diagonal Pattern Moving !");
            }
            else
            {
                Debug.Log("Random Pattern Moving !");
            }


            Vector2 pos = ConvertTransformToMousePos(currGrid.transform.position);
            targetGrid = GetGridByMousePos(pos);

            UpdateInputList();

        }

        void UpdateInputList()
        {
            inputGridsList.Clear();
            currLevel.touchTextData = "";

            // gridId x is behaving as row(i) .
            //gridId y is behaving as column(j).
            //It is reverse in terms of x and y.
            int firstX = startingGrid.gridID.x;
            int firstY = startingGrid.gridID.y;
            int secondX = targetGrid.gridID.x;
            int secondY = targetGrid.gridID.y;

            int xDiff = secondX - firstX;
            int yDiff = secondY - firstY;

            if (secondX - firstX > 0)
            {
                if (secondY - firstY > 0)
                {
                    draggingDirection = GameController.InputDirection.BOTTOM_RIGHT;

                    for (int i = 0; i <= xDiff; i++)
                    {
                        Grid grid = GetGridByGridID(firstX + i, firstY + i);
                        OnNewLetterAddEvent?.Invoke(grid);
                    }
                }
                else if (secondY - firstY == 0)
                {
                    draggingDirection = GameController.InputDirection.BOTTOM;

                    for (int i = firstX; i <= secondX; i++)
                    {
                        Grid grid = GetGridByGridID(i, firstY);
                        OnNewLetterAddEvent?.Invoke(grid);
                    }
                }
                else if (secondY - firstY < 0)
                {
                    draggingDirection = GameController.InputDirection.BOTTOM_LEFT;
                    int counter = 0;
                    for (int i = 0; i <= xDiff; i++)
                    {
                        Grid grid = GetGridByGridID(firstX + counter, firstY - i);
                        OnNewLetterAddEvent?.Invoke(grid);
                        counter++;
                    }
                }
            }
            else if (secondX - firstX == 0)
            {
                if (secondY - firstY > 0)
                {
                    draggingDirection = GameController.InputDirection.RIGHT;

                    for (int i = firstY; i <= secondY; i++)
                    {
                        Grid grid = GetGridByGridID(firstX, i);
                        OnNewLetterAddEvent?.Invoke(grid);
                    }
                }
                else if (secondY - firstY == 0)
                {
                    draggingDirection = GameController.InputDirection.NONE;
                    OnNewLetterAddEvent?.Invoke(startingGrid);
                }
                else if (secondY - firstY < 0)
                {
                    draggingDirection = GameController.InputDirection.LEFT;

                    for (int i = firstY; i >= secondY; i--)
                    {
                        Grid grid = GetGridByGridID(firstX, i);
                        OnNewLetterAddEvent?.Invoke(grid);
                    }
                }
            }
            else if (secondX - firstX < 0)
            {
                if (secondY - firstY > 0)
                {
                    draggingDirection = GameController.InputDirection.TOP_RIGHT;

                    int counter = 0;
                    for (int i = 0; i <= yDiff; i++)
                    {
                        Grid grid = GetGridByGridID(firstX - i, firstY + counter);
                        OnNewLetterAddEvent?.Invoke(grid);
                        counter++;
                    }
                }
                else if (secondY - firstY == 0)
                {
                    draggingDirection = GameController.InputDirection.TOP;

                    for (int i = firstX; i >= secondX; i--)
                    {
                        Grid grid = GetGridByGridID(i, firstY);
                        OnNewLetterAddEvent?.Invoke(grid);
                    }

                }
                else if (secondY - firstY < 0)
                {
                    draggingDirection = GameController.InputDirection.TOP_LEFT;

                    for (int i = 0; i >= yDiff; i--)
                    {
                        Grid grid = GetGridByGridID(firstX + i, firstY + i);
                        OnNewLetterAddEvent?.Invoke(grid);
                    }
                }
            }

        }

        Grid GetGridByGridID(int i, int j)
        {
            int index = i * currLevel.gridSize.x + j;
            Grid grid = currLevel.GetGridContainerTrans().GetChild(index).gameObject.GetComponent<Grid>();
            Debug.Log($"Index {index}, Grid Name : {grid.name}");
            return grid;
        }


        void CheckAnswer()
        {
            string ans = currLevel.touchTextData;

            //if(ans == || )
        }


    }
}
