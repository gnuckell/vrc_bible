
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Setting_OwnerButton : Setting_Button
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private string claimed_message;
    [SerializeField] private string unclaimed_message;

	public override void Refresh()
	{
        base.Refresh();

        if (deed.claimant != null)
            label.text = deed.claimant.displayName;
        else
            label.text = "Unclaimed";

        // if (deed.has_been_claimed)
        //     // label.text = deed.is_owner ? claimed_message : $"Owner: {deed.claimant.displayName}";
        //     label.text = $"Owner: {deed.claimant.displayName}";
        // else
        //     label.text = unclaimed_message;
	}
}
