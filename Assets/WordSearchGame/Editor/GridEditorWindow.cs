using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class GridEditorWindow : EditorWindow
{
    private int numRows = 3;
    private int numCols = 3;
    private List<List<char>> gridData = new List<List<char>>();

    [MenuItem("Custom/Create Grid Editor")]
    static void Init()
    {
        GridEditorWindow window = (GridEditorWindow)EditorWindow.GetWindow(typeof(GridEditorWindow));
        window.Show();
    }

    private void OnGUI()
    {
        numRows = EditorGUILayout.IntField("Rows", numRows);
        numCols = EditorGUILayout.IntField("Columns", numCols);

        if (GUILayout.Button("Create Grid"))
        {
            CreateGrid();
        }

        // Display grid data fields
        for (int row = 0; row < numRows; row++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int col = 0; col < numCols; col++)
            {
                gridData[row][col] = EditorGUILayout.TextField(gridData[row][col].ToString())[0];
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Save CSV File"))
        {
            SaveGridDataToCSV();
        }
    }

    private void CreateGrid()
    {
        gridData.Clear();
        for (int row = 0; row < numRows; row++)
        {
            gridData.Add(new List<char>());
            for (int col = 0; col < numCols; col++)
            {
                gridData[row].Add(' ');
            }
        }
    }

    private void SaveGridDataToCSV()
    {
        string filePath = EditorUtility.SaveFilePanel("Save Grid Data", "", "gridData", "csv");

        if (!string.IsNullOrEmpty(filePath))
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int row = 0; row < numRows; row++)
                {
                    string rowText = string.Join(",", gridData[row]);
                    writer.WriteLine(rowText);
                }
            }

            Debug.Log("Grid data saved to CSV: " + filePath);
        }
    }
}
