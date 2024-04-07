
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public sealed class TransButton : Button
{
	public string title;
	public string abbr;
	public TextAsset books;
	public TextAsset address;
	public TextAsset content;

	void Start()
	{
		text.text = GetButtonText(index);
	}

	public override void OnClick()
	{
		// host.trans_index = index;
		if (host == null)
		{
			/**	Spawn a new bible with this translation
			*/
		}
		else
		{
			/**	Update the existing bible with this translation
			*/
			UpdateHost();
			host.active_window_index = EBibleWindow.Reader;
		}

	}

	public void UpdateHost()
	{
		host.Init(title, abbr, books, address, content);
	}

	protected override string GetButtonText(int i) => $"{title} <sub>{abbr}</sub>";
}
