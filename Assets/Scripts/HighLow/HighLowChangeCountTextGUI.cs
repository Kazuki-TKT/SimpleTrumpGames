using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Newtonsoft.Json.Linq;

namespace KazukiTrumpGame.HighLow
{
    /// <summary>
    /// ���s�̔���ɂ��A�J�E���g�e�L�X�g�̒l��ύX����N���X
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

        //���������Ɏ��s����郁�\�b�h
        private void WinCountChanged(int newValue)
        {
            if (newValue == 0) winText.text = newValue.ToString();
            else
                CountTextAnimation(winText, newValue);
        }

        //���������Ɏ��s����郁�\�b�h
        private void LoseCountChanged(int newValue)
        {
            if (newValue == 0) loseText.text = newValue.ToString();
            else
                CountTextAnimation(loseText, newValue);
        }

        //�������������Ɏ��s����郁�\�b�h
        private void DrawCountChanged(int newValue)
        {
            if (newValue == 0) drawText.text = newValue.ToString();
            else
                CountTextAnimation(drawText, newValue);
        }

        /// <summary>
        /// �e�L�X�g�̃A�j���[�V����
        /// </summary>
        void CountTextAnimation(TextMeshProUGUI countText, int count)
        {
            float duration = 0.2f;
            countText.text = count.ToString();
            // �e�L�X�g�̏����X�P�[����ۑ�
            Vector3 originalScale = countText.transform.localScale;

            // �e�L�X�g���g�傷��Tween
            var scaleUpTween = countText.transform.DOScale(originalScale * 1.3f, duration)
                .SetEase(Ease.OutQuad); // �g�傷��A�j���[�V����

            // �e�L�X�g�����̃T�C�Y�ɖ߂�Tween
            var scaleDownTween = countText.transform.DOScale(originalScale, duration)
                .SetEase(Ease.InQuad) // ���̃T�C�Y�ɖ߂�A�j���[�V����
                .SetDelay(duration); // �g�傪�I�������ɊJ�n

            // �g��ƌ��ɖ߂�Tween��A�����Ď��s
            scaleUpTween.OnComplete(() => scaleDownTween.Restart());
        }
    }
}
