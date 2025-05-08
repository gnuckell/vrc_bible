
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

	public virtual void ResetChildren()
	{
		Clear();

		RectTransform last_row = null;
		for (int y = 0; y < _max_rows && y * _max_buttons_in_row < max_buttons; y++)
		{
			last_row = (RectTransform)Instantiate(_pref_row, _row_parent).transform;
			for (int x = 0; x < _max_buttons_in_row; x++)
			{
				var xy = y * _max_buttons_in_row + x;
				if (xy >= max_buttons) break;

				var button = Instantiate(_pref_button, last_row).GetComponent<Button>();
				button.Init(_host, xy);
			}
		}

		if (last_row == null) return;

		var remaining_slot_count = (_max_buttons_in_row - (last_row.childCount % _max_buttons_in_row)) % _max_buttons_in_row;
		for (int x = 0; x < remaining_slot_count; x++)
		{
			Instantiate(_pref_placeholder, last_row);
		}
	}

	public virtual void UpdateVisibleChildren(int index)
	{
		GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;

		for (var y = 0; y < _row_parent.childCount; y++)
		{
			var row = _row_parent.GetChild(y);

			// // For some reason, this line SOMETIMES likes to pretend that the index is 0 on this line, but it's the correct value in the next loop. I do NOT understand what's going on with that. Must be Unity going senile. I refuse to believe I'm going insane.
			//
			row.gameObject.SetActive(y * _max_buttons_in_row < index);
			if (!row.gameObject.activeSelf) continue;

			for (var x = 0; x < row.childCount; x++)
			{
				var xy = y * _max_buttons_in_row + x;
				row.GetChild(x).GetComponent<Button>().SetVisible(xy < index);
			}
		}
	}
}
