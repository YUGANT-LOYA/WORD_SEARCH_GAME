using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class QuesGrid : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI quesText;
        public bool isMarked;

        public string QuesTextData
        {
            get => quesText.text;
            set => quesText.text = value;
        }

        public void StrikeQues()
        {
            string str = $"<b><s>{QuesTextData}</s></b>";
            quesText.color = Color.gray;
            QuesTextData = str;
        }
    }
}
