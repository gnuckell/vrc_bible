
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

    [UdonSynced] private bool _has_been_claimed = false;
    public VRCPlayerApi claimant {
        get => _has_been_claimed ? Networking.GetOwner(gameObject) : null;
        set {
            _has_been_claimed = value != null;
            if (_has_been_claimed)
                Networking.SetOwner(value, gameObject);

            Refresh();
            RequestSerialization();

            // privacy.Sync();
            foreach (var script in listener_components) {
                script.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "Sync");
            }
        }
    }


    public bool is_owner => _has_been_claimed && Networking.IsOwner(gameObject);
    public bool is_owner_or_unclaimed => !_has_been_claimed || Networking.IsOwner(gameObject);
    public bool is_claimed_by_other => _has_been_claimed && !Networking.IsOwner(gameObject);


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

    private void Refresh()
    {
        _label.text = $"{pickup_name} :: " + (_has_been_claimed ? $"{claimant.displayName} [{claimant.playerId}]" : "<none>");
    }

    public override void OnDeserialization()
    {
        Refresh();
    }

    public void ClaimLocalPlayer() => claimant = Networking.LocalPlayer;
    public void Unclaim() => claimant = null;

//     [SerializeField] private UdonSharpBehaviour[] despawn_components;
//     [SerializeField] private Setting_Privacy privacy;

//     [UdonSynced] private bool _has_been_claimed = false;
//     // public bool has_been_claimed => _has_been_claimed;

//     public bool is_owner => _has_been_claimed && Networking.IsOwner(gameObject);
//     public bool is_owner_or_unclaimed => !_has_been_claimed || Networking.IsOwner(gameObject);
//     public bool is_claimed_by_other => _has_been_claimed && !Networking.IsOwner(gameObject);

//     private TextMeshProUGUI _label;

//     void Start()
//     {
//         _label = GetComponent<TextMeshProUGUI>();

//         Refresh();
//     }

//     public void Despawn()
//     {
//         SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "Despawn_");

//         foreach (var usb in despawn_components)
//             usb.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "Despawn");
//     }
//     public void Despawn_()
//     {
//         Unclaim();
//     }

}
