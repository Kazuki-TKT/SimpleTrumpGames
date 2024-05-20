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
    /// ブラックジャックのゲームを管理するクラス
    /// </summary>
    public class BlackJackSceneDirector : MonoBehaviour, IGameController
    {
        //勝敗のタイプ
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

        //順番のタイプ
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

        //バーストのイベント
        public event Action<int> onBurstEvent;

        //ゲームで使用するトランプのリスト
        List<CardController> cards;
        public GameObject fieldObj;

        //UI
        [SerializeField]
        CustomButton customHitButton, customStayButton;

        //それぞれのスコア(トランプの合計値)
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

        //それぞれの手札のリスト
        public List<CardController> playerHand;
        public List<CardController> dealerHand;

        //山札のインデックス
        int cardIndex;

        //待機時間
        const float NextWaitTime = 2;

        //1枚目のトランプの配布位置
        public float playerPositionX, playerPositionZ, dealerPositionZ;

        //ディーラーが最初にカードをオープンしたかどうかの真偽地
        bool dealerFirstOpen = false;

        //キャンセルトークン
        CancellationTokenSource cts;

        private void Start()
        {
            SetBlackJackButtons(false);
            customHitButton.onClickCallback = () => OnClickHit().Forget();
            customStayButton.onClickCallback = () => OnClickStay();
        }

        //ゲームの初期化
        public void InitialGame()
        {
            CurrentJudgeType = JudgeType.INITIAL;
            PlayerScore = DealerScore = 0;
            dealerFirstOpen = false;
            SetBlackJackButtons(false);
        }

        //リスタート
        public void ReStartGame()
        {
            CardsDirector.Instance.DestroyCard();//場のカードを削除
            StartGame();
        }

        //ゲームスタート
        public void StartGame()
        {
            cts = new CancellationTokenSource();
            var token = cts.Token;
            InitialGame();
            StartGameTask(token).Forget();
        }

        //ゲームを止める
        public void StopGame()
        {
            cts.Cancel();
        }

        /// <summary>
        /// ゲームをスタートするUniTaskを用いたメソッド
        /// </summary>
        async UniTask StartGameTask(CancellationToken token)
        {
            try
            {
                //シャッフルしたカードを取得
                cards = CardsDirector.Instance.GetShuffleCards();

                //初期位置
                InitializeCardPositions();

                //手札の初期化
                playerHand = new List<CardController>();
                dealerHand = new List<CardController>();

                cardIndex = 0;

                //それぞれの1枚目追加
                await HitCard(playerHand, true, token);
                await HitCard(dealerHand, false, token);
                //それぞれの2枚目追加
                await HitCard(playerHand, true, token);
                await HitCard(dealerHand, true, token);

                //プレイヤーの現在の手札の値を取得
                PlayerScore = GetScore(playerHand);

                //プレイヤーのターンに切替
                CurrentTurnType = TurnType.PLAYER;

                //指定秒待機
                await UniTask.Delay(TimeSpan.FromSeconds(NextWaitTime), cancellationToken: token);

                //ボタンを表示
                SetBlackJackButtons(true);
            }
            catch
            {
                Debug.LogWarning("StartGameの処理がキャンセルされました");
            }
        }

        /// <summary>
        /// プレイヤーかディーラーが山札から1枚カードを引くメソッド
        /// </summary>
        async UniTask<bool> HitCard(List<CardController> hand, bool open, CancellationToken token)
        {
            try
            {
                //プレイヤーの初期位置
                float x = playerPositionX;
                float z = playerPositionZ;

                //ディーラーの初期位置
                if (dealerHand == hand)
                {
                    z = dealerPositionZ;
                }

                //カードがあるなら右に並べる
                if (0 < hand.Count)
                {
                    x = hand[hand.Count - 1].transform.position.x;
                    z = hand[hand.Count - 1].transform.position.z;
                }

                //山札のインデックスからカードを取得
                CardController card = cards[cardIndex];
                card.transform.localScale = new Vector3(0.015f, 0.015f, 0.015f);

                //カードを指定位置に配るまで待機
                await card.transform.DOLocalMove(new Vector3(x + 0.1f, 0, z), 0.2f).AsyncWaitForCompletion();

                //カードをオープン
                if (open)
                {
                    card.FlipCardAnimation(true);
                }

                //リストに追加
                hand.Add(card);
                cardIndex++;

                return true;
            }
            catch
            {
                Debug.LogWarning("HitCardの処理がキャンセルされました");
                return true;
            }

        }

        /// <summary>
        /// 手札を計算するメソッド
        /// </summary>
        int GetScore(List<CardController> hand)
        {
            int score = 0;
            List<CardController> ace = new List<CardController>();//Aは1か11かで判断したいので確保する

            foreach (var item in hand)
            {
                int no = item.number;
                if (1 == no)
                {
                    ace.Add(item);
                }
                else if (10 < no)//JQKの計算（絵札は全て10）
                {
                    no = 10;
                }

                score += no;
            }

            //Aを11にする
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
        /// プレイヤーヒットボタン
        /// </summary>
        async UniTaskVoid OnClickHit()
        {
            if (currentTurnType != TurnType.PLAYER) return;//プレイヤーターンじゃない場合は以降の処理を行わない

            SetBlackJackButtons(false);//ボタン非表示

            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    await HitCard(playerHand, true, cts.Token);//プレイヤーのカードが配り終えるまで待機
                    PlayerScore = GetScore(playerHand);//プレイヤーの手札の値を取得
                    if (21 < PlayerScore)//21を超えたらバースト
                    {
                        dealerHand[0].FlipCardAnimation();//ディーラーの1枚目の手札公開
                        DealerScore = GetScore(dealerHand);//ディーラーの手札の合計値を取得
                        onBurstEvent?.Invoke(0);//プレイヤーのバーストイベント実行
                    }
                    else//21超えていないならボタンを再表示
                    {
                        SetBlackJackButtons(true);
                    }
                }
                catch
                {
                    Debug.LogWarning("OnClickHitの処理がキャンセルされました");
                }
            }
        }

        /// <summary>
        /// プレイヤーステイボタン
        /// </summary>
        void OnClickStay()
        {
            if (currentTurnType != TurnType.PLAYER) return;//プレイヤーターンじゃない場合は以降の処理を行わない
            SetBlackJackButtons(false);//ボタンを押せないように非表示
            DealerHit().Forget();//ディーラーの行動に移る
            CurrentTurnType = TurnType.DEALER;//ターンをディーラーに
        }

        /// <summary>
        /// ディーラーの行動
        /// </summary>
        async UniTaskVoid DealerHit()
        {
            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    //指定秒待機
                    await UniTask.Delay(TimeSpan.FromSeconds(NextWaitTime), cancellationToken: cts.Token);

                    DealerScore = GetScore(dealerHand);//ディーラーの手札の値を取得
                                                       //プレイヤーがステイ後だけに行われる処理
                    if (!dealerFirstOpen)
                    {
                        dealerHand[0].FlipCardAnimation();//伏せられている1枚目をオープン
                        await UniTask.Delay(TimeSpan.FromSeconds(0.6), cancellationToken: cts.Token);//指定秒待機
                        dealerFirstOpen = true;
                    }

                    //18未満の場合
                    if (18 > DealerScore)
                    {
                        await HitCard(dealerHand, true, cts.Token);//ディーラーが引くまで待機
                        DealerScore = GetScore(dealerHand);//ディーラーの手札の値を取得
                    }

                    //手札の値が21より上の場合
                    if (21 < DealerScore)
                    {
                        onBurstEvent?.Invoke(1);//ディーラーのバーストイベント実行
                    }
                    else if (17 < DealerScore)//手札の値が17より上の場合
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(0.5));//指定秒待機
                        DetermineWinner();//勝敗を決める
                    }
                    else//もう一度処理を繰り返す
                    {
                        DealerHit().Forget();//もう一度行う
                    }
                }
                catch
                {
                    Debug.LogWarning("DealerHitの処理がキャンセルされました");
                }

            }
                
        }

        //勝敗を決めるメソッド
        void DetermineWinner()
        {
            if (PlayerScore < DealerScore) // ディーラー勝利パターン
            {
                CurrentJudgeType = JudgeType.LOSE;
            }
            else if (PlayerScore == DealerScore) // 引き分けパターン
            {
                CurrentJudgeType = JudgeType.DRAW;
            }
            else // プレイヤー勝利パターン
            {
                CurrentJudgeType = JudgeType.WIN;
            }
        }

        //初期位置を決めるメソッド
        void InitializeCardPositions()
        {
            foreach (var item in cards)
            {
                item.transform.position = new Vector3(0.34f, 0, 0.15f);
                item.FlipCard(false);
            }
        }

        //ボタンの表示形式
        void SetBlackJackButtons(bool active)
        {
            customHitButton.gameObject.SetActive(active);
            customStayButton.gameObject.SetActive(active);
        }
    }
}
