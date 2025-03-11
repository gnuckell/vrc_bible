
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BiblePickup : UdonSharpBehaviour
{
	[SerializeField] private BibleHost host;
	[SerializeField] private BibleOwner owner;
	[SerializeField] private TextMeshProUGUI ownerNameLabel;

    public override void OnPickup()
    {
		owner.ClaimLocal();
		// host.TryClaim();
    }
}
