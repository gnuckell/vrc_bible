
using System.Collections.Generic;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

// Describes/tracks who the owner is.
public class BibleDeed : UdonSharpBehaviour
{
    // These components will have networked functions called on them from here whenever the claimant changes.
    [SerializeField] public UdonSharpBehaviour[] listener_components;


    [UdonSynced] private bool _is_claimed = false;
    public bool is_claimed => _is_claimed;
    public bool is_claimed_by_other => _is_claimed && !Networking.IsOwner(gameObject);
    public bool is_owner => _is_claimed && Networking.IsOwner(gameObject);
    public bool is_owner_or_unclaimed => !_is_claimed || Networking.IsOwner(gameObject);

    public VRCPlayerApi claimant {
        get => _is_claimed ? Networking.GetOwner(gameObject) : null;
        private set {
            _is_claimed = value != null;
            if (_is_claimed)
                Networking.SetOwner(value, gameObject);

            Sync();
        }
    }

    private string pickup_name { get {
        var pickup_parent = transform.parent.parent.GetComponent<BiblePickup>();
        if (pickup_parent == null) return "<global>";
        return pickup_parent.gameObject.name;
    }}

    private TextMeshProUGUI _label;


    private void Start()
    {
        _label = GetComponent<TextMeshProUGUI>();
        Refresh();
    }

	public override void OnPlayerLeft(VRCPlayerApi player)
	{
        if (Networking.Master == claimant)
            Unclaim();

        base.OnPlayerLeft(player);
	}

    public void Sync()
    {
        Refresh();
        RequestSerialization();

        foreach (var script in listener_components) {
            Networking.SetOwner(Networking.GetOwner(gameObject), script.gameObject);
            script.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "Sync");
        }
    }

    public void Refresh()
    {
        _label.text = $"{pickup_name} :: " + (_is_claimed ? $"{claimant.displayName} [{claimant.playerId}]" : "<none>");
    }

    public override void OnDeserialization()
    {
        Refresh();
    }

    public void ClaimLocalToggle() => claimant = _is_claimed ? null : Networking.LocalPlayer;
    public void ClaimLocalPlayer() => claimant = Networking.LocalPlayer;
    public void Unclaim() => claimant = null;
}
