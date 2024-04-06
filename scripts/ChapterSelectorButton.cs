
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public sealed class ChapterSelectorButton : Button
{
	public override void OnClick()
	{
		host.reader.Reset(index);
		host.active_window_index = EBibleWindow.Reader;
	}

	protected override string GetButtonText(int index) => $"{host.CHAPTER_LOCALS[index] + 1}";
}
