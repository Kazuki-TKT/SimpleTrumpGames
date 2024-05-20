using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace KazukiTrumpGame.BlackJack
{
    /// <summary>
    /// �Q�[�����̃u���b�N�W���b�N�̃X�R�A�ɉ����ăe�L�X�g�A�j���[�V��������N���X
    /// </summary>
    public class BlackJackScoreTextAnimation : MonoBehaviour
    {
        BlackJackSceneDirector sceneDirector;

        //UI
        [SerializeField]
        TextMeshProUGUI playerScoreText, dealerScoreText;

        //�f�t�H���g�̃J���[�A�o�[�X�g�̃J���[
        public Color defaultColor, burstColor;

        //1�O�̃X�R�A��ۑ�����ϐ�
        int playerBeforeScore,dealerBeforeScore=0;

        void Start()
        {
            sceneDirector = GameObject.FindGameObjectWithTag("GameDirector").GetComponent<BlackJackSceneDirector>();

            //�o�^
            sceneDirector.onPleyerScoreChanged += HandlePlayerScoreChanged;
            sceneDirector.onDealerScoreChanged += HandleDealerScoreChanged;
        }

        private void OnDestroy()
        {
            //����
            sceneDirector.onPleyerScoreChanged -= HandlePlayerScoreChanged;
            sceneDirector.onDealerScoreChanged -= HandleDealerScoreChanged;
        }

        //�v���C���[�̃X�R�A�ɑ΂��ēo�^���郁�\�b�h
        void HandlePlayerScoreChanged(int score)
        {
            ChangeScore(playerScoreText, playerBeforeScore, score);
        }

        //�f�B�[���[�̃X�R�A�ɑ΂��ēo�^���郁�\�b�h
        void HandleDealerScoreChanged(int score)
        {
            ChangeScore(dealerScoreText, dealerBeforeScore, score);
        }

        //�X�R�A��
        void ChangeScore(TextMeshProUGUI targetText, int beforeScore, int score)
        {
            if (score == 0||beforeScore==score)//�X�R�A��0���A�ς��Ȃ������ꍇ
            {
                targetText.text = 0.ToString();//�e�L�X�g��0
                targetText.color = defaultColor;//�J���[���f�t�H���g
            }
            else
            {
                if (score > 21)//21��������
                {
                    targetText.color = burstColor;//�J���[���o�[�X�g
                }
                TextAnimation.AnimateNumber(beforeScore, score, 0.3f, targetText);//�e�L�X�g�A�j���[�V����
                beforeScore = score;
            }
        }

       
    }
}
