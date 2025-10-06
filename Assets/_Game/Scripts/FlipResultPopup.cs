using UnityEngine;
using TMPro;
using DG.Tweening;

namespace FlipCoin.Game
{
    public class FlipResultPopup : MonoBehaviour
	{
        [SerializeField] private TextMeshProUGUI popupPrefab;
        [SerializeField] private RectTransform popupParent; 
        [SerializeField] private float uiRisePixels = 30f; 
        [SerializeField] private float worldRiseDistance = 0.6f; 
        [SerializeField] private float duration = 0.6f;
        [SerializeField] private Vector2 screenOffset = new Vector2(0f, 40f); 
        [SerializeField] private Vector3 worldOffset = new Vector3(0f, 0.8f, 0f); 
        [SerializeField] private Ease ease = Ease.OutCubic;
        [SerializeField] private Canvas targetCanvas;

        public void Show(bool isHead, Vector3 worldPosition)
		{
            if (popupPrefab == null || targetCanvas == null)
			{
                Debug.LogWarning("FlipResultPopup: UI prefab veya Canvas atanmamış.");
				return;
			}
            float d = duration <= 0.05f ? 0.6f : duration; 
            var cam = Camera.main;

            if (targetCanvas.renderMode == RenderMode.WorldSpace)
            {
                var popup = Instantiate(popupPrefab, popupParent != null ? popupParent : (targetCanvas.transform as RectTransform));
                popup.text = isHead ? "HEADS" : "TAILS";
                popup.color = isHead ? new Color(0.25f, 0.85f, 0.4f) : new Color(0.9f, 0.25f, 0.25f);
                var t = popup.transform;
                t.position = worldPosition + worldOffset;
                var group = popup.gameObject.GetComponent<CanvasGroup>();
                if (group == null) group = popup.gameObject.AddComponent<CanvasGroup>();
                group.alpha = 1f;
                Sequence s = DOTween.Sequence();
                s.Join(t.DOMoveY(t.position.y + worldRiseDistance, d).SetEase(ease));
                s.Join(group.DOFade(0f, d).SetEase(Ease.Linear));
                s.OnComplete(() => { if (popup != null) Destroy(popup.gameObject); });
            }
            else
            {
                var popup = Instantiate(popupPrefab, popupParent != null ? popupParent : (targetCanvas.transform as RectTransform));
                popup.text = isHead ? "HEADS" : "TAILS";
                popup.color = isHead ? new Color(0.25f, 0.85f, 0.4f) : new Color(0.9f, 0.25f, 0.25f);
                var rt = popup.rectTransform;
                Vector2 screen = RectTransformUtility.WorldToScreenPoint(cam, worldPosition) + screenOffset;
                RectTransform canvasRT = targetCanvas.transform as RectTransform;
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, screen, targetCanvas.worldCamera, out localPoint);
                rt.anchoredPosition = localPoint;
                var group = popup.gameObject.GetComponent<CanvasGroup>();
                if (group == null) group = popup.gameObject.AddComponent<CanvasGroup>();
                group.alpha = 1f;
                Sequence s = DOTween.Sequence();
                s.Join(rt.DOAnchorPos(localPoint + new Vector2(0f, uiRisePixels), d).SetEase(ease));
                s.Join(group.DOFade(0f, d).SetEase(Ease.Linear));
                s.OnComplete(() => { if (popup != null) Destroy(popup.gameObject); });
            }
		}
	}
}


