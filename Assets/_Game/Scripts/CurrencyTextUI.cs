using UnityEngine;
using TMPro;

namespace FlipCoin.Game
{
	public class CurrencyTextUI : MonoBehaviour
	{
		[SerializeField] private CurrencyManager currencyManager;
		[SerializeField] private TMP_Text targetText;
		[SerializeField] private string prefix = "Coins: ";
		[SerializeField] private string numberFormat = "N0"; // Ornek: 12,345

		private void Awake()
		{
			if (currencyManager == null)
			{
				currencyManager = FindObjectOfType<CurrencyManager>();
			}
			if (targetText == null)
			{
				targetText = GetComponent<TMP_Text>();
			}
		}

		private void OnEnable()
		{
			if (currencyManager != null)
			{
				currencyManager.OnCurrencyChanged += HandleCurrencyChanged;
			}
			RefreshNow();
		}

		private void OnDisable()
		{
			if (currencyManager != null)
			{
				currencyManager.OnCurrencyChanged -= HandleCurrencyChanged;
			}
		}

		private void HandleCurrencyChanged(double newValue)
		{
			RefreshNow();
		}

		private void RefreshNow()
		{
			if (targetText == null || currencyManager == null)
			{
				return;
			}
			targetText.text = prefix + currencyManager.Coins.ToString(numberFormat);
		}
	}
}


