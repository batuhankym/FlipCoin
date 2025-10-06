using UnityEngine;
using UnityEngine.UI;

namespace FlipCoin.Game
{
	public class InGameMenuController : MonoBehaviour
	{
		[Header("Mute Button")]
		[SerializeField] private Button muteButton;
		[SerializeField] private Image muteImage;
		[SerializeField] private Sprite spriteMuted;
		[SerializeField] private Sprite spriteUnmuted;

		[Header("Exit Button")]
		[SerializeField] private Button exitButton;

		[Header("State")]
		[SerializeField] private bool isMuted;

		private void Reset()
		{
			// Inspector otomatik doldurma
			if (muteButton == null) muteButton = transform.Find("Mute")?.GetComponent<Button>();
			if (exitButton == null) exitButton = transform.Find("Exit")?.GetComponent<Button>();
			if (muteImage == null && muteButton != null) muteImage = muteButton.GetComponent<Image>();
		}

		private void Awake()
		{
			if (muteButton != null)
			{
				muteButton.onClick.AddListener(ToggleMute);
			}
			if (exitButton != null)
			{
				exitButton.onClick.AddListener(ExitGame);
			}
			ApplyMuteVisual();
			ApplyMuteAudio();
		}

		public void ToggleMute()
		{
			isMuted = !isMuted;
			ApplyMuteVisual();
			ApplyMuteAudio();
		}

		private void ApplyMuteVisual()
		{
			if (muteImage == null) return;
			if (isMuted)
			{
				if (spriteMuted != null) muteImage.sprite = spriteMuted;
			}
			else
			{
				if (spriteUnmuted != null) muteImage.sprite = spriteUnmuted;
			}
		}

		private void ApplyMuteAudio()
		{
			AudioListener.pause = isMuted;
			AudioListener.volume = isMuted ? 0f : 1f;
		}

		public void ExitGame()
		{
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
			#else
			Application.Quit();
			#endif
		}
	}
}


