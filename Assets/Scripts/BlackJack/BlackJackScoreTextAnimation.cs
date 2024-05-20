using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace KazukiTrumpGame.BlackJack
{
    /// <summary>
    /// ゲーム中のブラックジャックのスコアに応じてテキストアニメーションするクラス
    /// </summary>
    public class BlackJackScoreTextAnimation : MonoBehaviour
    {
        BlackJackSceneDirector sceneDirector;

        //UI
        [SerializeField]
        TextMeshProUGUI playerScoreText, dealerScoreText;

        //デフォルトのカラー、バーストのカラー
        public Color defaultColor, burstColor;

        //1つ前のスコアを保存する変数
        int playerBeforeScore,dealerBeforeScore=0;

        void Start()
        {
            sceneDirector = GameObject.FindGameObjectWithTag("GameDirector").GetComponent<BlackJackSceneDirector>();

            //登録
            sceneDirector.onPleyerScoreChanged += HandlePlayerScoreChanged;
            sceneDirector.onDealerScoreChanged += HandleDealerScoreChanged;
        }

        private void OnDestroy()
        {
            //解除
            sceneDirector.onPleyerScoreChanged -= HandlePlayerScoreChanged;
            sceneDirector.onDealerScoreChanged -= HandleDealerScoreChanged;
        }

        //プレイヤーのスコアに対して登録するメソッド
        void HandlePlayerScoreChanged(int score)
        {
            ChangeScore(playerScoreText, playerBeforeScore, score);
        }

        //ディーラーのスコアに対して登録するメソッド
        void HandleDealerScoreChanged(int score)
        {
            ChangeScore(dealerScoreText, dealerBeforeScore, score);
        }

        //スコアを
        void ChangeScore(TextMeshProUGUI targetText, int beforeScore, int score)
        {
            if (score == 0||beforeScore==score)//スコアが0か、変わらなかった場合
            {
                targetText.text = 0.ToString();//テキストを0
                targetText.color = defaultColor;//カラーをデフォルト
            }
            else
            {
                if (score > 21)//21超えた時
                {
                    targetText.color = burstColor;//カラーをバースト
                }
                TextAnimation.AnimateNumber(beforeScore, score, 0.3f, targetText);//テキストアニメーション
                beforeScore = score;
            }
        }

       
    }
}
