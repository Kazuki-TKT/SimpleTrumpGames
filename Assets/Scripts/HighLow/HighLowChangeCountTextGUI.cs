using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Newtonsoft.Json.Linq;

namespace KazukiTrumpGame.HighLow
{
    /// <summary>
    /// 勝敗の判定により、カウントテキストの値を変更するクラス
    /// </summary>
    public class HighLowChangeCountTextGUI : MonoBehaviour
    {
        HighLowSceneDirector sceneDirector;

        [SerializeField]
        TextMeshProUGUI winText, loseText, drawText;
        void Start()
        {
            sceneDirector = GameObject.FindGameObjectWithTag("GameDirector").GetComponent<HighLowSceneDirector>();
            sceneDirector.OnJudgeTypeChanged += HandleJudgeTypeChanged;
        }

        private void OnDestroy()
        {
            sceneDirector.OnJudgeTypeChanged -= HandleJudgeTypeChanged;
        }

        void HandleJudgeTypeChanged(JudgeType newJudge)
        {
            switch (newJudge)
            {
                case JudgeType.WIN:
                    WinCountChanged(sceneDirector.WinCount);
                    break;
                case JudgeType.LOSE:
                    LoseCountChanged(sceneDirector.LoseCount);
                    break;
                case JudgeType.DRAW:
                    DrawCountChanged(sceneDirector.DrawCount);
                    break;
                case JudgeType.INITIAL:
                    if (sceneDirector.WinCount + sceneDirector.LoseCount + sceneDirector.DrawCount == 0)
                    {
                        winText.text = loseText.text=drawText.text= 0.ToString();
                    }
                    break;
            }
        }

        //勝った時に実行されるメソッド
        private void WinCountChanged(int newValue)
        {
            if (newValue == 0) winText.text = newValue.ToString();
            else
                CountTextAnimation(winText, newValue);
        }

        //負けた時に実行されるメソッド
        private void LoseCountChanged(int newValue)
        {
            if (newValue == 0) loseText.text = newValue.ToString();
            else
                CountTextAnimation(loseText, newValue);
        }

        //引き分けた時に実行されるメソッド
        private void DrawCountChanged(int newValue)
        {
            if (newValue == 0) drawText.text = newValue.ToString();
            else
                CountTextAnimation(drawText, newValue);
        }

        /// <summary>
        /// テキストのアニメーション
        /// </summary>
        void CountTextAnimation(TextMeshProUGUI countText, int count)
        {
            float duration = 0.2f;
            countText.text = count.ToString();
            // テキストの初期スケールを保存
            Vector3 originalScale = countText.transform.localScale;

            // テキストを拡大するTween
            var scaleUpTween = countText.transform.DOScale(originalScale * 1.3f, duration)
                .SetEase(Ease.OutQuad); // 拡大するアニメーション

            // テキストを元のサイズに戻すTween
            var scaleDownTween = countText.transform.DOScale(originalScale, duration)
                .SetEase(Ease.InQuad) // 元のサイズに戻るアニメーション
                .SetDelay(duration); // 拡大が終わった後に開始

            // 拡大と元に戻るTweenを連続して実行
            scaleUpTween.OnComplete(() => scaleDownTween.Restart());
        }
    }
}
