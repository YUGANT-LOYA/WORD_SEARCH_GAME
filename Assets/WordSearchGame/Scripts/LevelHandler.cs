using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class LevelHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] Level currLevel;
        [SerializeField] Grid currGrid, startingGrid, secondGrid, endGrid;
        [SerializeField] LayerMask gridLayerMask;

        public List<Grid> totalGridsList, inputGridsList;

        public delegate void NewLetterDelegate(Grid grid);
        NewLetterDelegate OnNewLetterAddEvent, OnLetterRemoveEvent,OnDragInputEvent;

        public delegate void GameCompleteDelegate();
        GameCompleteDelegate OnGameCompleteEvent;

        public GameController.InputDirection draggingDirection;
        GameController.InputDirection currDirection;

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
                bool isLayerSame = (gridLayerMask.value & (1 << eventData.pointerCurrentRaycast.gameObject.layer)) > 0;
                Grid gridScript = currObj.GetComponent<Grid>();

                if (isLayerSame && !IsGridExistInList(gridScript))
                {
                    //Debug.Log("GameObj : " + eventData.pointerCurrentRaycast.gameObject);
                    InputStartData(gridScript,eventData);
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            GameObject currObj = eventData.pointerCurrentRaycast.gameObject;

            if (currObj != null)
            {
                bool isLayerSame = (gridLayerMask.value & (1 << eventData.pointerCurrentRaycast.gameObject.layer)) > 0;
                Grid gridScript = currObj.GetComponent<Grid>();

                if (isLayerSame && !IsGridExistInList(gridScript))
                {
                    //Debug.Log("GameObj : " + eventData.pointerCurrentRaycast.gameObject);
                    DraggingData(gridScript,eventData);
                    OnDragInputEvent?.Invoke(gridScript);
                }

                currLevel.GetLineRenderer().positionCount = 2;
                Vector3 pos = Camera.main.WorldToViewportPoint(eventData.position);
                currLevel.GetLineRenderer().SetPosition(1, eventData.position);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            startingGrid = null;
            InputDataReset();
        }

        void InputStartData(Grid gridScript,PointerEventData pointerEventData)
        {
            startingGrid = gridScript;
            currGrid = startingGrid;
            currLevel.GetLineRenderer().positionCount = 1;
            Vector3 pos = Camera.main.WorldToViewportPoint(pointerEventData.position);
            currLevel.GetLineRenderer().SetPosition(0, pointerEventData.position);
            OnNewLetterAddEvent?.Invoke(gridScript);
        }

        private void DraggingInput(Grid gridScript)
        {
            //x behaves as i and y behaves as j.
            if(currGrid.gridID.x - startingGrid.gridID.x == 0)
            {
                //It means that constant X Axis.
                Debug.Log("X Direction Pattern Moving !");
            }
            else if(currGrid.gridID.y - startingGrid.gridID.y == 0)
            {
                //It means that constant Y Axis.
                Debug.Log("Y Direction Pattern Moving !");
            }
            else if(currGrid.gridID.y - startingGrid.gridID.y == currGrid.gridID.x - startingGrid.gridID.x)
            {
                //Any Diagonal Direction Entered.
                Debug.Log("Diagonal Pattern Moving !");
            }
            else
            {
                Debug.Log("Random Pattern Moving !");
            }

            inputGridsList.Add(gridScript);
            gridScript.isMarked = true;
            gridScript.SetGridColor(Color.cyan);
        }


        void DraggingData(Grid gridScript,PointerEventData pointerEventData)
        {
            currGrid = gridScript;

            if (inputGridsList.Count == 1 && startingGrid != gridScript)
            {
                currLevel.GetLineRenderer().positionCount = 2;
                currLevel.GetLineRenderer().SetPosition(1, pointerEventData.position);
                secondGrid = gridScript;
            }

            if (secondGrid != null)
            {
                CheckDirection(startingGrid, secondGrid);

            }
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

        void AddNewLetter(Grid gridObj)
        {
            inputGridsList.Add(gridObj);
            gridObj.isMarked = true;
            gridObj.SetGridColor(Color.cyan);
        }

        void CheckDirection(Grid startScript, Grid currGridScript)
        {
            // gridId x is behaving as column(j).
            //gridId y is behaving as row(i).
            //It is reverse in terms of x and y.
            int firstX = startingGrid.gridID.x;
            int firstY = startingGrid.gridID.y;
            int secondX = secondGrid.gridID.x;
            int secondY = secondGrid.gridID.y;

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

            switch (draggingDirection)
            {
                case GameController.InputDirection.NONE:

                    Debug.Log("None !");

                    break;

                case GameController.InputDirection.TOP:

                    Debug.Log("TOP !");


                    break;

                case GameController.InputDirection.TOP_LEFT:

                    Debug.Log("TOP LEFT !");

                    break;

                case GameController.InputDirection.TOP_RIGHT:

                    Debug.Log("TOP RIGHT !");

                    break;

                case GameController.InputDirection.BOTTOM:

                    Debug.Log("BOTTOM !");

                    break;

                case GameController.InputDirection.BOTTOM_LEFT:

                    Debug.Log("BOTTOM LEFT !");

                    break;

                case GameController.InputDirection.BOTTOM_RIGHT:

                    Debug.Log("BOTTOM RIGHT !");

                    break;
            }
        }

        void InputDataReset()
        {
            for (int i = 0; i < inputGridsList.Count; i++)
            {
                if (!inputGridsList[i].isCorrect)
                {
                    //Debug.Log($"Input Element {inputGridsList[i].name} Color Reset !");
                    inputGridsList[i].SetGridColor(Color.white);
                }
            }

            inputGridsList.Clear();
            currGrid = null;
            secondGrid = null;
            startingGrid = null;
            draggingDirection = GameController.InputDirection.NONE;
        }

        void ResetListData()
        {
            inputGridsList.Clear();
            totalGridsList.Clear();
        }
    }
}
