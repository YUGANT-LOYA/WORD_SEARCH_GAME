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

                // Display input fields for each cell in the gridData
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
                            Debug.Log("Is Null or Empty or WhiteSpace !");
                            levelGenerator.validData = false;
                            break;
                        }
                    }
                }

                if (levelGenerator.validData)
                {
                    Debug.Log("Is Valid !");
                    if (GUILayout.Button("Finalize Level"))
                    {
                        Debug.Log("Finalize Level Clicked !");
                        ExportDataToCSV(levelGenerator);
                    }
                }

                GUILayout.Space(5f);

                if (GUILayout.Button("Clear Data"))
                {
                    Debug.Log("Clear Data Clicked !");
                    for (int i = 0; i < levelGenerator.numRows; i++)
                    {
                        for (int j = 0; j < levelGenerator.numColumns; j++)
                        {
                            levelGenerator.gridData[i, j] = "";
                        }
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


            string assetsFolderPath = Application.dataPath;

            // Specify the relative path within the "Assets" folder where you want to save the CSV file
            string relativeFilePath = $"WordSearchGame/LevelData/Level_{levelGenerator.levelNum}.csv";

            // Combine the paths to get the full path of the CSV file
            string filePath = Path.Combine(assetsFolderPath, relativeFilePath);

            // Ensure the directory structure exists
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            // Save the CSV file

            if (!string.IsNullOrEmpty(filePath))
            {
                File.WriteAllText(filePath, csvContent.ToString());
            }

            // Refresh the Unity Asset Database to make the file visible in the Editor
            AssetDatabase.Refresh();

            Debug.Log("CSV file saved to: " + filePath);
        }
    }
}