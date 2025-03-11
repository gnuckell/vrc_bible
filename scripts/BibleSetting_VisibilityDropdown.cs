
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BibleSetting_VisibilityDropdown : UdonSharpBehaviour
{
    [SerializeField] private BibleOwner owner;
    [SerializeField] private TMP_Dropdown dropdown;

    [UdonSynced] private int value_SYNC;

    public override void OnDeserialization()
    {
        dropdown.SetValueWithoutNotify(value_SYNC);
    }

    public void OnValueChanged()
    {
        owner.privacy = dropdown.value;

        value_SYNC = dropdown.value;
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        RequestSerialization();
    }
}
