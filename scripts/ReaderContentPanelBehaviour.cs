
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ReaderContentPanelBehaviour : UdonSharpBehaviour
{
	[SerializeField] private GameObject pref_book_title_text;
	[SerializeField] private GameObject pref_chapter_title_text;
	[SerializeField] private GameObject pref_content_text;

	private TextMeshProUGUI book_title_text;
	private TextMeshProUGUI chapter_title_text;
	private TextMeshProUGUI content_text;

	public int chapter_index = 0;

	private BibleReader inst_reader;

	public void Init(BibleReader reader, int chapt)
	{
		inst_reader = reader;
		chapter_index = chapt;

		if (reader.CHAPTER_LOCALS[chapt] == 0)
		{
			book_title_text = Instantiate(pref_book_title_text, transform).GetComponent<TextMeshProUGUI>();
			book_title_text.text = $"{reader.BOOK_NAMES[reader.CHAPTER_BOOKS[chapt]]}";
		}
		chapter_title_text = Instantiate(pref_chapter_title_text, transform).GetComponent<TextMeshProUGUI>();
		chapter_title_text.text = $"Chapter {reader.CHAPTER_LOCALS[chapt] + 1}";

		content_text = Instantiate(pref_content_text, transform).GetComponent<TextMeshProUGUI>();
		content_text.text = reader.CreateChapterText(chapt);
	}
}
