using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KazukiTrumpGame.BlackJack
{
    /// <summary>
    /// �u���b�N�W���b�N�̌��ʂ�\��������
    /// </summary>
    public class BlackJackResultGUI : MonoBehaviour
    {
        [SerializeField]
        BlackJackSceneDirector sceneDirector;

        //UI
        [SerializeField]
        TextMeshProUGUI resultText, playerCountText, dealerCountText;

        //�\��������I�u�W�F�N�g�Q
        [SerializeField]
        GameObject[] setObjects;

        private void Start()
        {
            sceneDirector.onJudgeChanged += HandleJudgeTypeChanged;//�o�^
        }
        private void OnDestroy()
        {
            sceneDirector.onJudgeChanged -= HandleJudgeTypeChanged;//����
        }

        //�o�^���郁�\�b�h
        void HandleJudgeTypeChanged(JudgeType newJudge)
        {
            if (newJudge == JudgeType.INITIAL) return;//INITIAL�̏ꍇ�̓��^�[��
            switch (newJudge)
            {
                case JudgeType.WIN://�����̏ꍇ
                    resultText.text = "YOU WIN!!";
                    resultText.color = CardsDirector.Instance.WIN_COLOR;
                    break;
                case JudgeType.LOSE://�����̏ꍇ
                    resultText.text = "YOU LOSE...";
                    resultText.color = CardsDirector.Instance.LOSE_COLOR;
                    break;
                case JudgeType.DRAW://���������̏ꍇ
                    resultText.text = "DRAW";
                    resultText.color = CardsDirector.Instance.DRAW_COLOR;
                    break;
                case JudgeType.INITIAL:
                    break;
            }

            //�X�R�A��\��
            playerCountText.text = sceneDirector.PlayerScore.ToString();
            dealerCountText.text = sceneDirector.DealerScore.ToString();

            //�I�u�W�F�N�g�Q���\��
            SetObject(setObjects, false);

            //�p�l���𓮂���
            StartCoroutine(MovePanel(-50));
        }

        //���X�^�[�g�{�^���ɓo�^����
        public void ReStartGame()
        {
            SetObject(setObjects, true);//�I�u�W�F�N�g�Q��\��
            StartCoroutine(MovePanel(1000));//�p�l���𓮂���
            sceneDirector.ReStartGame();//�Q�[�����ĊJ
        }

        //�I�u�W�F�N�g�𓮂������\�b�h
        IEnumerator MovePanel(float positionY)
        {
            gameObject.transform.DOLocalMoveY(positionY, 0.5f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(0.5f);
        }

        //�I�u�W�F�N�g���Z�b�g���郁�\�b�h
        void SetObject(GameObject[] objects, bool active)
        {
            foreach (GameObject gameObject in objects)
            {
                gameObject.SetActive(active);
            }
        }

    }
}