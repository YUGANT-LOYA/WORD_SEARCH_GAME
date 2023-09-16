using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class DataHandler : MonoBehaviour
    {
        public static DataHandler Instance;

        [Header("Prefab Holders")]
        public GameObject coinPrefab;
        public GameObject levelPrefab;
        public LineRenderer lineRendererPrefab;
        public GameObject hintCirclePrefab;

        [Header("Data Info")]
        [SerializeField] List<Color> totalColors = new List<Color>();
        [Tooltip("When player first time start playing, Initial coins player have")]
        public int initial_Coins = 300;
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

        public List<Color> PickColors(int numOfColor)
        {
            List<Color> colorList = new List<Color>();
            List<Color> tempColor = new List<Color>(totalColors);

            for(int i = 0;i<numOfColor;i++)
            {
                if(tempColor.Count <= 0)
                {
                    tempColor = totalColors;
                }

                int index = Random.Range(0, tempColor.Count);
                
                colorList.Add(tempColor[index]);
                tempColor.Remove(tempColor[index]);
            }

            return colorList;
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

        public int TotalCoin
        {
            get
            {
                return PlayerPrefs.GetInt(StringHelper.COIN_AVAIL, initial_Coins);
            }
            set
            {
                PlayerPrefs.SetInt(StringHelper.COIN_AVAIL, value);
            }
        }
    }
}
