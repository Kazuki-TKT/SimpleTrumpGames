using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;

namespace KazukiTrumpGame
{
    public class TextAnimation : MonoBehaviour
    {
        public static TextAnimation Instance;

        /// <summary>
        /// 結果を表示するテキストアニメーション
        /// </summary>
        public static void ResultTextAnimation(TextMeshProUGUI resultText, Color color, string result)
        {
            resultText.color = color;
            resultText.text = result;
            DOTweenTMPAnimator animator = new DOTweenTMPAnimator(resultText);

            for (int i = 0; i < animator.textInfo.characterCount; i++)
            {
                //0.5秒かけて上に1.2倍に拡大、ただし開始は0.1秒ずつずらす
                animator.DOScaleChar(i, Vector3.one * 1.2f, 0.1f).SetDelay(i * 0.05f);
            }
        }

         public static  void AnimateNumber(int fromValue, int toValue, float duration, TextMeshProUGUI targetText)
        {
            // 値を格納する変数
            int currentValue = fromValue;

            // DOTweenでアニメーションを作成
            DOTween.To(() => currentValue, x =>
            {
                currentValue = x;
                targetText.text = currentValue.ToString();
            }, toValue, duration).SetEase(Ease.Linear);
        }
    }
}
