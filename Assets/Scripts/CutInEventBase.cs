using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KazukiTrumpGame
{
    public abstract class CutInEventBase : MonoBehaviour
    {
        //�f�t�H���g��X���W�ƁA�^�[�Q�b�g�ƂȂ�X���W�̒l
        [SerializeField]
        protected float defultPositionX, targetPositionX;

        //�^�[�����o�p���\�b�h
        protected IEnumerator MoveCutInTextObject(GameObject turnObject,float duration)
        {
            //�I�u�W�F�N�g�\��
            turnObject.SetActive(true);

            //�q�I�u�W�F�N�g�擾
            Transform childTransform = turnObject.transform.GetChild(0);
            GameObject childObject = childTransform.gameObject;

            //��ʒ����Ɉړ�
            childObject.transform.DOLocalMoveX(0, 0.5f);

            //�w��b�ҋ@
            yield return new WaitForSeconds(duration);

            //��ʊO�ֈړ�
            childObject.transform.DOLocalMoveX(targetPositionX, 0.5f).OnComplete(() =>
            {
                childObject.transform.DOLocalMoveX(defultPositionX, 0);//���̍��W�ɖ߂�
                turnObject.SetActive(false);//�I�u�W�F�N�g��\��
            });
        }
    }
}
