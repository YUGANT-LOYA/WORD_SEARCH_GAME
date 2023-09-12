using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class UI_Manager : MonoBehaviour
    {
        public Button restartButton;

        private void Awake()
        {
            restartButton.onClick.AddListener(Restart);
        }

        public void Restart()
        {
            GameController.Instance.RestartLevel();


        }

        public void Next()
        {
            GameController.Instance.NextLevel();
        }

        public void Previous()
        {
            GameController.Instance.PreviousLevel();
        }

        public void UseHint()
        {
            GameController.Instance.GetLevelHandler().ShowHint();
        }
    }
}
