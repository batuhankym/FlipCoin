using UnityEngine;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
    using System.Collections;

namespace FlipCoin.Game
{
	public class FlipHistoryUI : MonoBehaviour
	{
		[SerializeField] private RectTransform contentRoot; 
		[SerializeField] private TMP_Text entryPrefab; 
		[SerializeField] private int maxEntries = 15;
		[Header("Layout")]
		[SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
		[SerializeField] private float spacing = 4f;
		[SerializeField] private float textLineSpacing = 0f;
		[SerializeField] private bool configureLayoutOptions = true;
		[SerializeField] private int paddingTop = 0;
		[SerializeField] private int paddingBottom = 0;
		[SerializeField] private int paddingLeft = 0;
		[SerializeField] private int paddingRight = 0;
		[SerializeField] private Color headsColor = new Color(0.25f, 0.85f, 0.4f);
		[SerializeField] private Color tailsColor = new Color(0.9f, 0.25f, 0.25f);
		[SerializeField] private float enterOffsetY = 30f; 
		[SerializeField] private float enterDuration = 0.2f;
		[SerializeField] private Ease enterEase = Ease.OutCubic;
		[SerializeField] private bool useScaleAnimation = true; 
		[SerializeField] private float scaleFrom = 0.95f;

		[Header("Scroll")]
		[SerializeField] private ScrollRect scrollRect; 
		[SerializeField] private bool autoScrollToBottom = true;

		private readonly List<RectTransform> entries = new List<RectTransform>();

		private void Awake()
		{
			if (contentRoot == null)
			{
				contentRoot = GetComponent<RectTransform>();
			}
			if (verticalLayoutGroup == null && contentRoot != null)
			{
				verticalLayoutGroup = contentRoot.GetComponent<VerticalLayoutGroup>();
			}
			if (verticalLayoutGroup != null)
			{
				verticalLayoutGroup.spacing = spacing;
				if (configureLayoutOptions)
				{
					verticalLayoutGroup.padding.top = paddingTop;
					verticalLayoutGroup.padding.bottom = paddingBottom;
					verticalLayoutGroup.padding.left = paddingLeft;
					verticalLayoutGroup.padding.right = paddingRight;
					verticalLayoutGroup.childControlHeight = true; 
					verticalLayoutGroup.childForceExpandHeight = false; 
				}
			}
		}

		private void Start()
		{
			ScrollToBottom();
		}

		public void AddFlipResult(bool isHead)
		{
			if (entryPrefab == null || contentRoot == null)
			{
				Debug.LogWarning("FlipHistoryUI: entryPrefab veya contentRoot atanmamış.");
				return;
			}

			TMP_Text newText = Instantiate(entryPrefab, contentRoot);
			newText.text = isHead ? "HEADS" : "TAILS";
			newText.color = isHead ? headsColor : tailsColor;
			newText.fontStyle = isHead ? FontStyles.Bold : FontStyles.Normal;
			newText.lineSpacing = textLineSpacing;
			newText.margin = Vector4.zero;

			var layoutElement = newText.GetComponent<LayoutElement>();
			if (layoutElement != null)
			{
				layoutElement.minHeight = -1f;
				layoutElement.preferredHeight = -1f;
				layoutElement.flexibleHeight = -1f;
			}

			RectTransform rt = newText.rectTransform;
			rt.SetAsLastSibling();

			CanvasGroup cg = newText.GetComponent<CanvasGroup>();
			if (cg == null) cg = newText.gameObject.AddComponent<CanvasGroup>();
			cg.alpha = 0f;
			cg.DOFade(1f, enterDuration).SetEase(enterEase);

			if (verticalLayoutGroup != null || useScaleAnimation)
			{
				Vector3 startScale = Vector3.one * scaleFrom;
				rt.localScale = startScale;
				rt.DOScale(1f, enterDuration).SetEase(enterEase);
			}
			else
			{
				Vector2 targetPos = rt.anchoredPosition;
				rt.anchoredPosition = targetPos + new Vector2(0f, enterOffsetY);
				rt.DOAnchorPos(targetPos, enterDuration).SetEase(enterEase);
			}

			entries.Add(rt);
			TrimIfNeeded();
			ScrollToBottom();
		}

		private void TrimIfNeeded()
		{
			while (entries.Count > maxEntries)
			{
				RectTransform first = entries[0];
				entries.RemoveAt(0);
				if (first != null)
				{
					Destroy(first.gameObject);
				}
			}
			ScrollToBottom();
		}

		private void ScrollToBottom()
		{
			if (!autoScrollToBottom || scrollRect == null || contentRoot == null) return;
			Canvas.ForceUpdateCanvases();
			LayoutRebuilder.ForceRebuildLayoutImmediate(contentRoot);
			StartCoroutine(LateScrollToBottom());
		}

		private IEnumerator LateScrollToBottom()
		{
			yield return null; 
			float target = (contentRoot.pivot.y >= 0.5f) ? 0f : 1f;
			scrollRect.verticalNormalizedPosition = target;
		}
	}
}


