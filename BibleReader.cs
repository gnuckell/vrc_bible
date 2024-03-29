
using System;
using System.Text;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BibleReader : UdonSharpBehaviour
{
	private const char SEP = '\n';
	private const int MAX_DISPLAY_VERSES = 3; // Longest chapter is Psalm 119 with 176 verses. Longest book is Psalm with 150 chapters.
	public TextMeshProUGUI inst_text;
	public TextAsset verse_lut;
	public TextAsset[] translation_lut;

	[SerializeField]
	private int translation_index = 0;

	private int chapter_index = 0;

	private string local_verse_lut;
	private string local_translation_lut;

	void Start()
	{
		local_verse_lut = verse_lut.text;
		SwitchTranslation(translation_index);
		Interact();
	}

	public override void Interact()
	{
		SetText(BuildChapter(0, chapter_index));
		chapter_index++;
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
	}

	private string BuildChapter(int book, int chapter)
	{
		return BuildVerseRange(GetLineIndex(book, chapter), GetChapterLength(book, chapter));
	}

	private string BuildVerseRange(int line, int length)
	{
		length = Math.Min(length, MAX_DISPLAY_VERSES);
		var result = string.Empty;

		var char_start = NthIndexOf(local_translation_lut, SEP, line);
		for (var i = 0; i < length; i++)
		{
			var char_end = local_translation_lut.IndexOf(SEP, char_start);
			result += ' ' + local_translation_lut.Substring(char_start, char_end - char_start);
			char_start = char_end + 1;
		}

		return result.Substring(1);
	}

	private int GetLineIndex(int book, int chapter, int verse = 0)
	{
		var search_string = GetSearchString(book, chapter, verse);
		var char_index = local_verse_lut.IndexOf(search_string);
		return CountOfChar(local_verse_lut, SEP, char_index);
	}

	private string GetSearchString(int book, int chapter, int verse = 0)
	{
		return book.ToString("00") + chapter.ToString("000") + verse.ToString("000");
	}

	private int GetChapterLength(int book, int chapter)
	{
		var start = GetLineIndex(book, chapter);
		var i = start;
		do
		{
			i++;
		} while (GetLocation_Chapter(i) == chapter);
		return i - start;
	}

	private int GetLocation_Book(int line)
	{
		var char_start = NthIndexOf(local_verse_lut, SEP, line);
		return int.Parse(local_verse_lut.Substring(char_start, 2));
	}

	private int GetLocation_Chapter(int line)
	{
		var char_start = NthIndexOf(local_verse_lut, SEP, line);
		return int.Parse(local_verse_lut.Substring(char_start + 2, 3));
	}

	private int GetLocation_Verse(int line)
	{
		var char_start = NthIndexOf(local_verse_lut, SEP, line);
		return int.Parse(local_verse_lut.Substring(char_start + 5, 3));
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
			start_index = sample.IndexOf(c, start_index);
			result++;
		}
		return result;
	}
}
