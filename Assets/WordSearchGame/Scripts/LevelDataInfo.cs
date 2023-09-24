using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace YugantLoyaLibrary.WordSearchGame
{
    [CreateAssetMenu(fileName = "LevelDataInfo", menuName = "LevelData")]
    public class LevelDataInfo : ScriptableObject
    {
        [System.Serializable]
        public struct LevelInfo
        {
            public Vector2Int gridSize;
            //[TextArea(10, 10)]
            //public string gridData;
            public TextAsset levelCsv;
            public List<WordInfo> words;
        }

        [System.Serializable]
        public struct WordInfo
        {
            public string word;
            public Vector2Int firstLetterGridVal;
            public GameController.InputDirection dir;
        }
        
        public List<LevelInfo> levelInfo;

    }
}
