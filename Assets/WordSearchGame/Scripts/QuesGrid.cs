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

        public string quesTextData
        {
            get
            {
                return quesText.text;
            }
            set
            {
                quesText.text = value;
            }
        }

        public void StrikeQues()
        {
            string str = $"<b><s>{quesTextData}</s></b>";
            quesTextData = str;
        }
    }
}
