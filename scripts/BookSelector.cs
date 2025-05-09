
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public sealed class BookSelector : ButtonGrid
{
	protected override void Start()
	{
		base.Start();

		ResetChildren();
	}

	public override void ResetChildren()
	{
		max_buttons = host.max_book_count;

		base.ResetChildren();
	}
}
