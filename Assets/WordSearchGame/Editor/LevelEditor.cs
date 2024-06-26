using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using YugantLoyaLibrary.WordSearchGame;

namespace WordSearchGame.Editor
{
    [CustomEditor(typeof(LevelGenerator))]
    public class LevelEditor : UnityEditor.Editor
    {
        private SerializedProperty _editorWordList, _enumListProperty, _levelDataInfo, _levelData, _dataRetrievingFile;

        private void OnEnable()
        {
            //All Direction in which the word the Level Generator Finds.
            _enumListProperty = serializedObject.FindProperty("directions");

            //List of Words that needs to be find in the Grid.
            _editorWordList = serializedObject.FindProperty("wordList");

            //This is the reference of the LevelDataInfo Scriptable Object.
            _levelDataInfo = serializedObject.FindProperty("levelDataInfo");

            //This is generated by the Level Generator.
            _levelData = serializedObject.FindProperty("levelData");

            //This is used when you need to convert/Fill Csv File data into ScriptableObj.
            _dataRetrievingFile = serializedObject.FindProperty("retrieveDatafile");
        }

        public override void OnInspectorGUI()
        {
            LevelGenerator levelGenerator = (LevelGenerator)target;

            // Display the numRows and numColumns fields
            levelGenerator.levelNum = EditorGUILayout.IntField("Level Number ", levelGenerator.levelNum);
            levelGenerator.numRows = EditorGUILayout.IntField("Number of Rows", levelGenerator.numRows);
            levelGenerator.numColumns = EditorGUILayout.IntField("Number of Columns", levelGenerator.numColumns);

            serializedObject.Update();


            EditorGUILayout.PropertyField(_dataRetrievingFile);
            EditorGUILayout.PropertyField(_levelDataInfo);
            EditorGUILayout.PropertyField(_enumListProperty);

            GUILayout.Space(10f);

            EditorGUILayout.PropertyField(_levelData);
            EditorGUILayout.PropertyField(_editorWordList);


            levelGenerator.directions =
                new GameController.InputDirection[Enum.GetNames(typeof(GameController.InputDirection)).Length - 1];

            // Create a 2D array for gridData
            if (levelGenerator.numRows > 0 && levelGenerator.numColumns > 0)
            {
                if (levelGenerator.gridData == null ||
                    levelGenerator.gridData.GetLength(0) != levelGenerator.numRows ||
                    levelGenerator.gridData.GetLength(1) != levelGenerator.numColumns)
                {
                    levelGenerator.gridData = new string[levelGenerator.numRows, levelGenerator.numColumns];
                }

                int count = 0;

                foreach (GameController.InputDirection val in Enum.GetValues(typeof(GameController.InputDirection)))
                {
                    if (val != GameController.InputDirection.NONE)
                    {
                        levelGenerator.directions[count] = val;
                        count++;
                    }
                }

                for (int i = 0; i < levelGenerator.numRows; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int j = 0; j < levelGenerator.numColumns; j++)
                    {
                        string str = EditorGUILayout.TextField(levelGenerator.gridData[i, j]);

                        if (!string.IsNullOrEmpty(str))
                        {
                            levelGenerator.gridData[i, j] = str[0].ToString().ToUpper().Trim();
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Space(10f);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Refresh Level Generator Script"))
                {
                    Debug.Log("Refreshing Script !");

                    levelGenerator.levelData = new LevelDataInfo.LevelInfo();

                    for (int i = 0; i < levelGenerator.numRows; i++)
                    {
                        for (int j = 0; j < levelGenerator.numColumns; j++)
                        {
                            levelGenerator.gridData[i, j] = "";
                        }
                    }

                    levelGenerator.wordList.Clear();

                    serializedObject.ApplyModifiedProperties();
                }

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(10f);

                EditorGUILayout.BeginHorizontal();

                if (levelGenerator.wordList.Count > 0)
                {
                    if (GUILayout.Button("Find Words"))
                    {
                        Debug.Log("Finding Words in Level !");
                        //levelGenerator.levelData = new LevelDataInfo.LevelInfo();
                        levelGenerator.dataInfo.words.Clear();
                        serializedObject.ApplyModifiedProperties();

                        for (int i = 0; i < levelGenerator.wordList.Count; i++)
                        {
                            levelGenerator.letterIndex = 0;
                            levelGenerator.currCheckingGrid = Vector2Int.zero;
                            string word = levelGenerator.wordList[i].ToUpper();
                            Debug.Log($"Word {i} : {word}");

                            if (!string.IsNullOrEmpty(word) || !string.IsNullOrWhiteSpace(word))
                            {
                                Vector2Int gridId = levelGenerator.FindWord(word, out var gridDir);

                                if (gridId.y != -1 || gridId.x != -1)
                                {
                                    UpdateLevelDataInfo(word, levelGenerator, gridDir, gridId);
                                }
                                else
                                {
                                    Debug.LogError($"{word} Word Not Found !!");
                                }
                            }
                        }

                        LevelDataInfo.LevelInfo levelInfo = new LevelDataInfo.LevelInfo()
                        {
                            levelCsv = levelGenerator.dataInfo.levelCsv,
                            words = levelGenerator.dataInfo.words,
                            gridSize = levelGenerator.dataInfo.gridSize
                        };

                        levelGenerator.levelData = levelInfo;
                    }

                    GUILayout.Space(5f);


                    if (GUILayout.Button("Finalize Level"))
                    {
                        Debug.Log("Finalizing Level !");
                        string filePath = levelGenerator.ExportDataToCsv(levelGenerator);

                        if (filePath != null)
                        {
                            TextAsset textFile = AssetDatabase.LoadAssetAtPath<TextAsset>($"Assets/{filePath}");

                            if (textFile != null)
                            {
                                levelGenerator.levelData.levelCsv = textFile;
                                Debug.Log("CSV : " + levelGenerator.levelData.levelCsv.name);
                            }
                        }
                    }

                    GUILayout.Space(5f);

                    if (levelGenerator.dataInfo.words.Count > 0)
                    {
                        if (GUILayout.Button("Add To Scriptable Object "))
                        {
                            Debug.Log("Adding Data to Scriptable Obj !");
                            levelGenerator.FillScriptableObj();
                        }
                    }
                }


                EditorGUILayout.EndHorizontal();

                GUILayout.Space(10f);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Fill Empty Spaces with Random Data"))
                {
                    Debug.Log("Filling Empty Spaces with Random Data!");
                    for (int i = 0; i < levelGenerator.numRows; i++)
                    {
                        for (int j = 0; j < levelGenerator.numColumns; j++)
                        {
                            string str = levelGenerator.gridData[i, j];

                            if (string.IsNullOrWhiteSpace(str) || string.IsNullOrEmpty(str))
                            {
                                levelGenerator.gridData[i, j] = levelGenerator.GenerateRandom_ASCII_Code();
                            }
                        }
                    }

                    Debug.Log("Grids Filled !");
                }

                GUILayout.Space(10f);

                if (GUILayout.Button("Delete Existing File with Same Name"))
                {
                    Debug.Log($"Deleting Level_{levelGenerator.levelNum} file!!");
                    string assetsFolderPath = Application.dataPath;
                    string relativeFilePath = $"WordSearchGame/LevelData/Level_{levelGenerator.levelNum}.csv";
                    string filePath = Path.Combine(assetsFolderPath, relativeFilePath);

                    if (File.Exists(filePath))
                    {
                        Debug.Log($"Deleted the file {levelGenerator.levelNum} !");
                        File.Delete(filePath);
                    }
                    else
                    {
                        Debug.Log($"No File Exists with this Level_{levelGenerator.levelNum} !");
                    }
                }

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5f);

                if (_dataRetrievingFile != null)
                {
                    if (GUILayout.Button("Retrieve Data From CSV "))
                    {
                        Debug.Log("Retrieving Data !");
                        levelGenerator.FillDataFromCsv(levelGenerator);
                    }
                }


                // Apply modifications to the serialized object
                serializedObject.ApplyModifiedProperties();
            }
        }


        private static void UpdateLevelDataInfo(string word, LevelGenerator levelGenerator,
            GameController.InputDirection dir, Vector2Int firstLetterID)
        {
            levelGenerator.dataInfo.gridSize = new Vector2Int(levelGenerator.gridData.GetLength(0),
                levelGenerator.gridData.GetLength(1));

            LevelDataInfo.WordInfo wordInfo = new LevelDataInfo.WordInfo
            {
                firstLetterGridVal = firstLetterID,
                word = word,
                dir = dir
            };

            levelGenerator.dataInfo.words.Add(wordInfo);

            Debug.Log("Curr Word : " + word);
            Debug.Log("Curr Word Info : " + levelGenerator.dataInfo.words.Count);
        }
    }
}