
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Setting_Dropdown : BibleDeedListener
{
    [HideInInspector] public TMP_Dropdown dropdown;

    [UdonSynced] private int _value_SYNC;
    public int value
    {
        get => _value_SYNC;
        set {
            _value_SYNC = value;
            Sync();
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
