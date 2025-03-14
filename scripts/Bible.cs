
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class Bible : UdonSharpBehaviour
{
    [SerializeField] private UdonSharpBehaviour[] despawn_components;
    [SerializeField] private Setting_Privacy privacy;

    [UdonSynced] private bool _has_been_claimed = false;
    // public bool has_been_claimed => _has_been_claimed;

    public bool is_owner => _has_been_claimed && Networking.IsOwner(gameObject);
    public bool is_owner_or_unclaimed => !_has_been_claimed || Networking.IsOwner(gameObject);

    public VRCPlayerApi owner => _has_been_claimed ? Networking.GetOwner(gameObject) : null;
    // public string label_text => owner != null ? $"[{owner.playerId}] {owner.displayName}" : "<none>";
    public string label_text => $"{pickup_name}: " + (owner != null ? $"[{owner.playerId}] {owner.displayName}" : "<none>");

    private string pickup_name { get {
        var pickup_parent = transform.parent.parent.GetComponent<BiblePickup>();
        if (pickup_parent == null) return "<global>";
        return pickup_parent.gameObject.name;
    }}

    private TextMeshProUGUI _label;

    void Start()
    {
        _label = GetComponent<TextMeshProUGUI>();

        Refresh();
    }

    public void Despawn()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "Despawn_");

        foreach (var usb in despawn_components)
            usb.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "Despawn");
    }
    public void Despawn_()
    {
        Unclaim();
    }

    public override void OnDeserialization()
    {
        Refresh();
    }

    public void ClaimLocal() => Claim(Networking.LocalPlayer);

    public void Claim(VRCPlayerApi player)
    {
        _has_been_claimed = true;
        Networking.SetOwner(player, gameObject);

        Refresh();
        RequestSerialization();

        privacy.Sync();
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
