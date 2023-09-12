using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class LevelGenerator : MonoBehaviour
    {
        public int levelNum;
        public int numRows;
        public int numColumns;
        public string[,] gridData;
        [HideInInspector] public bool validData = true;

        // Initialize the grid data
        private void Start()
        {
            gridData = new string[numRows, numColumns];
        }
    }
}
