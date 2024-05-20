using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using DG.Tweening;

namespace KazukiTrumpGame.Memory
{
    public class MemorySceneDirector : MonoBehaviour, IGameController
    {
        TurnType currentTurn;
        public event Action<TurnType> onCurrentTurnChanged;
        public TurnType CurrentTurn
        {
            get => currentTurn;
            set
            {
                currentTurn = value;
                onCurrentTurnChanged?.Invoke(currentTurn);
            }
        }

        public GameObject ButtonObjects;

        [SerializeField] MemoryCutInGUI memoryCutIn;

        // ゲームで使うカード
        public
        List<CardController> cards;

        // 縦横何枚並べるか
        int width = 5;
        int height = 4;

        // 取得したカードを保存するリスト
        public List<CardController> getCards;

        //選択したカード
        CardController firstCard, secondCard;

        // ゲーム終了フラグ
        bool isGameEnd;

        //山札のポジション
        public Vector3 stackPosition, targetPosition;

        //キャンセルトークン
        CancellationTokenSource cts;

        void Start()
        {
            //StartGame();
        }

        void Update()
        {
            // プレイヤーターンじゃないなら処理をしない
            if (CurrentTurn != TurnType.PLAYER) return;

            // マウスが離された時
            if (Input.GetMouseButtonUp(0))
            {
                // 2枚目がヌルじゃない場合はリターン
                if (secondCard) return;

                // Rayを飛ばして当たり判定をとる
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    // ヒットしたゲームオブジェクトからCardControllerを取得
                    CardController card = hit.collider.gameObject.GetComponent<CardController>();

                    // 1枚目のカードじゃないかもうめくったカードなら終了
                    if (!card || firstCard == card) return;

                    // カードオープン
                    card.FlipCardAnimation();

                    // 1枚目のカードを保存
                    if (firstCard == null)
                    {
                        firstCard = card;
                        return;
                    }

                    //2枚目のカードを保存した後、合っているかチェック
                    if (secondCard == null)
                    {
                        secondCard = card;
                        CheckSelectedCards(cts.Token).Forget();
                    }
                }
            }
        }

        public void StartGame()
        {
            // 各種フラグ初期化
            InitialGame();
            StartCoroutine(SetCardCorutin());
        }

        /// <summary>
        /// カードを生成して並べるコルーチン
        /// </summary>
        IEnumerator SetCardCorutin()
        {
            // シャッフルされたカードを取得
            cards = CardsDirector.Instance.GetMemoryCards();

            foreach (var item in cards)//初期位置と向きを設定
            {
                item.transform.position = stackPosition;
                item.FlipCard(false);
            }

            yield return new WaitForSeconds(0.5f);

            // カード全体を真ん中にずらすためのオフセット
            Vector2 offset = new Vector2((width - 1) / 2.0f, (height - 1) / 2.0f);

            // カード枚数が足りないとき、エラーを表示する
            if (cards.Count < width * height)
            {
                Debug.LogError("カードが足りません");
            }

            // カードを並べる
            for (int i = 0; i < width * height; i++)
            {
                // 表示位置
                float x = (i % width - offset.x) * CardController.Width;
                float y = (i / width - offset.y) * CardController.Height;

                cards[i].transform.DOMove(new Vector3(x, 0, y), 0.2f);
                yield return new WaitForSeconds(0.1f);
            }

            memoryCutIn.CutInObject(TurnType.PLAYER);
            yield return new WaitForSeconds(1.5f);

            //PLAYERターンにする
            CurrentTurn = TurnType.PLAYER;
            ButtonObjects.SetActive(true);
        }

        private async UniTaskVoid CheckSelectedCards(CancellationToken token)
        {
            try
            {
                await UniTask.Delay(1, cancellationToken: token);  // アニメーションのための短い待機

                if (firstCard.number == secondCard.number)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: token);  // カードを見せるための待機

                    //取得カードリストに追加
                    var selectCards = new List<CardController> { firstCard, secondCard };
                    getCards.AddRange(selectCards);

                    //取得したカードの移動演出
                    await firstCard.gameObject.transform.DOMove(targetPosition, 0.2f);
                    firstCard.FlipCard(false);
                    await secondCard.gameObject.transform.DOMove(targetPosition, 0.2f);
                    secondCard.FlipCard(false);

                    //カードリストから削除
                    cards.Remove(firstCard);
                    cards.Remove(secondCard);

                    //ゲーム終了チェック
                    //isGameEnd = CheckGameEnd();
                    if (CheckGameEnd())
                    {
                        // タイマーを停止してゲームクリア時間を表示
                        CurrentTurn = TurnType.RESULT;
                        Debug.Log("ゲーム終了");
                        StopGame();
                        memoryCutIn.CutInObject(TurnType.RESULT);
                        await UniTask.Delay(TimeSpan.FromSeconds(1.5), cancellationToken: token);
                    }
                }
                else
                {
                    await UniTask.Delay(1000, cancellationToken: token);  // カードを見せるための待機

                    //カードを伏せる
                    firstCard.FlipCardAnimation(false);
                    secondCard.FlipCardAnimation(false);
                }
            }
            catch
            {
                Debug.LogWarning("CheckSelectedCardsの処理をキャンセル致しました");
            }

            //ヌルにする
            firstCard = null;
            secondCard = null;
        }


        private bool CheckGameEnd()
        {
            if (cards.Count != 0)
            {
                return false;
            }
            return true;
        }

        public void StopGame()
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }
        }

        public void ReStartGame()
        {
            CardsDirector.Instance.DestroyCard();
            StartGame();
        }

        public void InitialGame()
        {
            CurrentTurn = TurnType.INITIAL;
            cts = new CancellationTokenSource();
            getCards = new List<CardController>();
            ButtonObjects.SetActive(false);
            firstCard = secondCard = null;
        }
    }
}

