using UnityEngine;

namespace FlipCoin.Game
{
	public enum UpgradeType
	{
		HeadsChance,
		SecondsFlipTime,
		HeadsComboMultiplier,
		BaseCoinWorth
	}

	public class UpgradeManager : MonoBehaviour
	{
		[Header("Bagimliliklar")]
		[SerializeField] private CurrencyManager currencyManager;

	[Header("Baslangic Degerleri")]
	[SerializeField] private float startHeadsChance = 0.15f; 
	[SerializeField] private float startSecondsFlipTime = 0.8f; 
	[SerializeField] private float startHeadsComboMultiplier = 0.05f; 
	[SerializeField] private double startBaseCoinWorth = 1d;

	[Header("Yukseltme Artis Oranlari")]
	[SerializeField] private float headsChanceIncrease = 0.05f; 
	[SerializeField] private float flipTimeDecrease = 0.1f; 
	[SerializeField] private float comboMultiplierIncrease = 0.1f; 
	[SerializeField] private double baseCoinWorthIncrease = 0.5d; 

	[Header("Baslangic Maliyetleri (x2.5 artis)")]
	[SerializeField] private double costHeadsChance = 10d;
	[SerializeField] private double costSecondsFlipTime = 10d;
	[SerializeField] private double costHeadsComboMultiplier = 10d;
	[SerializeField] private double costBaseCoinWorth = 10d;

		private float currentHeadsChance;
		private float currentSecondsFlipTime;
		private float currentHeadsComboMultiplier;
		private double currentBaseCoinWorth;

		public System.Action OnUpgradesChanged;

		private void Awake()
		{
			if (currencyManager == null)
			{
				currencyManager = FindObjectOfType<CurrencyManager>();
			}
			currentHeadsChance = Mathf.Clamp01(startHeadsChance);
			currentSecondsFlipTime = Mathf.Max(0.1f, startSecondsFlipTime); 
			currentHeadsComboMultiplier = Mathf.Max(0f, startHeadsComboMultiplier);
			currentBaseCoinWorth = System.Math.Max(0d, startBaseCoinWorth);
		}

		public float CurrentHeadsChance => currentHeadsChance;
		public float CurrentSecondsFlipTime => currentSecondsFlipTime;
		public float CurrentHeadsComboMultiplier => currentHeadsComboMultiplier;
		public double CurrentBaseCoinWorth => currentBaseCoinWorth;

		public float HeadsChanceIncrease => headsChanceIncrease;
		public float FlipTimeDecrease => flipTimeDecrease;
		public float ComboMultiplierIncrease => comboMultiplierIncrease;
		public double BaseCoinWorthIncrease => baseCoinWorthIncrease;

		public double GetCurrentCost(UpgradeType type)
		{
			switch (type)
			{
				case UpgradeType.HeadsChance: return costHeadsChance;
				case UpgradeType.SecondsFlipTime: return costSecondsFlipTime;
				case UpgradeType.HeadsComboMultiplier: return costHeadsComboMultiplier;
				case UpgradeType.BaseCoinWorth: return costBaseCoinWorth;
			}
			return 0d;
		}

		public bool TryBuyUpgrade(UpgradeType type)
		{
			if (currencyManager == null)
			{
				currencyManager = FindObjectOfType<CurrencyManager>();
			}
			double price = GetCurrentCost(type);

			switch (type)
			{
				case UpgradeType.HeadsChance:
					if (currentHeadsChance >= 1f - 1e-6f)
					{
						Debug.Log("Upgrade reddedildi: HeadsChance zaten 1.0 (maksimum).");
						return false;
					}
					break;
				case UpgradeType.SecondsFlipTime:
					if (currentSecondsFlipTime <= 0.1f)
					{
						Debug.Log("Upgrade reddedildi: FlipTime zaten minimum (0.1s).");
						return false;
					}
					break;
			}

			if (currencyManager == null || !currencyManager.TrySpend(price))
			{
				return false;
			}

			switch (type)
			{
				case UpgradeType.HeadsChance:
					float beforeChance = currentHeadsChance;
					currentHeadsChance = 1f - (1f - currentHeadsChance) * (1f - headsChanceIncrease);
					currentHeadsChance = Mathf.Clamp01(currentHeadsChance);
					costHeadsChance *= 2.5d;
					Debug.Log($"Upgrade: HeadsChance {beforeChance:F3} -> {currentHeadsChance:F3}, nextCost={costHeadsChance:F0}");
					break;
				case UpgradeType.SecondsFlipTime:
					float beforeTime = currentSecondsFlipTime;
					currentSecondsFlipTime = Mathf.Max(0.1f, currentSecondsFlipTime * (1f - flipTimeDecrease));
					costSecondsFlipTime *= 2.5d;
					Debug.Log($"Upgrade: SecondsFlipTime {beforeTime:F2} -> {currentSecondsFlipTime:F2}, nextCost={costSecondsFlipTime:F0}");
					break;
				case UpgradeType.HeadsComboMultiplier:
					float beforeCombo = currentHeadsComboMultiplier;
					currentHeadsComboMultiplier = Mathf.Max(0f, currentHeadsComboMultiplier + comboMultiplierIncrease);
					costHeadsComboMultiplier *= 2.5d;
					Debug.Log($"Upgrade: HeadsComboMultiplier {beforeCombo:F2} -> {currentHeadsComboMultiplier:F2}, nextCost={costHeadsComboMultiplier:F0}");
					break;
				case UpgradeType.BaseCoinWorth:
					double beforeWorth = currentBaseCoinWorth;
					currentBaseCoinWorth = System.Math.Max(0d, currentBaseCoinWorth + baseCoinWorthIncrease);
					costBaseCoinWorth *= 2.5d;
					Debug.Log($"Upgrade: BaseCoinWorth {beforeWorth:F1} -> {currentBaseCoinWorth:F1}, nextCost={costBaseCoinWorth:F0}");
					break;
			}
			OnUpgradesChanged?.Invoke();
			return true;
		}

		public void BuyHeadsChance() { TryBuyUpgrade(UpgradeType.HeadsChance); }
		public void BuySecondsFlipTime() { TryBuyUpgrade(UpgradeType.SecondsFlipTime); }
		public void BuyHeadsComboMultiplier() { TryBuyUpgrade(UpgradeType.HeadsComboMultiplier); }
		public void BuyBaseCoinWorth() { TryBuyUpgrade(UpgradeType.BaseCoinWorth); }

		public string GetUpgradeDescription(UpgradeType type)
		{
			switch (type)
			{
				case UpgradeType.HeadsChance:
					float newChance = 1f - (1f - currentHeadsChance) * (1f - headsChanceIncrease);
					newChance = Mathf.Clamp01(newChance);
					return $"Yazı şansı: %{(currentHeadsChance * 100):F1} → %{(newChance * 100):F1}";
				case UpgradeType.SecondsFlipTime:
					float newTime = Mathf.Max(0.1f, currentSecondsFlipTime * (1f - flipTimeDecrease));
					return $"Flip süresi: {currentSecondsFlipTime:F2}s → {newTime:F2}s";
				case UpgradeType.HeadsComboMultiplier:
					float newCombo = currentHeadsComboMultiplier + comboMultiplierIncrease;
					return $"Kombo çarpanı: {currentHeadsComboMultiplier:F2} → {newCombo:F2}";
				case UpgradeType.BaseCoinWorth:
					double newWorth = currentBaseCoinWorth + baseCoinWorthIncrease;
					return $"Temel değer: {currentBaseCoinWorth:F1} → {newWorth:F1}";
				default:
					return "Bilinmeyen yükseltme";
			}
		}

		public string GetUpgradeEffectSummary(UpgradeType type)
		{
			switch (type)
			{
				case UpgradeType.HeadsChance:
					return $"+{headsChanceIncrease * 100f:F0}% HEADS CHANCE";
				case UpgradeType.SecondsFlipTime:
					float delta = Mathf.Max(0f, currentSecondsFlipTime * flipTimeDecrease);
					return $"-{delta:F2} SECONDS FLIP TIME";
				case UpgradeType.HeadsComboMultiplier:
					return $"+{comboMultiplierIncrease:F2}x HEADS COMBO MULT";
				case UpgradeType.BaseCoinWorth:
					return $"+{baseCoinWorthIncrease:F2} COIN VALUE";
			}
			return string.Empty;
		}

		public bool CanAffordUpgrade(UpgradeType type)
		{
			if (currencyManager == null) return false;
			return currencyManager.Coins >= GetCurrentCost(type);
		}

		public bool IsUpgradeAtMax(UpgradeType type)
		{
			switch (type)
			{
				case UpgradeType.HeadsChance:
					return currentHeadsChance >= 1f - 1e-6f;
				case UpgradeType.SecondsFlipTime:
					return currentSecondsFlipTime <= 0.1f;
				default:
					return false;
			}
		}
	}
}


