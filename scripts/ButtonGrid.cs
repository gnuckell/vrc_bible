
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

	[SerializeField] private BibleHost _host;
	public BibleHost host => _host;
	[SerializeField] private RectTransform _row_parent;

	[SerializeField] private int _max_rows = 1;
	[SerializeField] private int _max_buttons_in_row = 1;
	[SerializeField] public int max_buttons = 1;

	protected virtual void Start()
	{
		Reset(0);
	}

	private void Clear()
	{
		var length = _row_parent.childCount;
		for (var i = 0; i < length; i++)
			Destroy(_row_parent.GetChild(i).gameObject);
	}

	public virtual void Reset(int index)
	{
		Clear();

		for (int i = 0; i < _max_rows && i * _max_buttons_in_row < max_buttons; i++)
		{
			var row = Instantiate(_pref_row, _row_parent).transform;
			for (int j = 0; j < _max_buttons_in_row; j++)
			{
				var ij = i * _max_buttons_in_row + j;
				if (ij == max_buttons) return;

				var button = Instantiate(_pref_button, row).GetComponent<Button>();
				button.Init(_host, GetButtonIndex(index, ij));
			}
		}
	}

	protected virtual int GetButtonIndex(int index, int local) => local;
}
