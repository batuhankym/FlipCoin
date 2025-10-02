using UnityEngine;
using DG.Tweening;

namespace FlipCoin.Game
{
	public class CoinFlipper : MonoBehaviour
	{
		[Header("Flip Ayarlari")]
		[SerializeField] private float flipDurationSeconds = 0.8f;
		[SerializeField] private int flipRotations = 2;
		[SerializeField] private Vector3 rotationAxis = new Vector3(1f, 0f, 0f); // X ekseninde flip hissi

		[Header("Ziplama Ayarlari")] 
		[SerializeField] private float jumpHeight = 2f; // y ekseninde havalanma miktari

		[Header("Girdi")] 
		[SerializeField] private bool listenForSpace = true;

		[Header("Ses")] 
		[SerializeField] private AudioClip coinFlipClip;
		[SerializeField] private AudioSource audioSource;

		[Header("Yoneticiler")] 
		[SerializeField] private UpgradeManager upgradeManager;
		[SerializeField] private CurrencyManager currencyManager;

		[Header("Sonuc/RNG")] 
		[Range(0f, 1f)]
		[SerializeField] private float headChance = 0.15f; // Baslangicta %15 heads (UpgradeManager yoksa kullanilir)
		[SerializeField] private bool initialHeadUp = true; // Baslangic rotasyonu baş mi?

		public bool LastResultIsHead { get; private set; }
		public System.Action<bool> OnFlipCompleted; // true=head, false=tail

		private int currentHeadsCombo;
		private SpriteRenderer spriteRenderer;

		private Vector3 initialPosition;
		private Quaternion initialRotation;

		private Sequence activeSequence;

		private bool isFlipping;

		private void Awake()
		{
			initialPosition = transform.position;
			initialRotation = transform.rotation;
			spriteRenderer = GetComponent<SpriteRenderer>();
			if (audioSource == null)
			{
				audioSource = GetComponent<AudioSource>();
				if (audioSource == null)
				{
					audioSource = gameObject.AddComponent<AudioSource>();
				}
				audioSource.playOnAwake = false;
				audioSource.spatialBlend = 0f;
			}
		}

		private void Update()
		{
			if (listenForSpace && !isFlipping && Input.GetKeyDown(KeyCode.Space))
			{
				TriggerFlip();
			}
		}

		/// <summary>
		/// UI Button gibi harici tetiklemeler icin cagirin.
		/// </summary>
		public void TriggerFlip()
		{
			if (isFlipping)
			{
				return;
			}

			isFlipping = true;
			if (coinFlipClip != null && audioSource != null)
			{
				audioSource.PlayOneShot(coinFlipClip);
			}

			// Her seferinde ayni noktadan baslamak icin konumu/rotasyonu sabitle
			transform.position = initialPosition;
			transform.rotation = initialRotation;

			// Degerleri yoneticilerden cek (varsa)
			if (upgradeManager == null)
			{
				upgradeManager = FindObjectOfType<UpgradeManager>();
			}
			if (currencyManager == null)
			{
				currencyManager = FindObjectOfType<CurrencyManager>();
			}

			float chance = upgradeManager != null ? upgradeManager.CurrentHeadsChance : headChance;
			float duration = upgradeManager != null ? upgradeManager.CurrentSecondsFlipTime : flipDurationSeconds;

			// RNG: Sonucu belirle (true=head)
			bool resultIsHead = Random.value < chance;
			LastResultIsHead = resultIsHead;

			// Hedef rotasyon (FastBeyond360 ile tam turlar atarak, secilen yuze gore 0/180° offset)
			Vector3 axisNorm = rotationAxis.sqrMagnitude > 0.0001f ? rotationAxis.normalized : Vector3.right;
			bool needHalfTurnRelativeToInitial = (initialHeadUp != resultIsHead);
			float relativeEndOffset = needHalfTurnRelativeToInitial ? 360f : 0f;
			Vector3 targetEuler = initialRotation.eulerAngles + (axisNorm * (360f * flipRotations + relativeEndOffset));

			// Yukari asagi hareket: Y ekseninde ziplama
			float half = duration * 0.5f;

			activeSequence?.Kill();
			activeSequence = DOTween.Sequence();

			// Donme ve pozisyon degisimi ayni anda baslasin
			activeSequence.Insert(0f, transform.DORotate(targetEuler, duration, RotateMode.FastBeyond360).SetEase(Ease.OutCubic));
			activeSequence.Insert(0f, transform.DOMoveY(initialPosition.y + jumpHeight, half).SetEase(Ease.OutQuad));
			// 0.5x sure sonra asagi inis baslasin
			activeSequence.Insert(half, transform.DOMoveY(initialPosition.y, half).SetEase(Ease.InQuad));

			activeSequence.OnComplete(() =>
			{
				// Tam deterministik bitis: pozisyonu sabitle, yuzu hedefe snap et (0/180°)
				transform.position = initialPosition;
				transform.rotation = Quaternion.Euler(initialRotation.eulerAngles + (axisNorm * relativeEndOffset));
				// Odul/Kombo
				if (resultIsHead)
				{
					if (spriteRenderer != null) spriteRenderer.sortingOrder = 1;
					currentHeadsCombo++;
					double baseWorth = upgradeManager != null ? upgradeManager.CurrentBaseCoinWorth : 1d;
					float comboMult = upgradeManager != null ? upgradeManager.CurrentHeadsComboMultiplier : 0f;
					double reward = baseWorth * (1d + (double)comboMult * currentHeadsCombo);
					currencyManager?.AddCoins(reward);
                    
				}
				else
				{
					if (spriteRenderer != null) spriteRenderer.sortingOrder = 0;
					currentHeadsCombo = 0;
				}
				isFlipping = false;
				OnFlipCompleted?.Invoke(resultIsHead);
			});
		}

		// Inspector uzerinden ekseni hizlica ayarlamak icin yardimci metodlar
		[ContextMenu("Eksen: X (Flip)")]
		private void SetAxisX()
		{
			rotationAxis = Vector3.right;
		}

		[ContextMenu("Eksen: Y")]
		private void SetAxisY()
		{
			rotationAxis = Vector3.up;
		}

		[ContextMenu("Eksen: Z")]
		private void SetAxisZ()
		{
			rotationAxis = Vector3.forward;
		}
	}
}


