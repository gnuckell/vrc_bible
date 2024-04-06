
using System;
using System.Text;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.Core;
using VRC.SDKBase;
using VRC.Udon;

public class BibleReader : UdonSharpBehaviour
{
	#region Fields

	[SerializeField] private GameObject pref_content;

	[SerializeField] private BibleHost _host;
	public BibleHost host => _host;

	[SerializeField] private ScrollRect _scroll_rect;

	#endregion
	#region Pinions


	private bool prox_is_awaiting_scroll_head;
	private float prox_preheight_scroll_head;

	private string trans_lut;

	private readonly int[] CHAPTER_HEADS = new int[BibleHost.MAX_CHAPTER_COUNT];

	private ReaderContentPanelBehaviour content_head;
	private ReaderContentPanelBehaviour content_tail;
	private ReaderContentPanelBehaviour _content_focused;
	private ReaderContentPanelBehaviour content_focused
	{
		get => _content_focused;
		set
		{
			_content_focused = value;
			host.chapter_index = value.chapter_index;
		}
	}


	#endregion
	#region Properties

	#endregion

	#region General

	public void Init()
	{
		_scroll_rect = GetComponent<ScrollRect>();

		SwitchTranslation(host.trans_index);
	}

	void Update()
	{
		if (prox_is_awaiting_scroll_head)
			OnScrollPastHeadActually();

		/** <<============================================================>> **/

		var focus = CalculateFocusedChild();
		if (focus != _content_focused) content_focused = focus;
	}

	public void SwitchTranslation(int index)
	{
		trans_lut = host.translation_docs[index].text;

		var start = 0;
		for (var i = 0; i < CHAPTER_HEADS.Length; i++)
		{
			CHAPTER_HEADS[i] = start;
			start = BibleUtils.NthIndexOf(trans_lut, BibleHost.SEP, host.CHAPTER_LENGTHS[i], start);
		}

		Reset();
	}

	public void Reset() => Reset(host.chapter_index);
	public void Reset(int chapt)
	{
		Clear();

		content_head = CreateChapterContent(chapt);
		content_tail = content_head;

		_scroll_rect.verticalNormalizedPosition = 1f;
		SetFocusedChild(content_head);
	}

	private void Clear()
	{
		var length = _scroll_rect.content.childCount;
		for (var i = 0; i < length; i++)
			Destroy(_scroll_rect.content.GetChild(i).gameObject);
	}

	#endregion
	#region Indexing

	private ReaderContentPanelBehaviour GetNextContent(ReaderContentPanelBehaviour inst) => _scroll_rect.content.GetChild(BibleUtils.GetChildIndex(_scroll_rect.content, inst.transform) + 1).GetComponent<ReaderContentPanelBehaviour>();
	private ReaderContentPanelBehaviour GetPrevContent(ReaderContentPanelBehaviour inst) => _scroll_rect.content.GetChild(BibleUtils.GetChildIndex(_scroll_rect.content, inst.transform) - 1).GetComponent<ReaderContentPanelBehaviour>();

	#endregion
	#region Scroll Events

	public void OnScrollValueChanged()
	{
		if (_scroll_rect.verticalNormalizedPosition > 1f) OnScrollPastHead();
		else if (_scroll_rect.verticalNormalizedPosition < 0f) OnScrollPastTail();
	}

	private void OnScrollPastHead()
	{
		if (prox_is_awaiting_scroll_head) return;
		prox_is_awaiting_scroll_head = true;
		prox_preheight_scroll_head = GetCalculatedContentHeight();

		var content = CreateChapterContent(content_head.chapter_index - 1);
		if (content == null) return;

		content.transform.SetAsFirstSibling();
		if (content.chapter_index == 1)
		{
			// var title = CreateNewBookTitle(content.book_index);
			// title.SetAsFirstSibling();
		}

		content_head = content;
		_scroll_rect.enabled = false;
	}

	private void OnScrollPastHeadActually()
	{
		var post_height = GetCalculatedContentHeight();

		var fake_scroll_y = _scroll_rect.verticalNormalizedPosition;
		var scroll_y = fake_scroll_y / (post_height / prox_preheight_scroll_head);
		_scroll_rect.verticalNormalizedPosition = scroll_y;
		_scroll_rect.enabled = true;
		prox_is_awaiting_scroll_head = false;
	}

	private void OnScrollPastTail()
	{
		var content = CreateChapterContent(content_tail.chapter_index + 1);
		if (content == null) return;

		if (content.chapter_index == 1)
		{
			// var title = CreateNewBookTitle(content.book_index);
			// title.SetAsLastSibling();
		}
		content.transform.SetAsLastSibling();

		content_tail = content;
	}

	private float GetCalculatedContentHeight()
	{
		return _scroll_rect.content.rect.height;
	}

	private void SetFocusedChild(ReaderContentPanelBehaviour inst)
	{
		_content_focused = inst;
		host.chapter_index = inst.chapter_index;
	}

	private ReaderContentPanelBehaviour CalculateFocusedChild()
	{
		for (int i = _scroll_rect.content.childCount - 1; i >= 0; i--)
		{
			var iChild = (RectTransform)_scroll_rect.content.GetChild(i);
			var iPosition = 1f + (iChild.anchoredPosition.y / _scroll_rect.content.rect.height);
			if (_scroll_rect.verticalNormalizedPosition < iPosition)
				return iChild.GetComponent<ReaderContentPanelBehaviour>();
		}
		return _content_focused;
	}

	private float GetNormalizedPositionInScrollView(RectTransform rect)
	{
		var height = 0f;
		for (int i = 0; i < _scroll_rect.content.childCount; i++)
		{
			var iChild = (RectTransform)_scroll_rect.content.GetChild(i);
			if (iChild == rect) break;
			height += iChild.rect.height;
		}
		return 1f - (height / _scroll_rect.content.rect.height);
	}

	#endregion
	#region Content Creation

	private ReaderContentPanelBehaviour CreateChapterContent(int chapter)
	{
		if (chapter < 0 || chapter >= BibleHost.MAX_CHAPTER_COUNT) return null;

		var obj = Instantiate(pref_content, _scroll_rect.content);
		var panel = obj.GetComponent<ReaderContentPanelBehaviour>();

		panel.Init(this, chapter);
		return panel;
	}

	public string CreateChapterText(int chapter)
	{
		var result = string.Empty;

		var char_head = CHAPTER_HEADS[chapter];
		for (var i = 0; i < host.CHAPTER_LENGTHS[chapter]; i++)
		{
			var char_end = trans_lut.IndexOf(BibleHost.SEP, char_head);
			result += $" {GetRichVerseNumber(i)}{trans_lut.Substring(char_head, char_end - char_head)}";
			char_head = char_end + 1;
		}
		return result.Substring(1);
	}

	#endregion
	#region Statics

	private static string GetRichVerseNumber(int index) => $"<sup>{index + 1}</sup>";

	#endregion
}
