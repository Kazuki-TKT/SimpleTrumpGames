using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace KazukiTrumpGame.HighLow
{
    public class HighLowResultGUI : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI resultText,winCountText,loseCountText,drawText;

        [SerializeField]
        GameObject[] setObjects;
        public void EndGame(int win,int lose,int draw)
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

        public void ReStartGame()
        {
            SetObject(setObjects, true);
            StartCoroutine(MovePanel(1000));
        }

        IEnumerator MovePanel(float positionY)
        {
            gameObject.transform.DOLocalMoveY(positionY, 0.5f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(0.5f);
        }

        void SetObject(GameObject[] objects,bool active)
        {
            foreach(GameObject gameObject in objects)
            {
                gameObject.SetActive(active);
            }
        }
    }
}
