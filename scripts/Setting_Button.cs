
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Setting_Button : BibleDeedListener
{
    [HideInInspector] public UnityEngine.UI.Button button;

	protected override void Start()
	{
        button = GetComponent<UnityEngine.UI.Button>();

		base.Start();
	}

	public override void Refresh()
	{
        button.interactable = deed.is_owner;
	}
}
