using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace KazukiTrumpGame.BlackJack
{
    /// <summary>
    /// �^�[���̃^�C�v�ɂ���ď���������N���X
    /// </summary>
    public class BlackJuckTurnGUI : CutInEventBase
    {
        BlackJackSceneDirector sceneDirector;

        //GUI
        [SerializeField]
        GameObject playerTurnObject, dealerTurnObject,playerBurstObject,dealerBurstObject,menuButtonObject;

        private void Start()
        {
            sceneDirector = GameObject.FindGameObjectWithTag("GameDirector").GetComponent<BlackJackSceneDirector>();

            //�o�^
            sceneDirector.onTurnChanged += HandleTurnTypeChanged;
            sceneDirector.onBurstEvent += HandleBurstEvent;
        }

        private void OnDestroy()
        {
            //����
            sceneDirector.onTurnChanged -= HandleTurnTypeChanged;
            sceneDirector.onBurstEvent -= HandleBurstEvent;
        }

        void HandleTurnTypeChanged(TurnType newTurn)
        {
            //�^�[���̃^�C�v�ɂ�蕪��
            switch (newTurn)
            {
                case TurnType.PLAYER://�v���C���[
                    StartCoroutine(MoveCutInTextObject(playerTurnObject,1.5f));
                    break;
                case TurnType.DEALER://�f�B�[���[
                    StartCoroutine(MoveCutInTextObject(dealerTurnObject, 1.5f));
                    menuButtonObject.SetActive(false);
                    break;
                case TurnType.INITIAL://����
                    menuButtonObject.SetActive(true);
                    break;
            }
        }

        //�o�[�X�g���̃��\�b�h
        void HandleBurstEvent(int who)
        {
            switch (who)
            {
                case 0://�v���C���[
                    StartCoroutine(MoveBurstTextObject(playerBurstObject,JudgeType.LOSE));
                    break;
                case 1://�f�B�[���[
                    StartCoroutine(MoveBurstTextObject(dealerBurstObject, JudgeType.WIN));
                    break;
            }
        }

        IEnumerator MoveBurstTextObject(GameObject burstObject,JudgeType judgeType)
        {
            //�I�u�W�F�N�g�\��
            burstObject.SetActive(true);

            //�q�I�u�W�F�N�g�擾
            Transform childTransform = burstObject.transform.GetChild(0);
            GameObject childObject = childTransform.gameObject;

            //��ʒ����Ɉړ�
            childObject.transform.DOLocalMoveX(0, 0.5f);

            //�w��b�ҋ@
            yield return new WaitForSeconds(1.5f);

            //��ʊO�ֈړ�
            childObject.transform.DOLocalMoveX(targetPositionX, 0.5f).OnComplete(() =>
            {
                childObject.transform.DOLocalMoveX(defultPositionX, 0);//���̍��W�ɖ߂�
                sceneDirector.CurrentJudgeType = judgeType;
                burstObject.SetActive(false);//�I�u�W�F�N�g��\��
            });
        }
    }
}
