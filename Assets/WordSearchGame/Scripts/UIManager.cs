using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace YugantLoyaLibrary.WordSearchGame
{
    public class UIManager : MonoBehaviour
    {
        public float coinAnimTime = 1.5f, coinRotateAngle = 810f, maxCoinScale = 45f;
        public TextMeshProUGUI coinText;
        public GameObject winPanel;
        public GameObject menuGameObj, coinHolderGm;
        public Ease coinMovementEase;
        private float _coinTextUpdateTime;
        
        
        public void CoinCollectionAnimation(int coinToAdd)
        {
            StartCoroutine(nameof(PlayCoinAnim), coinToAdd);
        }

        private IEnumerator PlayCoinAnim(int coinToBeAdded)
        {
            CallWinPanel();
            _coinTextUpdateTime = coinAnimTime / 2;
            float xVal = Random.Range(-0.5f, 0.5f);
            float yVal = Random.Range(-0.5f, 0.5f);
            bool isCoinTextUpdating = false;
            int totalCoin = GameController.instance.coinPoolSize;
            Transform trans = GameController.instance.coinContainerTran;
            float coinSpawningTime = coinAnimTime / 2;
            float coinMovementTime = (coinSpawningTime) / totalCoin;

            for (int i = 0; i < GameController.instance.coinPoolSize; i++)
            {
                GameObject coin = DataHandler.instance.GetCoin();
                Vector2 position = coinText.transform.position;
                coin.transform.localScale = Vector3.one * maxCoinScale;
                coin.transform.rotation = quaternion.identity;
                coin.transform.position = new Vector3(xVal, yVal, 0f);
                coin.SetActive(true);

                yield return new WaitForSeconds(coinMovementTime);
                
                coin.transform.DORotate(new Vector3(coinRotateAngle, coinRotateAngle,
                    coinRotateAngle), _coinTextUpdateTime, RotateMode.FastBeyond360).SetEase(coinMovementEase);
                
                coin.transform.DOMove(new Vector2(position.x - 0.65f, position.y), _coinTextUpdateTime)
                    .SetEase(coinMovementEase).OnComplete(
                        () =>
                        {
                            DataHandler.instance.ResetCoin(coin);
                            if (isCoinTextUpdating) return;
                            isCoinTextUpdating = true;
                            StartCoroutine(UpdateCoinText(coinToBeAdded, (_coinTextUpdateTime/2)));
                        });
                
                xVal = Random.Range(-0.5f, 0.5f);
                yVal = Random.Range(-0.5f, 0.5f);
            }
        }

        void CallWinPanel()
        {
            GameController.instance.uiManager.winPanel.SetActive(true);
        }

        private IEnumerator UpdateCoinText(int coinToBeAdded, float coinMoveTime, Action callback = null)
        {
            int startVal = int.Parse(coinText.text);
            float time = coinMoveTime / (float)coinToBeAdded;

            for (int i = 1; i <= coinToBeAdded; i++)
            {
                coinText.text = $"{startVal + i}";
                yield return new WaitForSeconds(time);
            }
        }

        public void PrevLevel()
        {
            GameController.instance.PreviousLevel();
        }

        public void NextLevel()
        {
            GameController.instance.NextLevel();
        }
    }
}