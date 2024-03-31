
using System;
using System.Text;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class BibleReader : UdonSharpBehaviour
{
	public const float STREAM_MARGIN = 10f;
	private const char SEP = '\n';
	private const int VERSE_OFFSET = 0;

	public readonly string[] BOOK_NAMES = new string[66];
	public readonly string[] BOOK_ABBRS = new string[66];
	public readonly int[] BOOK_SIZES = new int[66];

	[SerializeField] private TextAsset verse_lut;
	[SerializeField] private TextAsset[] translation_lut;
	[SerializeField] private GameObject pref_text;
	private ScrollRect inst_scroll;

	[SerializeField] private TextMeshProUGUI inst_text;

	[SerializeField] private int translation_index = 0;
	[SerializeField] private float print_speed = 40f;

	private int chapter_index = 0;

	private string local_verse_lut;
	private string local_translation_lut;

	private float print_index = 0f;

	private bool prox_is_awaiting_scroll_head;
	private float prox_preheight_scroll_head;

	void Start()
	{
		inst_scroll = GetComponent<ScrollRect>();

		local_verse_lut = verse_lut.text;
		SwitchTranslation(translation_index);
		Interact();
	}

	void Update()
	{
		if (prox_is_awaiting_scroll_head)
			OnScrollPastHeadActually();

		/** <<============================================================>> **/

		// print_index += print_speed * Time.deltaTime;
		// inst_text.maxVisibleCharacters = Mathf.FloorToInt(print_index);
	}

	public override void Interact()
	{
		chapter_index++;
		SetText(BuildChapter(1, chapter_index));
	}

	public void SwitchTranslation(int i)
	{
		translation_index = i;
		SwitchTranslation(translation_lut[i]);
	}
	public void SwitchTranslation(TextAsset data)
	{
		local_translation_lut = data.text;
	}

	public void SetText(string input)
	{
		inst_text.text = input;

		print_index = 0f;
		inst_text.maxVisibleCharacters = 0;
	}

	public TextMeshProUGUI inst_text_debug;

	public void OnScrollValueChanged()
	{
		var scroll_y = inst_scroll.verticalScrollbar.value;

		inst_text_debug.text = scroll_y.ToString();

		if (scroll_y > 1f) OnScrollPastHead();
		else if (scroll_y < 0f) OnScrollPastTail();
	}

	private void OnScrollPastHead()
	{
		if (prox_is_awaiting_scroll_head) return;
		prox_is_awaiting_scroll_head = true;
		prox_preheight_scroll_head = GetCalculatedContentHeight();

		var temp_new_text = (RectTransform)Instantiate(pref_text, inst_scroll.content).transform;
		temp_new_text.SetAsFirstSibling();
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
		var inst_text = Instantiate(pref_text, inst_scroll.content);
		inst_text.transform.SetAsLastSibling();
	}

	public void OnDestroyChild()
	{
	}

	private string BuildChapter(int book, int chapter)
	{
		return BuildVerseRange(GetLineIndex(book, chapter), GetChapterLength(book, chapter));
	}

	private string BuildVerseRange(int line, int length)
	{
		var result = string.Empty;

		var char_start = NthIndexOf(local_translation_lut, SEP, line - (VERSE_OFFSET + 1));
		for (var i = 0; i < length - 1; i++)
		{
			var char_end = local_translation_lut.IndexOf(SEP, char_start);
			result += $" {GetRichVerse(GetLocation_Verse(line + i))}{local_translation_lut.Substring(char_start, char_end - char_start)}";
			char_start = char_end + 1;
		}

		return result.Substring(1);
	}

	private int GetLineIndex(int book, int chapter, int verse = 1)
	{
		var search_string = GetSearchString(book, chapter, verse);
		var char_index = local_verse_lut.IndexOf(search_string);
		return CountOfChar(local_verse_lut, SEP, char_index);
	}

	private string GetSearchString(int book, int chapter, int verse = 1)
	{
		return book.ToString("00") + chapter.ToString("000") + verse.ToString("000");
	}
	private string GetSearchString(int line)
	{
		var char_start = NthIndexOf(local_verse_lut, '\n', line - (VERSE_OFFSET + 1));
		return local_verse_lut.Substring(char_start, 8);
	}

	private int GetChapterLength(int book, int chapter)
	{
		var start = GetLineIndex(book, chapter);
		var i = start;
		do
		{
			i++;
		} while (GetLocation_Chapter(i) == chapter);
		return i - start + 1;
	}

	private int GetLocation_Book(int line)
	{
		var char_start = NthIndexOf(local_verse_lut, SEP, line - 1);
		return int.Parse(local_verse_lut.Substring(char_start, 2));
	}

	private int GetLocation_Chapter(int line)
	{
		var char_start = NthIndexOf(local_verse_lut, SEP, line - 1);
		return int.Parse(local_verse_lut.Substring(char_start + 2, 3));
	}

	private int GetLocation_Verse(int line)
	{
		var char_start = NthIndexOf(local_verse_lut, SEP, line - 1);
		return int.Parse(local_verse_lut.Substring(char_start + 5, 3));
	}

	private static string GetRichVerse(int verse)
	{
		return $"<sup>{verse}</sup>";
	}

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
