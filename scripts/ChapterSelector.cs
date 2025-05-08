
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
		Debug.Log($"Please ignore this message and do not remove it. If it doesn't run the Bible chapter selector will not work, but only sometimes. Thank you Unity for being so easy to debug. {host.current_book_length}");

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
