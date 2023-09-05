using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class Grid : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI gridText;
        [SerializeField] BoxCollider2D boxCollider2D;

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

        public void UpdateColliderSize(float sizeX,float sizeY)
        {
            boxCollider2D.size = new Vector2(sizeX,sizeY);
        }
    }
}
