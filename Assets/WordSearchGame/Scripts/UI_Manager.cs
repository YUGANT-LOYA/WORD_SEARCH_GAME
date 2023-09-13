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
        public Button restartButton;

        bool isRestarting;

        private void Awake()
        {
            restartButton.onClick.AddListener(Restart);
        }

        public void Restart()
        {
            GameController.Instance.StartGame();
            if (isRestarting)
                return;

            //isRestarting = true;
            //RotateGridContainer();
            
        }

        private void RotateGridContainer()
        {
            Level currLevel = GameController.Instance.GetCurrentLevel();
            Transform trans = currLevel.rotationContainer;
            LevelHandler levelHandler = GameController.Instance.GetLevelHandler();
            levelHandler.SetLevelRunningBool(false);
            Transform gridContainer = currLevel.GetGridContainerTrans();

            if (currLevel.gridSize.y == currLevel.gridSize.x)
            {
                Debug.Log("Rotate If !");

                trans.DORotate(new Vector3(trans.localRotation.eulerAngles.x, trans.localRotation.eulerAngles.y, trans.localRotation.eulerAngles.z + 90f),levelHandler.timeToRotateGrid).OnComplete(()=>
                {
                    isRestarting = false;
                    levelHandler.SetLevelRunningBool(true);
                });

                gridContainer.DORotate(new Vector3(gridContainer.rotation.eulerAngles.x, gridContainer.rotation.eulerAngles.y, gridContainer.rotation.eulerAngles.z + 90f), levelHandler.timeToRotateGrid).OnComplete(() =>
                {
                    isRestarting = false;
                    levelHandler.SetLevelRunningBool(true);
                });

                for (int i = 0; i < gridContainer.childCount; i++)
                {
                    GameObject gm = gridContainer.GetChild(i).gameObject;
                    gm.transform.DORotate(new Vector3(gm.transform.localRotation.eulerAngles.x, gm.transform.localRotation.eulerAngles.y, gm.transform.localRotation.eulerAngles.z), levelHandler.timeToRotateGrid);
                }
            }
            else
            {
                Debug.Log("Rotate Else !");
                trans.DORotate(new Vector3(trans.localRotation.eulerAngles.x, trans.localRotation.eulerAngles.y, trans.localRotation.eulerAngles.z + 180f), levelHandler.timeToRotateGrid).OnComplete(() =>
                {
                    isRestarting = false;
                    levelHandler.SetLevelRunningBool(true);
                });

                gridContainer.DORotate(new Vector3(gridContainer.localRotation.eulerAngles.x, gridContainer.localRotation.eulerAngles.y, gridContainer.localRotation.eulerAngles.z + 180f), levelHandler.timeToRotateGrid).OnComplete(() =>
                {
                    isRestarting = false;
                    levelHandler.SetLevelRunningBool(true);
                });

                for (int i = 0; i < gridContainer.childCount; i++)
                {
                    GameObject gm = gridContainer.GetChild(i).gameObject;
                    gm.transform.DORotate(new Vector3(gm.transform.rotation.eulerAngles.x, gm.transform.rotation.eulerAngles.y, gm.transform.rotation.eulerAngles.z), levelHandler.timeToRotateGrid);
                }
            }
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
