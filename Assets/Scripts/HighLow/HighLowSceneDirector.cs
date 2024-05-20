using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using DG.Tweening;

namespace KazukiTrumpGame.HighLow
{
    /// <summary>
    /// �n�C�����[�̃Q�[�����Ǘ�����N���X
    /// </summary>
    public class HighLowSceneDirector : MonoBehaviour, IGameController
    {
        //���s�̃^�C�v
        JudgeType currentJudgeType;
        public event Action<JudgeType> OnJudgeTypeChanged;
        public JudgeType CurrentJudgeType
        {
            get => currentJudgeType;
            set
            {
                currentJudgeType = value;
                switch (currentJudgeType)
                {
                    case JudgeType.WIN:
                        winCount++;
                        break;
                    case JudgeType.LOSE:
                        loseCount++;
                        break;
                    case JudgeType.DRAW:
                        drawCount++;
                        break;
                    case JudgeType.INITIAL:
                        break;
                }
                OnJudgeTypeChanged?.Invoke(currentJudgeType);
            }
        }

        //GUI
        [SerializeField] GameObject buttonHighObject, buttonLowObject;
        CustomButton customButtonHigh, customButtonLow;
        [SerializeField] CustomButton customButtonStartGame;
        [SerializeField] CustomButton[] customRestartButton;

        //�Q�[���Ŏg���J�[�h
        List<CardController> cards;
        
        //���݂̃C���f�b�N�X
        int cardIndex;

        //������
        int winCount, loseCount, drawCount;
        public int WinCount{get => winCount;}
        public int LoseCount{get => loseCount;}
        public int DrawCount{get => drawCount;}

        //�Q�[���J�E���g
        [SerializeField] TextMeshProUGUI gameCountText;
        int gameCount = 26;
        int GameCount
        {
            get => gameCount;
            set
            {
                gameCount += value;
                GameCountTextChange();
            }
        }

        //�ҋ@����
        const float NextWaitTimer = 2.0f;

        //�|�W�V����
        public Vector3 stackPosition, leftCardPosition, rightCardPosition;

        //
        [SerializeField] HighLowResultGUI resultGUI;

        void Start()
        {
            //�J�X�^���{�^���̎擾
            if (buttonHighObject != null) customButtonHigh = buttonHighObject.GetComponent<CustomButton>();
            if (buttonLowObject != null) customButtonLow = buttonLowObject.GetComponent<CustomButton>();

            //���ꂼ��̃J�X�^���{�^���ɏ�����o�^
            customButtonHigh.onClickCallback = () => OnClickHigh();
            customButtonLow.onClickCallback = () => OnClickLow();
            customButtonStartGame.onClickCallback = () => StartGame();
            foreach(CustomButton customButton in customRestartButton)
            {
                customButton.onClickCallback = () => ReStartGame();
            }

            //�{�^�����\��
            SetHighLowButtons(false);
        }

        //�Q�[�����X�^�[�g������
        public void StartGame()
        {
            InitialGame();
            CurrentJudgeType = JudgeType.INITIAL;
            StartCoroutine(StartGameCoroutin());
        }

        //�Q�[�����X�^�[�g�����鏀�����s���R���[�`��
        IEnumerator StartGameCoroutin()
        {
            //�R�D�쐬
            cards = CardsDirector.Instance.GetHighLowCards();
            foreach(var item in cards)//�����ʒu�ƌ�����ݒ�
            {
                item.transform.position = stackPosition;
                item.FlipCard(false);
            }
            yield return new WaitForSeconds(0.5f);

            //�g�����v��2���Z�b�g����
            StartCoroutine(DealCards());

            //�{�^�����Z�b�g
            SetHighLowButtons(true);
        }

        public void StopGame()
        {
           
        }

        //�Q�[�������X�^�[�g������
        public void ReStartGame()
        {
            foreach(CardController card in cards)
            {
                Destroy(card.gameObject);//���X�g�ɂ���I�u�W�F�N�g���폜
            }
            cards.Clear();//���X�g���N���A
            InitialGame();//������
            CurrentJudgeType = JudgeType.INITIAL;//�����ύX
            StartCoroutine(StartGameCoroutin());//�Q�[���X�^�[�g
        }

        //�Q�[���̏�����
        public void InitialGame()
        {
            gameCount = 26;
            GameCountTextChange();
            winCount = loseCount = drawCount = cardIndex =0;
            customButtonStartGame.transform.parent.gameObject.SetActive(false);
        }

        /// <summary>
        /// ���s�𔻒肵�A���̃Q�[�����s�����\�b�h
        /// </summary>
        void CheckHighLow(bool high)
        {
            
            //�E�̃g�����v���I�[�v��
            cards[cardIndex + 1].GetComponent<CardController>().FlipCardAnimation();

            int leftTmpNumber = cards[cardIndex].GetComponent<CardController>().number;//���̃g�����v�̐���
            int rightTmpNumber = cards[cardIndex + 1].GetComponent<CardController>().number;//�E�̃g�����v�̐���

            if (leftTmpNumber == rightTmpNumber)//�����������������ꍇ
            {
                CurrentJudgeType = JudgeType.DRAW;
            }
            else if (high)//HIGH��I�񂾏ꍇ
            {
                if (leftTmpNumber < rightTmpNumber)//�������g�����v�̐������傫�������ꍇ
                {
                    CurrentJudgeType = JudgeType.WIN;
                }
                else
                {
                    CurrentJudgeType = JudgeType.LOSE;
                }
            }
            else//LOW��I�񂾏ꍇ
            {
                if (leftTmpNumber > rightTmpNumber)//�������g�����v�̐����������������ꍇ
                {
                    CurrentJudgeType = JudgeType.WIN;
                }
                else
                {
                    CurrentJudgeType = JudgeType.LOSE;
                }
            }
            StartCoroutine(NextCards());
        }

        /// <summary>
        /// ���̃J�[�h���Z�b�g����R���[�`��
        /// �R�D���g�����������A�I���������s��
        /// </summary>
        IEnumerator NextCards()
        {
            //--�w��b�ҋ@
            yield return new WaitForSeconds(NextWaitTimer);

            CurrentJudgeType = JudgeType.INITIAL;

            //�g�����v��Еt����
            cards[cardIndex].gameObject.SetActive(false);
            cards[cardIndex + 1].gameObject.SetActive(false);

            //���̃J�[�h��1����
            cardIndex += 2;

            if (cards.Count - 1 <= cardIndex)//�S�Ă̎R�D���g���؂���
            {
                resultGUI.EndGame(winCount,loseCount,drawCount);
            }
            else//���̃Q�[��
            {
                StartCoroutine(DealCards());
            }
        }

        /// <summary>
        /// �g�����v��2���Z�b�g���郁�\�b�h
        /// </summary>
        IEnumerator DealCards()
        {
            GameCount=-1;

            //�����ɃI�[�v��������Ԃ̃g�����v��z�u
            cards[cardIndex].transform.DOMove(leftCardPosition, 0.2f);
            cards[cardIndex].GetComponent<CardController>().FlipCard();
            cards[cardIndex].transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

            yield return new WaitForSeconds(0.2f);

            //�E���ɕ�������Ԃ̃g�����v��z�u
            cards[cardIndex + 1].transform.DOMove(rightCardPosition, 0.2f);
            cards[cardIndex + 1].transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

            //�{�^���̕\��
            SetHighLowButtons(true);
        }

        // HIGH�{�^�������������Ɏ��s���郁�\�b�h
        void OnClickHigh()
        {
            SetHighLowButtons(false);//�둀��h�~
            CheckHighLow(true);
        }

        // LOW�{�^�������������Ɏ��s���郁�\�b�h
        void OnClickLow()
        {
            SetHighLowButtons(false);//�둀��h�~
            CheckHighLow(false);
        }

        //�n�C���\�{�^���̕\���ύX
        void SetHighLowButtons(bool active)
        {
            buttonHighObject.SetActive(active);
            buttonLowObject.SetActive(active);
        }

        void GameCountTextChange()
        {
            gameCountText.text = $"�c��{gameCount}�Q�[��";
        }
    }
}
