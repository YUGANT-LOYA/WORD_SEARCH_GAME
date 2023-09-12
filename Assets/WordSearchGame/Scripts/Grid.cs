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

    }
}
