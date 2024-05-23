using System.Collections.Generic;
using UnityEngine;

namespace KazukiTrumpGame.Poker
{
    /// <summary>
    /// ポーカーの役の判定をするクラス
    /// </summary>
    public class PokerCheckHand : MonoBehaviour
    {
        PokerSceneDirector sceneDirector;

        private void Start()
        {
            sceneDirector = GameObject.FindGameObjectWithTag("GameDirector").GetComponent<PokerSceneDirector>();
        }

        // 役を精算する
        public void CheckHandRank()
        {
            // フラッシュチェック
            bool flush = true;
            // 1枚目のカードのマーク
            SuitType suit = sceneDirector.Hand[0].suit;

            foreach (var item in sceneDirector.Hand)
            {
                // 1枚目と違ったら終了
                if (suit != item.suit)
                {
                    flush = false;
                    break;
                }
            }

            // ストレートチェック
            bool straight = false;
            for (int i = 0; i < sceneDirector.Hand.Count; i++)
            {
                // 何枚数字が連続したか
                int straightcount = 0;
                // 現在のカード番号
                int cardno = sceneDirector.Hand[i].number;

                // 1枚目から連続しているか調べる
                for (int j = 0; j < sceneDirector.Hand.Count; j++)
                {
                    // 同じカードはスキップ
                    if (i == j) continue;

                    // 見つけたい数字は現在の数字+1
                    int targetno = cardno + 1;
                    // 13の次は1
                    if (13 < targetno) targetno = 1;

                    // ターゲットの数字発見
                    if (targetno == sceneDirector.Hand[j].number)
                    {
                        // 連続回数をカウント
                        straightcount++;
                        // 今回のカード番号(次回+1される)
                        cardno = sceneDirector.Hand[j].number;
                        // jはまた0から始める
                        j = -1;
                    }
                }

                if (3 < straightcount)
                {
                    straight = true;
                    break;
                }
            }

            // 同じ数字のチェック
            int pair = 0;
            bool threecard = false;
            bool fourcard = false;
            List<CardController> checkcards = new List<CardController>();

            for (int i = 0; i < sceneDirector.Hand.Count; i++)
            {
                if (checkcards.Contains(sceneDirector.Hand[i])) continue;

                // 同じ数字のカード枚数
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

                // ワンペア、ツーペア、スリーカード、フォーカード判定
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

            // フルハウス
            bool fullhouse = false;
            if (1 == pair && threecard)
            {
                fullhouse = true;
            }

            // ストレートフラッシュ
            bool straightflush = false;
            if (flush && straight)
            {
                straightflush = true;
            }

            // 役の判定
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

            // コイン取得
            //playerCoin += addcoin;

            //// テキスト更新
            //updateTexts();
            //textGameInfo.text = infotext + addcoin;

            //// 次回のゲーム用
            //betCoin = 0;
            //setButtonsInPlay(false);
        }
    }
}
