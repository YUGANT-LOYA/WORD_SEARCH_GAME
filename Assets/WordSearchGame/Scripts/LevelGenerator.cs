using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class LevelGenerator : MonoBehaviour
    {
        [Tooltip("CSV File will be names According to Level number as Level_{levelNum}")]
        public int levelNum;

        public LevelDataInfo levelDataInfo;

        [Tooltip("The Words which you need to find in the Grids and add to LevelDataInfo")]
        public List<string> wordList = new List<string>();

        public int numRows;
        public int numColumns;
        public int letterIndex;
        [Tooltip("Fill The Grid With Data")] public string[,] gridData;
        public bool validData = true;

        [Tooltip("All Directions in which words will can be marked and will find the word inside the Grid")]
        public GameController.InputDirection[] directions;

        private GameController.InputDirection _matchingDir;
        public Vector2Int currCheckingGrid;

        [Tooltip("Data that is filled by Level Generator and filled in Scriptable Object")]
        public LevelDataInfo.LevelInfo levelData;

        public LevelDataInfo.LevelInfo dataInfo;

        [Tooltip(
            "If You need to Copy CSV File Data to Scriptable Object, assign csv File and Click Retrieve File Data To Scriptable Object")]
        public TextAsset retrieveDatafile;

        private void Start()
        {
            gridData = new string[numRows, numColumns];
        }

#if UNITY_EDITOR
        public string ExportDataToCsv(LevelGenerator levelGenerator)
        {
            string[,] data = levelGenerator.gridData;
            StringBuilder csvContent = new StringBuilder();

            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    csvContent.Append(!string.IsNullOrWhiteSpace(data[i, j]) ? data[i, j] : " ");
                    csvContent.Append(",");
                }

                csvContent.AppendLine();
            }

            csvContent.AppendLine();

            for (int i = 0; i < levelGenerator.levelData.words.Count; i++)
            {
                csvContent.Append(levelGenerator.levelData.words[i].word);
                csvContent.AppendLine();
            }

            //Asset Folder Path
            string assetsFolderPath = Application.dataPath;

            // Specify the relative path within the "Assets" folder where you want to save the CSV file
            string relativeFilePath = $"WordSearchGame/LevelData/Level_{levelGenerator.levelNum}.csv";

            // Combine the paths to get the full path of the CSV file
            string filePath = Path.Combine(assetsFolderPath, relativeFilePath);

            // Save the CSV file

            if (!string.IsNullOrEmpty(filePath) && !File.Exists(filePath))
            {
                Debug.Log("File Creating !!");
                File.WriteAllText(filePath, csvContent.ToString());
            }
            else
            {
                Debug.LogError($"File Already Exists with name Level_{levelGenerator.levelNum} !!");
                return null;
            }

            if (!Directory.Exists(filePath))
            {
                Debug.Log("Folder Creating !");
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            // Refresh the Unity Asset Database to make the file visible in the Editor
            AssetDatabase.Refresh();

            Debug.Log("CSV file saved to: " + filePath);

            return relativeFilePath;
        }
#endif

        public string GenerateRandom_ASCII_Code()
        {
            int randomAsciiVal = Random.Range(065, 091);
            char letter = (char)randomAsciiVal;

            return letter.ToString();
        }

        public Vector2Int FindWord(string word, out GameController.InputDirection dir)
        {
            Vector2Int wordMatchingGridID;

            Debug.Log("New Word Finding !");
            _matchingDir = GameController.InputDirection.NONE;

            for (int i = currCheckingGrid.x; i < gridData.GetLength(0); i++)
            {
                for (int j = currCheckingGrid.y; j < gridData.GetLength(1); j++)
                {
                    if (gridData[i, j] == word[letterIndex].ToString())
                    {
                        Debug.Log($"Letter {letterIndex} : {word[letterIndex]}");
                        wordMatchingGridID = new Vector2Int(i, j);
                        int tempLetterIndex = letterIndex;
                        for (int k = 0; k < directions.Length; k++)
                        {
                            bool isAvail = CheckDirection(i, j, directions[k], out var newIndex);
                            Debug.Log($"I : {i} , J : {j}");
                            Debug.Log($"K {k} : {isAvail}");
                            Debug.Log($"DIR : {directions[k]}");
                            if (isAvail)
                            {
                                tempLetterIndex = CheckNextLetter(newIndex, word, tempLetterIndex);
                                Debug.Log("Temp Index : " + tempLetterIndex);
                                if (tempLetterIndex != -1)
                                {
                                    _matchingDir = directions[k];

                                    Debug.Log("Matching Dir : " + _matchingDir);
                                    int index = tempLetterIndex;
                                    MatchLetterInCurrDirection(word, newIndex, out var wordFound, index,
                                        out tempLetterIndex);

                                    if (wordFound)
                                    {
                                        Debug.Log("Word Found : " + true);
                                        dir = _matchingDir;
                                        return wordMatchingGridID;
                                    }
                                    else
                                    {
                                        Debug.Log("MOVING TO NEXT DIR ! ");
                                        tempLetterIndex = 0;
                                    }
                                }
                                else
                                {
                                    tempLetterIndex = 0;
                                }
                            }
                        }
                    }
                }
            }

            dir = _matchingDir;
            wordMatchingGridID = Vector2Int.one * -1;
            return wordMatchingGridID;
        }

        bool CheckDirection(int i, int j, GameController.InputDirection dir, out Vector2Int newIndex)
        {
            bool isDirAvail = false;

            switch (dir)
            {
                case GameController.InputDirection.TOP:

                    i--;
                    if (i >= 0)
                    {
                        isDirAvail = true;
                    }

                    break;

                case GameController.InputDirection.BOTTOM:

                    i++;

                    if (i < gridData.GetLength(0))
                    {
                        isDirAvail = true;
                    }

                    break;


                case GameController.InputDirection.LEFT:

                    j--;
                    if (j >= 0)
                    {
                        isDirAvail = true;
                    }

                    break;

                case GameController.InputDirection.RIGHT:

                    j++;
                    if (j < gridData.GetLength(1))
                    {
                        isDirAvail = true;
                    }

                    break;

                case GameController.InputDirection.TOP_LEFT:

                    i--;
                    j--;

                    if (i >= 0 && j >= 0)
                    {
                        isDirAvail = true;
                    }

                    break;

                case GameController.InputDirection.TOP_RIGHT:

                    i--;
                    j++;

                    if (i >= 0 && j < gridData.GetLength(1))
                    {
                        isDirAvail = true;
                    }

                    break;

                case GameController.InputDirection.BOTTOM_LEFT:

                    i++;
                    j--;

                    if (i < gridData.GetLength(0) && j >= 0)
                    {
                        isDirAvail = true;
                    }

                    break;

                case GameController.InputDirection.BOTTOM_RIGHT:

                    i++;
                    j++;

                    if (i < gridData.GetLength(0) && j < gridData.GetLength(1))
                    {
                        isDirAvail = true;
                    }

                    break;
            }

            newIndex = new Vector2Int(i, j);

            return isDirAvail;
        }

        int CheckNextLetter(Vector2Int newIndex, string word, int tempLetterIndex)
        {
            tempLetterIndex++;

            if (tempLetterIndex < word.Length && gridData[newIndex.x, newIndex.y] == word[tempLetterIndex].ToString())
            {
                Debug.Log($"Next Letter Matched of Grid {newIndex.x} {newIndex.y} : {word[tempLetterIndex]}");

                return tempLetterIndex;
            }

            return -1;
        }

        bool MatchLetterInCurrDirection(string word, Vector2Int newIndex, out bool isMatching, int index,
            out int tempLetterIndex)
        {
            isMatching = false;
            bool letterMatching = false;
            int i = newIndex.x;
            int j = newIndex.y;


            tempLetterIndex = index;

            if (tempLetterIndex < word.Length)
            {
                //Debug.Log("Letter Index : " + tempLetterIndex); 
                string letter = word[tempLetterIndex].ToString().ToUpper();

                switch (_matchingDir)
                {
                    case GameController.InputDirection.TOP:

                        if (i >= 0)
                        {
                            if (letter == gridData[i, j])
                            {
                                i--;
                                newIndex = new Vector2Int(i, j);
                                letterMatching = true;
                            }
                        }

                        break;

                    case GameController.InputDirection.BOTTOM:

                        if (i < gridData.GetLength(0))
                        {
                            if (letter == gridData[i, j])
                            {
                                i++;
                                newIndex = new Vector2Int(i, j);
                                letterMatching = true;
                            }
                        }

                        break;


                    case GameController.InputDirection.LEFT:

                        if (j >= 0)
                        {
                            if (letter == gridData[i, j])
                            {
                                j--;
                                newIndex = new Vector2Int(i, j);
                                letterMatching = true;
                            }
                        }

                        break;

                    case GameController.InputDirection.RIGHT:

                        if (j < gridData.GetLength(1))
                        {
                            if (letter == gridData[i, j])
                            {
                                j++;
                                newIndex = new Vector2Int(i, j);
                                letterMatching = true;
                            }
                        }

                        break;

                    case GameController.InputDirection.TOP_LEFT:


                        if (i >= 0 && j >= 0)
                        {
                            if (letter == gridData[i, j])
                            {
                                i--;
                                j--;
                                newIndex = new Vector2Int(i, j);
                                letterMatching = true;
                            }
                        }

                        break;

                    case GameController.InputDirection.TOP_RIGHT:

                        if (i >= 0 && j < gridData.GetLength(1))
                        {
                            if (letter == gridData[i, j])
                            {
                                i--;
                                j++;
                                newIndex = new Vector2Int(i, j);
                                letterMatching = true;
                            }
                        }

                        break;

                    case GameController.InputDirection.BOTTOM_LEFT:

                        if (i < gridData.GetLength(0) && j >= 0)
                        {
                            if (letter == gridData[i, j])
                            {
                                i++;
                                j--;
                                newIndex = new Vector2Int(i, j);
                                letterMatching = true;
                            }
                        }

                        break;

                    case GameController.InputDirection.BOTTOM_RIGHT:

                        if (i < gridData.GetLength(0) && j < gridData.GetLength(1))
                        {
                            if (letter == gridData[i, j])
                            {
                                i++;
                                j++;
                                newIndex = new Vector2Int(i, j);
                                letterMatching = true;
                            }
                        }

                        break;
                }


                if (letterMatching)
                {
                    if (word.Length - 1 == tempLetterIndex)
                    {
                        Debug.Log("Word Matched !");
                        isMatching = true;
                        return isMatching;
                    }
                    else
                    {
                        tempLetterIndex++;
                        Debug.Log($"Else Match Letter in Curr Direction !");
                        return MatchLetterInCurrDirection(word, newIndex, out isMatching, tempLetterIndex,
                            out tempLetterIndex);
                    }
                }
            }
            else
            {
                Debug.Log("Letter Index Exceed Word Length !");
            }

            Debug.Log("Matching Letter End ! ");
            return isMatching;
        }

#if UNITY_EDITOR
        public void FillScriptableObj()
        {
            LevelDataInfo.LevelInfo info = new LevelDataInfo.LevelInfo
            {
                gridSize = levelData.gridSize,
                words = new List<LevelDataInfo.WordInfo>(levelData.words),
                levelCsv = levelData.levelCsv
            };
            
            levelDataInfo.levelInfo.Add(info);

            EditorUtility.SetDirty(levelDataInfo);
            AssetDatabase.Refresh();
        }
#endif
        
        public void FillDataFromCsv(LevelGenerator levelGenerator)
        {
            TextAsset levelTextFile = levelGenerator.retrieveDatafile;
            string data = levelTextFile.text.Trim();
            Debug.Log("Level Data String : " + data);

            string[] lines = data.Split('\n');
            Debug.Log("Lines : " + lines.Length);

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (!string.IsNullOrEmpty(line) || !string.IsNullOrWhiteSpace(line))
                {
                    Debug.Log($"Line {i}  : {line}, Length Count : {line.Length}");

                    // Split the string by comma
                    string[] splitString = line.Split(',');

                    // Join the substrings to create a character array without commas
                    string charArrayString = string.Join("", splitString).ToUpper().Trim();

                    for (int j = 0; j < charArrayString.Length; j++)
                    {
                        levelGenerator.gridData[i, j] = charArrayString[j].ToString();
                        Debug.Log($"Grid Data {i} {j} : {charArrayString[j]}");
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}