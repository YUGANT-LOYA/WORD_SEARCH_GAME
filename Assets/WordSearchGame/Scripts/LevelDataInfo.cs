using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YugantLoyaLibrary.WordSearchGame
{
    [CreateAssetMenu(fileName = "LevelDataInfo", menuName = "LevelData")]
    public class LevelDataInfo : ScriptableObject
    {
        [System.Serializable]
        public struct LevelInfo
        {
            public Vector2Int gridSize;
            [TextArea(10, 10)]
            public string gridData;
            public TextAsset gridDataTextFile;
            public List<string> words;
        }

        public List<LevelInfo> levelInfo;
    }
}
