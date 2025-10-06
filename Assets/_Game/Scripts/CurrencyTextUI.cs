using UnityEngine;
using TMPro;
using System.Globalization;

namespace FlipCoin.Game
{
	public class CurrencyTextUI : MonoBehaviour
	{
		[SerializeField] private CurrencyManager currencyManager;
		[SerializeField] private TMP_Text targetText;
		[SerializeField] private string currencySymbol = "$";
		[SerializeField] private string numberFormat = "F2"; 
		[SerializeField] private bool useInvariantCulture = true; 

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
			if (string.IsNullOrEmpty(numberFormat) || numberFormat == "N0")
			{
				numberFormat = "F2";
			}
		}

		private void OnValidate()
		{
			if (string.IsNullOrEmpty(numberFormat) || numberFormat == "N0")
			{
				numberFormat = "F2";
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
			string formatted = useInvariantCulture
				? currencyManager.Coins.ToString(numberFormat, CultureInfo.InvariantCulture)
				: currencyManager.Coins.ToString(numberFormat);
			targetText.text = currencySymbol + formatted;
		}
	}
}


