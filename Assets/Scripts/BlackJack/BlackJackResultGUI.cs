using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace KazukiTrumpGame.BlackJack
{
    /// <summary>
    /// ブラックジャックの結果を表示させる
    /// </summary>
    public class BlackJackResultGUI : ResultGUIBase
    {
        private BlackJackSceneDirector sceneDirector;
        [SerializeField]
        private TextMeshProUGUI playerCountText, dealerCountText;

        private void Start()
        {
            sceneDirector = GameObject.FindGameObjectWithTag("GameDirector").GetComponent<BlackJackSceneDirector>();
            sceneDirector.onJudgeChanged += HandleJudgeTypeChanged;
        }

        private void OnDestroy()
        {
            sceneDirector.onJudgeChanged -= HandleJudgeTypeChanged;
        }

        private void HandleJudgeTypeChanged(JudgeType newJudge)
        {
            if (newJudge == JudgeType.INITIAL) return;

            switch (newJudge)
            {
                case JudgeType.WIN:
                    resultText.text = "YOU WIN!!";
                    resultText.color = CardsDirector.Instance.WIN_COLOR;
                    break;
                case JudgeType.LOSE:
                    resultText.text = "YOU LOSE...";
                    resultText.color = CardsDirector.Instance.LOSE_COLOR;
                    break;
                case JudgeType.DRAW:
                    resultText.text = "DRAW";
                    resultText.color = CardsDirector.Instance.DRAW_COLOR;
                    break;
            }

            playerCountText.text = sceneDirector.PlayerScore.ToString();
            dealerCountText.text = sceneDirector.DealerScore.ToString();
            SetObject(setObjects, false);
            StartCoroutine(MovePanel(-50));
        }

        protected override void OnRestartGame()
        {
            sceneDirector.ReStartGame();
        }
    }
}