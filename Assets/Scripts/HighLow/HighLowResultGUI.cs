using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace KazukiTrumpGame.HighLow
{
    public class HighLowResultGUI : ResultGUIBase
    {
        [SerializeField]
        TextMeshProUGUI winCountText,loseCountText,drawText;

        public void EndGame(int win, int lose, int draw)
        {
            SetObject(setObjects, false);

            if (win == lose)
            {
                resultText.text = "DRAW";
                resultText.color = CardsDirector.Instance.DRAW_COLOR;
            }
            else if (win > lose)
            {
                resultText.text = "YOU WIN!!";
                resultText.color = CardsDirector.Instance.WIN_COLOR;
            }
            else if (win < lose)
            {
                resultText.text = "YOU LOSE...";
                resultText.color = CardsDirector.Instance.LOSE_COLOR;
            }

            winCountText.text = win.ToString();
            loseCountText.text = lose.ToString();
            drawText.text = draw.ToString();

            StartCoroutine(MovePanel(-50));
        }

        protected override void OnRestartGame()
        {
            // HighLowのリスタート処理を追加
        }
    }
}
