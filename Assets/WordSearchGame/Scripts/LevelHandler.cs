using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class LevelHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] Level currLevel;
        [SerializeField] Grid currGrid;
        [SerializeField] LineRenderer lineRenderer;
        [SerializeField] LayerMask gridLayerMask;


        private void Awake()
        {
            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                if ((gridLayerMask.value & (1 << eventData.pointerCurrentRaycast.gameObject.layer)) > 0)
                {
                    Debug.Log("GameObj : " + eventData.pointerCurrentRaycast.gameObject);
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                if ((gridLayerMask.value & (1 << eventData.pointerCurrentRaycast.gameObject.layer)) > 0)
                {
                    Debug.Log("GameObj : " + eventData.pointerCurrentRaycast.gameObject);
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {

        }
    }
}
