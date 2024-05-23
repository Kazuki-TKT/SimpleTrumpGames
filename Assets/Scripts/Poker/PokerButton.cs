using UnityEngine;
using TMPro;

namespace KazukiTrumpGame.Poker
{
    /// <summary>
    /// ポーカーで扱うボタンを管理するクラス
    /// </summary>
    public class PokerButton : MonoBehaviour
    {
        PokerSceneDirector sceneDirector;

        //役の判定
        [SerializeField]
        PokerCheckHand pokerCheck;

        //BET用ボタン
        [SerializeField] CustomButton buttonBetCoinPlusOne, buttonBetCoinPlusTen, buttonBetCoinMinusOne, buttonBetCoinMinusTen;

        //プレイボタン
        [SerializeField] CustomButton buttonPlay;

        //交換ボタン
        [SerializeField] CustomButton buttonChange;

        //もう1回ゲームする用のボタン
        [SerializeField] CustomButton buttonRePlay;

        // ボタン内のテキスト
        TextMeshProUGUI textButtonChange;

        void Start()
        {
            sceneDirector = GameObject.FindGameObjectWithTag("GameDirector").GetComponent<PokerSceneDirector>();
            textButtonChange = buttonChange.gameObject.GetComponentInChildren<TextMeshProUGUI>();

            //処理登録
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
            // ボタン更新（選択枚数が0枚なら終了ボタンに変更）
            if (sceneDirector.CurrentTurn != PokerTurnype.Play) return;
            textButtonChange.text = "交換";
            if (1 > sceneDirector.SelectCards.Count)
            {
                textButtonChange.text = "勝負";
            }
        }

        //BETの値をプラスにするメソッド
        private void HandlePlayerCoinChanged(int coin)
        {
            if (1 > sceneDirector.PlayerCoin) return;

            //一旦非表示
            buttonBetCoinPlusTen.gameObject.SetActive(false);

            //所持コイン10以上
            if (sceneDirector.PlayerCoin > 10)buttonBetCoinPlusTen.gameObject.SetActive(true);
        }

        //BETの値をマイナスにするメソッド
        private void HandleBetCoinChanged(int coin)
        {
            //一旦非表示
            buttonBetCoinMinusTen.gameObject.SetActive(false);
            buttonBetCoinMinusOne.gameObject.SetActive(false);
            buttonPlay.gameObject.SetActive(false);

            //コインをマイナスにするボタン
            if (sceneDirector.BetCoin >=10) { buttonBetCoinMinusTen.gameObject.SetActive(true); }//10枚以上かけてた場合
            if (sceneDirector.BetCoin > 0) { buttonBetCoinMinusOne.gameObject.SetActive(true);
                buttonPlay.gameObject.SetActive(true);
            }//1枚以上かけてた場合
        }

        // コインをベットするメソッド
        void OnClickBetCoin(int betCoin)
        {
            if (1 > sceneDirector.PlayerCoin) return;

            // コインを減らしてテキストを更新
            sceneDirector.PlayerCoin=-betCoin;
            sceneDirector.BetCoin= betCoin;

            sceneDirector.UpdateTexts();
        }

        // ゲームプレイボタン
        void OnClickStateChange(PokerTurnype pokerTurn)
        {
            sceneDirector.CurrentTurn = pokerTurn;
        }

        // カード交換するメソッド
        void OnClickChange()
        {
            // 交換しないなら1回で終了
            if (1 > sceneDirector.SelectCards.Count)
            {
                sceneDirector.CardChangeCount = 0;
            }

            // 捨てカードを手札から削除
            foreach (var item in sceneDirector.SelectCards)
            {
                item.gameObject.SetActive(false);
                sceneDirector.Hand.Remove(item);
                // 捨てた分カードを追加
                sceneDirector.OpenHand(sceneDirector.AddHand());
            }
            AudioManager.Instance.PlaySound_SE(AudioManager.Instance.trumpOpenSE);
            sceneDirector.SelectCards.Clear();

            // 並べる
            sceneDirector.SortHand();
            

            // カード交換可能回数
            sceneDirector.CardChangeCount = -1; ;
            if (1 > sceneDirector.CardChangeCount)
            {
                // 役を精算する
                pokerCheck.CheckHandRank();
            }
        }
    }
}
