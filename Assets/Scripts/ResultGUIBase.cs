using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening;

namespace KazukiTrumpGame
{
    public abstract class ResultGUIBase : MonoBehaviour
    {
        [SerializeField]
        protected TextMeshProUGUI resultText;
        [SerializeField]
        protected GameObject[] setObjects;

        protected void SetObject(GameObject[] objects, bool active)
        {
            foreach (GameObject gameObject in objects)
            {
                gameObject.SetActive(active);
            }
        }

        protected IEnumerator MovePanel(float positionY)
        {
            gameObject.transform.DOLocalMoveY(positionY, 0.5f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(0.5f);
        }

        public void ReStartGame()
        {
            SetObject(setObjects, true);
            StartCoroutine(MovePanel(1000));
            OnRestartGame();
        }

        protected abstract void OnRestartGame();
    }
}

