using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KazukiTrumpGame
{
    public enum JudgeType
    {
        WIN,//勝利
        LOSE,//負け
        DRAW,//引き分け
        INITIAL//初期
    }

    public enum TurnType
    {
        PLAYER,//プレイヤー
        DEALER,//ディーラー
        RESULT,//結果
        INITIAL//初期
    }
    public class CardsDirector : MonoBehaviour
    {
        public static CardsDirector Instance;

        [SerializeField] List<GameObject> prefabSpades;
        [SerializeField] List<GameObject> prefabClubs;
        [SerializeField] List<GameObject> prefabDiamonds;
        [SerializeField] List<GameObject> prefabHearts;
        [SerializeField] List<GameObject> prefabJokers;

        public Color WIN_COLOR;//赤
        public Color LOSE_COLOR;//青
        public Color DRAW_COLOR;//黒

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // シャッフル
        public void ShuffleCards(List<CardController> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                int rnd = Random.Range(0, cards.Count);// 0からcards.Count未満のランダムな整数を生成
                CardController tmp = cards[i];// 現在のトランプを一時的に保存

                cards[i] = cards[rnd];// ランダムに選ばれたトランプと現在のトランプを入れ替え
                cards[rnd] = tmp;// 一時的に保存しておいたトランプをランダムな位置に配置
            }
        }

        // トランプ作成
        List<CardController> createCards(SuitType suittype,int count=-1)
        {
            List<CardController> ret = new List<CardController>();

            // トランプの種類(デフォルト)
            List<GameObject> prefabcards = prefabSpades;
            Color suitcolor = Color.black;

            switch (suittype)
            {
                case SuitType.Club:
                    prefabcards = prefabClubs;
                    break;
                case SuitType.Diamond:
                    prefabcards = prefabDiamonds;
                    suitcolor = Color.red;
                    break;
                case SuitType.Heart:
                    prefabcards = prefabHearts;
                    suitcolor = Color.red;
                    break;
                case SuitType.Joker:
                    prefabcards = prefabJokers;
                    break;
            }

            if (0 > count)
            {
                count = prefabcards.Count;
            }

            // トランプ生成
            for (int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(prefabcards[i]);


                BoxCollider boxCollider = obj.AddComponent<BoxCollider>();// 当たり判定追加

                Rigidbody rigidBody = obj.AddComponent<Rigidbody>();// 当たり判定検知用
                boxCollider.isTrigger = true;//当たり判定を変更
                rigidBody.isKinematic = true;//物理判定を行わない

                // トランプにデータをセット
                CardController cardController = obj.AddComponent<CardController>();

                cardController.suit = suittype;
                cardController.suitColor = suitcolor;
                cardController.playerNumber = -1;
                cardController.number = i + 1;

                ret.Add(cardController);
            }

            return ret;
        }

        /// <summary>
        /// ハイ&ローで使用するカードを作成するメソッド
        /// </summary>
        /// <returns></returns>
        public List<CardController> GetHighLowCards()
        {
            List<CardController> ret = new List<CardController>();
            ret.AddRange(createCards(SuitType.Spade));
            ret.AddRange(createCards(SuitType.Club));
            ret.AddRange(createCards(SuitType.Diamond));
            ret.AddRange(createCards(SuitType.Heart));

            ShuffleCards(ret);

            return ret;
        }
        /// <summary>
        /// トランプをシャッフルするメソッド
        /// </summary>
        /// <returns></returns>
        public List<CardController> GetShuffleCards()
        {
            List<CardController> ret = new List<CardController>();
            ret.AddRange(createCards(SuitType.Spade));
            ret.AddRange(createCards(SuitType.Club));
            ret.AddRange(createCards(SuitType.Diamond));
            ret.AddRange(createCards(SuitType.Heart));

            ShuffleCards(ret);

            return ret;
        }

        /// <summary>
        /// 神経衰弱で使うカードを作成
        /// </summary>
        /// <returns></returns>
        public List<CardController> GetMemoryCards() {
            List<CardController> ret = new List<CardController>();
            ret.AddRange(createCards(SuitType.Spade,10));
            ret.AddRange(createCards(SuitType.Diamond,10));
            ShuffleCards(ret);
            return ret;
        }

        public void DestroyCard()
        {
            var cards = GameObject.FindGameObjectsWithTag("Card");

            foreach(GameObject cardObject in cards)
            {
                Destroy(cardObject);
            }
        }
    }
}