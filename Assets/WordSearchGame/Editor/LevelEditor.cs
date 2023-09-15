using Codice.CM.Client.Differences;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace YugantLoyaLibrary.WordSearchGame
{

    [CustomEditor(typeof(LevelGenerator))]
    public class LevelEditor : Editor
    {
        SerializedProperty editorWordList, enumListProperty, levelDataInfo, levelData;

        private void OnEnable()
        {
            enumListProperty = serializedObject.FindProperty("directions");
            editorWordList = serializedObject.FindProperty("wordList");
            levelDataInfo = serializedObject.FindProperty("levelDataInfo");
            levelData = serializedObject.FindProperty("levelData");

        }

        public override void OnInspectorGUI()
        {
            LevelGenerator levelGenerator = (LevelGenerator)target;

            // Display the numRows and numColumns fields
            levelGenerator.levelNum = EditorGUILayout.IntField("Level Number ", levelGenerator.levelNum);
            levelGenerator.numRows = EditorGUILayout.IntField("Number of Rows", levelGenerator.numRows);
            levelGenerator.numColumns = EditorGUILayout.IntField("Number of Columns", levelGenerator.numColumns);

            serializedObject.Update();
            EditorGUILayout.PropertyField(levelDataInfo);
            EditorGUILayout.PropertyField(enumListProperty);

            GUILayout.Space(10f);

            EditorGUILayout.PropertyField(levelData);
            EditorGUILayout.PropertyField(editorWordList);



            levelGenerator.directions = new GameController.InputDirection[Enum.GetNames(typeof(GameController.InputDirection)).Length - 1];

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

                        if (!string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str))
                        {
                            levelGenerator.gridData[i, j] = str[0].ToString().ToUpper();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }


                levelGenerator.validData = true;

                for (int i = 0; i < levelGenerator.numRows; i++)
                {
                    for (int j = 0; j < levelGenerator.numColumns; j++)
                    {
                        if (string.IsNullOrEmpty(levelGenerator.gridData[i, j]) || string.IsNullOrWhiteSpace(levelGenerator.gridData[i, j]))
                        {
                            //Debug.Log("Is Null or Empty or WhiteSpace !");
                            levelGenerator.validData = false;
                            break;
                        }
                    }
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

                if (levelGenerator.validData)
                {
                    //Debug.Log("Is Valid !");
                    if (levelGenerator.wordList.Count > 0)
                    {
                        if (GUILayout.Button("Find Words"))
                        {
                            Debug.Log("Finding Words in Level !");
                            levelGenerator.levelData = new LevelDataInfo.LevelInfo();
                            levelGenerator.dataInfo.words.Clear();
                            serializedObject.ApplyModifiedProperties();

                            for (int i = 0; i < levelGenerator.wordList.Count; i++)
                            {
                                levelGenerator.letterIndex = 0;
                                levelGenerator.currCheckingGrid = Vector2Int.zero;
                                string word = levelGenerator.wordList[i].ToUpper();
                                Debug.Log($"Word {i} : {word}");
                                GameController.InputDirection gridDir;

                                if (!string.IsNullOrEmpty(word) || !string.IsNullOrWhiteSpace(word))
                                {
                                    Vector2Int gridId = levelGenerator.FindWord(word, out gridDir);

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

                            levelGenerator.levelData = levelGenerator.dataInfo;

                        }

                        if (levelGenerator.dataInfo.words.Count > 0)
                        {
                            if (GUILayout.Button("Add To Scriptable Object "))
                            {
                                Debug.Log("Adding Data to Scriptable Obj !");

                                levelGenerator.FillScriptableObj();
                            }
                        }
                    }

                    GUILayout.Space(5f);


                    if (GUILayout.Button("Finalize Level"))
                    {
                        Debug.Log("Finalizing Level !");
                        levelGenerator.ExportDataToCSV(levelGenerator);
                    }

                }

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(10f);

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Fill Grid With Random Data"))
                {
                    Debug.Log("Filling Grid With Random Data !");
                    for (int i = 0; i < levelGenerator.numRows; i++)
                    {
                        for (int j = 0; j < levelGenerator.numColumns; j++)
                        {
                            levelGenerator.gridData[i, j] = levelGenerator.GenerateRandom_ASCII_Code();
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

                // Apply modifications to the serialized object
                serializedObject.ApplyModifiedProperties();
            }
        }


        private void UpdateLevelDataInfo(string word, LevelGenerator levelGenerator, GameController.InputDirection dir, Vector2Int firstLetterID)
        {
            levelGenerator.dataInfo.gridSize = new Vector2Int(levelGenerator.gridData.GetLength(0), levelGenerator.gridData.GetLength(1));

            LevelDataInfo.WordInfo wordInfo = new LevelDataInfo.WordInfo();

            wordInfo.firstLetterGridVal = firstLetterID;
            wordInfo.word = word;
            wordInfo.dir = dir;
            levelGenerator.dataInfo.words.Add(wordInfo);

            Debug.Log("Curr Word : " + word);
            Debug.Log("Curr Word Info : " + levelGenerator.dataInfo.words.Count);
        }
    }
}