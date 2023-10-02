using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class LevelHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Serializable]
        public class LevelWords
        {
            public LevelDataInfo.WordInfo wordInfo;
            public bool isWordMarked;

            //Assigns only after Hint is called.
            public GameObject hintMarker;
        }

        private delegate void NewLetterDelegate(GridTile gridTile);
        private NewLetterDelegate _onNewLetterAddEvent;
        public delegate void GameCompleteDelegate();
        public GameCompleteDelegate onGameCompleteEvent;


        [Header("References")] [HideInInspector]
        public Camera cam;
        public ParticleSystem touchEffectRef;
        private Renderer _touchEffectRenderer;
        [SerializeField] Level currLevel;
        [FormerlySerializedAs("currGrid")] [SerializeField] GridTile currGridTile;
        [FormerlySerializedAs("startingGrid")] [SerializeField] GridTile startingGridTile;
        [FormerlySerializedAs("targetGrid")] [SerializeField] GridTile targetGridTile;
        [FormerlySerializedAs("lastGrid")] [SerializeField] GridTile lastGridTile;
        [SerializeField] LayerMask gridLayerMask;
        RectTransform _levelHandlerRect;
        public int showGridTillLevel = 4, coinPerLevel = 100;

        [Header("Level Info")] public char[][] gridData;
        public float timeToShowHint = 0.5f, timeToRotateGrid = 0.5f;
        public List<Color> levelLineColorList = new List<Color>();
        public List<GridTile> totalGridsList, inputGridsList;
        public List<LevelWords> wordList;
        public GameController.Direction mainDir;
        bool _isLevelRunning = true;
        private int _colorIndex;
        
        private void OnEnable()
        {
            _onNewLetterAddEvent += AddNewLetter;
            onGameCompleteEvent += LevelComplete;
        }

        private void OnDisable()
        {
            _onNewLetterAddEvent -= AddNewLetter;
            onGameCompleteEvent -= LevelComplete;
        }

        private void Awake()
        {
            Init();
        }

        void Init()
        {
            //Debug.Log("Level Handler Init Called !");
            cam = Camera.main;
            totalGridsList = new List<GridTile>();
            inputGridsList = new List<GridTile>();
            wordList = new List<LevelWords>();
            _levelHandlerRect = GetComponent<RectTransform>();
            _touchEffectRenderer = touchEffectRef.GetComponent<Renderer>();
        }

        public void LevelStartInit()
        {
            _isLevelRunning = true;
        }

        public void GetGridData()
        {
            TextAsset levelTextFile = GameController.instance.GetGridDataOfLevel();
            //string levelData = levelTextFile.text.Trim();
            string levelData = levelTextFile.text;
            //Debug.Log("Level Data String : " + levelData);
            int row = currLevel.gridSize.x;
            int col = currLevel.gridSize.y;
            gridData = new char[row][];
            string[] lines = levelData.Split('\n');
            //Debug.Log("Lines : " + lines.Length);

            for (int i = 0; i < row; i++)
            {
                gridData[i] = new char[col];
                //Debug.Log($"Line {i}  : " + lines[i]);

                // Split the string by comma
                string[] splitString = lines[i].Split(',');

                // Join the substrings to create a character array without commas
                string charArrayString = string.Join("", splitString).ToUpper();

                // Convert the character array string to a char array
                char[] charArray = charArrayString.ToCharArray();

                for (int j = 0; j < col; j++)
                {
                    gridData[i][j] = charArray[j];
                }
            }

            List<LevelDataInfo.WordInfo> list = GameController.instance.GetLevelDataInfo().words;

            foreach (var word in list)
            {
                LevelWords levelWords = new LevelWords
                {
                    wordInfo = word
                };
                this.wordList.Add(levelWords);
            }


            AssignLevelColors();
        }

        private void AssignLevelColors()
        {
            _colorIndex = 0;
            levelLineColorList = DataHandler.instance.PickColors(wordList.Count);
        }

        public void SetLevelRunningBool(bool canTouch)
        {
            _isLevelRunning = canTouch;
        }

        public void AssignLevel(Level levelScript)
        {
            currLevel = levelScript;
        }

        public RectTransform GetRectTransform()
        {
            return _levelHandlerRect;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isLevelRunning)
                return;
            
            touchEffectRef.gameObject.SetActive(true);
            Vector2 pos = cam.ScreenToWorldPoint(eventData.position);
            TouchEffect(true);
            UpdateTouchEffect(pos);
            
            GameObject currObj = eventData.pointerCurrentRaycast.gameObject;

            //Debug.Log("Curr Obj : " + currObj.name,currObj);

            if (currObj != null)
            {
                GridTile gridTile = currObj.GetComponent<GridTile>();

                if (IsLayerSame(currObj) && !IsGridExistInList(gridTile) && gridTile.isSelectable)
                {
                    //Debug.Log("GameObj : " + eventData.pointerCurrentRaycast.gameObject);
                    //Debug.Log("Mouse Pos : " + Input.mousePosition);
                    InputStartData(gridTile);
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isLevelRunning)
                return;

            Vector2 pos = cam.ScreenToWorldPoint(eventData.position);
            UpdateTouchEffect(pos);
            
            GameObject currObj = eventData.pointerCurrentRaycast.gameObject;
            
            if (currObj != null)
            {
                //Debug.Log("Dragging Obj : " + currObj.name, currObj);
                if (startingGridTile != null && IsLayerSame(currObj))
                {
                    GridTile gridTileScript = currObj.GetComponent<GridTile>();
                    if (gridTileScript.isSelectable)
                    {
                        currGridTile = gridTileScript;
                        SetTargetGrid();
                    }
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isLevelRunning)
                return;
            Debug.Log("Pointer Up");
            TouchEffect(false);
            startingGridTile = null;
            CheckAnswer();
        }

        void TouchEffect(bool isActive)
        {
            var mainModule =touchEffectRef.main;
            mainModule.loop = isActive;

            if (isActive)
            {
                touchEffectRef.Play();
            }
        }
        
        void UpdateTouchEffect(Vector2 pos)
        {
            touchEffectRef.transform.position = pos;
        }

        void SetTouchEffectColor(Color color)
        {
            _touchEffectRenderer.material.color = color;
        }
        
        void InputStartData(GridTile gridTileScript)
        {
            startingGridTile = gridTileScript;
            currGridTile = startingGridTile;
            currLevel.GetLineRenderer().gameObject.SetActive(true);
            SetLinePoints(1, 0, startingGridTile);
            SetLinePoints(2, 1, startingGridTile);
            _onNewLetterAddEvent?.Invoke(gridTileScript);
        }

        private void SetLinePoints(int totalPoints, int index, GridTile gridTile)
        {
            currLevel.GetLineRenderer().positionCount = totalPoints;
            Vector2 mousePos = ConvertTransformToMousePos(gridTile.transform.position);
            //Debug.Log("Line Mouse Pos : " + mousePos);
            currLevel.SetLineRendererPoint(index, mousePos);
        }

        private Vector3 ConvertMouseToLineRendererPoint(Vector3 mousePosition)
        {
            Vector3 worldMousePosition =
                cam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, cam.nearClipPlane));
            Vector3 pos = transform.InverseTransformPoint(worldMousePosition);

            //Debug.Log("linePoint : " + pos);

            return pos;
        }

        private Vector2 ConvertLinePointToMousePos(int index)
        {
            Vector3 worldPosition = currLevel.GetLineRenderer().transform
                .TransformPoint(currLevel.GetLineRenderer().GetPosition(index));

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

        GridTile GetGridByMousePos(Vector3 mousePos)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = mousePos
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
            {
                GameObject hitObject = result.gameObject;

                if (IsLayerSame(hitObject))
                {
                    GridTile gridTile = hitObject.GetComponent<GridTile>();

                    if (gridTile != null)
                    {
                        //Debug.Log("Grid Found : " + grid.gameObject.name);
                        return gridTile;
                    }
                }
            }

            return null;
        }

        bool IsGridExistInList(GridTile gridTileScript)
        {
            if (inputGridsList.Contains(gridTileScript))
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

        void AddNewLetter(GridTile gridTileObj)
        {
            if (gridTileObj != null)
            {
                lastGridTile = gridTileObj;
                currLevel.TouchTextData += gridTileObj.GridTextData;
                inputGridsList.Add(gridTileObj);
            }
        }

        void InputDataReset(bool isMarkedCorrect)
        {
            inputGridsList.Clear();
            mainDir = GameController.Direction.NONE;
            currLevel.TouchTextData = "";
            currGridTile = null;
            startingGridTile = null;
            targetGridTile = null;
            lastGridTile = null;

            if (!isMarkedCorrect)
            {
                Debug.Log("Not Correct !");
                currLevel.GetLineRenderer().positionCount = 0;
                currLevel.GetLineRenderer().gameObject.SetActive(false);
            }
            else
            {
                if (_isLevelRunning)
                {
                    _colorIndex++;
                    GenerateNewLine();
                }
            }
        }

        public void ResetLevelData()
        {
            inputGridsList.Clear();
            totalGridsList.Clear();
            wordList.Clear();
            mainDir = GameController.Direction.NONE;
        }

        void SetTargetGrid()
        {
            //x behaves as i and y behaves as j.
            if (currGridTile.GridID.x - startingGridTile.GridID.x == 0)
            {
                //It means that constant X Axis.
                mainDir = GameController.Direction.HORIZONTAL;
                SetLinePoints(2, 1, currGridTile);
                //Debug.Log("X Direction Pattern Moving !");
            }
            else if (currGridTile.GridID.y - startingGridTile.GridID.y == 0)
            {
                //It means that constant Y Axis.
                mainDir = GameController.Direction.VERTICAL;
                SetLinePoints(2, 1, currGridTile);
                //Debug.Log("Y Direction Pattern Moving !");
            }
            else if (currGridTile.GridID.y - startingGridTile.GridID.y == currGridTile.GridID.x - startingGridTile.GridID.x)
            {
                //Straight Diagonal Direction Entered.
                mainDir = GameController.Direction.STRAIGHT_DIAGONAL;
                SetLinePoints(2, 1, currGridTile);
                //Debug.Log("Straight Diagonal Pattern Moving !");
            }
            else if (currGridTile.GridID.y - startingGridTile.GridID.y == -(currGridTile.GridID.x - startingGridTile.GridID.x))
            {
                //Reverse Diagonal Direction Entered.
                mainDir = GameController.Direction.REVERSE_DIAGONAL;
                SetLinePoints(2, 1, currGridTile);
                //Debug.Log("Reverse Diagonal Pattern Moving !");
            }
            else
            {
                mainDir = GameController.Direction.NONE;
                //Debug.Log("Random Pattern Moving !");
            }

            bool isAllGridInBetweenAvail = IsAllGridAvail();

            if (isAllGridInBetweenAvail)
            {
                //Debug.Log("All Grid Available !");
                Vector2 targetPos = ConvertLinePointToMousePos(1);
                targetGridTile = GetGridByMousePos(targetPos);
                UpdateInputList();
            }
            else
            {
                //Debug.Log("Some Grid Not Available !");
                SetLinePoints(2, 1, lastGridTile);
            }
        }

        private bool IsAllGridAvail()
        {
            int firstX = startingGridTile.GridID.x;
            int firstY = startingGridTile.GridID.y;
            int secondX = currGridTile.GridID.x;
            int secondY = currGridTile.GridID.y;
            int xDiff = secondX - firstX;
            int yDiff = secondY - firstY;
            
            switch (mainDir)
            {
                case GameController.Direction.VERTICAL:
                    if (xDiff > 0)
                    {
                        for (int i = 1; i < xDiff; i++)
                        {
                            GridTile gridTile = GetGridByGridID(firstX + i, firstY);
                            if (gridTile != null && gridTile.isSelectable == false)
                            {
                                Debug.Log("Not Selectable Grid Name : " + gridTile.gameObject.name);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        int max = Mathf.Abs(xDiff);
                        for (int i = 0; i <= max; i++)
                        {
                            GridTile gridTile = GetGridByGridID(secondX + i, firstY);
                            if (gridTile != null && gridTile.isSelectable == false)
                            {
                                Debug.Log("Not Selectable Grids Name : " + gridTile.gameObject.name);
                                return false;
                            }
                        }
                    }

                    break;

                case GameController.Direction.HORIZONTAL:

                    if (yDiff > 0)
                    {
                        for (int i = 1; i < yDiff; i++)
                        {
                            GridTile gridTile = GetGridByGridID(firstX, firstY + i);
                            if (gridTile != null && gridTile.isSelectable == false)
                            {
                                Debug.Log("Not Selectable Grid Name : " + gridTile.gameObject.name);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        int max = Mathf.Abs(yDiff);
                        for (int i = 0; i <= max; i++)
                        {
                            GridTile gridTile = GetGridByGridID(firstX, firstY - i);
                            if (gridTile != null && gridTile.isSelectable == false)
                            {
                                Debug.Log("Not Selectable Grids Name : " + gridTile.gameObject.name);
                                return false;
                            }
                        }
                    }

                    break;

                case GameController.Direction.STRAIGHT_DIAGONAL:


                    if (yDiff > 0)
                    {
                        for (int i = 1; i < yDiff; i++)
                        {
                            GridTile gridTile = GetGridByGridID(firstX + i, firstY + i);
                            if (gridTile != null && gridTile.isSelectable == false)
                            {
                                Debug.Log("Not Selectable Grid Name : " + gridTile.gameObject.name);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        int max = Mathf.Abs(yDiff);
                        for (int i = 0; i <= max; i++)
                        {
                            GridTile gridTile = GetGridByGridID(firstX - i, firstY - i);
                            if (gridTile != null && gridTile.isSelectable == false)
                            {
                                Debug.Log("Not Selectable Grids Name : " + gridTile.gameObject.name);
                                return false;
                            }
                        }
                    }

                    break;

                case GameController.Direction.REVERSE_DIAGONAL:

                    if (yDiff > 0)
                    {
                        for (int i = 1; i < yDiff; i++)
                        {
                            GridTile gridTile = GetGridByGridID(firstX - i, firstY + i);
                            if (gridTile != null && gridTile.isSelectable == false)
                            {
                                Debug.Log("Not Selectable Grid Name : " + gridTile.gameObject.name);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        int max = Mathf.Abs(yDiff);
                        for (int i = 0; i <= max; i++)
                        {
                            GridTile gridTile = GetGridByGridID(firstX + i, firstY - i);
                            if (gridTile != null && gridTile.isSelectable == false)
                            {
                                Debug.Log("Not Selectable Grids Name : " + gridTile.gameObject.name);
                                return false;
                            }
                        }
                    }

                    break;
            }
            
            return true;
        }

        void UpdateInputList()
        {
            inputGridsList.Clear();
            currLevel.TouchTextData = "";

            // gridId x is behaving as row(i) .
            //gridId y is behaving as column(j).
            //It is reverse in terms of x and y.
            int firstX = startingGridTile.GridID.x;
            int firstY = startingGridTile.GridID.y;
            int secondX = targetGridTile.GridID.x;
            int secondY = targetGridTile.GridID.y;

            int xDiff = secondX - firstX;
            int yDiff = secondY - firstY;

            if (secondX - firstX > 0)
            {
                if (secondY - firstY > 0)
                {
                    for (int i = 0; i <= xDiff; i++)
                    {
                        GridTile gridTile = GetGridByGridID(firstX + i, firstY + i);
                        _onNewLetterAddEvent?.Invoke(gridTile);
                    }
                }
                else if (secondY - firstY == 0)
                {
                    for (int i = firstX; i <= secondX; i++)
                    {
                        GridTile gridTile = GetGridByGridID(i, firstY);
                        _onNewLetterAddEvent?.Invoke(gridTile);
                    }
                }
                else if (secondY - firstY < 0)
                {
                    int counter = 0;
                    for (int i = 0; i <= xDiff; i++)
                    {
                        GridTile gridTile = GetGridByGridID(firstX + counter, firstY - i);
                        _onNewLetterAddEvent?.Invoke(gridTile);
                        counter++;
                    }
                }
            }
            else if (secondX - firstX == 0)
            {
                if (secondY - firstY > 0)
                {
                    for (int i = firstY; i <= secondY; i++)
                    {
                        GridTile gridTile = GetGridByGridID(firstX, i);
                        _onNewLetterAddEvent?.Invoke(gridTile);
                    }
                }
                else if (secondY - firstY == 0)
                {
                    _onNewLetterAddEvent?.Invoke(startingGridTile);
                }
                else if (secondY - firstY < 0)
                {
                    for (int i = firstY; i >= secondY; i--)
                    {
                        GridTile gridTile = GetGridByGridID(firstX, i);
                        _onNewLetterAddEvent?.Invoke(gridTile);
                    }
                }
            }
            else if (secondX - firstX < 0)
            {
                if (secondY - firstY > 0)
                {
                    int counter = 0;
                    for (int i = 0; i <= yDiff; i++)
                    {
                        GridTile gridTile = GetGridByGridID(firstX - i, firstY + counter);
                        _onNewLetterAddEvent?.Invoke(gridTile);
                        counter++;
                    }
                }
                else if (secondY - firstY == 0)
                {
                    for (int i = firstX; i >= secondX; i--)
                    {
                        GridTile gridTile = GetGridByGridID(i, firstY);
                        _onNewLetterAddEvent?.Invoke(gridTile);
                    }
                }
                else if (secondY - firstY < 0)
                {
                    for (int i = 0; i >= yDiff; i--)
                    {
                        GridTile gridTile = GetGridByGridID(firstX + i, firstY + i);
                        _onNewLetterAddEvent?.Invoke(gridTile);
                    }
                }
            }
        }

        GridTile GetGridByGridID(int i, int j)
        {
            //Debug.Log($"I : {i}, J : {j}");
            if (i >= 0 && i < currLevel.gridSize.x && j >= 0 && j < currLevel.gridSize.y)
            {
                int index = i * currLevel.gridSize.x + j;

                GridTile gridTile = currLevel.GetGridContainerTrans().GetChild(index).gameObject.GetComponent<GridTile>();
                //Debug.Log($"Index {index}, Grid Name : {grid.name}");
                return gridTile;
            }

            return null;
        }

        void CheckAnswer()
        {
            string ans = currLevel.TouchTextData;
            bool isCorrect = false;
            //Debug.Log("Input Data : " + ans);
            List<LevelWords> levelWordList = wordList;
            foreach (var info in levelWordList)
            {
                string revStr = GetReverseString(info.wordInfo.word, out var originalString);
                //Debug.Log("Original Str : " + originalString);
                //Debug.Log("Rev Str : " + revStr);
                QuesGrid quesGrid = currLevel.GetQuesGrid(originalString, revStr);

                if ((originalString == ans || revStr == ans) && quesGrid != null && !quesGrid.isMarked)
                {
                    //Debug.Log("Correct !");
                    info.isWordMarked = true;
                    RemoveHint(info);
                    currLevel.UpdateQuesList(originalString);
                    isCorrect = true;
                    break;
                }
            }

            bool isComplete = currLevel.IsAllQuesMarked();

            if (isComplete)
            {
                onGameCompleteEvent?.Invoke();
            }

            InputDataReset(isCorrect);
        }

        private string GetReverseString(string str, out string originalString)
        {
            char[] revArr = str.ToCharArray();
            Array.Reverse(revArr);
            string revStr = new string(revArr);
            originalString = str;

            //Debug.Log("Str = " + str);
            //Debug.Log("Reverse Str = " + revStr);

            return revStr;
        }

        public string GetReverseString(string str)
        {
            string mainStr = str;
            char[] revArr = mainStr.ToCharArray();
            Array.Reverse(revArr);
            string revStr = new string(revArr);
        
            //Debug.Log("Str = " + str);
            //Debug.Log("Reverse Str = " + revStr);
        
            return revStr;
        }

        public void GenerateNewLine()
        {
            LineRenderer line = Instantiate(DataHandler.instance.lineRendererPrefab, currLevel.GetLineParentTrans());
            line.positionCount = 0;
            line.gameObject.SetActive(false);
            line.startWidth = currLevel.GetLineRendererWidth();
            line.endWidth = currLevel.GetLineRendererWidth();
            //Debug.Log("Color List Count : " + levelLineColorList.Count);
            //Debug.Log("Color Index " + colorIndex);
            Color color = levelLineColorList[_colorIndex];
            currLevel.SetLineRenderer(line, color);
            SetTouchEffectColor(color);
            touchEffectRef.gameObject.SetActive(false);
        }

        private void LevelComplete()
        {
            _isLevelRunning = false;
            GameController.instance.uiManager.CoinCollectionAnimation(coinPerLevel);

            //GameController.instance.NextLevel();
        }

        public void ShowHint()
        {
            //Debug.Log("Show Hint Called !");
            GridTile hintGridTile = null;
            int index = 0;

            for (int i = 0; i < wordList.Count; i++)
            {
                Vector2Int id = wordList[i].wordInfo.firstLetterGridVal;
                bool isWordMarked = wordList[i].isWordMarked;
                GridTile gridTile = GetGridByGridID(id.x, id.y);

                if (gridTile != null)
                {
                    if (!isWordMarked)
                    {
                        index = i;
                        wordList[i].isWordMarked = true;
                        hintGridTile = gridTile;
                        break;
                    }
                }
                else
                {
                    Debug.LogAssertion("Grid Not Found !");
                }
            }


            if (hintGridTile != null)
            {
                //Debug.Log("Hint Grid : " + hintGrid);
                PlayHintAnimation(hintGridTile, index);
            }
        }

        private void PlayHintAnimation(GridTile gridTile, int index)
        {
            GameObject hintObj = Instantiate(DataHandler.instance.hintCirclePrefab, currLevel.GetHintContainer());
            wordList[index].hintMarker = hintObj;
            hintObj.transform.position = gridTile.transform.position;
            Image hintImg = hintObj.GetComponent<Image>();
            //Color color = DataHandler.Instance.GetCurrentColor();
            //hintImg.color = new Color(color.r, color.g, color.b, 255f);
            hintImg.DOFillAmount(1f, timeToShowHint);
        }

        void RemoveHint(LevelWords levelWords)
        {
            if (levelWords.isWordMarked)
            {
                Destroy(levelWords.hintMarker);
            }
        }
    }
}