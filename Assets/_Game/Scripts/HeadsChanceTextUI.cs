using UnityEngine;
using TMPro;

namespace FlipCoin.Game
{
	public class HeadsChanceTextUI : MonoBehaviour
	{
		[SerializeField] private UpgradeManager upgradeManager;
		[SerializeField] private TMP_Text targetText;
		[SerializeField] private string numberFormat = "F1"; // %15.0 gibi

		private void Awake()
		{
			if (upgradeManager == null)
			{
				upgradeManager = FindObjectOfType<UpgradeManager>();
			}
			if (targetText == null)
			{
				targetText = GetComponent<TMP_Text>();
			}
		}

		private void OnEnable()
		{
			if (upgradeManager != null)
			{
				upgradeManager.OnUpgradesChanged += HandleUpgradesChanged;
			}
		}

		private void Start()
		{
			// Start, sahnedeki diger Awake'lerden sonra cagrilir; ilk degeri dogru yansitir
			RefreshNow();
		}

		private void OnDisable()
		{
			if (upgradeManager != null)
			{
				upgradeManager.OnUpgradesChanged -= HandleUpgradesChanged;
			}
		}

		private void HandleUpgradesChanged()
		{
			RefreshNow();
		}

		private void RefreshNow()
		{
			if (targetText == null || upgradeManager == null)
			{
				return;
			}
			float p = upgradeManager.CurrentHeadsChance;
			targetText.text = $"%{(p * 100f).ToString(numberFormat)}";
		}
	}
}


