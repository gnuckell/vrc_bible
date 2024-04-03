
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

	private BibleReader host;

	public void Init(BibleReader host, int chapt)
	{
		this.host = host;
		chapter_index = chapt;

		if (host.CHAPTER_LOCALS[chapt] == 0)
		{
			book_title_text = Instantiate(pref_book_title_text, transform).GetComponent<TextMeshProUGUI>();
			book_title_text.text = $"{host.BOOK_NAMES[host.CHAPTER_BOOKS[chapt]]}";
		}
		chapter_title_text = Instantiate(pref_chapter_title_text, transform).GetComponent<TextMeshProUGUI>();
		chapter_title_text.text = $"Chapter {host.CHAPTER_LOCALS[chapt] + 1}";

		content_text = Instantiate(pref_content_text, transform).GetComponent<TextMeshProUGUI>();
		content_text.text = host.CreateChapterText(chapt);
	}
}
