
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BiblePickup : UdonSharpBehaviour
{
	[SerializeField] public BibleOwner owner;

	// [UdonSynced] private VRCPlayerApi holder;

    // public override void OnPickup()
    // {
	// 	holder = Networking.LocalPlayer;
    // }

    // public override void OnDrop()
    // {
	// 	holder = null;
    // }

    public void ClaimHolder()
	{
		owner.ClaimLocal();
	}
}
