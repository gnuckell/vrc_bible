
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
	#region Constants

	private const char SEP = '\n';

	/// <summary>
	/// Total number books in the entire Bible.
	///</summary>
	public const int MAX_BOOK_COUNT = 66;

	/// <summary>
	/// Total number of chapters in the Bible. Used to simplify reference indeces and quicken verse loading.
	///</summary>
	public const int MAX_CHAPTER_COUNT = 1189;

	private const int LUT_REF_LENGTH = 8;
	private const int VERSE_OFFSET = 0;

	#endregion
	#region Fields

	[SerializeField] private TextAsset[] trans_doc;
	[SerializeField] private TextAsset book_doc;
	[SerializeField] private TextAsset address_doc;

	[SerializeField] private GameObject pref_content;

	[SerializeField] public int trans_index;

	#endregion
	#region Pinions

	private ScrollRect inst_scroll;

	private bool prox_is_awaiting_scroll_head;
	private float prox_preheight_scroll_head;

	private string trans_lut;
	private string book_lut;
	private string address_lut;

	public readonly string[] BOOK_NAMES = new string[MAX_BOOK_COUNT];
	public readonly string[] BOOK_ABBRS = new string[MAX_BOOK_COUNT];

	public readonly int[] CHAPTER_LOCALS = new int[MAX_CHAPTER_COUNT];
	public readonly int[] CHAPTER_BOOKS = new int[MAX_CHAPTER_COUNT];
	public readonly int[] CHAPTER_LENGTHS = new int[MAX_CHAPTER_COUNT];

	private readonly int[] CHAPTER_HEAD_INDECES = new int[MAX_CHAPTER_COUNT];

	private ReaderContentPanelBehaviour inst_content_head;
	private ReaderContentPanelBehaviour inst_content_tail;

	public int chapt_index = 0;

	#endregion
	#region Properties

	#endregion

	#region General

	void Start()
	{
		inst_scroll = GetComponent<ScrollRect>();

		book_lut = book_doc.text;

		for (var i = 0; i < MAX_BOOK_COUNT; i++)
		{
			var line_start = NthIndexOf(book_lut, SEP, i - 1);
			var line_end = NthIndexOf(book_lut, SEP, i) - 1;

			var line = book_lut.Substring(line_start, line_end - line_start);

			var name_char = NthIndexOf(line, ',', 0);
			var abbr_char = NthIndexOf(line, ',', 1);

			BOOK_NAMES[i] = line.Substring(0, name_char - 1);
			BOOK_ABBRS[i] = line.Substring(name_char, abbr_char - name_char - 1);
		}

		address_lut = address_doc.text;
		trans_lut = trans_doc[trans_index].text;

		var b = 0;
		var l = 0;
		var v = 0;
		var h = 0;
		for (var i = 0; i < MAX_CHAPTER_COUNT; i++)
		{
			var v_address = GetAddress(v);
			var j = 0;
			do j++; while (AddressesShareChapter(v_address, GetAddress(v + j)));

			CHAPTER_LENGTHS[i] = j;
			CHAPTER_HEAD_INDECES[i] = h;

			h = NthIndexOf(trans_lut, SEP, j - 1, h);
			v += j;

			CHAPTER_BOOKS[i] = b;
			CHAPTER_LOCALS[i] = l;
			if (AddressesShareBook(v_address, GetAddress(v)))
				l++;
			else
			{
				l = 0;
				b++;
			}
		}

		Clear();

		inst_content_head = CreateChapterContent(chapt_index);
		inst_content_tail = inst_content_head;
	}

	public void SwitchTranslation(int index)
	{
		trans_index = index;
		trans_lut = trans_doc[trans_index].text;

		var start = 0;
		for (var i = 0; i < MAX_CHAPTER_COUNT; i++)
		{
			CHAPTER_HEAD_INDECES[i] = start;
			start = NthIndexOf(trans_lut, SEP, CHAPTER_LENGTHS[i], start);
		}
	}

	private void Clear()
	{
		var length = inst_scroll.content.childCount;
		for (var i = 0; i < length; i++)
			Destroy(inst_scroll.content.GetChild(i).gameObject);
	}

	void Update()
	{
		if (prox_is_awaiting_scroll_head)
			OnScrollPastHeadActually();
	}

	#endregion
	#region Indexing

	private string GetAddress(int line) => address_lut.Substring(line * (LUT_REF_LENGTH + 1), LUT_REF_LENGTH);

	private bool AddressesShareBook(string a, string b) => a.Substring(0, 2) == b.Substring(0, 2);
	private bool AddressesShareChapter(string a, string b) => a.Substring(0, 5) == b.Substring(0, 5);

	#endregion
	#region Scroll Events

	public void OnScrollValueChanged()
	{
		if (inst_scroll.verticalNormalizedPosition > 1f) OnScrollPastHead();
		else if (inst_scroll.verticalNormalizedPosition < 0f) OnScrollPastTail();
	}

	private void OnScrollPastHead()
	{
		if (prox_is_awaiting_scroll_head) return;
		prox_is_awaiting_scroll_head = true;
		prox_preheight_scroll_head = GetCalculatedContentHeight();

		var content = CreateChapterContent(inst_content_head.chapter_index - 1);
		if (content == null) return;

		content.transform.SetAsFirstSibling();
		if (content.chapter_index == 1)
		{
			// var title = CreateNewBookTitle(content.book_index);
			// title.SetAsFirstSibling();
		}

		inst_content_head = content;
		inst_scroll.enabled = false;
	}

	private void OnScrollPastHeadActually()
	{
		var post_height = GetCalculatedContentHeight();

		var fake_scroll_y = inst_scroll.verticalScrollbar.value;
		var scroll_y = fake_scroll_y / (post_height / prox_preheight_scroll_head);
		inst_scroll.verticalNormalizedPosition = scroll_y;
		inst_scroll.enabled = true;
		prox_is_awaiting_scroll_head = false;
	}

	private void OnScrollPastTail()
	{
		var content = CreateChapterContent(inst_content_tail.chapter_index + 1);
		if (content == null) return;

		if (content.chapter_index == 1)
		{
			// var title = CreateNewBookTitle(content.book_index);
			// title.SetAsLastSibling();
		}
		content.transform.SetAsLastSibling();

		inst_content_tail = content;
	}

	private float GetCalculatedContentHeight()
	{
		return inst_scroll.content.rect.height;
	}

	#endregion
	#region Content Creation

	private ReaderContentPanelBehaviour CreateChapterContent(int chapt)
	{
		if (chapt < 0 || chapt >= MAX_CHAPTER_COUNT) return null;

		var obj = Instantiate(pref_content, inst_scroll.content);
		var panel = obj.GetComponent<ReaderContentPanelBehaviour>();

		panel.Init(this, chapt);
		return panel;
	}

	public string CreateChapterText(int chapt)
	{
		var result = string.Empty;

		var char_head = CHAPTER_HEAD_INDECES[chapt];
		for (var i = 0; i < CHAPTER_LENGTHS[chapt]; i++)
		{
			var char_end = trans_lut.IndexOf(SEP, char_head);
			result += $" {GetRichVerseNumber(i)}{trans_lut.Substring(char_head, char_end - char_head)}";
			char_head = char_end + 1;
		}
		return result.Substring(1);
	}

	#endregion
	#region Statics

	private static string GetRichVerseNumber(int index) => $"<sup>{index + 1}</sup>";

	private static int NthIndexOf(in string sample, char c, int n, int startIndex = 0)
	{
		if (n < 0) return 0;
		var result = startIndex;
		for (var i = 0; i <= n; i++)
			result = sample.IndexOf(c, result) + 1;
		return result;
	}

	private static int CountOfChar(in string sample, char c, int end)
	{
		var result = 0;
		var start_index = 0;
		while (start_index < end)
		{
			start_index = sample.IndexOf(c, start_index) + 1;
			result++;
		}
		return result;
	}
	#endregion
}
