using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class DataHandler : MonoBehaviour
    {
        public static DataHandler Instance;

        [SerializeField] List<Color> totalColors = new List<Color>();
        public int colorIndex = 0;

        private void Awake()
        {
            CreateSingleton();

            Init();
        }

        void Init()
        {

        }

        void CreateSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        public Color UpdateColor()
        {
            Color color = totalColors[colorIndex];
            colorIndex++;

            if(colorIndex >= totalColors.Count)
            {
                colorIndex = 0;
            }

            return color;
        }

        public Color GetCurrentColor()
        {
            Color color = totalColors[colorIndex];
            return color;
        }

        public int CurrLevelNumber
        {
            get
            {
                return PlayerPrefs.GetInt(StringHelper.LEVEL_NUM, 0);
            }
            set
            {
                PlayerPrefs.SetInt(StringHelper.LEVEL_NUM, value);
            }
        }
    }
}
