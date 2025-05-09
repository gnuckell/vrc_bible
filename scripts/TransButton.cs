
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public sealed class TransButton : Button
{
	public string title;
	public string abbr;
	public TextAsset content;

	protected override void Start()
	{
		base.Start();

		text.text = GetButtonText(index);
	}

	public override void OnClick()
	{
		// OnAnyoneClick();
		SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "OnAnyoneClick");
	}

	public void OnAnyoneClick()
	{
		if (host != null)
		{
			UpdateHost();
			host.active_window_index = EBibleWindow.Reader;
		}
	}

	public void UpdateHost()
	{
		host.Init(abbr, content);
	}

	protected override string GetButtonText(int i) => $"{title} <sub>{abbr}</sub>";
}
