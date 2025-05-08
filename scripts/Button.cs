
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public abstract class Button : UdonSharpBehaviour
{
	[SerializeField] protected TextMeshProUGUI text;

	[SerializeField] private BibleHost _host;
	public BibleHost host => _host;

	public int _index;
	public int index => _index;

	private Image image;
	private Color image_color;
	private UnityEngine.UI.Button button_component;

	protected virtual void Start()
	{
		image = GetComponent<Image>();
		image_color = image.color;
		button_component = GetComponent<UnityEngine.UI.Button>();
	}

	public void Init(BibleHost host, int index)
	{
		_host = host;
		_index = index;

		text.text = GetButtonText(index);
	}

	public void SetVisible(bool visible)
	{
		text.enabled = visible;
		image.color = visible ? image_color : Color.clear;
		button_component.enabled = visible;
	}

	public abstract void OnClick();
	protected abstract string GetButtonText(int i);
}