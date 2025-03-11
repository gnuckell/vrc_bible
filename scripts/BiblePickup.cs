
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BiblePickup : UdonSharpBehaviour
{
	[SerializeField] private BibleOwner owner;
	[SerializeField] private BibleSetting_VisibilityDropdown visibilityDropdown;

    public override void OnPickup()
    {
		owner.ClaimLocal();
		visibilityDropdown.OnValueChanged();
    }
}
