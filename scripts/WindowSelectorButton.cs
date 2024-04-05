
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class WindowSelectorButton : UdonSharpBehaviour
{
	[SerializeField] private BibleHost _host;
	[SerializeField] private EBibleWindow _window;

	void Start()
	{

	}

	public void OnClick()
	{
		_host.active_window_index = _window;
	}
}
