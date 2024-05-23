using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace KazukiTrumpGame
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CustomButton : MonoBehaviour,
        IPointerClickHandler,
    IPointerDownHandler,
    IPointerUpHandler
    {

        public System.Action onClickCallback;
        public UnityEvent unityEvent;

        [SerializeField] private CanvasGroup canvasGroup;
        // �^�b�v  
        public void OnPointerClick(PointerEventData eventData) {
            onClickCallback?.Invoke();
            unityEvent?.Invoke();
            AudioManager.Instance.PlaySound_SE(AudioManager.Instance.buttonClickSE);
        }

        // �^�b�v�_�E��  
        public void OnPointerDown(PointerEventData eventData) {
            transform.DOScale(0.95f, 0.24f).SetEase(Ease.OutCubic);
            canvasGroup.DOFade(0.8f, 0.24f).SetEase(Ease.OutCubic);
        }

        // �^�b�v�A�b�v  
        public void OnPointerUp(PointerEventData eventData) {
            transform.DOScale(1f, 0.24f).SetEase(Ease.OutCubic);
            canvasGroup.DOFade(1f, 0.24f).SetEase(Ease.OutCubic);
        }
    }
}
