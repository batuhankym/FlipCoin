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

		[Header("Baslangic Degerleri (Cok Dusuk Tutun)")]
		[SerializeField] private float startHeadsChance = 0.0015f; // %0.15 (dusuk baslangic, x10 ile kademeli artar)
		[SerializeField] private float startSecondsFlipTime = 0.8f; // sn
		[SerializeField] private float startHeadsComboMultiplier = 0.05f; // lineer artis carpani
		[SerializeField] private double startBaseCoinWorth = 1d;

		[Header("Baslangic Maliyetleri (x10 artis)")]
		[SerializeField] private double costHeadsChance = 1d;
		[SerializeField] private double costSecondsFlipTime = 1d;
		[SerializeField] private double costHeadsComboMultiplier = 1d;
		[SerializeField] private double costBaseCoinWorth = 1d;

		private float currentHeadsChance;
		private float currentSecondsFlipTime;
		private float currentHeadsComboMultiplier;
		private double currentBaseCoinWorth;

		private void Awake()
		{
			if (currencyManager == null)
			{
				currencyManager = FindObjectOfType<CurrencyManager>();
			}
			currentHeadsChance = Mathf.Clamp01(startHeadsChance);
			currentSecondsFlipTime = Mathf.Max(0.05f, startSecondsFlipTime);
			currentHeadsComboMultiplier = Mathf.Max(0f, startHeadsComboMultiplier);
			currentBaseCoinWorth = System.Math.Max(0d, startBaseCoinWorth);
		}

		public float CurrentHeadsChance => currentHeadsChance;
		public float CurrentSecondsFlipTime => currentSecondsFlipTime;
		public float CurrentHeadsComboMultiplier => currentHeadsComboMultiplier;
		public double CurrentBaseCoinWorth => currentBaseCoinWorth;

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

			// On-satis validasyonlari (cap kontrol vb.)
			switch (type)
			{
				case UpgradeType.HeadsChance:
					if (currentHeadsChance >= 1f - 1e-6f)
					{
						Debug.Log("Upgrade reddedildi: HeadsChance zaten 1.0 (maksimum).");
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
						currentHeadsChance = Mathf.Clamp01(currentHeadsChance * 10f);
					costHeadsChance *= 10d;
						Debug.Log($"Upgrade: HeadsChance {beforeChance} -> {currentHeadsChance}, nextCost={costHeadsChance}");
					break;
				case UpgradeType.SecondsFlipTime:
						float beforeTime = currentSecondsFlipTime;
						currentSecondsFlipTime = Mathf.Max(0.01f, currentSecondsFlipTime * 10f);
					costSecondsFlipTime *= 10d;
						Debug.Log($"Upgrade: SecondsFlipTime {beforeTime} -> {currentSecondsFlipTime}, nextCost={costSecondsFlipTime}");
					break;
				case UpgradeType.HeadsComboMultiplier:
						float beforeCombo = currentHeadsComboMultiplier;
						currentHeadsComboMultiplier = Mathf.Max(0f, currentHeadsComboMultiplier * 10f);
					costHeadsComboMultiplier *= 10d;
						Debug.Log($"Upgrade: HeadsComboMultiplier {beforeCombo} -> {currentHeadsComboMultiplier}, nextCost={costHeadsComboMultiplier}");
					break;
				case UpgradeType.BaseCoinWorth:
						double beforeWorth = currentBaseCoinWorth;
						currentBaseCoinWorth = System.Math.Max(0d, currentBaseCoinWorth * 10d);
					costBaseCoinWorth *= 10d;
						Debug.Log($"Upgrade: BaseCoinWorth {beforeWorth} -> {currentBaseCoinWorth}, nextCost={costBaseCoinWorth}");
					break;
			}
			return true;
		}

		// UI kolayligi icin helper metodlar
		public void BuyHeadsChance() { TryBuyUpgrade(UpgradeType.HeadsChance); }
		public void BuySecondsFlipTime() { TryBuyUpgrade(UpgradeType.SecondsFlipTime); }
		public void BuyHeadsComboMultiplier() { TryBuyUpgrade(UpgradeType.HeadsComboMultiplier); }
		public void BuyBaseCoinWorth() { TryBuyUpgrade(UpgradeType.BaseCoinWorth); }
	}
}


