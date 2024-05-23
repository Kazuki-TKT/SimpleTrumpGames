using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System;
using TMPro;

namespace KazukiTrumpGame.Poker
{
    public enum PokerTurnype
    {
        Bet,
        Play,
        Result,
        Initial
    }

    //役
    public enum PokerHandType
    {
        StraightFlush,//ストレートフラッシュ
        FourCard,//フォーカード
        FullHouse,//フルハウス
        Flush,//フラッシュ
        Straight,//ストレート
        ThreeCard,//スリーカード
        TwoPair,//ツーペア
        OnePair,//ワンペア
        None//役無
    }

    public class PokerSceneDirector : MonoBehaviour, IGameController
    {
        // 手札の役
        PokerHandType currentHandType;
        public PokerHandType PokerHandType
        {
            get => currentHandType;
            set
            {
                currentHandType = value;
                onPokerHandChanged?.Invoke(currentHandType);
            }
        }

        // 手札の役が変わったら呼び出されるイベント
        public event Action<PokerHandType> onPokerHandChanged;

        // ゲームの進行状況
        PokerTurnype currentTurn;
        public PokerTurnype CurrentTurn
        {
            get => currentTurn; set
            {
                currentTurn = value;
                onPokerTurnChanged?.Invoke(currentTurn);
            }
        }

        // ゲームの進行状況が変わったら呼び出されるイベント
        public event Action<PokerTurnype> onPokerTurnChanged;

        [SerializeField]
        GameObject betObject, //ベットフェーズのオブジェクト
            playObject,// プレイフェーズのオブジェクト
            resultObject;// 結果発表フェーズのオブジェクト

        // 結果発表GUI
        [SerializeField]
        PokerResultGUI resultGUI;

        //UI
        [SerializeField]
        TextMeshProUGUI textGameInfo, // ゲーム情報テキスト
            textRate, // 役の払い戻し率テキスト
            playerCoinText, // プレイヤーの持ちコインテキスト
            handTypeText;// 役名テキスト

        // 全カード
        List<CardController> cards;

        // 手札
        List<CardController> hand;
        public List<CardController> Hand { get => hand; }

        // 交換するカード
        List<CardController> selectCards;
        public List<CardController> SelectCards { get => selectCards; }

        // 山札のインデックス番号
        int dealCardCount;

        // プレイヤーの持ちコイン
        int playerCoin;
        public int PlayerCoin
        {
            get => playerCoin; set
            {
                int beforePlyerCoin = playerCoin;
                playerCoin += value;

                if (playerCoin < 0) playerCoin = 0;
                
                TextAnimation.AnimateNumber(beforePlyerCoin, playerCoin, 0.15f, playerCoinText);
                onPLayerCoinChange?.Invoke(playerCoin);
            }
        }

        // プレイヤーの持ちコインが変わったら呼び出されるイベント
        public event Action<int> onPLayerCoinChange;

        // 交換できる回数
        [SerializeField] int cardChangeCountMax;

        // ベットしたコイン
        int betCoin;
        public int BetCoin
        {
            get => betCoin; set
            {
                betCoin += value;
                if (betCoin < 0) betCoin = 0;
                onBetCoinChange?.Invoke(betCoin);
            }
        }

        // ベットしたコインが変わったら呼び出されるイベント
        public event Action<int> onBetCoinChange;

        // 交換した回数
        int cardChangeCount;
        public int CardChangeCount { get => cardChangeCount; set => cardChangeCount += value; }

        // アニメーション時間
        const float SortHandTime = 0.5f;

        // 倍率設定
        int straightFlushRate = 10;
        int fourCardRate = 8;
        int fullHouseRate = 6;
        int flushRate = 5;
        int straightRate = 4;
        int threeCardRate = 3;
        int twoPairRate = 2;
        int onePairRate = 1;

        void Start()
        {
            onPokerTurnChanged += HandlePokerTurnChanged;
        }
        private void OnDestroy()
        {
            onPokerTurnChanged -= HandlePokerTurnChanged;
        }

        void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                // Rayを作成して投射
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // ヒットしたオブジェクトからCardControllerを取得
                    CardController card = hit.collider.gameObject.GetComponent<CardController>();
                    if (CardChangeCount == 0) return;
                    // カード選択処理
                    SetSelectCard(card);
                }
            }
        }

        public void StartGame()
        {
            playerCoin = 50;
            betCoin = 0;
            playerCoinText.text = playerCoin.ToString();
            PlayerCoin = 0;
            CardsDirector.Instance.DestroyCard();
            UpdateTexts();
            InitialGame();

        }

        public void StopGame()
        {
        }

        public void ReStartGame()
        {
            CardsDirector.Instance.DestroyCard();
            betCoin = 0;
            UpdateTexts();
            InitialGame();
        }

        public void InitialGame()
        {
            // カード取得
            cards = CardsDirector.Instance.GetShuffleCards();

            // 配列データ初期化
            hand = new List<CardController>();
            selectCards = new List<CardController>();

            CurrentTurn = PokerTurnype.Bet;
        }

        //ターンの変化によって処理
        void HandlePokerTurnChanged(PokerTurnype newTurn)
        {
            betObject.SetActive(false);
            playObject.SetActive(false);
            resultObject.SetActive(false);

            switch (newTurn)
            {
                case PokerTurnype.Bet:
                    betObject.SetActive(true);
                    break;
                case PokerTurnype.Play:
                    playObject.SetActive(true);
                    SetPlayerHandCard();
                    break;
                case PokerTurnype.Result:
                    resultObject.SetActive(true);
                    break;
                case PokerTurnype.Initial:
                    break;
            }
        }

        // 手札を加える
        public CardController AddHand()
        {
            // 山札からカードを取得してインデックスを進める
            CardController card = cards[dealCardCount++];
            // 手札に加える
            hand.Add(card);
            // 引いたカードを返す
            return card;
        }

        // 手札をめくる
        public void OpenHand(CardController card)
        {
            // 回転アニメーション
            card.transform.DORotate(Vector3.zero, SortHandTime)
                .OnComplete(() => { card.FlipCard(); });
        }

        // 手札を並べる
        public void SortHand()
        {
            // 初期位置
            float x = -CardController.Width * 2;
            // 手札を枚数分並べる
            foreach (var item in hand)
            {
                // 表示位置へアニメーションして移動
                Vector3 pos = new Vector3(x, 0, 0);
                item.transform.DOMove(pos, SortHandTime);
                // 次回の表示位置x
                x += CardController.Width;
            }
        }

        // プレイヤーの手札をセット
        public void SetPlayerHandCard(bool deal = true)
        {
            // 手札、選択カードをリセット
            hand.Clear();
            selectCards.Clear();
            // カードを引ける回数をリセット
            cardChangeCount = cardChangeCountMax;
            // 山札から引いた枚数をリセット
            dealCardCount = 0;

            // カードシャッフル
           　CardsDirector.Instance.ShuffleCards(cards);

            // カード初期設定
            foreach (var item in cards)
            {
                // 捨て札は非表示状態なので表示する
                item.gameObject.SetActive(true);
                // 裏向きにする
                item.FlipCard(false);
                // 山札の場所へ
                item.transform.position = new Vector3(0, 0, 0.18f);
            }

            // ここから下は配る処理
            if (!deal) return;

            // 5枚配って表向きにする
            for (int i = 0; i < 5; i++)
            {
                OpenHand(AddHand());
            }
            AudioManager.Instance.PlaySound_SE(AudioManager.Instance.trumpOpenSE);

            // カードを並べる
            SortHand();
        }

        // カード選択状態
        public void SetSelectCard(CardController card)
        {
            // 選択できないカードなら終了
            if (!card || !card.isFrontUp) return;

            // カードの現在地
            Vector3 pos = card.transform.position;

            // 2回目選択されたら非選択
            if (selectCards.Contains(card))
            {
                pos.z -= 0.02f;
                selectCards.Remove(card);
            }
            // 選択状態（カード上限を超えないように）
            else if (cards.Count > dealCardCount + selectCards.Count)
            {
                pos.z += 0.02f;
                selectCards.Add(card);
            }

            // 更新された場所
            card.transform.position = pos;
        }

        //BET数を更新するメソッド
        public void UpdateTexts()
        {
            textGameInfo.text = "BET枚数 " + BetCoin;

            textRate.text = "ストレートフラッシュ " + (straightFlushRate * BetCoin) + "\n"
                + "フォーカード " + (fourCardRate * BetCoin) + "\n"
                + "フルハウス " + (fullHouseRate * BetCoin) + "\n"
                + "フラッシュ " + (flushRate * BetCoin) + "\n"
                + "ストレート " + (straightRate * BetCoin) + "\n"
                + "スリーカード " + (threeCardRate * BetCoin) + "\n"
                + "ツーペア " + (twoPairRate * BetCoin) + "\n"
                + "ワンペア " + (onePairRate * BetCoin) + "\n";
        }
        
       

        public void AddCoin()
        {
            int addCoin = 0;
            switch (currentHandType)
            {
                case PokerHandType.StraightFlush:
                    addCoin = straightFlushRate * betCoin;
                    handTypeText.text = "STRAIGHT FLUSH";
                    break;
                case PokerHandType.FourCard:
                    addCoin = fourCardRate * betCoin;
                    handTypeText.text = "FOUR CARD";
                    break;
                case PokerHandType.FullHouse:
                    addCoin = fullHouseRate * betCoin;
                    handTypeText.text = "FULL HOUSE";
                    break;
                case PokerHandType.Flush:
                    addCoin = flushRate * betCoin;
                    handTypeText.text = "FLUSH";
                    break;
                case PokerHandType.Straight:
                    addCoin = straightRate * betCoin;
                    handTypeText.text = "STRAIGHT";
                    break;
                case PokerHandType.ThreeCard:
                    addCoin = threeCardRate * betCoin;
                    handTypeText.text = "TREE CARD";
                    break;
                case PokerHandType.TwoPair:
                    addCoin = twoPairRate * betCoin;
                    handTypeText.text = "TWO PAIR";
                    break;
                case PokerHandType.OnePair:
                    addCoin = onePairRate * betCoin;
                    handTypeText.text = "ONE PAIR";
                    break;
                case PokerHandType.None:
                    addCoin = 0;
                    handTypeText.text = "NO HAND";
                    break;
            }

            CurrentTurn = PokerTurnype.Result;
            StartCoroutine(MoveHandText(addCoin));
        }

        IEnumerator MoveHandText(int addCoin)
        {
            yield return new WaitForSeconds(0.5f);
            resultGUI.MoveHandTypeText();
            yield return new WaitForSeconds(1f);
            PlayerCoin = addCoin;
        }
    }
}