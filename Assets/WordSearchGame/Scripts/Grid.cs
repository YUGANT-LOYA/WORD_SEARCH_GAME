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
        public Image gridBg;
        public bool isCorrect;
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

        public void SetGridBg(bool isActive)
        {
            Color gridColor = gridBg.color;
            gridBg.color = isActive ? new Color(gridColor.r,gridColor.g,gridColor.b,255f) : new Color(gridColor.r, gridColor.g, gridColor.b, 0f);
        }

    }
}
