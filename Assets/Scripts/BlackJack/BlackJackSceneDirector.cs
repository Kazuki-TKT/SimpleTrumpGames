using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace KazukiTrumpGame.BlackJack
{
    /// <summary>
    /// �u���b�N�W���b�N�̃Q�[�����Ǘ�����N���X
    /// </summary>
    public class BlackJackSceneDirector : MonoBehaviour, IGameController
    {
        //���s�̃^�C�v
        JudgeType currentJudgeType;
        public event Action<JudgeType> onJudgeChanged;
        public JudgeType CurrentJudgeType
        {
            get => currentJudgeType;
            set
            {
                currentJudgeType = value;
                onJudgeChanged?.Invoke(currentJudgeType);
            }
        }

        //���Ԃ̃^�C�v
        TurnType currentTurnType;
        public event Action<TurnType> onTurnChanged;
        public TurnType CurrentTurnType
        {
            get => currentTurnType;
            set
            {
                currentTurnType = value;
                onTurnChanged?.Invoke(currentTurnType);
            }
        }

        //�o�[�X�g�̃C�x���g
        public event Action<int> onBurstEvent;

        //�Q�[���Ŏg�p����g�����v�̃��X�g
        List<CardController> cards;
        public GameObject fieldObj;

        //UI
        [SerializeField]
        CustomButton customHitButton, customStayButton;

        //���ꂼ��̃X�R�A(�g�����v�̍��v�l)
        int playerScore, dealerScore;
        public Action<int> onPleyerScoreChanged, onDealerScoreChanged;
        public int PlayerScore
        {
            get => playerScore;
            set
            {
                playerScore = value;
                onPleyerScoreChanged?.Invoke(playerScore);
            }
        }
        public int DealerScore
        {
            get => dealerScore;
            set
            {
                dealerScore = value;
                onDealerScoreChanged?.Invoke(dealerScore);
            }
        }

        //���ꂼ��̎�D�̃��X�g
        public List<CardController> playerHand;
        public List<CardController> dealerHand;

        //�R�D�̃C���f�b�N�X
        int cardIndex;

        //�ҋ@����
        const float NextWaitTime = 2;

        //1���ڂ̃g�����v�̔z�z�ʒu
        public float playerPositionX, playerPositionZ, dealerPositionZ;

        //�f�B�[���[���ŏ��ɃJ�[�h���I�[�v���������ǂ����̐^�U�n
        bool dealerFirstOpen = false;

        //�L�����Z���g�[�N��
        CancellationTokenSource cts;

        private void Start()
        {
            SetBlackJackButtons(false);
            customHitButton.onClickCallback = () => OnClickHit().Forget();
            customStayButton.onClickCallback = () => OnClickStay();
        }

        //�Q�[���̏�����
        public void InitialGame()
        {
            CurrentJudgeType = JudgeType.INITIAL;
            PlayerScore = DealerScore = 0;
            dealerFirstOpen = false;
            SetBlackJackButtons(false);
        }

        //���X�^�[�g
        public void ReStartGame()
        {
            CardsDirector.Instance.DestroyCard();//��̃J�[�h���폜
            StartGame();
        }

        //�Q�[���X�^�[�g
        public void StartGame()
        {
            cts = new CancellationTokenSource();
            var token = cts.Token;
            InitialGame();
            StartGameTask(token).Forget();
        }

        //�Q�[�����~�߂�
        public void StopGame()
        {
            cts.Cancel();
        }

        /// <summary>
        /// �Q�[�����X�^�[�g����UniTask��p�������\�b�h
        /// </summary>
        async UniTask StartGameTask(CancellationToken token)
        {
            try
            {
                //�V���b�t�������J�[�h���擾
                cards = CardsDirector.Instance.GetShuffleCards();

                //�����ʒu
                InitializeCardPositions();

                //��D�̏�����
                playerHand = new List<CardController>();
                dealerHand = new List<CardController>();

                cardIndex = 0;

                //���ꂼ���1���ڒǉ�
                await HitCard(playerHand, true, token);
                await HitCard(dealerHand, false, token);
                //���ꂼ���2���ڒǉ�
                await HitCard(playerHand, true, token);
                await HitCard(dealerHand, true, token);

                //�v���C���[�̌��݂̎�D�̒l���擾
                PlayerScore = GetScore(playerHand);

                //�v���C���[�̃^�[���ɐؑ�
                CurrentTurnType = TurnType.PLAYER;

                //�w��b�ҋ@
                await UniTask.Delay(TimeSpan.FromSeconds(NextWaitTime), cancellationToken: token);

                //�{�^����\��
                SetBlackJackButtons(true);
            }
            catch
            {
                Debug.LogWarning("StartGame�̏������L�����Z������܂���");
            }
        }

        /// <summary>
        /// �v���C���[���f�B�[���[���R�D����1���J�[�h���������\�b�h
        /// </summary>
        async UniTask<bool> HitCard(List<CardController> hand, bool open, CancellationToken token)
        {
            try
            {
                //�v���C���[�̏����ʒu
                float x = playerPositionX;
                float z = playerPositionZ;

                //�f�B�[���[�̏����ʒu
                if (dealerHand == hand)
                {
                    z = dealerPositionZ;
                }

                //�J�[�h������Ȃ�E�ɕ��ׂ�
                if (0 < hand.Count)
                {
                    x = hand[hand.Count - 1].transform.position.x;
                    z = hand[hand.Count - 1].transform.position.z;
                }

                //�R�D�̃C���f�b�N�X����J�[�h���擾
                CardController card = cards[cardIndex];
                card.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);

                //�J�[�h���w��ʒu�ɔz��܂őҋ@
                await card.transform.DOLocalMove(new Vector3(x + 0.1f, 0, z), 0.2f).AsyncWaitForCompletion();

                //�J�[�h���I�[�v��
                if (open)
                {
                    card.FlipCardAnimation(true);
                }

                //���X�g�ɒǉ�
                hand.Add(card);
                cardIndex++;

                return true;
            }
            catch
            {
                Debug.LogWarning("HitCard�̏������L�����Z������܂���");
                return true;
            }

        }

        /// <summary>
        /// ��D���v�Z���郁�\�b�h
        /// </summary>
        int GetScore(List<CardController> hand)
        {
            int score = 0;
            List<CardController> ace = new List<CardController>();//A��1��11���Ŕ��f�������̂Ŋm�ۂ���

            foreach (var item in hand)
            {
                int no = item.number;
                if (1 == no)
                {
                    ace.Add(item);
                }
                else if (10 < no)//JQK�̌v�Z�i�G�D�͑S��10�j
                {
                    no = 10;
                }

                score += no;
            }

            //A��11�ɂ���
            foreach (var item in ace)
            {
                if ((score + 10) < 22)
                {
                    score += 10;
                }
            }

            return score;
        }

        /// <summary>
        /// �v���C���[�q�b�g�{�^��
        /// </summary>
        async UniTaskVoid OnClickHit()
        {
            if (currentTurnType != TurnType.PLAYER) return;//�v���C���[�^�[������Ȃ��ꍇ�͈ȍ~�̏������s��Ȃ�

            SetBlackJackButtons(false);//�{�^����\��

            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    await HitCard(playerHand, true, cts.Token);//�v���C���[�̃J�[�h���z��I����܂őҋ@
                    PlayerScore = GetScore(playerHand);//�v���C���[�̎�D�̒l���擾
                    if (21 < PlayerScore)//21�𒴂�����o�[�X�g
                    {
                        dealerHand[0].FlipCardAnimation();//�f�B�[���[��1���ڂ̎�D���J
                        DealerScore = GetScore(dealerHand);//�f�B�[���[�̎�D�̍��v�l���擾
                        onBurstEvent?.Invoke(0);//�v���C���[�̃o�[�X�g�C�x���g���s
                    }
                    else//21�����Ă��Ȃ��Ȃ�{�^�����ĕ\��
                    {
                        SetBlackJackButtons(true);
                    }
                }
                catch
                {
                    Debug.LogWarning("OnClickHit�̏������L�����Z������܂���");
                }
            }
        }

        /// <summary>
        /// �v���C���[�X�e�C�{�^��
        /// </summary>
        void OnClickStay()
        {
            if (currentTurnType != TurnType.PLAYER) return;//�v���C���[�^�[������Ȃ��ꍇ�͈ȍ~�̏������s��Ȃ�
            SetBlackJackButtons(false);//�{�^���������Ȃ��悤�ɔ�\��
            DealerHit().Forget();//�f�B�[���[�̍s���Ɉڂ�
            CurrentTurnType = TurnType.DEALER;//�^�[�����f�B�[���[��
        }

        /// <summary>
        /// �f�B�[���[�̍s��
        /// </summary>
        async UniTaskVoid DealerHit()
        {
            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    //�w��b�ҋ@
                    await UniTask.Delay(TimeSpan.FromSeconds(NextWaitTime), cancellationToken: cts.Token);

                    DealerScore = GetScore(dealerHand);//�f�B�[���[�̎�D�̒l���擾
                                                       //�v���C���[���X�e�C�ゾ���ɍs���鏈��
                    if (!dealerFirstOpen)
                    {
                        dealerHand[0].FlipCardAnimation();//�������Ă���1���ڂ��I�[�v��
                        await UniTask.Delay(TimeSpan.FromSeconds(0.6), cancellationToken: cts.Token);//�w��b�ҋ@
                        dealerFirstOpen = true;
                    }

                    //18�����̏ꍇ
                    if (18 > DealerScore)
                    {
                        await HitCard(dealerHand, true, cts.Token);//�f�B�[���[�������܂őҋ@
                        DealerScore = GetScore(dealerHand);//�f�B�[���[�̎�D�̒l���擾
                    }

                    //��D�̒l��21����̏ꍇ
                    if (21 < DealerScore)
                    {
                        onBurstEvent?.Invoke(1);//�f�B�[���[�̃o�[�X�g�C�x���g���s
                    }
                    else if (17 < DealerScore)//��D�̒l��17����̏ꍇ
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(0.5));//�w��b�ҋ@
                        DetermineWinner();//���s�����߂�
                    }
                    else//������x�������J��Ԃ�
                    {
                        DealerHit().Forget();//������x�s��
                    }
                }
                catch
                {
                    Debug.LogWarning("DealerHit�̏������L�����Z������܂���");
                }

            }
                
        }

        //���s�����߂郁�\�b�h
        void DetermineWinner()
        {
            if (PlayerScore < DealerScore) // �f�B�[���[�����p�^�[��
            {
                CurrentJudgeType = JudgeType.LOSE;
            }
            else if (PlayerScore == DealerScore) // ���������p�^�[��
            {
                CurrentJudgeType = JudgeType.DRAW;
            }
            else // �v���C���[�����p�^�[��
            {
                CurrentJudgeType = JudgeType.WIN;
            }
        }

        //�����ʒu�����߂郁�\�b�h
        void InitializeCardPositions()
        {
            foreach (var item in cards)
            {
                item.transform.position = new Vector3(0.34f, 0, 0.15f);
                item.FlipCard(false);
            }
        }

        //�{�^���̕\���`��
        void SetBlackJackButtons(bool active)
        {
            customHitButton.gameObject.SetActive(active);
            customStayButton.gameObject.SetActive(active);
        }
    }
}
