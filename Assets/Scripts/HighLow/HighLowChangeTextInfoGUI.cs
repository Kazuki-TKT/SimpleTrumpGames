using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace KazukiTrumpGame.HighLow
{
    /// <summary>
    /// ���s�̔����\��������N���X
    /// </summary>
    public class HighLowChangeTextInfoGUI : MonoBehaviour
    {
        HighLowSceneDirector sceneDirector;

        [SerializeField]
        TextMeshProUGUI textInfo;

        void Start()
        {
            sceneDirector = GameObject.FindGameObjectWithTag("GameDirector").GetComponent<HighLowSceneDirector>();
            
            //�o�^
            sceneDirector.OnJudgeTypeChanged += HandleJudgeTypeChanged;
        }

        private void OnDestroy()
        {
            //����
            sceneDirector.OnJudgeTypeChanged -= HandleJudgeTypeChanged;
        }

        //�o�^���郁�\�b�h
        void HandleJudgeTypeChanged(JudgeType newJudge)
        {
            switch (newJudge)//����ɂ�菈����ς���
            {
                case JudgeType.WIN://����
                    TextAnimation.ResultTextAnimation(textInfo,CardsDirector.Instance.WIN_COLOR, "YOU WIN!!");
                    break;
                case JudgeType.LOSE://����
                    TextAnimation.ResultTextAnimation(textInfo, CardsDirector.Instance.LOSE_COLOR, "YOU LOSE...");
                    break;
                case JudgeType.DRAW://��������
                    TextAnimation.ResultTextAnimation(textInfo, CardsDirector.Instance.DRAW_COLOR, "DRAW");
                    break;
                case JudgeType.INITIAL://����
                    textInfo.text = "";
                    break;
            }
        }
    }
}
