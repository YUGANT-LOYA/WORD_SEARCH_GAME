using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class UI_Manager : MonoBehaviour
    {

        private void Awake()
        {

        }

        public void PrevLevel()
        {
            GameController.Instance.PreviousLevel();
        }
        public void NextLevel()
        {
            GameController.Instance.NextLevel();
        }
    }
}
