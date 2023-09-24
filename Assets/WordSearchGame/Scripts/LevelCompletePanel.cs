using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YugantLoyaLibrary.WordSearchGame;

public class LevelCompletePanel : MonoBehaviour
{
    public Button nextButton;

    private void OnEnable()
    {
        nextButton.onClick.AddListener(()=>
        {
            GameController.instance.NextLevel();
        });
    }
}