using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class LevelHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("References")]
        [HideInInspector] public Camera cam;
        [SerializeField] Level currLevel;
        [SerializeField] LineRenderer lineRendererPrefab;
        [SerializeField] Grid currGrid, startingGrid, targetGrid;
        [SerializeField] LayerMask gridLayerMask;
        public RectTransform canvasRectTrans;
        CanvasGroup canvasGroup;

        public delegate void NewLetterDelegate(Grid grid);
        NewLetterDelegate OnNewLetterAddEvent;
        public delegate void GameCompleteDelegate();
        GameCompleteDelegate OnGameCompleteEvent;


        [Header("Level Info")]
        public char[][] gridData;
        public List<Grid> totalGridsList, inputGridsList;
        public List<string> answerList;
        public GameController.InputDirection draggingDirection;
        public GameController.Direction mainDir;
        Vector2 inputMousePos;
        bool isLevelRunning = true;


        private void OnEnable()
        {
            OnNewLetterAddEvent += AddNewLetter;
            OnGameCompleteEvent += LevelComplete;

        }

        private void OnDisable()
        {
            OnNewLetterAddEvent -= AddNewLetter;
            OnGameCompleteEvent -= LevelComplete;
        }

        private void Awake()
        {
            Init();
        }

        void Init()
        {
            //Debug.Log("Level Handler Init Called !");
            cam = Camera.main;
            canvasGroup = GetComponent<CanvasGroup>();
            totalGridsList = new List<Grid>();
            inputGridsList = new List<Grid>();
            answerList = new List<string>();
        }

        public void LevelStartInit()
        {
            isLevelRunning = true;
        }

        public void GetGridData()
        {
            string data = GameController.Instance.GetGridDataOfLevel();
            int row = currLevel.gridSize.x;
            int col = currLevel.gridSize.y;
            gridData = new char[row][];
            string[] lines = data.Split('\n');
            //Debug.Log("Lines : " + lines.Length);
            //Debug.Log("Row : " + row);
            //Debug.Log("Col : " + col);

            for (int i = 0; i < row; i++)
            {
                gridData[i] = new char[col];
                string str = lines[i];
                //Debug.Log(str.Length);
                //Debug.Log("str : " + str);
                for (int j = 0; j < col; j++)
                {
                    //Debug.Log(str[j]);
                    gridData[i][j] = str[j];
                }
            }

            List<string> ansList = GameController.Instance.GetLevelDataInfo().words;
            //Debug.Log("Ans List Count : " + ansList.Count);

            for (int i = 0; i < ansList.Count; i++)
            {
                answerList.Add(ansList[i].ToUpper());
            }

            //Debug.Log("Level Handler Ans List Count : " + answerList.Count);
        }

        public void AssignLevel(Level levelScript)
        {
            currLevel = levelScript;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isLevelRunning)
                return;

            GameObject currObj = eventData.pointerCurrentRaycast.gameObject;

            if (currObj != null)
            {
                Grid gridScript = currObj.GetComponent<Grid>();

                if (IsLayerSame(currObj) && !IsGridExistInList(gridScript))
                {
                    //Debug.Log("GameObj : " + eventData.pointerCurrentRaycast.gameObject);
                    InputStartData(gridScript);
                }
            }


        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isLevelRunning)
                return;

            GameObject currObj = eventData.pointerCurrentRaycast.gameObject;
            inputMousePos = Input.mousePosition;

            if (currObj != null && startingGrid != null && IsLayerSame(currObj))
            {
                Grid gridScript = currObj.GetComponent<Grid>();
                currGrid = gridScript;

                SetTargetGrid();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isLevelRunning)
                return;
            Debug.Log("Pointer Up");

            startingGrid = null;
            CheckAnswer();
        }

        void InputStartData(Grid gridScript)
        {
            inputMousePos = Input.mousePosition;
            startingGrid = gridScript;
            currGrid = startingGrid;
            currLevel.GetLineRenderer().gameObject.SetActive(true);
            SetLinePoints(1, 0, startingGrid);
            OnNewLetterAddEvent?.Invoke(gridScript);
        }

        Grid GetGridLineLastPoint()
        {
            if (currLevel.GetLineRenderer().positionCount == 2)
            {
                Vector2 pos = ConvertLinePointToMousePos(1);
                Grid grid = GetGridByMousePos(pos);

                if (grid != currGrid)
                {

                }
                else
                {

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

            //Debug.Log("linePoint : " + pos);

            return pos;
        }

        private Vector2 ConvertLinePointToMousePos(int index)
        {
            Vector3 worldPosition = currLevel.GetLineRenderer().transform.TransformPoint(currLevel.GetLineRenderer().GetPosition(index));

            // Convert world position to screen space
            Vector3 screenPosition = cam.WorldToScreenPoint(worldPosition);
            //Debug.Log("World Pos : " + screenPosition);

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
                        //Debug.Log("Grid Found : " + grid.gameObject.name);
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
            if (gridObj != null)
            {
                gridObj.isMarked = true;
                currLevel.touchTextData += gridObj.gridTextData;
                inputGridsList.Add(gridObj);
            }
        }

        void InputDataReset(bool isMarkedCorrect)
        {
            inputGridsList.Clear();
            mainDir = GameController.Direction.NONE;
            currLevel.touchTextData = "";
            currGrid = null;
            startingGrid = null;
            draggingDirection = GameController.InputDirection.NONE;

            if (!isMarkedCorrect)
            {
                Debug.Log("Not Correct !");
                currLevel.GetLineRenderer().positionCount = 0;
                currLevel.GetLineRenderer().gameObject.SetActive(false);
            }
            else
            {
                GenerateNewLine();
            }
        }

        public void ResetLevelData()
        {
            inputGridsList.Clear();
            totalGridsList.Clear();
            answerList.Clear();
            mainDir = GameController.Direction.NONE;
        }

        void SetTargetGrid()
        {
            //x behaves as i and y behaves as j.
            if (currGrid.gridID.x - startingGrid.gridID.x == 0)
            {
                //It means that constant X Axis.
                mainDir = GameController.Direction.HORIZONTAL;
                SetLinePoints(2, 1, currGrid);
                //Debug.Log("X Direction Pattern Moving !");
            }
            else if (currGrid.gridID.y - startingGrid.gridID.y == 0)
            {
                //It means that constant Y Axis.
                mainDir = GameController.Direction.VERTICAL;
                SetLinePoints(2, 1, currGrid);
                //Debug.Log("Y Direction Pattern Moving !");
            }
            else if (currGrid.gridID.y - startingGrid.gridID.y == currGrid.gridID.x - startingGrid.gridID.x)
            {
                //Straight Diagonal Direction Entered.
                mainDir = GameController.Direction.STRAIGHT_DIAGONAL;
                SetLinePoints(2, 1, currGrid);
                //Debug.Log("Straight Diagonal Pattern Moving !");
            }
            else if (currGrid.gridID.y - startingGrid.gridID.y == -(currGrid.gridID.x - startingGrid.gridID.x))
            {
                //Reverse Diagonal Direction Entered.
                mainDir = GameController.Direction.REVERSE_DIAGONAL;
                SetLinePoints(2, 1, currGrid);
                //Debug.Log("Reverse Diagonal Pattern Moving !");
            }
            else
            {
                mainDir = GameController.Direction.NONE;
                //Debug.Log("Random Pattern Moving !");
            }

            Vector2 targetPos = ConvertLinePointToMousePos(1);
            targetGrid = GetGridByMousePos(targetPos);
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
            //Debug.Log($"I : {i}, J : {j}");
            if (i >= 0 && i < currLevel.gridSize.x && j >= 0 && j < currLevel.gridSize.y)
            {
                int index = i * currLevel.gridSize.x + j;

                Grid grid = currLevel.GetGridContainerTrans().GetChild(index).gameObject.GetComponent<Grid>();
                //Debug.Log($"Index {index}, Grid Name : {grid.name}");
                return grid;
            }

            return null;
        }

        void CheckAnswer()
        {
            string ans = currLevel.touchTextData.ToUpper();
            bool isCorrect = false;
            //Debug.Log("Input Data : " + ans);
            //Debug.Log("Answer List Count : " + answerList.Count);
            foreach (string str in answerList)
            {
                string originalString;
                string revStr = GetReverseString(str, out originalString);

                //Debug.Log("Original Str : " + originalString);
                QuesGrid quesGrid = currLevel.GetQuesGrid(originalString, revStr);

                if ((originalString == ans || revStr == ans) && !quesGrid.isMarked)
                {
                    Debug.Log("Correct !");
                    currLevel.UpdateQuesList(originalString);
                    isCorrect = true;
                    break;
                }
            }

            InputDataReset(isCorrect);

            bool isComplete = currLevel.IsAllQuesMarked();

            if (isComplete)
            {
                OnGameCompleteEvent?.Invoke();
            }
        }

        public string GetReverseString(string str, out string originalString)
        {
            string mainStr = str.ToUpper();
            char[] revArr = mainStr.ToCharArray();
            Array.Reverse(revArr);
            string revStr = new string(revArr).ToUpper();
            originalString = mainStr;

            //Debug.Log("Str = " + str);
            //Debug.Log("Reverse Str = " + revStr);

            return revStr;
        }

        public string GetReverseString(string str)
        {
            string mainStr = str.ToUpper();
            char[] revArr = mainStr.ToCharArray();
            Array.Reverse(revArr);
            string revStr = new string(revArr).ToUpper();

            //Debug.Log("Str = " + str);
            //Debug.Log("Reverse Str = " + revStr);

            return revStr;
        }

        public void GenerateNewLine()
        {
            LineRenderer line = Instantiate(lineRendererPrefab, currLevel.GetLineParentTrans());
            line.positionCount = 0;
            line.gameObject.SetActive(false);
            line.startWidth = currLevel.GetLineRendererWidth();
            line.endWidth = currLevel.GetLineRendererWidth();
            Color color = DataHandler.Instance.GetColor();
            currLevel.SetLineRenderer(line, color);
        }

        private void LevelComplete()
        {
            isLevelRunning = false;

            GameController.Instance.NextLevel();
        }

    }
}
