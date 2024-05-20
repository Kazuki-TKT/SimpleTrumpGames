using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace KazukiTrumpGame.BlackJack
{
    /// <summary>
    /// ターンのタイプによって処理をするクラス
    /// </summary>
    public class BlackJuckTurnGUI : CutInEventBase
    {
        BlackJackSceneDirector sceneDirector;

        //GUI
        [SerializeField]
        GameObject playerTurnObject, dealerTurnObject,playerBurstObject,dealerBurstObject,menuButtonObject;

        private void Start()
        {
            sceneDirector = GameObject.FindGameObjectWithTag("GameDirector").GetComponent<BlackJackSceneDirector>();

            //登録
            sceneDirector.onTurnChanged += HandleTurnTypeChanged;
            sceneDirector.onBurstEvent += HandleBurstEvent;
        }

        private void OnDestroy()
        {
            //解除
            sceneDirector.onTurnChanged -= HandleTurnTypeChanged;
            sceneDirector.onBurstEvent -= HandleBurstEvent;
        }

        void HandleTurnTypeChanged(TurnType newTurn)
        {
            //ターンのタイプにより分岐
            switch (newTurn)
            {
                case TurnType.PLAYER://プレイヤー
                    StartCoroutine(MoveCutInTextObject(playerTurnObject,1.5f));
                    break;
                case TurnType.DEALER://ディーラー
                    StartCoroutine(MoveCutInTextObject(dealerTurnObject, 1.5f));
                    menuButtonObject.SetActive(false);
                    break;
                case TurnType.INITIAL://初期
                    menuButtonObject.SetActive(true);
                    break;
            }
        }

        //バースト時のメソッド
        void HandleBurstEvent(int who)
        {
            switch (who)
            {
                case 0://プレイヤー
                    StartCoroutine(MoveBurstTextObject(playerBurstObject,JudgeType.LOSE));
                    break;
                case 1://ディーラー
                    StartCoroutine(MoveBurstTextObject(dealerBurstObject, JudgeType.WIN));
                    break;
            }
        }

        IEnumerator MoveBurstTextObject(GameObject burstObject,JudgeType judgeType)
        {
            //オブジェクト表示
            burstObject.SetActive(true);

            //子オブジェクト取得
            Transform childTransform = burstObject.transform.GetChild(0);
            GameObject childObject = childTransform.gameObject;

            //画面中央に移動
            childObject.transform.DOLocalMoveX(0, 0.5f);

            //指定秒待機
            yield return new WaitForSeconds(1.5f);

            //画面外へ移動
            childObject.transform.DOLocalMoveX(targetPositionX, 0.5f).OnComplete(() =>
            {
                childObject.transform.DOLocalMoveX(defultPositionX, 0);//元の座標に戻す
                sceneDirector.CurrentJudgeType = judgeType;
                burstObject.SetActive(false);//オブジェクト非表示
            });
        }
    }
}
