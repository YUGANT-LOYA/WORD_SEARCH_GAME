using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class DataHandler : MonoBehaviour
    {
        public static DataHandler instance;

        [Header("Prefab Holders")]
        public GameObject coinPrefab;
        public GameObject levelPrefab,hintCirclePrefab, gridPrefab, quesPrefab;
        public LineRenderer lineRendererPrefab;

        [Header("Data Info")]
        [SerializeField] List<Color> totalColors = new List<Color>();
        [FormerlySerializedAs("initial_Coins")] [Tooltip("When player first time start playing, Initial coins player have")]
        public int initialCoins = 300;
        public int colorIndex = 0;


        private void Awake()
        {
            CreateSingleton();

            Init();
        }

        void CreateSingleton()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        
        void Init()
        {
            int poolSize = GameController.instance.coinPoolSize;
            
            for (int i = 0; i < poolSize; i++)
            {
                GameObject coin = Instantiate(coinPrefab, GameController.instance.coinContainerTran);
                coin.transform.localScale = Vector3.zero;
                coin.gameObject.SetActive(false);
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

        public GameObject GetCoin()
        {
            Transform tran = GameController.instance.coinContainerTran;
            
            foreach (Transform coinTran in tran)
            {
                if (!coinTran.gameObject.activeInHierarchy)
                {
                    return coinTran.gameObject;
                }
            }

            return null;
        }

        public void ResetCoin(GameObject coin)
        {
            coin.gameObject.SetActive(false);
            coin.transform.SetParent(GameController.instance.coinContainerTran);
            coin.transform.localScale = Vector3.one * (GameController.instance.uiManager.maxCoinScale / 2f);
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
                return PlayerPrefs.GetInt(StringHelper.COIN_AVAIL, initialCoins);
            }
            set
            {
                PlayerPrefs.SetInt(StringHelper.COIN_AVAIL, value);
            }
        }
    }
}
