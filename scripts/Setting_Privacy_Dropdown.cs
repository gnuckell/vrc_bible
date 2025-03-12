
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Setting_Privacy_Dropdown : UdonSharpBehaviour
{
    [SerializeField] private BibleOwner owner;
    [SerializeField] private Setting_Privacy privacy;
    [SerializeField] public TMP_Dropdown dropdown;

    public int value => dropdown.value;

    public override void OnDeserialization()
    {
        dropdown.SetValueWithoutNotify(privacy.level);
    }

    public void OnValueChanged()
    {
        owner.ClaimLocal();
    }

    public void Sync()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        RequestSerialization();
    }
}
