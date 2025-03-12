
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BiblePickup : UdonSharpBehaviour
{
	[SerializeField] private BibleOwner owner;

    public override void OnPickup()
    {
		owner.ClaimLocal();
    }
}
