using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KazukiTrumpGame
{
    public class TextGlowButtonAnimation : MonoBehaviour
    {
        //GUI
        TextMeshProUGUI textMeshProUGUI;

        //テキストメッシュプロのアニメーター
        private DOTweenTMPAnimator tmproAnimator;

        //カスタムボタン
        CustomButton customButton;

        //アニメーションの繰り返しで使用
        private bool isAnimating = true;

        Sequence tweenSequence;
        private void Start()
        {
            textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            customButton = transform.parent.gameObject.GetComponent<CustomButton>();
            textMeshProUGUI.color = Color.black;

            StartCoroutine(GlowTextCoroutine());
            customButton.onClickCallback = () =>
            {
                tweenSequence.Kill();
            };

        }

        private IEnumerator GlowTextCoroutine()
        {
            while (isAnimating)
            {
                GlowText();
                yield return new WaitForSeconds(0.07f * tmproAnimator.textInfo.characterCount + 0.4f);
            }
        }

        void GlowText()
        {
            if (tmproAnimator == null)
            {
                tmproAnimator = new DOTweenTMPAnimator(textMeshProUGUI);
            }

            for (int i = 0; i < tmproAnimator.textInfo.characterCount; ++i)
            {
                if (!tmproAnimator.textInfo.characterInfo[i].isVisible)
                {
                    continue;
                }

                tweenSequence = DOTween.Sequence()
                    .Append(tmproAnimator.DOColorChar(i, new Color(1f, 1f, 0.8f), 0.2f).SetLoops(2, LoopType.Yoyo))
                    .SetDelay(0.07f * i);
            }
        }
    }
}

