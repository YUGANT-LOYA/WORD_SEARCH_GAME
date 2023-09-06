using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class Grid : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI gridText;
        [SerializeField] Image gridImg;
        public bool isMarked,isCorrect;
        [SerializeField] Vector2Int id;

        public Vector2Int gridID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
        public string gridTextData
        {
            get
            {
                return gridText.text;
            }
            set
            {
                gridText.text = value;
            }
        }

        public void SetGridColor(Color color)
        {
            gridImg.color = color;
        }
    }
}
