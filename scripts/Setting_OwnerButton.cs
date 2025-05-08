
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Setting_OwnerButton : Setting_Button
{
    [SerializeField] private string claimed_message;
    [SerializeField] private string unclaimed_message;

    private TextMeshProUGUI label;

	protected override void Start()
	{
        label = transform.GetChild(0).GetComponent<TextMeshProUGUI>();

		base.Start();
	}

    public override void Refresh()
	{
        base.Refresh();

        if (deed.is_claimed)
            label.text = deed.is_owner ? claimed_message : $"Owner: {deed.claimant.displayName}";
        else
            label.text = unclaimed_message;
	}
}
