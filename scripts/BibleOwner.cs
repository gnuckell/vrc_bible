
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class BibleOwner : UdonSharpBehaviour
{
    [UdonSynced] private bool _has_been_claimed = false;
    // public bool has_been_claimed => _has_been_claimed;

    public bool is_local_owner => _has_been_claimed && Networking.IsOwner(gameObject);
    public bool is_unclaimed_or_owner => !_has_been_claimed || Networking.IsOwner(gameObject);

    public VRCPlayerApi owner => _has_been_claimed ? Networking.GetOwner(gameObject) : null;
    public string label_text => owner != null ? $"[{owner.playerId}] {owner.displayName}" : "<none>";

    private TextMeshProUGUI _label;

    void Start()
    {
        _label = GetComponent<TextMeshProUGUI>();

        Refresh();
    }

    public override void OnDeserialization()
    {
        Refresh();
    }

    public void ClaimLocal()
    {
        _has_been_claimed = true;
        Networking.SetOwner(Networking.LocalPlayer, gameObject);

        Refresh();
        RequestSerialization();
    }

    public void Unclaim()
    {
        _has_been_claimed = false;

        Refresh();
        RequestSerialization();
    }

    private void Refresh()
    {
        _label.text = label_text;
    }
}
