using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;

        [Header("References")]
        [SerializeField] LevelHandler levelHandler;
        [SerializeField] GameObject levelPrefab;
        

        Level currLevel;

        private void Awake()
        {
           CreateSingleton();
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


        void CreateLevel()
        {
            GameObject level = Instantiate(levelPrefab, levelHandler.gameObject.transform);
            currLevel = level.GetComponent<Level>();
        }
    }
}
