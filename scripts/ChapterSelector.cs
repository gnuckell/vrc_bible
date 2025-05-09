
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
		UpdateVisibleChildren(host.book_lengths[host.chapter_books[host.chapter_index]]);
	}

	public override void ResetChildren()
	{
		max_buttons = 0;
		foreach (var i in host.book_lengths)
			max_buttons = Math.Max(i, max_buttons);

		base.ResetChildren();
	}
}
