using UnityEngine;

namespace FlipCoin.Game
{
	public class UpgradeUI : MonoBehaviour
	{
		[SerializeField] private UpgradeManager upgradeManager;
		[SerializeField] private CurrencyManager currencyManager;

		private void Awake()
		{
			if (upgradeManager == null) upgradeManager = FindObjectOfType<UpgradeManager>();
			if (currencyManager == null) currencyManager = FindObjectOfType<CurrencyManager>();
		}

		public void BuyHeadsChance() { upgradeManager?.BuyHeadsChance(); }
		public void BuySecondsFlipTime() { upgradeManager?.BuySecondsFlipTime(); }
		public void BuyHeadsComboMultiplier() { upgradeManager?.BuyHeadsComboMultiplier(); }
		public void BuyBaseCoinWorth() { upgradeManager?.BuyBaseCoinWorth(); }
	}
}



