using UnityEngine;

namespace FlipCoin.Game
{
	public class CurrencyManager : MonoBehaviour
	{
		[SerializeField] private double startingCoins = 50000d;

		public double Coins { get; private set; }
		public System.Action<double> OnCurrencyChanged;

		private void Awake()
		{
			Coins = startingCoins;
		}

		public void AddCoins(double amount)
		{
			if (amount <= 0d)
			{
				return;
			}
			Coins += amount;
			Coins = System.Math.Round(Coins, 2, System.MidpointRounding.AwayFromZero);
			OnCurrencyChanged?.Invoke(Coins);
		}

		public bool TrySpend(double amount)
		{
			if (amount <= 0d)
			{
				return true;
			}
			if (Coins + 1e-9d < amount)
			{
				return false;
			}
			Coins -= amount;
			Coins = System.Math.Round(Coins, 2, System.MidpointRounding.AwayFromZero);
			OnCurrencyChanged?.Invoke(Coins);
			return true;
		}
	}
}


