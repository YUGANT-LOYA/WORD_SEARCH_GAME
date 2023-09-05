using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class DataHandler : MonoBehaviour
    {
        public static DataHandler Instance;



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

    }
}
