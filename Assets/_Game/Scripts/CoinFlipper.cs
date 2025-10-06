using UnityEngine;
using DG.Tweening;

namespace FlipCoin.Game
{
	public class CoinFlipper : MonoBehaviour
	{
		[Header("Flip Settings")]
		[SerializeField] private float flipDurationSeconds = 0.8f;
		[SerializeField] private int flipRotations = 2;
		[SerializeField] private Vector3 rotationAxis = new Vector3(1f, 0f, 0f); 

		[Header("Jump Settings")] 
		[SerializeField] private float jumpHeight = 2f; 

		[Header("Input")] 
		[SerializeField] private bool listenForSpace = true;

		[Header("Gameplay Locks")]
		[SerializeField] private bool flipEnabled = true;
		public bool FlipEnabled
		{
			get { return flipEnabled; }
			set
			{
				flipEnabled = value;
				Debug.Log($"[CoinFlipper] flipEnabled set to {flipEnabled}");
			}
		}

		[Header("Audio")] 
		[SerializeField] private AudioClip coinFlipClip;
		[SerializeField] private AudioSource audioSource;

		[Header("Managers")] 
		[SerializeField] private UpgradeManager upgradeManager;
		[SerializeField] private CurrencyManager currencyManager;
		[SerializeField] private FlipHistoryUI flipHistoryUI;
		[SerializeField] private FlipResultPopup resultPopup;

		[Header("Win")]
		[SerializeField] private int headsInRowToWin = 10;
		public System.Action OnWin;
		private bool hasWon;

		[Header("Result/RNG")] 
		[Range(0f, 1f)]
		[SerializeField] private float headChance = 0.15f; 
		[SerializeField] private bool initialHeadUp = true; 

		public bool LastResultIsHead { get; private set; }
		public System.Action<bool> OnFlipCompleted; 

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
			if (listenForSpace && flipEnabled && !isFlipping && Input.GetKeyDown(KeyCode.Space))
			{
				TriggerFlip();
			}
		}

		
		public void TriggerFlip()
		{
			if (!flipEnabled || isFlipping)
			{
				return;
			}

			isFlipping = true;
			if (coinFlipClip != null && audioSource != null)
			{
				audioSource.PlayOneShot(coinFlipClip);
			}

			transform.position = initialPosition;
			transform.rotation = initialRotation;

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

			bool resultIsHead = Random.value < chance;
			LastResultIsHead = resultIsHead;

			Vector3 axisNorm = rotationAxis.sqrMagnitude > 0.0001f ? rotationAxis.normalized : Vector3.right;
			bool needHalfTurnRelativeToInitial = (initialHeadUp != resultIsHead);
			float relativeEndOffset = needHalfTurnRelativeToInitial ? 360f : 0f;
			Vector3 targetEuler = initialRotation.eulerAngles + (axisNorm * (360f * flipRotations + relativeEndOffset));

			float half = duration * 0.5f;

			activeSequence?.Kill();
			activeSequence = DOTween.Sequence();

			activeSequence.Insert(0f, transform.DORotate(targetEuler, duration, RotateMode.FastBeyond360).SetEase(Ease.OutCubic));
			activeSequence.Insert(0f, transform.DOMoveY(initialPosition.y + jumpHeight, half).SetEase(Ease.OutQuad));
			activeSequence.Insert(half, transform.DOMoveY(initialPosition.y, half).SetEase(Ease.InQuad));

			activeSequence.OnComplete(() =>
			{
				transform.position = initialPosition;
				transform.rotation = Quaternion.Euler(initialRotation.eulerAngles + (axisNorm * relativeEndOffset));
				if (resultIsHead)
				{
					if (spriteRenderer != null) spriteRenderer.sortingOrder = 1;
					currentHeadsCombo++;
					double baseWorth = upgradeManager != null ? upgradeManager.CurrentBaseCoinWorth : 1d;
					float comboMult = upgradeManager != null ? upgradeManager.CurrentHeadsComboMultiplier : 0f;
					double reward = baseWorth * (1d + (double)comboMult * currentHeadsCombo);
					currencyManager?.AddCoins(reward);
					if (!hasWon && currentHeadsCombo >= headsInRowToWin)
					{
						hasWon = true;
						OnWin?.Invoke();
					}
                    
				}
				else
				{
					if (spriteRenderer != null) spriteRenderer.sortingOrder = 0;
					currentHeadsCombo = 0;
				}
				isFlipping = false;
				if (resultPopup == null)
				{
					resultPopup = FindObjectOfType<FlipResultPopup>();
				}
				resultPopup?.Show(resultIsHead, transform.position);
				flipHistoryUI?.AddFlipResult(resultIsHead);
				OnFlipCompleted?.Invoke(resultIsHead);
			});
		}

		public void SetFlipEnabled(bool enabled)
		{
			FlipEnabled = enabled;
		}

		public void EnableFlip()
		{
			SetFlipEnabled(true);
		}

		public void DisableFlip()
		{
			SetFlipEnabled(false);
		}

		[ContextMenu("Flip: Enable (Debug)")]
		private void CtxEnable()
		{
			EnableFlip();
		}

		[ContextMenu("Flip: Disable (Debug)")]
		private void CtxDisable()
		{
			DisableFlip();
		}

		[ContextMenu("Flip: Toggle (Debug)")]
		private void CtxToggle()
		{
			SetFlipEnabled(!FlipEnabled);
		}

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


