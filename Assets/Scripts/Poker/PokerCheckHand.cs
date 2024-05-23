using System.Collections.Generic;
using UnityEngine;

namespace KazukiTrumpGame.Poker
{
    /// <summary>
    /// �|�[�J�[�̖��̔��������N���X
    /// </summary>
    public class PokerCheckHand : MonoBehaviour
    {
        PokerSceneDirector sceneDirector;

        private void Start()
        {
            sceneDirector = GameObject.FindGameObjectWithTag("GameDirector").GetComponent<PokerSceneDirector>();
        }

        // ���𐸎Z����
        public void CheckHandRank()
        {
            // �t���b�V���`�F�b�N
            bool flush = true;
            // 1���ڂ̃J�[�h�̃}�[�N
            SuitType suit = sceneDirector.Hand[0].suit;

            foreach (var item in sceneDirector.Hand)
            {
                // 1���ڂƈ������I��
                if (suit != item.suit)
                {
                    flush = false;
                    break;
                }
            }

            // �X�g���[�g�`�F�b�N
            bool straight = false;
            for (int i = 0; i < sceneDirector.Hand.Count; i++)
            {
                // �����������A��������
                int straightcount = 0;
                // ���݂̃J�[�h�ԍ�
                int cardno = sceneDirector.Hand[i].number;

                // 1���ڂ���A�����Ă��邩���ׂ�
                for (int j = 0; j < sceneDirector.Hand.Count; j++)
                {
                    // �����J�[�h�̓X�L�b�v
                    if (i == j) continue;

                    // �������������͌��݂̐���+1
                    int targetno = cardno + 1;
                    // 13�̎���1
                    if (13 < targetno) targetno = 1;

                    // �^�[�Q�b�g�̐�������
                    if (targetno == sceneDirector.Hand[j].number)
                    {
                        // �A���񐔂��J�E���g
                        straightcount++;
                        // ����̃J�[�h�ԍ�(����+1�����)
                        cardno = sceneDirector.Hand[j].number;
                        // j�͂܂�0����n�߂�
                        j = -1;
                    }
                }

                if (3 < straightcount)
                {
                    straight = true;
                    break;
                }
            }

            // ���������̃`�F�b�N
            int pair = 0;
            bool threecard = false;
            bool fourcard = false;
            List<CardController> checkcards = new List<CardController>();

            for (int i = 0; i < sceneDirector.Hand.Count; i++)
            {
                if (checkcards.Contains(sceneDirector.Hand[i])) continue;

                // ���������̃J�[�h����
                int samenocount = 0;
                int cardno = sceneDirector.Hand[i].number;

                for (int j = 0; j < sceneDirector.Hand.Count; j++)
                {
                    if (i == j) continue;
                    if (cardno == sceneDirector.Hand[j].number)
                    {
                        samenocount++;
                        checkcards.Add(sceneDirector.Hand[j]);
                    }
                }

                // �����y�A�A�c�[�y�A�A�X���[�J�[�h�A�t�H�[�J�[�h����
                if (1 == samenocount)
                {
                    pair++;
                }
                else if (2 == samenocount)
                {
                    threecard = true;
                }
                else if (3 == samenocount)
                {
                    fourcard = true;
                }
            }

            // �t���n�E�X
            bool fullhouse = false;
            if (1 == pair && threecard)
            {
                fullhouse = true;
            }

            // �X�g���[�g�t���b�V��
            bool straightflush = false;
            if (flush && straight)
            {
                straightflush = true;
            }

            // ���̔���
            sceneDirector.PokerHandType = PokerHandType.None;

            if (straightflush)
            {
                sceneDirector.PokerHandType = PokerHandType.StraightFlush;
            }
            else if (fourcard)
            {
                sceneDirector.PokerHandType = PokerHandType.FourCard;
            }
            else if (fullhouse)
            {
                sceneDirector.PokerHandType = PokerHandType.FullHouse;
            }
            else if (flush)
            {
                sceneDirector.PokerHandType = PokerHandType.Flush;
            }
            else if (straight)
            {
                sceneDirector.PokerHandType = PokerHandType.StraightFlush;
            }
            else if (threecard)
            {
                sceneDirector.PokerHandType = PokerHandType.ThreeCard;
            }
            else if (2 == pair)
            {
                sceneDirector.PokerHandType = PokerHandType.TwoPair;

            }
            else if (1 == pair)
            {
                sceneDirector.PokerHandType = PokerHandType.OnePair;
            }

            sceneDirector.AddCoin();

            // �R�C���擾
            //playerCoin += addcoin;

            //// �e�L�X�g�X�V
            //updateTexts();
            //textGameInfo.text = infotext + addcoin;

            //// ����̃Q�[���p
            //betCoin = 0;
            //setButtonsInPlay(false);
        }
    }
}
