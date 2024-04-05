
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BookSelectorButton : Button
{
	public override void OnClick()
	{
		host.chapter_index = host.reader.BOOK_HEADS[index];
		host.active_window_index = EBibleWindow.ChapterSelector;
	}

	protected override string GetButtonText(int i) => $"{host.reader.BOOK_NAMES[i]}";
}
