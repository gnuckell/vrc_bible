
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BibleDeedListener : UdonSharpBehaviour
{
    [SerializeField] public BibleDeed deed;

    protected virtual void Start()
    {
        Refresh();
    }

    public override void OnDeserialization()
    {
        Refresh();
    }

    public virtual void Sync()
    {
        Networking.SetOwner(deed.claimant, gameObject);
        Refresh();
        RequestSerialization();
    }

    public virtual void Refresh() { }
}
