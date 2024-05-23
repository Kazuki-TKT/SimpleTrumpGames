using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace KazukiTrumpGame
{
    // マーク
    public enum SuitType
    {
        Spade,
        Club,
        Diamond,
        Heart,
        Joker,
    }

    public class CardController : MonoBehaviour
    {
        // カードサイズ
        public const float Width = 0.06f;
        public const float Height = 0.09f;

        // このカードのマーク
        public SuitType suit;

        // 番号
        public int number;

        // どのプレイヤーのカードか
        public int playerNumber;

        // 神経衰弱やカップルで並べたときの内部番号
        public int Index;

        // 手札初期値(3D)
        public Vector3 handPosition;

        // 手札初期値(x,y)
        public Vector2Int indexPosition;

        // カラー
        public Color suitColor;

        // 表面が上になっているか
        public bool isFrontUp;

        // カードをめくる(引数無しで表側にする)
        public void FlipCard(bool frontup = true)
        {
            float anglez = 0;
            if (!frontup)
            {
                anglez = 180;
            }

            isFrontUp = frontup;
            
            transform.eulerAngles = new Vector3(0, 0, anglez);
        }
        public void FlipCardAnimation(bool frontup = true)
        {
            float anglez = 0;
            if (!frontup)
            {
                anglez = 180;
            }
            AudioManager.Instance.PlaySound_SE(AudioManager.Instance.trumpOpenSE);
            isFrontUp = frontup;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DORotate(new Vector3(0, 0, anglez), 0.2f))
                .Join(transform.DOMoveY(0.05f,0.2f))
                .OnComplete(()=> {
                    transform.DOMoveY(0f, 0.2f);
                    });
        }
        
    }
}