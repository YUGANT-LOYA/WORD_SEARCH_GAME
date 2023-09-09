using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
}
