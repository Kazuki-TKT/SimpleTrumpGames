using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KazukiTrumpGame
{
    public abstract class CutInEventBase : MonoBehaviour
    {
        //デフォルトのX座標と、ターゲットとなるX座標の値
        [SerializeField]
        protected float defultPositionX, targetPositionX;

        //ターン演出用メソッド
        protected IEnumerator MoveCutInTextObject(GameObject turnObject,float duration)
        {
            //オブジェクト表示
            turnObject.SetActive(true);

            //子オブジェクト取得
            Transform childTransform = turnObject.transform.GetChild(0);
            GameObject childObject = childTransform.gameObject;

            //画面中央に移動
            childObject.transform.DOLocalMoveX(0, 0.5f);

            //指定秒待機
            yield return new WaitForSeconds(duration);

            //画面外へ移動
            childObject.transform.DOLocalMoveX(targetPositionX, 0.5f).OnComplete(() =>
            {
                childObject.transform.DOLocalMoveX(defultPositionX, 0);//元の座標に戻す
                turnObject.SetActive(false);//オブジェクト非表示
            });
        }
    }
}
