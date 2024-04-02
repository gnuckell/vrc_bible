
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
	private const char SEP = '\n';

	/// <summary>
	/// Total number books in the entire Bible.
	///</summary>
	public const int MAX_BOOK_COUNT = 66;

	/// <summary>
	/// Total number of chapters in the Bible. Used to simplify reference indeces and quicken verse loading.
	///</summary>
	public const int MAX_CHAPTER_COUNT = 1191;

	/// <summary>
	/// Total number of verses in the entire Bible.
	///</summary>
	public const int MAX_VERSE_COUNT = 31102;

	public const int LUT_REF_LENGTH = 8;

	private const int VERSE_OFFSET = 0;

	public readonly string[] BOOK_NAMES = new string[MAX_BOOK_COUNT];
	public readonly string[] BOOK_ABBRS = new string[MAX_BOOK_COUNT];
	public readonly int[] BOOK_SIZES = new int[MAX_BOOK_COUNT];
	public readonly int[] CHAPTER_LOCATIONS = new int[MAX_CHAPTER_COUNT];

	[SerializeField] private TextAsset book_lut;
	[SerializeField] private TextAsset verse_lut;
	[SerializeField] private TextAsset[] translation_lut;


	[SerializeField] private GameObject pref_title;
	[SerializeField] private GameObject pref_content;

	private ScrollRect inst_scroll;
	private ReaderContentPanelBehaviour inst_content_start;
	private ReaderContentPanelBehaviour inst_content_end;

	[SerializeField] private int translation_index = 0;
	[SerializeField] private int book_index = 1;
	[SerializeField] private int chapter_index = 49;

	private string local_verse_lut;
	private string local_translation_lut;

	private float print_index = 0f;

	private bool prox_is_awaiting_scroll_head;
	private float prox_preheight_scroll_head;

	void Start()
	{
		inst_scroll = GetComponent<ScrollRect>();

		var local_info_lut = book_lut.text;
		for (var i = 0; i < MAX_BOOK_COUNT; i++)
		{
			var line_start = NthIndexOf(local_info_lut, SEP, i - 1);
			var line_end = NthIndexOf(local_info_lut, SEP, i) - 1;

			var line = local_info_lut.Substring(line_start, line_end - line_start);

			var a = NthIndexOf(line, ',', 0);
			var b = NthIndexOf(line, ',', 1);

			BOOK_NAMES[i] = line.Substring(0, a - 1);
			BOOK_ABBRS[i] = line.Substring(a, b - a - 1);
			BOOK_SIZES[i] = int.Parse(line.Substring(b, line.Length - b));
		}

		local_verse_lut = verse_lut.text;
		SwitchTranslation(translation_index);

		Refresh();
	}

	void Update()
	{
		if (prox_is_awaiting_scroll_head)
			OnScrollPastHeadActually();
	}

	public void Refresh() => Refresh(book_index, chapter_index);
	public void Refresh(int book, int chapter)
	{
		for (int i = 0; i < inst_scroll.content.childCount; i++)
			Destroy(inst_scroll.content.GetChild(i).gameObject);

		/** <<============================================================>> **/

		inst_content_start = CreateNewChapterContent(book, chapter);
		inst_content_end = inst_content_start;

		if (inst_content_start.chapter_index == 1)
		{
			var title = CreateNewBookTitle(inst_content_start.book_index);
			title.SetAsFirstSibling();
		}
	}

	public void SwitchTranslation(int trans)
	{
		translation_index = trans;
		SwitchTranslation(translation_lut[trans]);
	}
	public void SwitchTranslation(TextAsset data)
	{
		local_translation_lut = data.text;

		var c = 0;
		var abs_line = 1;
		var local_book = 1;
		var local_chapter = 0;
		for (int abs_chapter = 1; abs_chapter <= CHAPTER_LOCATIONS.Length; abs_chapter++)
		{
			while (GetLocation_Book(abs_line) == local_book && GetLocation_ChapterLocal(abs_line) == local_chapter)
			{
				abs_line++;
				c = local_translation_lut.IndexOf(SEP, c) + 1;
			}

			local_book = GetLocation_Book(abs_line);
			local_chapter = GetLocation_ChapterLocal(abs_line);

			CHAPTER_LOCATIONS[abs_chapter - 1] = c;
		}

		Debug.Log($"[0]={CHAPTER_LOCATIONS[0]}");
		Debug.Log($"[1]={CHAPTER_LOCATIONS[1]}");
		Debug.Log($"[1188]={CHAPTER_LOCATIONS[1188]}");

		Debug.Log($"[1]={GetLocation_ChapterGlobal(1)}");
		Debug.Log($"[31084]={GetLocation_ChapterGlobal(31084)}");
	}

	public void OnScrollValueChanged()
	{
		var scroll_y = inst_scroll.verticalNormalizedPosition;
		if (scroll_y > 1f) OnScrollPastHead();
		else if (scroll_y < 0f) OnScrollPastTail();
	}

	private void OnScrollPastHead()
	{
		if (prox_is_awaiting_scroll_head) return;
		prox_is_awaiting_scroll_head = true;
		prox_preheight_scroll_head = GetCalculatedContentHeight();

		var content = CreateNewChapterContent(inst_content_start.book_index, inst_content_start.chapter_index - 1);

		if (content == null) return;

		content.transform.SetAsFirstSibling();
		if (content.chapter_index == 1)
		{
			var title = CreateNewBookTitle(content.book_index);
			title.SetAsFirstSibling();
		}

		inst_content_start = content;
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

	private float GetCalculatedContentHeight()
	{
		return inst_scroll.content.rect.height;
		// var transform = inst_scroll.content.transform;

		// var result = 0f;
		// for (int i = 0; i < transform.childCount; i++)
		// 	result += ((RectTransform)transform.GetChild(i)).rect.height;
		// return result;
	}

	private void OnScrollPastTail()
	{
		var content = CreateNewChapterContent(inst_content_end.book_index, inst_content_end.chapter_index + 1);

		if (content == null) return;

		if (content.chapter_index == 1)
		{
			var title = CreateNewBookTitle(content.book_index);
			title.SetAsLastSibling();
		}
		content.transform.SetAsLastSibling();

		inst_content_end = content;
	}

	private ReaderContentPanelBehaviour CreateNewChapterContent(int book, int chapter)
	{
		if (chapter < 1)
		{
			book--;
			if (book < 1) return null;
			chapter = BOOK_SIZES[book - 1];
		}
		else if (chapter > BOOK_SIZES[book - 1])
		{
			book++;
			if (book > MAX_BOOK_COUNT) return null;
			chapter = 1;
		}

		var obj = Instantiate(pref_content, inst_scroll.content);
		var panel = obj.GetComponent<ReaderContentPanelBehaviour>();

		panel.Init(BuildChapterText(book, chapter), book, chapter);
		return panel;
	}

	private Transform CreateNewBookTitle(int book)
	{
		var obj = Instantiate(pref_title, inst_scroll.content);
		var text = obj.GetComponent<TextMeshProUGUI>();

		text.text = BOOK_NAMES[book - 1];
		return obj.transform;
	}

	private string BuildChapterText(int book, int chapter)
	{
		return BuildVerseRange(GetLineIndex(book, chapter), GetChapterLength(book, chapter));
	}

	private string BuildVerseRange(int line, int length)
	{
		var result = string.Empty;

		var global = GetLocation_ChapterGlobal(line);
		Debug.Log($"Looking for book {GetLocation_Book(line)}, chapter {GetLocation_ChapterLocal(line)}, Global chapter found: {global}");
		var char_start = CHAPTER_LOCATIONS[global];
		for (var i = 0; i < length - 1; i++)
		{
			var char_end = local_translation_lut.IndexOf(SEP, char_start);
			result += $" {GetRichVerseNumber(GetLocation_Verse(line + i))}{local_translation_lut.Substring(char_start, char_end - char_start)}";
			char_start = char_end + 1;
		}

		return result.Substring(1);
	}

	private int GetLineIndex(int book, int chapter, int verse = 1)
	{
		var search_string = GetLocation_String(book, chapter, verse);
		var char_index = local_verse_lut.IndexOf(search_string);
		return GetLutLine(char_index);
	}

	private int GetChapterLength(int book, int chapter)
	{
		var start = GetLineIndex(book, chapter);
		var i = start;
		do
		{
			i++;
		} while (GetLocation_ChapterLocal(i) == chapter);
		return i - start + 1;

		// use CHAPTER_LENGTHS[i];
	}

	private string GetLocation_String(int book, int chapter, int verse = 1) => book.ToString("00") + chapter.ToString("000") + verse.ToString("000");
	private string GetLocation_String(int line) => local_verse_lut.Substring(GetLutLocation(line), LUT_REF_LENGTH);

	private int GetLocation_Book(int line) => int.Parse(local_verse_lut.Substring(GetLutLocation(line), 2));
	private int GetLocation_ChapterGlobal(int line)
	{
		var result = GetLocation_ChapterLocal(line) - 1;
		for (var book = GetLocation_Book(line) - 1; book > 0; book--)
			result += BOOK_SIZES[book - 1];
		return result;
	}
	private int GetLocation_ChapterLocal(int line) => int.Parse(local_verse_lut.Substring(GetLutLocation(line) + 2, 3));
	private int GetLocation_Verse(int line) => int.Parse(local_verse_lut.Substring(GetLutLocation(line) + 5, 3));


	private int GetLutLocation(int line) => (line - 1) * (LUT_REF_LENGTH + 1);
	private int GetLutLine(int location) => (location / (LUT_REF_LENGTH + 1)) + 1;

	private static string GetRichVerseNumber(int index) => $"<sup>{index}</sup>";

	private static int NthIndexOf(in string sample, char c, int count)
	{
		var result = 0;
		for (var i = 0; i <= count; i++)
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
}
