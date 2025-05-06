
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ReaderContentPanelBehaviour : UdonSharpBehaviour
{
	[SerializeField] private TextMeshProUGUI book_title_text;
	[SerializeField] private TextMeshProUGUI chapter_title_text;
	[SerializeField] private TextMeshProUGUI content_text;

	public int chapter_index = 0;

	public void Init(BibleReaderContent reader, int chapt)
	{
		chapter_index = chapt;

		if (reader.host.CHAPTER_LOCALS[chapt] == 0)
			book_title_text.text = $"{reader.host.BOOK_NAMES[reader.host.CHAPTER_BOOKS[chapt]]}";
		else
			Destroy(book_title_text.gameObject);

		if (reader.host.BOOK_LENGTHS[reader.host.CHAPTER_BOOKS[chapt]] > 1)
			chapter_title_text.text = $"Chapter {reader.host.CHAPTER_LOCALS[chapt] + 1}";
		else
			Destroy(chapter_title_text.gameObject);

		content_text.text = reader.CreateChapterText(chapt);

		content_text.fontSize = 10f;
	}
}
