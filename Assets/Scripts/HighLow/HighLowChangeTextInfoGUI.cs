using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace KazukiTrumpGame.HighLow
{
    /// <summary>
    /// Ÿ”s‚Ì”»’è‚ğ•\¦‚³‚¹‚éƒNƒ‰ƒX
    /// </summary>
    public class HighLowChangeTextInfoGUI : MonoBehaviour
    {
        HighLowSceneDirector sceneDirector;

        [SerializeField]
        TextMeshProUGUI textInfo;

        void Start()
        {
            sceneDirector = GameObject.FindGameObjectWithTag("GameDirector").GetComponent<HighLowSceneDirector>();
            
            //“o˜^
            sceneDirector.OnJudgeTypeChanged += HandleJudgeTypeChanged;
        }

        private void OnDestroy()
        {
            //‰ğœ
            sceneDirector.OnJudgeTypeChanged -= HandleJudgeTypeChanged;
        }

        //“o˜^‚·‚éƒƒ\ƒbƒh
        void HandleJudgeTypeChanged(JudgeType newJudge)
        {
            switch (newJudge)//”»’è‚É‚æ‚èˆ—‚ğ•Ï‚¦‚é
            {
                case JudgeType.WIN://Ÿ‚¿
                    TextAnimation.ResultTextAnimation(textInfo,CardsDirector.Instance.WIN_COLOR, "YOU WIN!!");
                    break;
                case JudgeType.LOSE://•‰‚¯
                    TextAnimation.ResultTextAnimation(textInfo, CardsDirector.Instance.LOSE_COLOR, "YOU LOSE...");
                    break;
                case JudgeType.DRAW://ˆø‚«•ª‚¯
                    TextAnimation.ResultTextAnimation(textInfo, CardsDirector.Instance.DRAW_COLOR, "DRAW");
                    break;
                case JudgeType.INITIAL://‰Šú
                    textInfo.text = "";
                    break;
            }
        }
    }
}
