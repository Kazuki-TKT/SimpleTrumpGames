using UnityEngine;
using TMPro;

namespace KazukiTrumpGame.Poker
{
    /// <summary>
    /// �|�[�J�[�ň����{�^�����Ǘ�����N���X
    /// </summary>
    public class PokerButton : MonoBehaviour
    {
        PokerSceneDirector sceneDirector;

        //���̔���
        [SerializeField]
        PokerCheckHand pokerCheck;

        //BET�p�{�^��
        [SerializeField] CustomButton buttonBetCoinPlusOne, buttonBetCoinPlusTen, buttonBetCoinMinusOne, buttonBetCoinMinusTen;

        //�v���C�{�^��
        [SerializeField] CustomButton buttonPlay;

        //�����{�^��
        [SerializeField] CustomButton buttonChange;

        //����1��Q�[������p�̃{�^��
        [SerializeField] CustomButton buttonRePlay;

        // �{�^�����̃e�L�X�g
        TextMeshProUGUI textButtonChange;

        void Start()
        {
            sceneDirector = GameObject.FindGameObjectWithTag("GameDirector").GetComponent<PokerSceneDirector>();
            textButtonChange = buttonChange.gameObject.GetComponentInChildren<TextMeshProUGUI>();

            //�����o�^
            buttonBetCoinPlusOne.onClickCallback += ()=>OnClickBetCoin(1);
            buttonBetCoinPlusTen.onClickCallback += () => OnClickBetCoin(10);
            buttonBetCoinMinusOne.onClickCallback += () => OnClickBetCoin(-1);
            buttonBetCoinMinusTen.onClickCallback += () => OnClickBetCoin(-10);
            buttonPlay.onClickCallback += () => OnClickStateChange(PokerTurnype.Play);
            buttonRePlay.onClickCallback += () => OnClickStateChange(PokerTurnype.Bet);
            buttonChange.onClickCallback += () => OnClickChange();

            sceneDirector.onPLayerCoinChange += HandlePlayerCoinChanged;
            sceneDirector.onBetCoinChange += HandleBetCoinChanged;
        }

        private void FixedUpdate()
        {
            // �{�^���X�V�i�I�𖇐���0���Ȃ�I���{�^���ɕύX�j
            if (sceneDirector.CurrentTurn != PokerTurnype.Play) return;
            textButtonChange.text = "����";
            if (1 > sceneDirector.SelectCards.Count)
            {
                textButtonChange.text = "����";
            }
        }

        //BET�̒l���v���X�ɂ��郁�\�b�h
        private void HandlePlayerCoinChanged(int coin)
        {
            if (1 > sceneDirector.PlayerCoin) return;

            //��U��\��
            buttonBetCoinPlusTen.gameObject.SetActive(false);

            //�����R�C��10�ȏ�
            if (sceneDirector.PlayerCoin > 10)buttonBetCoinPlusTen.gameObject.SetActive(true);
        }

        //BET�̒l���}�C�i�X�ɂ��郁�\�b�h
        private void HandleBetCoinChanged(int coin)
        {
            //��U��\��
            buttonBetCoinMinusTen.gameObject.SetActive(false);
            buttonBetCoinMinusOne.gameObject.SetActive(false);
            buttonPlay.gameObject.SetActive(false);

            //�R�C�����}�C�i�X�ɂ���{�^��
            if (sceneDirector.BetCoin >=10) { buttonBetCoinMinusTen.gameObject.SetActive(true); }//10���ȏォ���Ă��ꍇ
            if (sceneDirector.BetCoin > 0) { buttonBetCoinMinusOne.gameObject.SetActive(true);
                buttonPlay.gameObject.SetActive(true);
            }//1���ȏォ���Ă��ꍇ
        }

        // �R�C�����x�b�g���郁�\�b�h
        void OnClickBetCoin(int betCoin)
        {
            if (1 > sceneDirector.PlayerCoin) return;

            // �R�C�������炵�ăe�L�X�g���X�V
            sceneDirector.PlayerCoin=-betCoin;
            sceneDirector.BetCoin= betCoin;

            sceneDirector.UpdateTexts();
        }

        // �Q�[���v���C�{�^��
        void OnClickStateChange(PokerTurnype pokerTurn)
        {
            sceneDirector.CurrentTurn = pokerTurn;
        }

        // �J�[�h�������郁�\�b�h
        void OnClickChange()
        {
            // �������Ȃ��Ȃ�1��ŏI��
            if (1 > sceneDirector.SelectCards.Count)
            {
                sceneDirector.CardChangeCount = 0;
            }

            // �̂ăJ�[�h����D����폜
            foreach (var item in sceneDirector.SelectCards)
            {
                item.gameObject.SetActive(false);
                sceneDirector.Hand.Remove(item);
                // �̂Ă����J�[�h��ǉ�
                sceneDirector.OpenHand(sceneDirector.AddHand());
            }
            AudioManager.Instance.PlaySound_SE(AudioManager.Instance.trumpOpenSE);
            sceneDirector.SelectCards.Clear();

            // ���ׂ�
            sceneDirector.SortHand();
            

            // �J�[�h�����\��
            sceneDirector.CardChangeCount = -1; ;
            if (1 > sceneDirector.CardChangeCount)
            {
                // ���𐸎Z����
                pokerCheck.CheckHandRank();
            }
        }
    }
}
