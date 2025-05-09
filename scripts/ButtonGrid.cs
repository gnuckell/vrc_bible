
using System.Collections.Generic;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class ButtonGrid : UdonSharpBehaviour
{
	[SerializeField] private GameObject _pref_row;
	[SerializeField] private GameObject _pref_button;
	[SerializeField] private GameObject _pref_placeholder;

	[SerializeField] private BibleHost _host;
	public BibleHost host => _host;
	[SerializeField] private RectTransform _row_parent;

	[SerializeField] private int _max_rows = 1;
	[SerializeField] private int _max_buttons_in_row = 1;
	[SerializeField] public int max_buttons = 1;

	protected virtual void Start()
	{
		ResetChildren();
	}

	private void Clear()
	{
		var length = _row_parent.childCount;
		for (var i = 0; i < length; i++)
			Destroy(_row_parent.GetChild(i).gameObject);
	}

	private void Resize(int count)
	{
		var length = _row_parent.childCount;
		for (int y = 0; y * _max_buttons_in_row < count || y < length; y++)
		{
			RectTransform current_row = (RectTransform)(y < length ? _row_parent.GetChild(y) : Instantiate(_pref_row, _row_parent).transform);

			if (y * _max_buttons_in_row >= count)
			{
				Destroy(current_row.gameObject);
				continue;
			}

			for (int x = 0; x < _max_buttons_in_row; x++)
			{
				var i = y * _max_buttons_in_row + x;
				var current_button = (x < current_row.childCount ? current_row.GetChild(x) : Instantiate(_pref_button, current_row).transform).GetComponent<Button>();

				current_button.Init(_host, i);
				current_button.SetVisible(i < count);
			}
		}
	}

	public virtual void ResetChildren()
	{
		Resize(max_buttons);
	}

	public virtual void UpdateVisibleChildren(int index)
	{
		GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;

		for (var y = 0; y < _row_parent.childCount; y++)
		{
			var row = _row_parent.GetChild(y);
			row.gameObject.SetActive(y * _max_buttons_in_row < index);
			if (!row.gameObject.activeSelf) continue;

			for (var x = 0; x < row.childCount; x++)
			{
				var i = y * _max_buttons_in_row + x;
				row.GetChild(x).GetComponent<Button>().SetVisible(i < index);
			}
		}
	}

	public void UpdateLabels(GameObject prefab)
	{
		for (var y = 0; y < _row_parent.childCount; y++)
		{
			var row = _row_parent.GetChild(y);
			for (var x = 0; x < row.childCount; x++)
			{
				var button = row.GetChild(x).GetComponent<Button>();
				button.RewriteLabel(prefab);
			}
		}
	}
}
