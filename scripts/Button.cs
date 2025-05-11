
using System.Collections.Generic;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public abstract class Button : UdonSharpBehaviour
{
	// [SerializeField] protected TextMeshProUGUI label;

	[SerializeField] private BibleHost _host;
	public BibleHost host => _host;

	public int _index;
	public int index => _index;

	private TextMeshProUGUI _label;
	public TextMeshProUGUI label => _label;

	private Image image;
	private Color image_color;
	private UnityEngine.UI.Button button_component;

	public virtual void Start()
	{
		if (image != null) return;

		_label = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		image = GetComponent<Image>();
		image_color = image.color;
		button_component = GetComponent<UnityEngine.UI.Button>();
	}

	public void Init(BibleHost host, int index)
	{
		Start();

		_host = host;
		_index = index;

		_label.text = GetButtonText(index);
	}

	public void SetVisible(bool visible)
	{
		_label.enabled = visible;
		image.color = visible ? image_color : Color.clear;
		button_component.enabled = visible;
	}

	public void RewriteLabel(GameObject prefab)
	{
		var temp = _label.text;
		Destroy(_label.gameObject);
		_label = Instantiate(prefab, transform).GetComponent<TextMeshProUGUI>();
		_label.text = temp;
	}

	public abstract void OnClick();
	protected virtual string GetButtonText(int i) => _label.text;
}