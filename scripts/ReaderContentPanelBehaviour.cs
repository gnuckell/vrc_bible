
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

		if (reader.host.chapter_locals[chapt] == 0)
			book_title_text.text = $"{reader.host.book_names[reader.host.chapter_books[chapt]]}";
		else
			Destroy(book_title_text.gameObject);

		if (reader.host.book_lengths[reader.host.chapter_books[chapt]] > 1)
			chapter_title_text.text = $"Chapter {reader.host.chapter_locals[chapt] + 1}";
		else
			Destroy(chapter_title_text.gameObject);

		content_text.text = reader.host.CreateChapterText(chapt);
	}
}
