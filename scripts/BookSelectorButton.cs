
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BookSelectorButton : Button
{
	public override void OnClick()
	{
		host.chapter_index = host.book_heads[index];
		Networking.SetOwner(Networking.LocalPlayer, host.reader.gameObject);
		host.reader.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetContent");
		host.active_window_index = host.book_lengths[index] == 1 ? EBibleWindow.Reader : EBibleWindow.ChapterSelector;
	}

	protected override string GetButtonText(int i) => i < host.book_names.Length ? $"{host.book_names[i]}" : "???";
}
