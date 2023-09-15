using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class LevelGenerator : MonoBehaviour
    {
        public int levelNum;
        public LevelDataInfo levelDataInfo;
        public List<string> wordList = new List<string>();
        public int numRows;
        public int numColumns;
        public int letterIndex = 0;
        public string[,] gridData;
        public bool validData = true;
        public GameController.InputDirection[] directions;
        GameController.InputDirection matchingDir;
        public Vector2Int currCheckingGrid;
        public LevelDataInfo.LevelInfo levelData;
        public LevelDataInfo.LevelInfo dataInfo;

        private void OnEnable()
        {

        }

        // Initialize the grid data
        private void Start()
        {
            gridData = new string[numRows, numColumns];
        }
        public void ExportDataToCSV(LevelGenerator levelGenerator)
        {
            string[,] data = levelGenerator.gridData;
            StringBuilder csvContent = new StringBuilder();

            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    csvContent.Append(data[i, j]);
                    csvContent.Append(",");
                }
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
                return;
            }

            if (!Directory.Exists(filePath))
            {
                Debug.Log("Folder Creating !");
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            // Refresh the Unity Asset Database to make the file visible in the Editor
            AssetDatabase.Refresh();

            Debug.Log("CSV file saved to: " + filePath);
        }

        public string GenerateRandom_ASCII_Code()
        {
            int randomASCII_Val = Random.Range(065, 091);
            char letter = (char)randomASCII_Val;

            return letter.ToString();
        }

        public Vector2Int FindWord(string word, out GameController.InputDirection dir)
        {
            Vector2Int wordMatchingGridID = Vector2Int.one * -1;

            Debug.Log("New Word Finding !");
            matchingDir = GameController.InputDirection.NONE;

            for (int i = currCheckingGrid.x; i < gridData.GetLength(0); i++)
            {
                for (int j = currCheckingGrid.y; j < gridData.GetLength(1); j++)
                {
                    if (gridData[i, j] == word[letterIndex].ToString())
                    {
                        Debug.Log($"Letter {letterIndex} : {word[letterIndex]}");
                        wordMatchingGridID = new Vector2Int(i, j);
                        bool wordFound = false;
                        int tempLetterIndex = letterIndex;
                        for (int k = 0; k < directions.Length; k++)
                        {
                            if (wordFound)
                                continue;

                            wordFound = false;
                            Vector2Int newIndex;
                            bool isAvail = CheckDirection(i, j, directions[k], out newIndex);
                            Debug.Log($"I : {i} , J : {j}");
                            Debug.Log($"K {k} : {isAvail}");
                            Debug.Log($"DIR : {directions[k]}");
                            if (isAvail)
                            {
                                Debug.Log("Temp Index : " + tempLetterIndex);
                                tempLetterIndex = CheckNextLetter(newIndex, word, tempLetterIndex);
                                Debug.Log("Temp Index : " + tempLetterIndex);
                                if (tempLetterIndex != -1)
                                {
                                    matchingDir = directions[k];

                                    Debug.Log("Matching Dir : " + matchingDir);
                                    int index = tempLetterIndex;
                                    MatchLetterInCurrDirection(word, newIndex, out wordFound, index, out tempLetterIndex);

                                    if (wordFound)
                                    {
                                        Debug.Log("Word Found : " + wordFound);
                                        dir = matchingDir;

                                        //UpdateLevelDataInfo(word);

                                        return wordMatchingGridID;
                                    }
                                    else
                                    {
                                        Debug.Log("MOVING TO NEXT DIR ! ");

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

            dir = matchingDir;
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


        bool MatchLetterInCurrDirection(string word, Vector2Int newIndex, out bool isMatching, int index, out int tempLetterIndex)
        {
            isMatching = false;
            bool letterMatching = false;
            int i = newIndex.x;
            int j = newIndex.y;


            tempLetterIndex = index;

            if (tempLetterIndex < word.Length)
            {
                //Debug.Log("Letter Index : " + tempLetterIndex); 
                string letter = word[tempLetterIndex].ToString();

                switch (matchingDir)
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
                        return MatchLetterInCurrDirection(word, newIndex, out isMatching, tempLetterIndex, out tempLetterIndex);

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

        public void FillScriptableObj()
        {
            //Fills first 

            LevelDataInfo.LevelInfo info = new LevelDataInfo.LevelInfo();

            info = levelData;

            levelDataInfo.levelInfo.Add(info);

            EditorUtility.SetDirty(levelDataInfo);

        }

    }
}
