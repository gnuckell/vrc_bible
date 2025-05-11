
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class WindowSelectorButton : Button
{
	[SerializeField] private EBibleWindow _window;

	public override void Start()
	{
		base.Start();
	}

	public override void OnClick()
	{
		host.active_window_index = _window;
	}
}
