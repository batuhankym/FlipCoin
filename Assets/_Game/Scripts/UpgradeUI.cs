using UnityEngine;
using TMPro;

namespace FlipCoin.Game
{
	public class UpgradeUI : MonoBehaviour
	{
		[SerializeField] private UpgradeManager upgradeManager;
		[SerializeField] private CurrencyManager currencyManager;

		[Header("UI Elementleri")]
		[SerializeField] private TMP_Text headsChanceDescription;
		[SerializeField] private TMP_Text flipTimeDescription;
		[SerializeField] private TMP_Text comboDescription;
		[SerializeField] private TMP_Text coinWorthDescription;

		private void Awake()
		{
			if (upgradeManager == null) upgradeManager = FindObjectOfType<UpgradeManager>();
			if (currencyManager == null) currencyManager = FindObjectOfType<CurrencyManager>();
		}

		private void Start()
		{
			RefreshUI();
		}

		private void OnEnable()
		{
			if (currencyManager != null)
			{
				currencyManager.OnCurrencyChanged += OnCurrencyChanged;
			}
			if (upgradeManager != null)
			{
				upgradeManager.OnUpgradesChanged += OnUpgradesChanged;
			}
		}

		private void OnDisable()
		{
			if (currencyManager != null)
			{
				currencyManager.OnCurrencyChanged -= OnCurrencyChanged;
			}
			if (upgradeManager != null)
			{
				upgradeManager.OnUpgradesChanged -= OnUpgradesChanged;
			}
		}

		private void OnCurrencyChanged(double newValue)
		{
			RefreshUI();
		}

		private void OnUpgradesChanged()
		{
			RefreshUI();
		}

		private void RefreshUI()
		{
			if (upgradeManager == null) return;

			// Her yükseltme için açıklama ve durum güncelle
			UpdateUpgradeText(UpgradeType.HeadsChance, headsChanceDescription);
			UpdateUpgradeText(UpgradeType.SecondsFlipTime, flipTimeDescription);
			UpdateUpgradeText(UpgradeType.HeadsComboMultiplier, comboDescription);
			UpdateUpgradeText(UpgradeType.BaseCoinWorth, coinWorthDescription);
		}

		private void UpdateUpgradeText(UpgradeType type, TMP_Text textElement)
		{
			if (textElement == null || upgradeManager == null) return;

			string effect = upgradeManager.GetUpgradeEffectSummary(type);
			double cost = upgradeManager.GetCurrentCost(type);
			bool canAfford = upgradeManager.CanAffordUpgrade(type);
			bool isMaxed = upgradeManager.IsUpgradeAtMax(type);

			string priceLine = $"${cost:F2}";
			textElement.text = $"{effect}\n{priceLine}";
			textElement.color = isMaxed ? Color.yellow : (canAfford ? Color.green : Color.red);
		}

		public void BuyHeadsChance() 
		{ 
			if (upgradeManager?.TryBuyUpgrade(UpgradeType.HeadsChance) == true)
			{
				RefreshUI();
			}
		}
		
		public void BuySecondsFlipTime() 
		{ 
			if (upgradeManager?.TryBuyUpgrade(UpgradeType.SecondsFlipTime) == true)
			{
				RefreshUI();
			}
		}
		
		public void BuyHeadsComboMultiplier() 
		{ 
			if (upgradeManager?.TryBuyUpgrade(UpgradeType.HeadsComboMultiplier) == true)
			{
				RefreshUI();
			}
		}
		
		public void BuyBaseCoinWorth() 
		{ 
			if (upgradeManager?.TryBuyUpgrade(UpgradeType.BaseCoinWorth) == true)
			{
				RefreshUI();
			}
		}
	}
}



