using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace FlipCoin.Game
{
	public class PlayButtonTween : MonoBehaviour
	{
		[Header("Targets")]
		[SerializeField] private RectTransform target;
		[SerializeField] private Graphic colorTarget; // optional

		[Header("Scale Pulse")]
		[SerializeField] private float scaleMultiplier = 1.08f;
		[SerializeField] private float scaleDuration = 0.8f;
		[SerializeField] private Ease scaleEase = Ease.InOutSine;

		[Header("Color (Optional)")]
		[SerializeField] private bool animateColor = false;
		[SerializeField] private Color colorA = Color.white;
		[SerializeField] private Color colorB = new Color(1f, 1f, 1f, 0.85f);
		[SerializeField] private float colorDuration = 1.2f;
		[SerializeField] private Ease colorEase = Ease.InOutSine;

		[Header("Behaviour")]
		[SerializeField] private bool playOnEnable = true;
		[SerializeField] private bool useUnscaledTime = true;

		[Header("Play Action")]
		[SerializeField] private bool loadOnClick = true;
		[SerializeField] private string gameSceneName = "MainLevel";
		[SerializeField] private float clickTransitionDelay = 0.05f; // isim animasyonu vs. için minik gecikme

		private Vector3 initialScale = Vector3.one;
		private Sequence activeSequence;

		private void Reset()
		{
			target = GetComponent<RectTransform>();
			colorTarget = GetComponent<Graphic>();
		}

		private void Awake()
		{
			if (target == null)
			{
				target = GetComponent<RectTransform>();
			}
			if (colorTarget == null)
			{
				colorTarget = GetComponent<Graphic>();
			}
		}

		private void OnEnable()
		{
			if (!playOnEnable) return;
			Play();
		}

		public void Play()
		{
			KillTween();
			if (target == null) return;

			initialScale = target.localScale;
			activeSequence = DOTween.Sequence();
			activeSequence.SetUpdate(useUnscaledTime);

			// Scale pulse
			activeSequence.Append(target.DOScale(initialScale * scaleMultiplier, scaleDuration)
				.SetEase(scaleEase)
				.SetUpdate(useUnscaledTime));
			activeSequence.Append(target.DOScale(initialScale, scaleDuration)
				.SetEase(scaleEase)
				.SetUpdate(useUnscaledTime));

			// Optional color tween (yoyo)
			if (animateColor && colorTarget != null)
			{
				activeSequence.Join(colorTarget.DOColor(colorB, colorDuration)
					.SetEase(colorEase)
					.SetUpdate(useUnscaledTime));
				activeSequence.Append(colorTarget.DOColor(colorA, colorDuration)
					.SetEase(colorEase)
					.SetUpdate(useUnscaledTime));
			}

			activeSequence.SetLoops(-1, LoopType.Restart);
		}

		public void StopAndReset()
		{
			KillTween();
			if (target != null)
			{
				target.localScale = initialScale;
			}
		}

		private void OnDisable()
		{
			StopAndReset();
		}

		private void OnDestroy()
		{
			KillTween();
		}

		public void OnPlayClicked()
		{
			if (!loadOnClick || string.IsNullOrEmpty(gameSceneName)) return;
			// Menüde kalmış olabilecek zaman ölçeğini düzelt
			if (Time.timeScale != 1f) Time.timeScale = 1f;
			// Küçük bir gecikme ile sahne yükle (tıklama sesi/animasyonu için alan)
			DOVirtual.DelayedCall(clickTransitionDelay, () =>
			{
				SceneManager.LoadScene(gameSceneName);
			}).SetUpdate(useUnscaledTime);
		}

		private void KillTween()
		{
			if (activeSequence != null)
			{
				activeSequence.Kill();
				activeSequence = null;
			}
		}
	}
}


