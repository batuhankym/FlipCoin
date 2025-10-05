using UnityEngine;
using DG.Tweening;

namespace FlipCoin.Game
{
	public class WinUI : MonoBehaviour
	{
		[SerializeField] private CanvasGroup panel;
		[SerializeField] private CoinFlipper coinFlipper;
		[SerializeField] private float fadeDuration = 0.35f;

		private void Awake()
		{
			if (panel == null) panel = GetComponent<CanvasGroup>();
			if (coinFlipper == null) coinFlipper = FindObjectOfType<CoinFlipper>();
			HideImmediate();
		}

		private void OnEnable()
		{
			if (coinFlipper != null)
			{
				coinFlipper.OnWin += HandleWin;
			}
		}

		private void OnDisable()
		{
			if (coinFlipper != null)
			{
				coinFlipper.OnWin -= HandleWin;
			}
		}

		private void HandleWin()
		{
			Show();
		}

		public void Show()
		{
			panel.DOKill();
			panel.gameObject.SetActive(true);
			panel.alpha = 0f;
			panel.interactable = true;
			panel.blocksRaycasts = true;
			panel.DOFade(1f, fadeDuration).SetEase(Ease.OutQuad);
		}

		public void Hide()
		{
			panel.DOKill();
			panel.DOFade(0f, fadeDuration).SetEase(Ease.OutQuad).OnComplete(() =>
			{
				panel.interactable = false;
				panel.blocksRaycasts = false;
				panel.gameObject.SetActive(false);
			});
		}

		public void HideImmediate()
		{
			if (panel == null) return;
			panel.alpha = 0f;
			panel.interactable = false;
			panel.blocksRaycasts = false;
			panel.gameObject.SetActive(false);
		}

		// UI Button
		public void OnClickRetry()
		{
			Hide();
		}
	}
}


