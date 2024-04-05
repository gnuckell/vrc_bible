
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public abstract class Button : UdonSharpBehaviour
{
	[SerializeField] private TextMeshProUGUI text;

	private BibleHost _host;
	public BibleHost host => _host;

	private int _index;
	public int index => _index;

	public void Init(BibleHost host, int index)
	{
		_host = host;
		_index = index;

		text.text = GetButtonText(index);
	}

	public abstract void OnClick();
	protected abstract string GetButtonText(int i);
}