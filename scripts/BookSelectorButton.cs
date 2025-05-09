
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BookSelectorButton : Button
{
	public override void OnClick()
	{
		host.chapter_index = host.book_heads[index];
		if (host.book_lengths[index] == 1)
		{
			host.reader.ResetContent(host.chapter_index);
			host.active_window_index = EBibleWindow.Reader;
		}
		else
		{
			host.active_window_index = EBibleWindow.ChapterSelector;
		}
	}

	protected override string GetButtonText(int i) => host.book_names[i];
}
