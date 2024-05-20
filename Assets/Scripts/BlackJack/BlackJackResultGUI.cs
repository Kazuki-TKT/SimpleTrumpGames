using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KazukiTrumpGame.BlackJack
{
    /// <summary>
    /// ブラックジャックの結果を表示させる
    /// </summary>
    public class BlackJackResultGUI : MonoBehaviour
    {
        [SerializeField]
        BlackJackSceneDirector sceneDirector;

        //UI
        [SerializeField]
        TextMeshProUGUI resultText, playerCountText, dealerCountText;

        //表示させるオブジェクト群
        [SerializeField]
        GameObject[] setObjects;

        private void Start()
        {
            sceneDirector.onJudgeChanged += HandleJudgeTypeChanged;//登録
        }
        private void OnDestroy()
        {
            sceneDirector.onJudgeChanged -= HandleJudgeTypeChanged;//解除
        }

        //登録するメソッド
        void HandleJudgeTypeChanged(JudgeType newJudge)
        {
            if (newJudge == JudgeType.INITIAL) return;//INITIALの場合はリターン
            switch (newJudge)
            {
                case JudgeType.WIN://勝ちの場合
                    resultText.text = "YOU WIN!!";
                    resultText.color = CardsDirector.Instance.WIN_COLOR;
                    break;
                case JudgeType.LOSE://負けの場合
                    resultText.text = "YOU LOSE...";
                    resultText.color = CardsDirector.Instance.LOSE_COLOR;
                    break;
                case JudgeType.DRAW://引き分けの場合
                    resultText.text = "DRAW";
                    resultText.color = CardsDirector.Instance.DRAW_COLOR;
                    break;
                case JudgeType.INITIAL:
                    break;
            }

            //スコアを表示
            playerCountText.text = sceneDirector.PlayerScore.ToString();
            dealerCountText.text = sceneDirector.DealerScore.ToString();

            //オブジェクト群を非表示
            SetObject(setObjects, false);

            //パネルを動かす
            StartCoroutine(MovePanel(-50));
        }

        //リスタートボタンに登録する
        public void ReStartGame()
        {
            SetObject(setObjects, true);//オブジェクト群を表示
            StartCoroutine(MovePanel(1000));//パネルを動かす
            sceneDirector.ReStartGame();//ゲームを再開
        }

        //オブジェクトを動かすメソッド
        IEnumerator MovePanel(float positionY)
        {
            gameObject.transform.DOLocalMoveY(positionY, 0.5f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(0.5f);
        }

        //オブジェクトをセットするメソッド
        void SetObject(GameObject[] objects, bool active)
        {
            foreach (GameObject gameObject in objects)
            {
                gameObject.SetActive(active);
            }
        }

    }
}