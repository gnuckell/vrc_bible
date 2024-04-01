
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ReaderContentPanelBehaviour : UdonSharpBehaviour
{
	[SerializeField] public TextMeshProUGUI chapter_text;
	[SerializeField] public TextMeshProUGUI content_text;

	public int book_index = 0;
	public int chapter_index = 0;

	public void Init(string text, int book, int chapter)
	{
		book_index = book;
		chapter_index = chapter;

		chapter_text.text = $"Chapter {chapter}";
		content_text.text = text;
	}
}
