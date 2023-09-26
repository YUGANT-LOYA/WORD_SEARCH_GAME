using System;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class Grid : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI gridText;
        public Image gridBg;
        public bool isSelectable = true;
        [SerializeField] private Vector2Int id;

        public Vector2Int GridID
        {
            get => id;
            set => id = value;
        }
        public string GridTextData
        {
            get => gridText.text;
            set => gridText.text = value;
        }

        public void SetGridBg(bool isActive)
        {
            Color gridColor = gridBg.color;
            gridBg.color = isActive ? new Color(gridColor.r,gridColor.g,gridColor.b,255f) : new Color(gridColor.r, gridColor.g, gridColor.b, 0f);
        }

        private void OnDestroy()
        {
            DOTween.Kill(this, false);
        }
    }
}
