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
		[SerializeField] private CanvasGroup overrideCanvasGroup; // Parşömen'in CanvasGroup'u (isteğe bağlı)
		[SerializeField] private UnityEngine.UI.Graphic[] extraUiGraphicsToFade; // CanvasGroup kapsamazsa
		[SerializeField] private SpriteRenderer[] extraSpriteRenderersToFade; // UI degilse (SpriteRenderer)

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
		[SerializeField] private bool preferParentCanvasGroup = true; // Parşömen gibi üst objeyi fade et
		[SerializeField] private bool destroyCanvasGroupOwner = false; // Fade bitince parşömeni kapat/yok et

		private Coroutine playRoutine;

		private void Awake()
		{
			if (targetText == null)
			{
				targetText = GetComponentInChildren<TMP_Text>();
			}
			// CanvasGroup seçim önceliği: override → parent (tercih) → local mevcut → local ekle
			if (overrideCanvasGroup != null)
			{
				canvasGroup = overrideCanvasGroup;
			}
			else
			{
				CanvasGroup found = null;
				if (preferParentCanvasGroup)
				{
					found = GetComponentInParent<CanvasGroup>();
				}
				if (found == null)
				{
					found = GetComponent<CanvasGroup>();
				}
				if (found == null)
				{
					found = gameObject.AddComponent<CanvasGroup>();
				}
				canvasGroup = found;
			}
			// Güvenli: input etkileşimini de kapatıp/açalım
			canvasGroup.ignoreParentGroups = false;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = true;
			if (audioSource == null)
			{
				audioSource = GetComponent<AudioSource>();
				if (audioSource == null)
				{
					audioSource = gameObject.AddComponent<AudioSource>();
				}
				audioSource.clip = null; // sahnede atanmis bir clip varsa calmasin
				audioSource.playOnAwake = false; // acilis seslerini engelle
				audioSource.Stop(); // varsa calmayi kesin
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
			bool skipFirstSound = true; // ilk karakterde ses oynatma

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
					if (!skipFirstSound && typeSound != null && audioSource != null && (Time.time - lastSoundTime) >= minSoundInterval)
					{
						audioSource.PlayOneShot(typeSound);
						lastSoundTime = Time.time;
					}
					skipFirstSound = false;
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
			// Parent CanvasGroup kullanılıyorsa, parent'ın alpha'sını da bu coroutine yönetir
			canvasGroup.DOFade(0f, 0.4f).SetEase(Ease.OutQuad).SetUpdate(true);
			// Ek hedefleri de güvence altına al (ayrı Canvas / SpriteRenderer durumları)
			if (extraUiGraphicsToFade != null)
			{
				foreach (var g in extraUiGraphicsToFade)
				{
					if (g == null) continue;
					g.DOKill();
					g.DOFade(0f, 0.4f).SetEase(Ease.OutQuad).SetUpdate(true);
				}
			}
			if (extraSpriteRenderersToFade != null)
			{
				foreach (var sr in extraSpriteRenderersToFade)
				{
					if (sr == null) continue;
					sr.DOKill();
					sr.DOFade(0f, 0.4f).SetEase(Ease.OutQuad).SetUpdate(true);
				}
			}
			yield return new WaitForSeconds(0.45f);
			if (destroyCanvasGroupOwner && canvasGroup != null)
			{
				Destroy(canvasGroup.gameObject);
				yield break;
			}
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


