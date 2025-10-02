using UnityEngine;
#if DOTWEEN_ENABLED || DOTWEEN
using DG.Tweening;
#endif

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

		private Vector3 initialPosition;
		private Quaternion initialRotation;

#if DOTWEEN_ENABLED || DOTWEEN
		private Sequence activeSequence;
#endif

		private bool isFlipping;

		private void Awake()
		{
			initialPosition = transform.position;
			initialRotation = transform.rotation;
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

#if !(DOTWEEN_ENABLED || DOTWEEN)
			Debug.LogWarning("DOTween bulunamadi. Lütfen DOTween'i projeye ekleyin.");
			return;
#else
			isFlipping = true;

			// Her seferinde ayni noktadan baslamak icin konumu/rotasyonu sabitle
			transform.position = initialPosition;
			transform.rotation = initialRotation;

			// Hedef rotasyon (FastBeyond360 ile tam turlar atarak flip hissi)
			Vector3 targetEuler = initialRotation.eulerAngles + (rotationAxis.normalized * 360f * flipRotations);

			// Yukari asagi hareket: Y ekseninde ziplama
			float half = flipDurationSeconds * 0.5f;

			activeSequence?.Kill();
			activeSequence = DOTween.Sequence();

			// Donme ve pozisyon degisimi ayni anda baslasin
			activeSequence.Insert(0f, transform.DORotate(targetEuler, flipDurationSeconds, RotateMode.FastBeyond360).SetEase(Ease.OutCubic));
			activeSequence.Insert(0f, transform.DOMoveY(initialPosition.y + jumpHeight, half).SetEase(Ease.OutQuad));
			// 0.5x sure sonra asagi inis baslasin
			activeSequence.Insert(half, transform.DOMoveY(initialPosition.y, half).SetEase(Ease.InQuad));

			activeSequence.OnComplete(() =>
			{
				// Tam deterministik bitis: baslangic poz/rot'a sabitle
				transform.position = initialPosition;
				transform.rotation = initialRotation;
				isFlipping = false;
			});
#endif
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


