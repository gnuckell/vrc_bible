
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public sealed class BookSelector : ButtonGrid
{
	protected override void Start()
	{
		base.Start();

		Reset(0);
	}
}
