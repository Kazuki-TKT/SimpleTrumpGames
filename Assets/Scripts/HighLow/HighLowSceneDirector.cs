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
    /// ハイ＆ローのゲームを管理するクラス
    /// </summary>
    public class HighLowSceneDirector : MonoBehaviour, IGameController
    {
        //勝敗のタイプ
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

        //ゲームで使うカード
        List<CardController> cards;
        
        //現在のインデックス
        int cardIndex;

        //勝利数
        int winCount, loseCount, drawCount;
        public int WinCount{get => winCount;}
        public int LoseCount{get => loseCount;}
        public int DrawCount{get => drawCount;}

        //ゲームカウント
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

        //待機時間
        const float NextWaitTimer = 2.0f;

        //ポジション
        public Vector3 stackPosition, leftCardPosition, rightCardPosition;

        //
        [SerializeField] HighLowResultGUI resultGUI;

        void Start()
        {
            //カスタムボタンの取得
            if (buttonHighObject != null) customButtonHigh = buttonHighObject.GetComponent<CustomButton>();
            if (buttonLowObject != null) customButtonLow = buttonLowObject.GetComponent<CustomButton>();

            //それぞれのカスタムボタンに処理を登録
            customButtonHigh.onClickCallback = () => OnClickHigh();
            customButtonLow.onClickCallback = () => OnClickLow();
            customButtonStartGame.onClickCallback = () => StartGame();
            foreach(CustomButton customButton in customRestartButton)
            {
                customButton.onClickCallback = () => ReStartGame();
            }

            //ボタンを非表示
            SetHighLowButtons(false);
        }

        //ゲームをスタートさせる
        public void StartGame()
        {
            InitialGame();
            CurrentJudgeType = JudgeType.INITIAL;
            StartCoroutine(StartGameCoroutin());
        }

        //ゲームをスタートさせる準備を行うコルーチン
        IEnumerator StartGameCoroutin()
        {
            //山札作成
            cards = CardsDirector.Instance.GetHighLowCards();
            foreach(var item in cards)//初期位置と向きを設定
            {
                item.transform.position = stackPosition;
                item.FlipCard(false);
            }
            yield return new WaitForSeconds(0.5f);

            //トランプを2枚セットする
            StartCoroutine(DealCards());

            //ボタンをセット
            SetHighLowButtons(true);
        }

        public void StopGame()
        {
           
        }

        //ゲームをリスタートさせる
        public void ReStartGame()
        {
            foreach(CardController card in cards)
            {
                Destroy(card.gameObject);//リストにあるオブジェクトを削除
            }
            cards.Clear();//リストをクリア
            InitialGame();//初期化
            CurrentJudgeType = JudgeType.INITIAL;//判定を変更
            StartCoroutine(StartGameCoroutin());//ゲームスタート
        }

        //ゲームの初期化
        public void InitialGame()
        {
            gameCount = 26;
            GameCountTextChange();
            winCount = loseCount = drawCount = cardIndex =0;
            customButtonStartGame.transform.parent.gameObject.SetActive(false);
        }

        /// <summary>
        /// 勝敗を判定し、次のゲームを行うメソッド
        /// </summary>
        void CheckHighLow(bool high)
        {
            
            //右のトランプをオープン
            cards[cardIndex + 1].GetComponent<CardController>().FlipCardAnimation();

            int leftTmpNumber = cards[cardIndex].GetComponent<CardController>().number;//左のトランプの数字
            int rightTmpNumber = cards[cardIndex + 1].GetComponent<CardController>().number;//右のトランプの数字

            if (leftTmpNumber == rightTmpNumber)//数字が同じだった場合
            {
                CurrentJudgeType = JudgeType.DRAW;
            }
            else if (high)//HIGHを選んだ場合
            {
                if (leftTmpNumber < rightTmpNumber)//伏せたトランプの数字が大きかった場合
                {
                    CurrentJudgeType = JudgeType.WIN;
                }
                else
                {
                    CurrentJudgeType = JudgeType.LOSE;
                }
            }
            else//LOWを選んだ場合
            {
                if (leftTmpNumber > rightTmpNumber)//伏せたトランプの数字が小さかった場合
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
        /// 次のカードをセットするコルーチン
        /// 山札を使いきった時、終了処理を行う
        /// </summary>
        IEnumerator NextCards()
        {
            //--指定秒待機
            yield return new WaitForSeconds(NextWaitTimer);

            CurrentJudgeType = JudgeType.INITIAL;

            //トランプを片付ける
            cards[cardIndex].gameObject.SetActive(false);
            cards[cardIndex + 1].gameObject.SetActive(false);

            //次のカードの1枚目
            cardIndex += 2;

            if (cards.Count - 1 <= cardIndex)//全ての山札を使い切った
            {
                resultGUI.EndGame(winCount,loseCount,drawCount);
            }
            else//次のゲーム
            {
                StartCoroutine(DealCards());
            }
        }

        /// <summary>
        /// トランプを2枚セットするメソッド
        /// </summary>
        IEnumerator DealCards()
        {
            GameCount=-1;

            //左側にオープンした状態のトランプを配置
            cards[cardIndex].transform.DOMove(leftCardPosition, 0.2f);
            cards[cardIndex].GetComponent<CardController>().FlipCard();
            cards[cardIndex].transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

            yield return new WaitForSeconds(0.2f);

            //右側に伏せた状態のトランプを配置
            cards[cardIndex + 1].transform.DOMove(rightCardPosition, 0.2f);
            cards[cardIndex + 1].transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

            //ボタンの表示
            SetHighLowButtons(true);
        }

        // HIGHボタンを押した時に実行するメソッド
        void OnClickHigh()
        {
            SetHighLowButtons(false);//誤操作防止
            CheckHighLow(true);
        }

        // LOWボタンを押した時に実行するメソッド
        void OnClickLow()
        {
            SetHighLowButtons(false);//誤操作防止
            CheckHighLow(false);
        }

        //ハイロ―ボタンの表示変更
        void SetHighLowButtons(bool active)
        {
            buttonHighObject.SetActive(active);
            buttonLowObject.SetActive(active);
        }

        void GameCountTextChange()
        {
            gameCountText.text = $"残り{gameCount}ゲーム";
        }
    }
}
