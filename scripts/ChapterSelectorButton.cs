
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public sealed class ChapterSelectorButton : Button
{
	public override void OnClick()
	{
		host.chapter_index = host.book_heads[host.book_index] + index;
		Networking.SetOwner(Networking.LocalPlayer, host.reader.gameObject);
		host.reader.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ResetContent");
		host.active_window_index = EBibleWindow.Reader;
	}

	protected override string GetButtonText(int index) => $"{index + 1}";
}
