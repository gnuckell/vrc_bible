
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Setting_Dropdown : BibleDeedListener
{
    [SerializeField] private bool reset_on_unclaim = true;

    [HideInInspector] public TMP_Dropdown dropdown;

    [UdonSynced] private int _value_SYNC;
    public int value
    {
        get => _value_SYNC;
        set {
            _value_SYNC = value;

            // Use the base method to allow us to set the value even if the deed is unclaimed.
            // The normal method will only be called at the moment the deed is unclaimed.
            base.Sync();
        }
    }

    private int initial_value;

    protected override void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        initial_value = dropdown.value;

        if (Networking.IsMaster) value = initial_value;

        base.Start();
    }

	public override void Sync()
	{
        if (reset_on_unclaim && !deed.is_claimed) _value_SYNC = initial_value;

		base.Sync();
	}

	public override void Refresh()
	{
        dropdown.interactable = deed.is_owner_or_unclaimed;
        dropdown.SetValueWithoutNotify(value);
	}

    public void OnValueChanged()
    {
        value = dropdown.value;
    }
}
