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
        [SerializeField] BoxCollider2D boxCollider2D;
        [SerializeField] Image gridImg;
        public GameObject straightColorGm,startColorGm;
        public bool isMarked,isCorrect;
        [SerializeField] Vector2Int id;
        [SerializeField] Vector2 colorImgSize;
        [SerializeField] float straightLineWidthOffset = 20f;

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


        public void SetBoxColliderSize(float width,float height)
        {
            boxCollider2D.size = new Vector2(width, height);
        }
        public void SetGridColor(Color color)
        {
            gridImg.color = color;
        }

        public void SetLineColorTransform(float width,float height)
        {
            colorImgSize = new Vector2(width - straightLineWidthOffset, height);
            straightColorGm.GetComponent<RectTransform>().sizeDelta = colorImgSize;
            startColorGm.GetComponent<RectTransform>().sizeDelta = colorImgSize;
        }
    }
}
