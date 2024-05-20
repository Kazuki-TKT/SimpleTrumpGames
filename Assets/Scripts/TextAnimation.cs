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
        /// ���ʂ�\������e�L�X�g�A�j���[�V����
        /// </summary>
        public static void ResultTextAnimation(TextMeshProUGUI resultText, Color color, string result)
        {
            resultText.color = color;
            resultText.text = result;
            DOTweenTMPAnimator animator = new DOTweenTMPAnimator(resultText);

            for (int i = 0; i < animator.textInfo.characterCount; i++)
            {
                //0.5�b�����ď��1.2�{�Ɋg��A�������J�n��0.1�b�����炷
                animator.DOScaleChar(i, Vector3.one * 1.2f, 0.1f).SetDelay(i * 0.05f);
            }
        }

         public static  void AnimateNumber(int fromValue, int toValue, float duration, TextMeshProUGUI targetText)
        {
            // �l���i�[����ϐ�
            int currentValue = fromValue;

            // DOTween�ŃA�j���[�V�������쐬
            DOTween.To(() => currentValue, x =>
            {
                currentValue = x;
                targetText.text = currentValue.ToString();
            }, toValue, duration).SetEase(Ease.Linear);
        }
    }
}
