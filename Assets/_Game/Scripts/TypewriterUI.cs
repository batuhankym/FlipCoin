using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace FlipCoin.Game
{
	public class TypewriterUI : MonoBehaviour
	{
		[Header("Bagimliliklar")]
		[SerializeField] private TMP_Text targetText;
		[SerializeField] private CanvasGroup canvasGroup;

		[Header("Icerik")]
		[SerializeField] private List<string> lines = new List<string>();

		[Header("Zamanlama")]
		[SerializeField] private float charactersPerSecond = 30f; // 30 harf/sn
		[SerializeField] private float linePauseSeconds = 0.5f; // satir bitince bekleme
		[SerializeField] private float totalDisplaySeconds = 3f; // tum metin bittikten sonra kalma suresi

		[Header("Ses")] 
		[SerializeField] private AudioClip typeSound;
		[SerializeField] private AudioSource audioSource;
		[SerializeField] private float minSoundInterval = 0.03f; // ses spamini onlemek icin

		[Header("Davranis")]
		[SerializeField] private bool playOnEnable = true;
		[SerializeField] private bool clearOnStart = true;
		[SerializeField] private bool destroyOnFinish = false;

		private Coroutine playRoutine;

		private void Awake()
		{
			if (targetText == null)
			{
				targetText = GetComponentInChildren<TMP_Text>();
			}
			if (canvasGroup == null)
			{
				canvasGroup = GetComponent<CanvasGroup>();
				if (canvasGroup == null)
				{
					canvasGroup = gameObject.AddComponent<CanvasGroup>();
				}
			}
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

		private void OnEnable()
		{
			if (playOnEnable)
			{
				StartTypewriter();
			}
		}

		public void SetLines(IEnumerable<string> newLines)
		{
			lines.Clear();
			lines.AddRange(newLines);
		}

		public void StartTypewriter()
		{
			if (playRoutine != null)
			{
				StopCoroutine(playRoutine);
			}
			playRoutine = StartCoroutine(PlayRoutine());
		}

		private IEnumerator PlayRoutine()
		{
			canvasGroup.alpha = 1f;
			if (clearOnStart && targetText != null)
			{
				targetText.text = string.Empty;
			}
			float charDelay = charactersPerSecond > 0.01f ? (1f / charactersPerSecond) : 0.033f;
			float lastSoundTime = -999f;

			for (int i = 0; i < lines.Count; i++)
			{
				string line = lines[i] ?? string.Empty;
				for (int c = 0; c < line.Length; c++)
				{
					char ch = line[c];
					if (targetText != null)
					{
						targetText.text += ch;
					}
					// harf sesi (çok sık çalmamak için min interval)
					if (typeSound != null && audioSource != null && (Time.time - lastSoundTime) >= minSoundInterval)
					{
						audioSource.PlayOneShot(typeSound);
						lastSoundTime = Time.time;
					}
					yield return new WaitForSeconds(charDelay);
				}
				// satir bitti, alt satira gec
				if (i < lines.Count - 1 && targetText != null)
				{
					targetText.text += "\n";
				}
				yield return new WaitForSeconds(linePauseSeconds);
			}

			// tum metin ekranda kalma suresi
			yield return new WaitForSeconds(totalDisplaySeconds);

			// fade out
			canvasGroup.DOKill();
			canvasGroup.DOFade(0f, 0.4f).SetEase(Ease.OutQuad);
			yield return new WaitForSeconds(0.45f);
			if (destroyOnFinish)
			{
				Destroy(gameObject);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
	}
}


