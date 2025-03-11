
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BibleOwner : UdonSharpBehaviour
{
    [SerializeField] private GameObject visible_object;
    [SerializeField] private Collider ui_collider;
    [SerializeField] private Collider prop_collider;

    private int _privacy = 1;
    [UdonSynced] private int _privacy_SYNC = 1;
    public int privacy {
        get => _privacy_SYNC;
        set
        {
            value = _has_been_claimed ? value : 2;
            if (_privacy_SYNC == value) return;
            _privacy_SYNC = value;
            UpdatePrivacy();

            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            RequestSerialization();
        }
    }
    private void UpdatePrivacy()
    {
        if (_privacy == _privacy_SYNC) return;
        _privacy = _privacy_SYNC;

        visible_object.SetActive(is_unclaimed_or_owner || _privacy >= 1);

        ui_collider.GetComponent<Canvas>().enabled = is_unclaimed_or_owner || _privacy >= 1;
        ui_collider.enabled = is_unclaimed_or_owner || _privacy >= 2;

        if (prop_collider != null)
            prop_collider.enabled = is_unclaimed_or_owner || _privacy >= 2;
    }

    [UdonSynced] private bool _has_been_claimed = false;
    // public bool has_been_claimed => _has_been_claimed;

    public bool is_local_owner => Networking.IsOwner(gameObject) && _has_been_claimed;
    public bool is_unclaimed_or_owner => Networking.IsOwner(gameObject) || !_has_been_claimed;

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
        privacy = privacy;
    }
}
