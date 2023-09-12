using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace YugantLoyaLibrary.WordSearchGame
{

    [CustomEditor(typeof(LevelGenerator))]
    public class LevelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            LevelGenerator levelGenerator = (LevelGenerator)target;

            // Display the numRows and numColumns fields
            levelGenerator.levelNum = EditorGUILayout.IntField("Level Number ", levelGenerator.levelNum);
            levelGenerator.numRows = EditorGUILayout.IntField("Number of Rows", levelGenerator.numRows);
            levelGenerator.numColumns = EditorGUILayout.IntField("Number of Columns", levelGenerator.numColumns);

            // Create a 2D array for gridData
            if (levelGenerator.numRows > 0 && levelGenerator.numColumns > 0)
            {
                if (levelGenerator.gridData == null ||
                    levelGenerator.gridData.GetLength(0) != levelGenerator.numRows ||
                    levelGenerator.gridData.GetLength(1) != levelGenerator.numColumns)
                {
                    levelGenerator.gridData = new string[levelGenerator.numRows, levelGenerator.numColumns];
                }


                for (int i = 0; i < levelGenerator.numRows; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int j = 0; j < levelGenerator.numColumns; j++)
                    {
                        string str = EditorGUILayout.TextField(levelGenerator.gridData[i, j]);

                        if (!string.IsNullOrEmpty(str) && !string.IsNullOrWhiteSpace(str))
                        {
                            levelGenerator.gridData[i, j] = str[0].ToString();
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

                if (levelGenerator.validData)
                {
                    //Debug.Log("Is Valid !");
                    if (GUILayout.Button("Finalize Level"))
                    {
                        Debug.Log("Finalizing Level !");
                        ExportDataToCSV(levelGenerator);
                    }
                }

                GUILayout.Space(5f);

                if (GUILayout.Button("Clear Data"))
                {
                    Debug.Log("Clearing Data !");
                    for (int i = 0; i < levelGenerator.numRows; i++)
                    {
                        for (int j = 0; j < levelGenerator.numColumns; j++)
                        {
                            levelGenerator.gridData[i, j] = "";
                        }
                    }
                    Debug.Log("Data Cleared !");
                }

                GUILayout.Space(5f);

                if (GUILayout.Button("Fill Grid With Random Data"))
                {
                    Debug.Log("Filling Grid With Random Data !");
                    for (int i = 0; i < levelGenerator.numRows; i++)
                    {
                        for (int j = 0; j < levelGenerator.numColumns; j++)
                        {
                            levelGenerator.gridData[i, j] = GenerateRandom_ASCII_Code();
                        }
                    }
                    Debug.Log("Grids Filled !");
                }

                GUILayout.Space(5f);

                if (GUILayout.Button("Delete Existing File with Same Name"))
                {
                    Debug.Log($"Deleting Level_{levelGenerator.levelNum} file!!");
                    string assetsFolderPath = Application.dataPath;
                    string relativeFilePath = $"WordSearchGame/LevelData/Level_{levelGenerator.levelNum}.csv";
                    string filePath = Path.Combine(assetsFolderPath, relativeFilePath);

                    if(File.Exists(filePath))
                    {
                        Debug.Log($"Deleted the file {levelGenerator.levelNum} !");
                        File.Delete(filePath);
                    }
                    else
                    {
                        Debug.Log($"No File Exists with this Level_{levelGenerator.levelNum} !");
                    }
                }

                // Apply modifications to the serialized object
                serializedObject.ApplyModifiedProperties();
            }
        }

        void ExportDataToCSV(LevelGenerator levelGenerator)
        {
            string[,] data = levelGenerator.gridData;
            StringBuilder csvContent = new StringBuilder();

            for (int i = 0;i < data.GetLength(0);i++)
            {
                for(int j = 0;j < data.GetLength(1);j++)
                {
                    csvContent.Append(data[i,j]);
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

        string GenerateRandom_ASCII_Code()
        {
            int randomASCII_Val = Random.Range(065, 091);
            char letter = (char)randomASCII_Val;

            return letter.ToString();
        }
    }
}