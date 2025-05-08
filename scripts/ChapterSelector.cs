
using System;
using System.Linq;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public sealed class ChapterSelector : ButtonGrid
{
	protected override void Start() { }

	private void OnEnable()
	{
		// Debug.Log(host.current_book_length);

		UpdateVisibleChildren(host.current_book_length);
	}

	public override void ResetChildren()
	{
		max_buttons = 0;
		foreach (var i in host.BOOK_LENGTHS)
			max_buttons = Math.Max(i, max_buttons);

		base.ResetChildren();
	}
}
